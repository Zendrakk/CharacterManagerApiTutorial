using CharacterManagerApiTutorial.Models.Auth;
using CharacterManagerApiTutorial.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace CharacterManagerApiTutorial.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService, ILogger<AuthService> logger) : ControllerBase
    {
        private readonly IAuthService _authService = authService;
        private readonly ILogger<AuthService> _logger = logger;


        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(UserDto request)
        {
            _logger.LogInformation("Register attempt for user: {Username}", request.Username);

            var result = await _authService.RegisterAsync(request);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Service failed to register user with error: {Error}", result.Error);
                return BadRequest(result.Error);
            }

            return Ok(result.Value);
        }

        [HttpPost("login")]
        public async Task<ActionResult<TokenResponseDto>> Login(UserDto request)
        {
            _logger.LogInformation("Login attempt for user: {Username}", request.Username);

            var result = await _authService.LoginAsync(request);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Service failed to log in user: {Error}", result.Error);
                return BadRequest(result.Error);
            }

            return Ok(result.Value);
        }

        [EnableRateLimiting("Fixed")]
        [HttpPost("refresh-token")]
        public async Task<ActionResult<TokenResponseDto>> RefreshTokenAsync(RefreshTokenRequestDto request)
        {
            _logger.LogInformation("Refresh token attempt for user: {UserId}", request.UserId);

            var result = await _authService.RefreshTokensAsync(request);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Service failed to refresh token for user {userid}: {Error}", request.UserId, result.Error);
                return Unauthorized(result.Error);
            }

            return Ok(result.Value);
        }
    }
}
