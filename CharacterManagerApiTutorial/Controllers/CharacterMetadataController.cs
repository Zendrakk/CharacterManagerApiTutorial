using CharacterManagerApiTutorial.Services;
using Microsoft.AspNetCore.Mvc;

namespace CharacterManagerApiTutorial.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CharacterMetadataController(ICharacterMetadataService characterMetadataService, ILogger<AuthService> logger) : ControllerBase
    {
        private readonly ICharacterMetadataService _characterMetadataService = characterMetadataService;
        private readonly ILogger<AuthService> _logger = logger;


        [HttpGet("factiontypes")]
        public async Task<IActionResult> GetFactionTypes()
        {
            _logger.LogInformation("Retrieving faction types from database...");

            var result = await _characterMetadataService.GetFactionTypesAsync();

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Service failed to retrieve faction types from the database with the following error: {Error}", result.Error);
                return NotFound(result.Error);
            }

            return Ok(result.Value);
        }

        [HttpGet("racetypes")]
        public async Task<IActionResult> GetRaceTypes()
        {
            _logger.LogInformation("Retrieving race types from database...");

            var result = await _characterMetadataService.GetRaceTypesAsync();

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Service failed to retrieve race types from the database with the following error: {Error}", result.Error);
                return NotFound(result.Error);
            }

            return Ok(result.Value);
        }

        [HttpGet("classtypes")]
        public async Task<IActionResult> GetClassTypes()
        {
            _logger.LogInformation("Retrieving class types from database...");

            var result = await _characterMetadataService.GetClassTypesAsync();

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Service failed to retrieve class types from the database with the following error: {Error}", result.Error);
                return NotFound(result.Error);
            }

            return Ok(result.Value);
        }

        [HttpGet("charactermappings")]
        public async Task<IActionResult> GetCharacterMappings()
        {
            _logger.LogInformation("Retrieving character mappings from database...");

            var result = await _characterMetadataService.GetCharacterMappingsAsync();

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Service failed to retrieve character mappings from the database with the following error: {Error}", result.Error);
                return NotFound(result.Error);
            }

            return Ok(result.Value);
        }

        [HttpGet("realms")]
        public async Task<IActionResult> GetRealms()
        {
            _logger.LogInformation("Retrieving realms from database...");

            var result = await _characterMetadataService.GetRealmsAsync();

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Service failed to retrieve realms from the database with the following error: {Error}", result.Error);
                return NotFound(result.Error);
            }

            return Ok(result.Value);
        }
    }
}
