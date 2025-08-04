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
    }
}
