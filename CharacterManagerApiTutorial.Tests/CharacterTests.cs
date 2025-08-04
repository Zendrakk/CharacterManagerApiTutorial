using CharacterManagerApiTutorial.Models;
using CharacterManagerApiTutorial.Services;
using CharacterManagerApiTutorial.Data;
using Microsoft.EntityFrameworkCore;

namespace CharacterManagerApiTutorial.Tests
{
    public class CharacterTests
    {
        //====================================================
        //  GET CHARACTERS TESTS
        //====================================================

        [Fact]
        public async Task GetCharacters_EmptyUserGuid_ReturnsFailure()
        {
            // Arrange
            var characterService = CreateCharacterService(out var context);
            var userGuid = Guid.NewGuid();
            context.Characters.AddRange(
                new Character { Id = 1, Name = "tester", Level = 40, FactionId = 1, RaceId = 1, ClassId = 1, RealmId = 1, UserId = userGuid },
                new Character { Id = 2, Name = "tester2", Level = 25, FactionId = 2, RaceId = 5, ClassId = 3, RealmId = 1, UserId = userGuid }
            );
            await context.SaveChangesAsync();

            // Act
            var result = await characterService.GetCharactersAsync(Guid.Empty);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Null(result.Value);
            Assert.NotNull(result.Error);
            Assert.Equal("Invalid user ID.", result.Error);

        }

        [Fact]
        public async Task GetCharacters_ValidRequest_ReturnsSuccess()
        {
            // Arrange
            var characterService = CreateCharacterService(out var context);
            var userGuid = Guid.NewGuid();
            context.Characters.AddRange(
                new Character { Id = 1, Name = "tester", Level = 40, FactionId = 1, RaceId = 1, ClassId = 1, RealmId = 1, UserId = userGuid },
                new Character { Id = 2, Name = "tester2", Level = 25, FactionId = 2, RaceId = 5, ClassId = 3, RealmId = 1, UserId = userGuid }
            );
            await context.SaveChangesAsync();

            // Act
            var result = await characterService.GetCharactersAsync(userGuid);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.IsType<Result<List<Character>>>(result);
            Assert.Equal(2, result.Value.Count);
            Assert.Contains(result.Value, c => c.Name == "tester" && c.UserId == userGuid);
            Assert.Contains(result.Value, c => c.Name == "tester2" && c.UserId == userGuid);
        }

        [Fact]
        public async Task GetCharacters_ValidEmptyList_ReturnsSuccess()
        {
            // Arrange
            var userGuid = Guid.NewGuid();
            var characterService = CreateCharacterService(out var context);

            // Act
            var result = await characterService.GetCharactersAsync(userGuid);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.IsType<Result<List<Character>>>(result);
            Assert.Empty(result.Value);
        }


        //====================================================
        //  GET CHARACTER BY ID TESTS
        //====================================================

        [Fact]
        public async Task GetCharacterById_InvalidId_ReturnsFailure()
        {
            // Arrange
            var characterService = CreateCharacterService(out var context);
            var userGuid = Guid.NewGuid();
            context.Characters.Add(new Character { Id = 1, Name = "tester", Level = 40, FactionId = 1, RaceId = 1, ClassId = 1, RealmId = 1, UserId = userGuid });
            await context.SaveChangesAsync();

            // Act
            var result = await characterService.GetCharacterByIdAsync(0, userGuid);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.Error);
            Assert.Null(result.Value);
            Assert.Equal("Character not found or access denied.", result.Error);
        }

        [Fact]
        public async Task GetCharacterById_InvalidUserGuid_ReturnsFailure()
        {
            // Arrange
            var characterService = CreateCharacterService(out var context);
            var userGuid = Guid.NewGuid();
            context.Characters.Add(new Character { Id = 1, Name = "tester", Level = 40, FactionId = 1, RaceId = 1, ClassId = 1, RealmId = 1, UserId = userGuid });
            await context.SaveChangesAsync();

            // Act
            var result = await characterService.GetCharacterByIdAsync(1, Guid.Empty);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.Error);
            Assert.Null(result.Value);
            Assert.Equal("Invalid user ID.", result.Error);
        }

        [Fact]
        public async Task GetCharacterById_ValidRequest_ReturnsSuccess()
        {
            // Arrange
            var characterService = CreateCharacterService(out var context);
            var userGuid = Guid.NewGuid();
            context.Characters.Add(new Character { Id = 1, Name = "Tester", Level = 40, FactionId = 1, RaceId = 1, ClassId = 1, RealmId = 1, UserId = userGuid });
            await context.SaveChangesAsync();

            // Act
            var result = await characterService.GetCharacterByIdAsync(1, userGuid);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.IsType<Result<Character>>(result);
            Assert.Equal("Tester", result.Value.Name);
            Assert.Equal(userGuid, result.Value.UserId);
        }


        //====================================================
        //  CREATE CHARACTER TESTS
        //====================================================

        [Fact]
        public async Task CreateCharacter_NullRequest_ReturnsFailure()
        {
            // Arrange
            var characterService = CreateCharacterService(out var context);
            var userGuid = Guid.NewGuid();
            var newCharacter = new Character { Id = 1, Name = "Tester", Level = 10, FactionId = 1, RaceId = 1, ClassId = 1, RealmId = 1 };
            newCharacter = null;

            // Act
            var result = await characterService.CreateCharacterAsync(newCharacter!, userGuid);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Null(result.Value);
            Assert.Equal("Character is null.", result.Error);
        }

        [Fact]
        public async Task CreateCharacter_InvalidFactionToRace_ReturnsFailure()
        {
            // Arrange
            var characterService = CreateCharacterService(out var context);
            var userGuid = Guid.NewGuid();
            var newCharacter = new Character { Id = 1, Name = "Tester", Level = 10, FactionId = 2, RaceId = 1, ClassId = 1, RealmId = 1 };

            // Act
            var result = await characterService.CreateCharacterAsync(newCharacter, userGuid);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Null(result.Value);
            Assert.Equal("Invalid faction-race-class combination.", result.Error);
        }

        [Fact]
        public async Task CreateCharacter_InvalidRaceToClass_ReturnsFailure()
        {
            // Arrange
            var characterService = CreateCharacterService(out var context);
            var userGuid = Guid.NewGuid();
            var newCharacter = new Character { Id = 1, Name = "Tester", Level = 30, FactionId = 1, RaceId = 2, ClassId = 3, RealmId = 1 };

            // Act
            var result = await characterService.CreateCharacterAsync(newCharacter, userGuid);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Null(result.Value);
            Assert.Equal("Invalid faction-race-class combination.", result.Error);
        }

        [Fact]
        public async Task CreateCharacter_InvalidNameLength_ReturnsFailure()
        {
            // Arrange
            var characterService = CreateCharacterService(out var context);
            var userGuid = Guid.NewGuid();
            var newCharacter = new Character { Id = 1, Name = "TesterTesterTester", Level = 40, FactionId = 1, RaceId = 1, ClassId = 1, RealmId = 1 };

            // Act
            var result = await characterService.CreateCharacterAsync(newCharacter, userGuid);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Null(result.Value);
            Assert.Equal("Name is invalid or exceeds 15 characters.", result.Error);
        }

        [Fact]
        public async Task CreateCharacter_InvalidLevel_ReturnsFailure()
        {
            // Arrange
            var characterService = CreateCharacterService(out var context);
            var userGuid = Guid.NewGuid();
            var newCharacter = new Character { Id = 1, Name = "Tester", Level = 0, FactionId = 1, RaceId = 1, ClassId = 1, RealmId = 1 };

            // Act
            var result = await characterService.CreateCharacterAsync(newCharacter, userGuid);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Null(result.Value);
            Assert.Equal("Level must be between 1 and 50.", result.Error);
        }

        [Fact]
        public async Task CreateCharacter_DuplicateName_ReturnsFailure()
        {
            // Arrange
            var characterService = CreateCharacterService(out var context);
            var userGuid = Guid.NewGuid();
            var newCharacter = new Character { Id = 1, Name = "Tester", Level = 30, FactionId = 1, RaceId = 1, ClassId = 1, RealmId = 2 };
            var newCharacter2 = new Character { Id = 2, Name = "Tester", Level = 20, FactionId = 1, RaceId = 1, ClassId = 1, RealmId = 2 };

            // Act
            var result = await characterService.CreateCharacterAsync(newCharacter, userGuid);
            var result2 = await characterService.CreateCharacterAsync(newCharacter2, userGuid);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.False(result2.IsSuccess);
            Assert.NotNull(result);
            Assert.IsType<Result<Character>>(result);
            Assert.Equal("Character with the same name already exists.", result2.Error);
        }

        [Fact]
        public async Task CreateCharacter_InvalidUserGuid_ReturnsFailure()
        {
            // Arrange
            var characterService = CreateCharacterService(out var context);
            var newCharacter = new Character { Id = 1, Name = "Tester", Level = 10, FactionId = 1, RaceId = 1, ClassId = 1, RealmId = 1 };

            // Act
            var result = await characterService.CreateCharacterAsync(newCharacter, Guid.Empty);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Null(result.Value);
            Assert.Equal("Invalid user ID.", result.Error);
        }

        [Fact]
        public async Task CreateCharacter_ValidRequest_ReturnsSuccess()
        {
            // Arrange
            var characterService = CreateCharacterService(out var context);
            var userGuid = Guid.NewGuid();
            var newCharacter = new Character { Id = 1, Name = "Tester", Level = 20, FactionId = 1, RaceId = 1, ClassId = 1, RealmId = 1 };

            // Act
            var result = await characterService.CreateCharacterAsync(newCharacter, userGuid);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.IsType<Result<Character>>(result);
            Assert.Equal("tester", result.Value.Name);
            Assert.Equal(1, result.Value.RealmId);
            Assert.Equal(userGuid, result.Value.UserId);
        }


        //====================================================
        //  UPDATE CHARACTER TESTS
        //====================================================

        [Fact]
        public async Task UpdateCharacter_InvalidUserGuid_ReturnsFailure()
        {
            // Arrange
            var characterService = CreateCharacterService(out var context);
            var userGuid = Guid.NewGuid();
            var newCharacter = new Character { Id = 1, Name = "tester", Level = 10, FactionId = 1, RaceId = 1, ClassId = 1, RealmId = 1, UserId = userGuid };
            context.Characters.Add(newCharacter);
            await context.SaveChangesAsync();

            // Act
            var result = await characterService.UpdateCharacterAsync(1, newCharacter, Guid.Empty);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Invalid user ID.", result.Error);

        }

        [Fact]
        public async Task UpdateCharacter_UserGuidDoesNotMatch_ReturnsFailure()
        {
            // Arrange
            var characterService = CreateCharacterService(out var context);
            var userGuid = Guid.NewGuid();
            var newCharacter = new Character { Id = 1, Name = "tester", Level = 10, FactionId = 1, RaceId = 1, ClassId = 1, RealmId = 1, UserId = userGuid };
            context.Characters.Add(newCharacter);
            await context.SaveChangesAsync();

            // Act
            var result = await characterService.UpdateCharacterAsync(1, newCharacter, Guid.NewGuid());

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Character not found or access denied.", result.Error);

        }

        [Fact]
        public async Task UpdateCharacter_InvalidRealmId_ReturnsFailure()
        {
            // Arrange
            var characterService = CreateCharacterService(out var context);
            var userGuid = Guid.NewGuid();
            var newCharacter = new Character { Id = 1, Name = "tester", Level = 10, FactionId = 1, RaceId = 1, ClassId = 1, RealmId = 1, UserId = userGuid };
            context.Characters.Add(newCharacter);
            await context.SaveChangesAsync();

            // Act
            newCharacter.RealmId = 100000;
            var result = await characterService.UpdateCharacterAsync(1, newCharacter, userGuid);
            var updated = context.Characters.Find(1);

            // Assert
            Assert.NotNull(updated);
            Assert.False(result.IsSuccess);
            Assert.Equal("Invalid realm ID.", result.Error);
        }

        [Fact]
        public async Task UpdateCharacter_ModifyCharacterValues_ReturnsSuccess()
        {
            // Arrange
            var characterService = CreateCharacterService(out var context);
            var userGuid = Guid.NewGuid();
            var newCharacter = new Character { Id = 1, Name = "tester", Level = 10, FactionId = 1, RaceId = 1, ClassId = 1, RealmId = 1, UserId = userGuid };
            context.Characters.Add(newCharacter);
            await context.SaveChangesAsync();

            // Act
            newCharacter.Name = "Updated Name";
            newCharacter.Level = 30;
            newCharacter.FactionId = 2;
            newCharacter.RaceId = 4;
            newCharacter.ClassId = 2;
            newCharacter.RealmId = 5;
            var result = await characterService.UpdateCharacterAsync(1, newCharacter, userGuid);
            var updated = context.Characters.Find(1);

            // Assert
            Assert.NotNull(updated);
            Assert.True(result.IsSuccess);
            Assert.Equal("updated name", updated.Name);
            Assert.Equal(30, updated.Level);
            Assert.Equal(2, updated.FactionId);
            Assert.Equal(4, updated.RaceId);
            Assert.Equal(2, updated.ClassId);
            Assert.Equal(5, updated.RealmId);
        }


        //====================================================
        //  DELETE CHARACTER TESTS
        //====================================================

        [Fact]
        public async Task DeleteCharacter_InvalidCharacterId_ReturnsFailure()
        {
            // Arrange
            var characterService = CreateCharacterService(out var context);
            var userGuid = Guid.NewGuid();
            var newCharacter = new Character { Id = 1, Name = "tester", Level = 10, FactionId = 1, RaceId = 1, ClassId = 1, RealmId = 1, UserId = userGuid };
            context.Characters.Add(newCharacter);
            await context.SaveChangesAsync();

            // Act
            var result = await characterService.DeleteCharacterAsync(0, userGuid);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
            Assert.Equal("Character not found or access denied.", result.Error);
        }

        [Fact]
        public async Task DeleteCharacter_InvalidUserGuid_ReturnsFailure()
        {
            // Arrange
            var characterService = CreateCharacterService(out var context);
            var userGuid = Guid.NewGuid();
            var newCharacter = new Character { Id = 1, Name = "tester", Level = 10, FactionId = 1, RaceId = 1, ClassId = 1, RealmId = 1, UserId = userGuid };
            context.Characters.Add(newCharacter);
            await context.SaveChangesAsync();

            // Act
            var result = await characterService.DeleteCharacterAsync(1, Guid.Empty);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
            Assert.Equal("Invalid user ID.", result.Error);
        }

        [Fact]
        public async Task DeleteCharacter_IncorrectUserGuid_ReturnsFailure()
        {
            // Arrange
            var characterService = CreateCharacterService(out var context);
            var userGuid = Guid.NewGuid();
            var newCharacter = new Character { Id = 1, Name = "tester", Level = 10, FactionId = 1, RaceId = 1, ClassId = 1, RealmId = 1, UserId = userGuid };
            context.Characters.Add(newCharacter);
            await context.SaveChangesAsync();

            // Act
            var result = await characterService.DeleteCharacterAsync(1, Guid.NewGuid());

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
            Assert.Equal("Character not found or access denied.", result.Error);
        }

        [Fact]
        public async Task DeleteCharacter_ValidRequest_ReturnsSuccess()
        {
            // Arrange
            var characterService = CreateCharacterService(out var context);
            var userGuid = Guid.NewGuid();
            context.Characters.Add(new Character { Id = 1, Name = "Tester", Level = 40, FactionId = 1, RaceId = 1, ClassId = 1, RealmId = 1, UserId = userGuid });
            await context.SaveChangesAsync();

            // Act
            var result = await characterService.DeleteCharacterAsync(1, userGuid);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess, "Deletion should succeed.");
            Assert.Equal(1, result.Value);
            Assert.IsType<Result<int>>(result);
        }


        //====================================================
        //  PRIVATE METHODS
        //====================================================


        /// <summary>
        /// Creates an instance of <see cref="CharacterService"/> using an in-memory database context.
        /// </summary>
        private static CharacterService CreateCharacterService(out CharacterManagerDbContext context)
        {
            // Create a new in-memory DbContext for isolated testing
            context = GetInMemoryDbContext();

            // Seeds the provided FantasyGameTutorialDbContext with predefined realm and race/class mappings
            AddRealms(context);
            AddRaceClassMappings(context);

            // Return a new instance of the CharacterService using the test context
            return new CharacterService(context);
        }


        /// <summary>
        /// Creates a new instance of the in-memory FantasyGameTutorialDbContext for testing purposes.
        /// Each call generates a unique database to ensure test isolation.
        /// </summary>
        private static CharacterManagerDbContext GetInMemoryDbContext()
        {
            // Create a new in-memory database with a unique name for test isolation
            var options = new DbContextOptionsBuilder<CharacterManagerDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())  // unique DB for isolation
                .Options;

            // Return a new DbContext instance using the in-memory options
            return new CharacterManagerDbContext(options);
        }


        /// <summary>
        /// Seeds the provided <see cref="FantasyGameTutorialDbContext"/> with predefined realm data.
        /// </summary>
        private async static void AddRealms(CharacterManagerDbContext context)
        {
            context.AddRange(
                new Realm { Id = 1, Name = "Frostgard", Type = "Neutral" },
                new Realm { Id = 2, Name = "Barren Land", Type = "Neutral" },
                new Realm { Id = 3, Name = "Wraithwind", Type = "PVP" },
                new Realm { Id = 4, Name = "Amber Expanse", Type = "Neutral" },
                new Realm { Id = 5, Name = "Boiling Isle", Type = "PVP" }
            );
            await context.SaveChangesAsync();
        }


        /// <summary>
        /// Seeds the <see cref="CharacterManagerDbContext"/> with predefined race-class-faction mappings.
        /// </summary>
        /// <remarks>
        /// This method establishes valid character combinations by associating races, classes, and factions.
        /// These mappings define which class options are available to each race under a specific faction,
        /// which is essential for validating character creation logic in tests.
        /// </remarks>
        private async static void AddRaceClassMappings(CharacterManagerDbContext context)
        {
            context.AddRange(
                // Human (Good Guys)
                new CharacterMappings { Id = 1, FactionId = 1, RaceId = 1, ClassId = 1 },
                new CharacterMappings { Id = 2, FactionId = 1, RaceId = 1, ClassId = 2 },
                new CharacterMappings { Id = 3, FactionId = 1, RaceId = 1, ClassId = 3 },
                new CharacterMappings { Id = 4, FactionId = 1, RaceId = 1, ClassId = 4 },
                new CharacterMappings { Id = 5, FactionId = 1, RaceId = 1, ClassId = 5 },
                new CharacterMappings { Id = 6, FactionId = 1, RaceId = 1, ClassId = 6 },

                // Dwarf (Good Guys)
                new CharacterMappings { Id = 7, FactionId = 1, RaceId = 2, ClassId = 1 },
                new CharacterMappings { Id = 8, FactionId = 1, RaceId = 2, ClassId = 2 },
                new CharacterMappings { Id = 9, FactionId = 1, RaceId = 2, ClassId = 5 },
                new CharacterMappings { Id = 10, FactionId = 1, RaceId = 2, ClassId = 6 },

                // Elf (Good Guys)
                new CharacterMappings { Id = 11, FactionId = 1, RaceId = 3, ClassId = 2 },
                new CharacterMappings { Id = 12, FactionId = 1, RaceId = 3, ClassId = 3 },
                new CharacterMappings { Id = 13, FactionId = 1, RaceId = 3, ClassId = 4 },
                new CharacterMappings { Id = 14, FactionId = 1, RaceId = 3, ClassId = 5 },

                // Orc (Bad Guys)
                new CharacterMappings { Id = 15, FactionId = 2, RaceId = 4, ClassId = 1 },
                new CharacterMappings { Id = 16, FactionId = 2, RaceId = 4, ClassId = 2 },
                new CharacterMappings { Id = 17, FactionId = 2, RaceId = 4, ClassId = 4 },

                // Zombie (Bad Guys)
                new CharacterMappings { Id = 18, FactionId = 2, RaceId = 5, ClassId = 1 },
                new CharacterMappings { Id = 19, FactionId = 2, RaceId = 5, ClassId = 2 },
                new CharacterMappings { Id = 20, FactionId = 2, RaceId = 5, ClassId = 3 },
                new CharacterMappings { Id = 21, FactionId = 2, RaceId = 5, ClassId = 4 },
                new CharacterMappings { Id = 22, FactionId = 2, RaceId = 5, ClassId = 5 },

                // Alien (Bad Guys)
                new CharacterMappings { Id = 23, FactionId = 2, RaceId = 6, ClassId = 1 },
                new CharacterMappings { Id = 24, FactionId = 2, RaceId = 6, ClassId = 2 },
                new CharacterMappings { Id = 25, FactionId = 2, RaceId = 6, ClassId = 3 },
                new CharacterMappings { Id = 26, FactionId = 2, RaceId = 6, ClassId = 4 },
                new CharacterMappings { Id = 27, FactionId = 2, RaceId = 6, ClassId = 5 },
                new CharacterMappings { Id = 28, FactionId = 2, RaceId = 6, ClassId = 6 }
            );
            await context.SaveChangesAsync();
        }
    }
}
