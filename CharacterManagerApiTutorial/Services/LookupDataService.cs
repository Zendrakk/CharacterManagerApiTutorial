using CharacterManagerApiTutorial.Data;
using CharacterManagerApiTutorial.Models;
using Microsoft.EntityFrameworkCore;

namespace CharacterManagerApiTutorial.Services
{
    public class LookupDataService(CharacterManagerDbContext context, ILogger<LookupDataService> logger) : ILookupDataService
    {
        private readonly CharacterManagerDbContext _context = context;
        private readonly ILogger<LookupDataService> _logger = logger;


        /// <summary>
        /// Retrieves lookup data for races, classes, factions, and realms.
        /// Queries are run sequentially using the same DbContext instance to avoid threading issues. Data is returned inside a Result wrapper.
        /// </summary>
        public async Task<Result<LookupDataDto>> GetLookupDataAsync()
        {
            try
            {
                // Sequentially run each lookup query in the same DbContext.
                // .AsNoTracking() improves performance since this is read-only data
                var raceTypes = await _context.RaceTypes
                    .AsNoTracking()
                    .ToListAsync();

                var classTypes = await _context.ClassTypes
                    .AsNoTracking()
                    .ToListAsync();

                var factionTypes = await _context.FactionTypes
                    .AsNoTracking()
                    .ToListAsync();

                var realms = await _context.Realms
                    .AsNoTracking()
                    .ToListAsync();

                // Create a new DTO to hold the lookup data results from the database
                var lookupDataResult = new LookupDataDto
                {
                    RaceTypes = raceTypes,
                    ClassTypes = classTypes,
                    FactionTypes = factionTypes,
                    Realms = realms
                };

                // Return success with the lookup data results
                _logger.LogInformation("Successfully retrieved lookup data.");
                return Result<LookupDataDto>.Success(lookupDataResult);
            }
            catch (Exception ex)
            {
                // Catch any exceptions and return failure with the exception message
                _logger.LogError(ex, "Failed to retrieve lookup data.");
                return Result<LookupDataDto>.Failure($"Failed to retrieve lookup data: {ex.Message}");
            }
        }
    }
}
