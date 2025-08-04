using CharacterManagerApiTutorial.Models;
using CharacterManagerApiTutorial.Data;
using Microsoft.EntityFrameworkCore;

namespace CharacterManagerApiTutorial.Services
{
    public class CharacterMetadataService(CharacterManagerDbContext context) : ICharacterMetadataService
    {
        private readonly CharacterManagerDbContext _context = context;

        public async Task<Result<List<FactionType>>> GetFactionTypesAsync()
        {
            try
            {
                // Retrieve all faction types asynchronously from the database
                var factionTypes = await _context.FactionTypes.ToListAsync();

                // Return failure if the list is null or empty
                if (factionTypes == null || factionTypes.Count == 0)
                    return Result<List<FactionType>>.Failure("No factions found.");

                // Return success with the list of faction types
                return Result<List<FactionType>>.Success(factionTypes);
            }
            catch (Exception ex)
            {
                // Catch any exceptions and return failure with the exception message
                return Result<List<FactionType>>.Failure($"Failed to retrieve factions: {ex.Message}");
            }
        }

        public async Task<Result<List<RaceType>>> GetRaceTypesAsync()
        {
            try
            {
                // Retrieve all race types asynchronously from the database
                var raceTypes = await _context.RaceTypes.ToListAsync();

                // Return failure if the list is null or empty
                if (raceTypes == null || raceTypes.Count == 0)
                    return Result<List<RaceType>>.Failure("No races found.");

                // Return success with the list of race types
                return Result<List<RaceType>>.Success(raceTypes);
            }
            catch (Exception ex)
            {
                // Catch exceptions and return a failure result with the error message
                return Result<List<RaceType>>.Failure($"Failed to retrieve races: {ex.Message}");
            }
        }

        public async Task<Result<List<ClassType>>> GetClassTypesAsync()
        {
            try
            {
                // Retrieve all class types asynchronously from the database
                var classTypes = await _context.ClassTypes.ToListAsync();

                // Return failure if the list is null or empty
                if (classTypes == null || classTypes.Count == 0)
                    return Result<List<ClassType>>.Failure("No classes found.");

                // Return success with the list of class types
                return Result<List<ClassType>>.Success(classTypes);
            }
            catch (Exception ex)
            {
                // Catch exceptions and return a failure result with the error message
                return Result<List<ClassType>>.Failure($"Failed to retrieve classes: {ex.Message}");
            }
        }

        public async Task<Result<List<CharacterMappings>>> GetCharacterMappingsAsync()
        {
            try
            {
                // Retrieve all character mappings asynchronously from the database
                var characterMappings = await _context.CharacterMappings.ToListAsync();

                // Return failure if the list is null or empty
                if (characterMappings == null || characterMappings.Count == 0)
                    return Result<List<CharacterMappings>>.Failure("No character mappings found.");

                // Return success with the list of character mappings
                return Result<List<CharacterMappings>>.Success(characterMappings);
            }
            catch (Exception ex)
            {
                // Catch exceptions and return a failure result with the error message
                return Result<List<CharacterMappings>>.Failure($"Failed to retrieve character mappings: {ex.Message}");
            }
        }

        public async Task<Result<List<Realm>>> GetRealmsAsync()
        {
            try
            {
                // Retrieve all realms asynchronously from the database
                var realms = await _context.Realms.ToListAsync();

                // Return failure if the list is null or empty
                if (realms == null || realms.Count == 0)
                    return Result<List<Realm>>.Failure("No realms found.");

                // Return success with the list of realms
                return Result<List<Realm>>.Success(realms);
            }
            catch (Exception ex)
            {
                // Catch exceptions and return a failure result with the error message
                return Result<List<Realm>>.Failure($"Failed to retrieve realms: {ex.Message}");
            }
        }
    }
}
