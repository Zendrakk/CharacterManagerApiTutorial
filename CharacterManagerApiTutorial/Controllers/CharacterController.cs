using CharacterManagerApiTutorial.Models;
using CharacterManagerApiTutorial.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CharacterManagerApiTutorial.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Roles = "user")]
    [ApiController]
    public class CharacterController(ICharacterService characterService, ILogger<AuthService> logger) : ControllerBase
    {
        private readonly ICharacterService _characterService = characterService;
        private readonly ILogger<AuthService> _logger = logger;


        // GET: api/character
        [HttpGet]
        public async Task<IActionResult> GetCharacters()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null || !Guid.TryParse(userId, out var userGuid) || userGuid == Guid.Empty)
                return Unauthorized();

            _logger.LogInformation("GetCharacters attempt for user ID: {UserGuid}", userGuid);

            var result = await _characterService.GetCharactersAsync(userGuid);
            if (!result.IsSuccess)
            {
                _logger.LogWarning("Service failed to get all characters with the following error: {Error}", result.Error);
                return NotFound(result.Error);
            }

            return Ok(result.Value);
        }

        // GET: api/character/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetCharacterById(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null || !Guid.TryParse(userId, out var userGuid) || userGuid == Guid.Empty)
                return Unauthorized();

            _logger.LogInformation("GetCharacterById attempt for user ID: {UserGuid}", userGuid);

            var result = await _characterService.GetCharacterByIdAsync(id, userGuid);
            if (!result.IsSuccess)
            {
                _logger.LogWarning("Service failed to get character by ID with the following error: {Error}", result.Error);
                return NotFound(result.Error);
            }

            return Ok(result.Value);
        }

        // POST: api/character
        [HttpPost]
        public async Task<IActionResult> CreateCharacter([FromBody] Character newCharacter)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null || !Guid.TryParse(userId, out var userGuid) || userGuid == Guid.Empty)
                return Unauthorized();

            _logger.LogInformation("CreateCharacter attempt for user ID: {UserGuid}", userGuid);

            var result = await _characterService.CreateCharacterAsync(newCharacter, userGuid);
            if (!result.IsSuccess)
            {
                _logger.LogWarning("Service failed to create character with the following error: {Error}", result.Error);
                return BadRequest(result.Error);
            }

            return CreatedAtAction(nameof(GetCharacterById), new { id = result.Value!.Id }, result.Value);
        }

        // PUT: api/character/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateCharacter(int id, [FromBody] Character updatedCharacter)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null || !Guid.TryParse(userId, out var userGuid) || userGuid == Guid.Empty)
                return Unauthorized();

            _logger.LogInformation("UpdateCharacter attempt for user ID: {UserGuid}", userGuid);

            var result = await _characterService.UpdateCharacterAsync(id, updatedCharacter, userGuid);
            if (!result.IsSuccess)
            {
                _logger.LogWarning("Service failed to update character {id} with the following error: {Error}", id, result.Error);
                return BadRequest(result.Error);
            }

            return NoContent(); // or Ok() if you prefer returning a status
        }

        // DELETE: api/character/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null || !Guid.TryParse(userId, out var userGuid) || userGuid == Guid.Empty)
                return Unauthorized();

            _logger.LogInformation("DeleteCharacter attempt for user ID: {UserGuid}", userGuid);

            var result = await _characterService.DeleteCharacterAsync(id, userGuid);
            if (!result.IsSuccess)
            {
                _logger.LogWarning("Service failed to delete character {id} with the following error: {Error}", id, result.Error);
                return NotFound(result.Error);
            }

            return Ok(new { deletedId = result.Value });
        }
    }
}
