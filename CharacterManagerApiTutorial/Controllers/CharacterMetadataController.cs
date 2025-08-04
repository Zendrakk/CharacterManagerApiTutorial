using CharacterManagerApiTutorial.Services;
using Microsoft.AspNetCore.Mvc;

namespace CharacterManagerApiTutorial.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CharacterMetadataController(ICharacterMetadataService characterMetadataService) : ControllerBase
    {
        private readonly ICharacterMetadataService _characterMetadataService = characterMetadataService;

        [HttpGet("factiontypes")]
        public async Task<IActionResult> GetFactionTypes()
        {
            var result = await _characterMetadataService.GetFactionTypesAsync();

            if (!result.IsSuccess)
                return NotFound(result.Error);

            return Ok(result.Value);
        }

        [HttpGet("racetypes")]
        public async Task<IActionResult> GetRaceTypes()
        {
            var result = await _characterMetadataService.GetRaceTypesAsync();

            if (!result.IsSuccess)
                return NotFound(result.Error);

            return Ok(result.Value);
        }

        [HttpGet("classtypes")]
        public async Task<IActionResult> GetClassTypes()
        {
            var result = await _characterMetadataService.GetClassTypesAsync();

            if (!result.IsSuccess)
                return NotFound(result.Error);

            return Ok(result.Value);
        }

        [HttpGet("charactermappings")]
        public async Task<IActionResult> GetCharacterMappings()
        {
            var result = await _characterMetadataService.GetCharacterMappingsAsync();

            if (!result.IsSuccess)
                return NotFound(result.Error);

            return Ok(result.Value);
        }

        [HttpGet("realms")]
        public async Task<IActionResult> GetRealms()
        {
            var result = await _characterMetadataService.GetRealmsAsync();

            if (!result.IsSuccess)
                return NotFound(result.Error);

            return Ok(result.Value);
        }
    }
}
