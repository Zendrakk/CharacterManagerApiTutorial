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
    public class CharacterController(ICharacterService characterService) : ControllerBase
    {
        private readonly ICharacterService _characterService = characterService;

        // GET: api/character
        [HttpGet]
        public async Task<IActionResult> GetCharacters()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null || !Guid.TryParse(userId, out var userGuid) || userGuid == Guid.Empty)
                return Unauthorized();

            var result = await _characterService.GetCharactersAsync(userGuid);
            if (!result.IsSuccess)
                return NotFound(result.Error);

            return Ok(result.Value);
        }

        // GET: api/character/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetCharacterById(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null || !Guid.TryParse(userId, out var userGuid) || userGuid == Guid.Empty)
                return Unauthorized();

            var result = await _characterService.GetCharacterByIdAsync(id, userGuid);
            if (!result.IsSuccess)
                return NotFound(result.Error);

            return Ok(result.Value);
        }

        // POST: api/character
        [HttpPost]
        public async Task<IActionResult> CreateCharacter([FromBody] Character newCharacter)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null || !Guid.TryParse(userId, out var userGuid) || userGuid == Guid.Empty)
                return Unauthorized();

            var result = await _characterService.CreateCharacterAsync(newCharacter, userGuid);
            if (!result.IsSuccess)
                return BadRequest(result.Error);

            if (result.IsSuccess && result.Value != null)
            {
                return CreatedAtAction(nameof(GetCharacterById), new { id = result.Value.Id }, result.Value);
            }
            else
            {
                return BadRequest(result.Error);
            }
        }

        // PUT: api/character/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateCharacter(int id, [FromBody] Character updatedCharacter)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null || !Guid.TryParse(userId, out var userGuid) || userGuid == Guid.Empty)
                return Unauthorized();

            var result = await _characterService.UpdateCharacterAsync(id, updatedCharacter, userGuid);
            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return NoContent(); // or Ok() if you prefer returning a status
        }

        // DELETE: api/character/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null || !Guid.TryParse(userId, out var userGuid) || userGuid == Guid.Empty)
                return Unauthorized();

            var result = await _characterService.DeleteCharacterAsync(id, userGuid);
            if (!result.IsSuccess)
                return NotFound(result.Error);

            return Ok(new { deletedId = result.Value });
        }
    }
}
