using CharacterManagerApiTutorial.Data;
using CharacterManagerApiTutorial.Models;
using Microsoft.EntityFrameworkCore;

namespace CharacterManagerApiTutorial.Services
{
    public class CharacterService(CharacterManagerDbContext context, ILogger<CharacterService> logger) : ICharacterService
    {
        private readonly CharacterManagerDbContext _context = context;
        private readonly ILogger<CharacterService> _logger = logger;


        /// <summary>
        /// Retrieves all characters associated with a specific user ID (userGuid).
        /// </summary>
        public async Task<Result<List<Character>>> GetCharactersAsync(Guid userGuid)
        {
            // Step 1: Validate userGuid
            if (userGuid == Guid.Empty)
                return Result<List<Character>>.Failure("Invalid user ID.");

            // Step 2: Fetch only characters created by this user
            var characters = await _context.Characters
                .Where(c => c.UserId == userGuid)
                .ToListAsync();

            // Step 3: Return success result with the list of characters
            _logger.LogInformation("Fetching characters for user ID: {UserGuid}", userGuid);
            return Result<List<Character>>.Success(characters);
        }


        /// <summary>
        /// Retrieves specific character by its ID associated with a specific user ID (userGuid).
        /// </summary>
        public async Task<Result<Character>> GetCharacterByIdAsync(int id, Guid userGuid)
        {
            // Step 1: Validate userGuid
            if (userGuid == Guid.Empty)
                return Result<Character>.Failure("Invalid user ID.");

            // Step 2: Attempt to find the character by ID while ensuring it belongs to the authenticated user
            var character = await _context.Characters
                .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userGuid);

            // Step 3: Return failure if the character was not found
            if (character is null)
                return Result<Character>.Failure("Character not found or access denied.");

            // Step 4: Return success with the retrieved character
            _logger.LogInformation("Fetching character {id} for user ID: {UserGuid}", id, userGuid);
            return Result<Character>.Success(character);
        }


        /// <summary>
        /// Creates a new character for a specific user (userGuid) after validating input, rules, and constraints.
        /// </summary>
        public async Task<Result<Character>> CreateCharacterAsync(Character newCharacter, Guid userGuid)
        {
            // Step 1: Check to see if object is null.
            if (newCharacter is null)
                return Result<Character>.Failure("Character is null.");

            // Step 2: Normalize and trim input strings once
            newCharacter.Name = newCharacter.Name.Trim().ToLower();

            // Step 3: Associate character with user
            if (userGuid == Guid.Empty)
                return Result<Character>.Failure("Invalid user ID.");
            newCharacter.UserId = userGuid;

            // Step 4: Validate name length (Max characters = 15).
            if (string.IsNullOrWhiteSpace(newCharacter.Name) || newCharacter.Name.Length > 15)
                return Result<Character>.Failure("Name is invalid or exceeds 15 characters.");

            // Step 5: Validate level range.
            if (newCharacter.Level < 1 || newCharacter.Level > 50)
                return Result<Character>.Failure("Level must be between 1 and 50.");

            // Step 6: Validate realm.
            var isValidRealm = await ValidateRealm(newCharacter);
            if (!isValidRealm)
                return Result<Character>.Failure("Invalid realm ID.");

            // Step 7: Prevent duplicate characters.
            var isUnique = await CheckForDuplicateName(newCharacter);
            if (!isUnique)
                return Result<Character>.Failure("Character with the same name already exists.");

            // Step 8: Validate faction-race-class mapping.
            var isValidCombo = await ValidateFactionRaceClass(newCharacter);
            if (!isValidCombo)
                return Result<Character>.Failure("Invalid faction-race-class combination.");

            // Step 9: Persist to DB.
            try
            {
                _context.Characters.Add(newCharacter);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Creating new character {Name} for user ID: {UserGuid}", newCharacter.Name, userGuid);
                return Result<Character>.Success(newCharacter);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while saving new character for user ID: {UserGuid}", userGuid);
                return Result<Character>.Failure($"Failed to add character: {ex.Message}");
            }
        }


        /// <summary>
        /// Updates an existing character for a specific user (userGuid) after validating input, rules, and constraints.
        /// </summary>
        public async Task<Result> UpdateCharacterAsync(int id, Character updatedCharacter, Guid userGuid)
        {
            // Step 1: Check to see if object is null.
            if (updatedCharacter is null)
                return Result.Failure("Character is null.");

            // Step 2: Validate userGuid.
            if (userGuid == Guid.Empty)
                return Result<Character>.Failure("Invalid user ID.");

            // Step 3: Attempt to find the character by ID while ensuring it belongs to the authenticated user
            var existingCharacter = await _context.Characters
                .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userGuid);

            // Step 4: Return failure if the character was not found
            if (existingCharacter is null)
                return Result.Failure("Character not found or access denied.");

            // Step 5: Normalize and trim input strings once
            updatedCharacter.Name = updatedCharacter.Name.Trim().ToLower();

            // Step 6: Validate name length (max 15 chars).
            if (string.IsNullOrWhiteSpace(updatedCharacter.Name) || updatedCharacter.Name.Length > 15)
                return Result.Failure("Name is invalid or exceeds 15 characters.");

            // Step 7: Validate level range.
            if (updatedCharacter.Level < 1 || updatedCharacter.Level > 50)
                return Result.Failure("Level must be between 1 and 50.");

            // Step 8: Validate realm
            var isValidRealm = await ValidateRealm(updatedCharacter);
            if (!isValidRealm)
                return Result<Character>.Failure("Invalid realm ID.");

            // Step 9: Check for duplicate name+realm if either changed.
            if (!existingCharacter.Name.Equals(updatedCharacter.Name, StringComparison.OrdinalIgnoreCase) ||
                existingCharacter.RealmId != updatedCharacter.RealmId)
            {
                var isUnique = await CheckForDuplicateName(updatedCharacter);
                if (!isUnique)
                    return Result.Failure("Character with the same name already exists.");
            }

            // Step 10: Update properties.
            existingCharacter.Name = updatedCharacter.Name;
            existingCharacter.Level = updatedCharacter.Level;
            existingCharacter.FactionId = updatedCharacter.FactionId;
            existingCharacter.RaceId = updatedCharacter.RaceId;
            existingCharacter.ClassId = updatedCharacter.ClassId;
            existingCharacter.RealmId = updatedCharacter.RealmId;

            // Step 11: Validate faction-race-class combination.
            var isValidCombo = await ValidateFactionRaceClass(existingCharacter);
            if (!isValidCombo)
                return Result.Failure("Invalid faction-race-class combination.");

            // Step 12: Persist to DB.
            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation("Updated character with ID {id} for user ID: {UserGuid}", id, userGuid);
                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update character ID {id}", id);
                return Result.Failure($"Failed to update character: {ex.Message}");
            }
        }


        /// <summary>
        /// Deletes a character by ID for a specific user (userGuid), ensuring the character belongs to that user.
        /// </summary>
        public async Task<Result<int>> DeleteCharacterAsync(int id, Guid userGuid)
        {
            // Step 1: Validate userGuid.
            if (userGuid == Guid.Empty)
                return Result<int>.Failure("Invalid user ID.");

            // Step 2: Attempt to find the character by ID while ensuring it belongs to the authenticated user
            var character = await _context.Characters
                .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userGuid);

            // Step 3: Return failure if the character does not exist
            if (character is null)
                return Result<int>.Failure("Character not found or access denied.");

            try
            {
                _context.Characters.Remove(character);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Deleted character ID {Id} for user ID: {UserGuid}", id, userGuid);
                return Result<int>.Success(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting character ID {Id}", id);
                return Result<int>.Failure($"Failed to delete character: {ex.Message}");
            }
        }


        /// <summary>
        /// Validates whether the specified character has a valid realm assignment.
        /// </summary>
        private async Task<bool> ValidateRealm(Character character)
        {
            // Return false if the character is null, or Realm Id is non-positive
            if (character is null || character.RealmId <= 0)
                return false;

            // Check if a realm with the specified ID exists in the database
            var realmExists = await _context.Realms.AnyAsync(r => r.Id == character.RealmId);

            return realmExists;
        }


        /// <summary>
        /// Checks whether the given character's name and realm combination is unique.
        /// </summary>
        private async Task<bool> CheckForDuplicateName(Character character)
        {
            // Return false if the input character is null
            if (character is null)
                return false;

            // Retrieve all existing characters from the database
            var characters = await _context.Characters.ToListAsync();

            // Compare the input character's name and realm with each existing character
            for (int i = 0; i < characters.Count; i++)
            {
                // If a match is found (case-insensitive), it's a duplicate — return false
                if (string.Equals(character.Name, characters[i].Name, StringComparison.OrdinalIgnoreCase) &&
                    character.RealmId == characters[i].RealmId)
                {
                    return false;
                }
            }

            // No duplicates found — return true
            return true;
        }


        /// <summary>
        /// Validates that the given character's faction, race, and class combination is allowed based on predefined mappings in the database.
        /// </summary>
        private async Task<bool> ValidateFactionRaceClass(Character newCharacter)
        {
            List<CharacterMappings> raceList = [];

            // Ensure the character exists and the FactionId is within the valid range (1 = Good Guys, 2 = Bad Guys)
            if (newCharacter is null || newCharacter.FactionId < 1 || newCharacter.FactionId > 2)
                return false;

            var mappings = await _context.CharacterMappings.ToListAsync();

            // Loop through character mappings and collect only those that match the character's faction and race
            for (int i = 0; i < mappings.Count; i++)
            {
                if (mappings[i].FactionId == newCharacter.FactionId && mappings[i].RaceId == newCharacter.RaceId)
                {
                    raceList.Add(mappings[i]);
                }
            }

            // Validate that at least one race is available for the selected faction
            if (raceList.Count == 0)
                return false;

            // Verify that the chosen class is valid for the given race and faction combination
            for (int i = 0; i < raceList.Count; i++)
            {
                if (raceList[i].ClassId == newCharacter.ClassId)
                {
                    return true;
                }
            }

            // No valid class found for the given faction and race combination — validation fails
            return false;
        }
    }
}
