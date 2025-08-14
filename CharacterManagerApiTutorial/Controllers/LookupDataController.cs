using CharacterManagerApiTutorial.Models;
using CharacterManagerApiTutorial.Services;
using Microsoft.AspNetCore.Mvc;

namespace CharacterManagerApiTutorial.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LookupDataController(ILookupDataService lookupDataService, ILogger<LookupDataService> logger) : ControllerBase
    {
        private readonly ILookupDataService _lookupDataService = lookupDataService;
        private readonly ILogger<LookupDataService> _logger = logger;


        [HttpGet]
        public async Task<ActionResult<LookupDataDto>> GetLookupData()
        {
            _logger.LogInformation("Retrieving lookup data from database...");

            var result = await _lookupDataService.GetLookupDataAsync();

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Service failed to retrieve lookup data from the database with the following error: {Error}", result.Error);
                return NotFound(result.Error);
            }

            return Ok(result.Value);
        }
    }
}
