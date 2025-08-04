using CharacterManagerApiTutorial.Models;
using CharacterManagerApiTutorial.Services;
using CharacterManagerApiTutorial.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace CharacterManagerApiTutorial.Tests
{
    public class CharacterMetadataTests
    {
        //====================================================
        //  GET FACTIONTYPE TESTS
        //====================================================

        [Fact]
        public async Task GetFactionTypes_NoneInDatabase_ReturnsFailure()
        {
            // Arrange
            var characterMetadataService = CreateCharacterMetadataService(out var context);

            // Act
            var result = await characterMetadataService.GetFactionTypesAsync();

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Null(result.Value);
            Assert.NotNull(result.Error);
            Assert.Equal("No factions found.", result.Error);
        }

        [Fact]
        public async Task GetFactionTypes_ValidRequest_ReturnsSuccess()
        {
            // Arrange
            var characterMetadataService = CreateCharacterMetadataService(out var context, seedFactionTypes: true);

            // Act
            var result = await characterMetadataService.GetFactionTypesAsync();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.IsType<Result<List<FactionType>>>(result);
        }


        //====================================================
        //  GET RACETYPE TESTS
        //====================================================

        [Fact]
        public async Task GetRaceTypes_NoneInDatabase_ReturnsFailure()
        {
            // Arrange
            var characterMetadataService = CreateCharacterMetadataService(out var context);

            // Act
            var result = await characterMetadataService.GetRaceTypesAsync();

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Null(result.Value);
            Assert.NotNull(result.Error);
            Assert.Equal("No races found.", result.Error);
        }

        [Fact]
        public async Task GetRaceTypes_ValidRequest_ReturnsSuccess()
        {
            // Arrange
            var characterMetadataService = CreateCharacterMetadataService(out var context, seedRaceTypes: true);

            // Act
            var result = await characterMetadataService.GetRaceTypesAsync();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.IsType<Result<List<RaceType>>>(result);
        }


        //====================================================
        //  GET CLASSTYPE TESTS
        //====================================================

        [Fact]
        public async Task GetClassTypes_NoneInDatabase_ReturnsFailure()
        {
            // Arrange
            var characterMetadataService = CreateCharacterMetadataService(out var context);

            // Act
            var result = await characterMetadataService.GetClassTypesAsync();

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Null(result.Value);
            Assert.NotNull(result.Error);
            Assert.Equal("No classes found.", result.Error);
        }

        [Fact]
        public async Task GetClassTypes_ValidRequest_ReturnsSuccess()
        {
            // Arrange
            var characterMetadataService = CreateCharacterMetadataService(out var context, seedClassTypes: true);

            // Act
            var result = await characterMetadataService.GetClassTypesAsync();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.IsType<Result<List<ClassType>>>(result);
        }


        //====================================================
        //  GET CHARACTER MAPPINGS TESTS
        //====================================================

        [Fact]
        public async Task GetCharacterMappings_NoneInDatabase_ReturnsFailure()
        {
            // Arrange
            var characterMetadataService = CreateCharacterMetadataService(out var context);

            // Act
            var result = await characterMetadataService.GetCharacterMappingsAsync();

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Null(result.Value);
            Assert.NotNull(result.Error);
            Assert.Equal("No character mappings found.", result.Error);
        }

        [Fact]
        public async Task GetCharacterMappings_ValidRequest_ReturnsSuccess()
        {
            // Arrange
            var characterMetadataService = CreateCharacterMetadataService(out var context, seedCharacterMappings: true);

            // Act
            var result = await characterMetadataService.GetCharacterMappingsAsync();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.IsType<Result<List<CharacterMappings>>>(result);
        }


        //====================================================
        //  GET REALMS TESTS
        //====================================================

        [Fact]
        public async Task GetRealms_NoneInDatabase_ReturnsFailure()
        {
            // Arrange
            var characterMetadataService = CreateCharacterMetadataService(out var context);

            // Act
            var result = await characterMetadataService.GetRealmsAsync();

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Null(result.Value);
            Assert.NotNull(result.Error);
            Assert.Equal("No realms found.", result.Error);
        }

        [Fact]
        public async Task GetRealms_ValidRequest_ReturnsSuccess()
        {
            // Arrange
            var characterMetadataService = CreateCharacterMetadataService(out var context, seedRealms: true);

            // Act
            var result = await characterMetadataService.GetRealmsAsync();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.IsType<Result<List<Realm>>>(result);
        }


        //====================================================
        //  PRIVATE FUNCTIONS
        //====================================================


        /// <summary>
        /// Creates an instance of <see cref="CharacterMetadataService"/> using an in-memory database context.
        /// </summary>
        private static CharacterMetadataService CreateCharacterMetadataService(
            out CharacterManagerDbContext context,
            bool seedFactionTypes = false,
            bool seedRaceTypes = false,
            bool seedClassTypes = false,
            bool seedCharacterMappings = false,
            bool seedRealms = false
            )
        {
            // Create a new in-memory DbContext for isolated testing
            context = GetInMemoryDbContext();

            // Seeds the provided FantasyGameTutorialDbContext with predefined faction, race, class, character mapping, and realm mappings as needed
            if (seedFactionTypes)
                AddFactionTypes(context);
            if (seedRaceTypes)
                AddRaceTypes(context);
            if (seedClassTypes)
                AddClassTypes(context);
            if (seedCharacterMappings)
                AddCharacterMappings(context);
            if (seedRealms)
                AddRealms(context);

            // Return a new instance of the CharacterService using the test context
            return new CharacterMetadataService(context, NullLogger<CharacterMetadataService>.Instance);
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
        /// Seeds the database with predefined faction types for testing.
        /// </summary>
        private async static void AddFactionTypes(CharacterManagerDbContext context)
        {
            context.AddRange(
                new FactionType { Id = 1, Name = "Good Guys" },
                new FactionType { Id = 2, Name = "Bad Guys" }
            );
            await context.SaveChangesAsync();
        }


        /// <summary>
        /// Seeds the database with predefined race types for testing.
        /// </summary>
        private async static void AddRaceTypes(CharacterManagerDbContext context)
        {
            context.AddRange(
                new RaceType { Id = 1, Name = "Human" },
                new RaceType { Id = 2, Name = "Dwarf" },
                new RaceType { Id = 3, Name = "Elf" },
                new RaceType { Id = 4, Name = "Orc" },
                new RaceType { Id = 5, Name = "Zombie" },
                new RaceType { Id = 6, Name = "Alien" }
            );
            await context.SaveChangesAsync();
        }


        /// <summary>
        /// Seeds the database with predefined class types for testing.
        /// </summary>
        private async static void AddClassTypes(CharacterManagerDbContext context)
        {
            context.AddRange(
                new ClassType { Id = 1, Name = "Warrior" },
                new ClassType { Id = 2, Name = "Archer" },
                new ClassType { Id = 3, Name = "Wizard" },
                new ClassType { Id = 4, Name = "Ninja" },
                new ClassType { Id = 5, Name = "Medic" },
                new ClassType { Id = 6, Name = "Paladin" }
            );
            await context.SaveChangesAsync();
        }


        /// <summary>
        /// Seeds the database with predefined character mappings for testing.
        /// </summary>
        private async static void AddCharacterMappings(CharacterManagerDbContext context)
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


        /// <summary>
        /// Seeds the database with predefined realms for testing.
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
    }
}
