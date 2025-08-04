using CharacterManagerApiTutorial.Models;
using CharacterManagerApiTutorial.Models.Auth;

namespace CharacterManagerApiTutorial.Services
{
    public interface IAuthService
    {
        Task<Result<User>> RegisterAsync(UserDto user);
        Task<Result<TokenResponseDto>> LoginAsync(UserDto user);
        Task<Result<TokenResponseDto>> RefreshTokensAsync(RefreshTokenRequestDto request);
    }
}
