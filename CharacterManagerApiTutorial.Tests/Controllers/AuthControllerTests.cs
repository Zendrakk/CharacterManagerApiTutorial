using CharacterManagerApiTutorial.Controllers;
using CharacterManagerApiTutorial.Models;
using CharacterManagerApiTutorial.Models.Auth;
using CharacterManagerApiTutorial.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace CharacterManagerApiTutorial.Tests.Controllers
{
    public class AuthControllerTests
    {
        //====================================================
        //  REGISTER USER TESTS
        //====================================================

        [Fact]
        public async Task Register_ReturnsBadRequest_WhenRegistrationFails()
        {
            // Arrange
            var userDto = new UserDto { Username = "testuser", Password = "Password1" };

            var mockAuthService = new Mock<IAuthService>();
            mockAuthService
                .Setup(s => s.RegisterAsync(userDto))
                .ReturnsAsync(Result<User>.Failure("Username already exists."));

            var mockLogger = new Mock<ILogger<AuthService>>();
            var controller = new AuthController(mockAuthService.Object, mockLogger.Object);

            // Act
            var result = await controller.Register(userDto);

            // Assert
            var actionResult = Assert.IsType<ActionResult<User>>(result);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
            var errorMessage = Assert.IsType<string>(badRequestResult.Value);

            Assert.Equal("Username already exists.", errorMessage);

            mockLogger.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, _) => v.ToString()!.Equals("Service failed to register user with error: Username already exists.")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task Register_ReturnsOk_WhenSuccessful()
        {
            // Arrange
            var userDto = new UserDto { Username = "testuser", Password = "Password1" };
            var expectedUser = new User 
            { 
                Id = Guid.NewGuid(), 
                Username = "testuser",
                PasswordHash = "Hashedpw",
                Role = "user",
                CreatedAt = DateTime.UtcNow,
            };

            var mockAuthService = new Mock<IAuthService>();
            mockAuthService
                .Setup(s => s.RegisterAsync(userDto))
                .ReturnsAsync(Result<User>.Success(expectedUser));

            var mockLogger = new Mock<ILogger<AuthService>>();
            var controller = new AuthController(mockAuthService.Object, mockLogger.Object);

            // Act
            var result = await controller.Register(userDto);

            // Assert
            var actionResult = Assert.IsType<ActionResult<User>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var actualUser = Assert.IsType<User>(okResult.Value);

            Assert.Equal(expectedUser.Id, actualUser.Id);
            Assert.Equal(expectedUser.Username, actualUser.Username);
            Assert.Equal(expectedUser.PasswordHash, actualUser.PasswordHash);
            Assert.Equal(expectedUser.Role, actualUser.Role);
            Assert.Equal(expectedUser.RefreshToken, actualUser.RefreshToken);
            Assert.Equal(expectedUser.RefreshTokenExpiration, actualUser.RefreshTokenExpiration);
            Assert.Equal(expectedUser.CreatedAt, actualUser.CreatedAt, precision: TimeSpan.FromSeconds(1));

            mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, _) => v.ToString()!.Equals("Register attempt for user: " + userDto.Username + "")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }


        //====================================================
        //  LOGIN USER TESTS
        //====================================================

        [Fact]
        public async Task Login_ReturnsBadRequest_WhenLoginFails()
        {
            // Arrange
            var userDto = new UserDto { Username = "testuser", Password = "wrongpassword" };
            var errorMessage = "Invalid username or password.";

            var mockAuthService = new Mock<IAuthService>();
            mockAuthService
                .Setup(s => s.LoginAsync(userDto))
                .ReturnsAsync(Result<TokenResponseDto>.Failure(errorMessage));

            var mockLogger = new Mock<ILogger<AuthService>>();
            var controller = new AuthController(mockAuthService.Object, mockLogger.Object);

            // Act
            var result = await controller.Login(userDto);

            // Assert
            var actionResult = Assert.IsType<ActionResult<TokenResponseDto>>(result);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
            var returnedError = Assert.IsType<string>(badRequestResult.Value);

            Assert.Equal(errorMessage, returnedError);

            mockLogger.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, _) => v.ToString()!.Equals("Service failed to log in user: " + errorMessage + "")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task Login_ReturnsOk_WhenSuccessful()
        {
            // Arrange
            var userDto = new UserDto { Username = "testuser", Password = "Password1" };
            var expectedToken = new TokenResponseDto
            {
                AccessToken = "fake-jwt-token",
                RefreshToken = "fake-refresh-token"
            };

            var mockAuthService = new Mock<IAuthService>();
            mockAuthService
                .Setup(s => s.LoginAsync(userDto))
                .ReturnsAsync(Result<TokenResponseDto>.Success(expectedToken));

            var mockLogger = new Mock<ILogger<AuthService>>();
            var controller = new AuthController(mockAuthService.Object, mockLogger.Object);

            // Act
            var result = await controller.Login(userDto);

            // Assert
            var actionResult = Assert.IsType<ActionResult<TokenResponseDto>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var actualToken = Assert.IsType<TokenResponseDto>(okResult.Value);

            Assert.Equal(expectedToken.AccessToken, actualToken.AccessToken);
            Assert.Equal(expectedToken.RefreshToken, actualToken.RefreshToken);

            mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, _) => v.ToString()!.Equals("Login attempt for user: " + userDto.Username + "")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }


        //====================================================
        //  REFRESH TOKEN TESTS
        //====================================================

        [Fact]
        public async Task RefreshTokenAsync_ReturnsUnauthorized_WhenRefreshFails()
        {
            // Arrange
            var requestDto = new RefreshTokenRequestDto
            {
                UserId = Guid.NewGuid(),
                RefreshToken = "expired-or-invalid-token"
            };

            var errorMessage = "Refresh token is invalid or expired.";

            var mockAuthService = new Mock<IAuthService>();
            mockAuthService
                .Setup(s => s.RefreshTokensAsync(requestDto))
                .ReturnsAsync(Result<TokenResponseDto>.Failure(errorMessage));

            var mockLogger = new Mock<ILogger<AuthService>>();
            var controller = new AuthController(mockAuthService.Object, mockLogger.Object);

            // Act
            var result = await controller.RefreshTokenAsync(requestDto);

            // Assert
            var actionResult = Assert.IsType<ActionResult<TokenResponseDto>>(result);
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(actionResult.Result);
            var returnedError = Assert.IsType<string>(unauthorizedResult.Value);

            Assert.Equal(errorMessage, returnedError);

            mockLogger.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, _) => v.ToString()!.Equals("Service failed to refresh token for user " + requestDto.UserId + ": " + errorMessage + "")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task RefreshTokenAsync_ReturnsOk_WhenRefreshIsSuccessful()
        {
            // Arrange
            var requestDto = new RefreshTokenRequestDto
            {
                UserId = Guid.NewGuid(),
                RefreshToken = "valid-refresh-token"
            };

            var expectedToken = new TokenResponseDto
            {
                AccessToken = "new-access-token",
                RefreshToken = "new-refresh-token"
            };

            var mockAuthService = new Mock<IAuthService>();
            mockAuthService
                .Setup(s => s.RefreshTokensAsync(requestDto))
                .ReturnsAsync(Result<TokenResponseDto>.Success(expectedToken));

            var mockLogger = new Mock<ILogger<AuthService>>();
            var controller = new AuthController(mockAuthService.Object, mockLogger.Object);

            // Act
            var result = await controller.RefreshTokenAsync(requestDto);

            // Assert
            var actionResult = Assert.IsType<ActionResult<TokenResponseDto>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var actualToken = Assert.IsType<TokenResponseDto>(okResult.Value);

            Assert.Equal(expectedToken.AccessToken, actualToken.AccessToken);
            Assert.Equal(expectedToken.RefreshToken, actualToken.RefreshToken);

            mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains($"Refresh token attempt for user: " + requestDto.UserId + "")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }
    }
}
