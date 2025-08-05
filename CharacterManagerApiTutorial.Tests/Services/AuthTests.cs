using CharacterManagerApiTutorial.Data;
using CharacterManagerApiTutorial.Models.Auth;
using CharacterManagerApiTutorial.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System;

namespace CharacterManagerApiTutorial.Tests.Services
{
    public class AuthTests
    {
        //====================================================
        //  REGISTER USER TESTS
        //====================================================

        [Fact]
        public async Task RegisterAsync_NullRequest_ReturnsFailure()
        {
            // Arrange
            var authService = CreateAuthService(out _, out _);

            // Act
            var result = await authService.RegisterAsync(null!);  // Marked as cannot be null to suppress the warning in test results.

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Null(result.Value);
            Assert.Equal("Request is null.", result.Error);
        }


        [Fact]
        public async Task RegisterAsync_EmptyUsername_ReturnsFailure()
        {
            // Arrange
            var authService = CreateAuthService(out _, out _);
            var request = new UserDto { Username = "", Password = "Password1" };

            // Act
            var result = await authService.RegisterAsync(request);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Null(result.Value);
            Assert.Equal("Username and password are required.", result.Error);
        }


        [Fact]
        public async Task RegisterAsync_EmptyPassword_ReturnsFailure()
        {
            // Arrange
            var authService = CreateAuthService(out _, out _);
            var request = new UserDto { Username = "testuser", Password = "" };

            // Act
            var result = await authService.RegisterAsync(request);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Null(result.Value);
            Assert.Equal("Username and password are required.", result.Error);
        }


        [Fact]
        public async Task RegisterAsync_InvalidUsernameLengthMax_ReturnsFailure()
        {
            // Arrange
            var authService = CreateAuthService(out _, out _);
            var request = new UserDto { Username = "UsernameExceeds20Characters", Password = "Password1" };

            // Act
            var result = await authService.RegisterAsync(request);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Null(result.Value);
            Assert.Equal("Username must be between 3 and 20 characters.", result.Error);
        }

        [Fact]
        public async Task RegisterAsync_InvalidUsernameLengthMin_ReturnsFailure()
        {
            // Arrange
            var authService = CreateAuthService(out _, out _);
            var request = new UserDto { Username = "Us", Password = "Password1" };

            // Act
            var result = await authService.RegisterAsync(request);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Null(result.Value);
            Assert.Equal("Username must be between 3 and 20 characters.", result.Error);
        }


        [Fact]
        public async Task RegisterAsync_DuplicateUsername_ReturnsFailure()
        {
            // Arrange
            var authService = CreateAuthService(out var context, out _);
            var request = new UserDto { Username = "TestUser", Password = "Password1" };

            // Act
            var result = await authService.RegisterAsync(request);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);

            var user = await context.Users.FirstOrDefaultAsync(u => u.Username == "testuser");
            Assert.NotNull(user);

            // Act
            var result2 = await authService.RegisterAsync(request);

            // Assert
            Assert.False(result2.IsSuccess);
            Assert.Null(result2.Value);
            Assert.Equal("Username already exists.", result2.Error);
        }


        [Fact]
        public async Task RegisterAsync_ValidRequest_ReturnsSuccess()
        {
            // Arrange
            var authService = CreateAuthService(out var context, out var mockLogger);
            var request = new UserDto { Username = "TestUser", Password = "Password1" };

            // Act
            var result = await authService.RegisterAsync(request);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal("testuser", result.Value.Username);

            var user = await context.Users.FirstOrDefaultAsync(u => u.Username == "testuser");
            Assert.NotNull(user);
            Assert.NotEqual("Password1", user.PasswordHash);  // Password should be hashed!

            // Verify password hash is valid by re-hashing and verifying
            var hasher = new PasswordHasher<User>();
            var verifyResult = hasher.VerifyHashedPassword(user, user.PasswordHash, "Password1");
            Assert.Equal(PasswordVerificationResult.Success, verifyResult);

            mockLogger.Verify(x =>
                x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, _) => v.ToString()!.Equals("Registering user: 'testuser'")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
            mockLogger.Verify(x =>
                x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, _) => v.ToString()!.Equals("Successfully registered user: 'testuser'")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }


        //====================================================
        //  LOGIN USER TESTS
        //====================================================

        [Fact]
        public async Task LoginAsync_NullRequest_ReturnsFailure()
        {
            // Arrange
            var authService = CreateAuthService(out _, out _);

            // Act
            var result = await authService.LoginAsync(null!);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Null(result.Value);
            Assert.Equal("Request is null.", result.Error);
        }


        [Fact]
        public async Task LoginAsync_EmptyUsername_ReturnsFailure()
        {
            // Arrange
            var authService = CreateAuthService(out _, out _);

            var loginRequest = new UserDto
            {
                Username = "",
                Password = "Password1"
            };

            // Act
            var result = await authService.LoginAsync(loginRequest);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Null(result.Value);
            Assert.Equal("Username and password are required.", result.Error);
        }


        [Fact]
        public async Task LoginAsync_EmptyPassword_ReturnsFailure()
        {
            // Arrange
            var authService = CreateAuthService(out _, out _);

            var loginRequest = new UserDto
            {
                Username = "testuser",
                Password = ""
            };

            // Act
            var result = await authService.LoginAsync(loginRequest);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Null(result.Value);
            Assert.Equal("Username and password are required.", result.Error);
        }


        [Fact]
        public async Task LoginAsync_InvalidPassword_ReturnsFailure()
        {
            // Arrange
            var authService = CreateAuthService(out var context, out _);

            var user = new User { Username = "testuser" };
            user.PasswordHash = new PasswordHasher<User>().HashPassword(user, "Password1");
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var loginRequest = new UserDto
            {
                Username = "  TestUser  ", // Intentional spaces and casing to test normalization
                Password = "Password2"
            };

            // Act
            var result = await authService.LoginAsync(loginRequest);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Null(result.Value);
            Assert.Equal("Invalid username or password.", result.Error);
        }


        [Fact]
        public async Task LoginAsync_ValidCredentials_ReturnsSuccess()
        {
            // Arrange
            var authService = CreateAuthService(out var context, out var mockLogger);

            var user = new User { Username = "testuser" };
            user.PasswordHash = new PasswordHasher<User>().HashPassword(user, "Password1");
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var loginRequest = new UserDto
            {
                Username = "  TestUser  ", // Intentional spaces and casing to test normalization
                Password = "Password1"
            };

            // Act
            var result = await authService.LoginAsync(loginRequest);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.False(string.IsNullOrWhiteSpace(result.Value.AccessToken));
            Assert.False(string.IsNullOrWhiteSpace(result.Value.RefreshToken));

            mockLogger.Verify(x =>
                x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, _) => v.ToString()!.Equals("Successfully logged in user: 'testuser'")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }


        //====================================================
        //  REFRESH TOKEN TESTS
        //====================================================

        [Fact]
        public async Task RefreshTokenAsync_NullRequest_ReturnsFailure()
        {
            // Arrange
            var authService = CreateAuthService(out _, out _);

            // Act
            var result = await authService.RefreshTokensAsync(null!);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Null(result.Value);
            Assert.Equal("Invalid request.", result.Error);
        }


        [Fact]
        public async Task RefreshTokenAsync_EmptyUserId_ReturnsFailure()
        {
            // Arrange
            var authService = CreateAuthService(out _, out _);
            var request = new RefreshTokenRequestDto
            {
                UserId = Guid.Empty,
                RefreshToken = "valid-token"
            };

            // Act
            var result = await authService.RefreshTokensAsync(request);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Null(result.Value);
            Assert.Equal("Invalid request.", result.Error);
        }


        [Fact]
        public async Task RefreshTokenAsync_EmptyRefreshToken_ReturnsFailure()
        {
            // Arrange
            var authService = CreateAuthService(out _, out _);
            var request = new RefreshTokenRequestDto
            {
                UserId = Guid.NewGuid(),
                RefreshToken = "    "
            };

            // Act
            var result = await authService.RefreshTokensAsync(request);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Null(result.Value);
            Assert.Equal("Invalid request.", result.Error);
        }


        [Fact]
        public async Task RefreshTokenAsync_InvalidRefreshToken_ReturnsFailure()
        {
            // Arrange
            var authService = CreateAuthService(out _, out _);
            var request = new RefreshTokenRequestDto
            {
                UserId = Guid.NewGuid(),
                RefreshToken = "invalid"
            };

            // Act
            var result = await authService.RefreshTokensAsync(request);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Null(result.Value);
            Assert.Equal("Refresh token is invalid or expired.", result.Error);
        }


        [Fact]
        public async Task RefreshTokenAsync_ValidRefreshToken_ReturnsSuccess()
        {
            // Arrange
            var authService = CreateAuthService(out var context, out var mockLogger);

            var userId = Guid.NewGuid();
            var password = "Password1";
            var refreshToken = Guid.NewGuid().ToString();

            var user = new User
            {
                Id = userId,
                Username = "testuser",
                PasswordHash = new PasswordHasher<User>().HashPassword(null!, password),
                Role = "user",
                RefreshToken = refreshToken,
                RefreshTokenExpiration = DateTime.UtcNow.AddDays(7)
            };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var request = new RefreshTokenRequestDto
            {
                UserId = userId,
                RefreshToken = refreshToken
            };

            // Act
            var result = await authService.RefreshTokensAsync(request);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.False(string.IsNullOrWhiteSpace(result.Value.AccessToken));
            Assert.False(string.IsNullOrWhiteSpace(result.Value.RefreshToken));

            mockLogger.Verify(x =>
                x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, _) => v.ToString()!.Equals("Successfully refreshed token for user: '" + userId.ToString() + "'")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }


        //====================================================
        //  PRIVATE METHODS
        //====================================================


        /// <summary>
        /// Helper method to create a new instance of AuthService for testing purposes.
        /// It initializes a fresh in-memory database context and a mocked configuration, ensuring each test runs in isolation without side effects.
        /// </summary>
        private static AuthService CreateAuthService(out CharacterManagerDbContext context, out Mock<ILogger<AuthService>> mockLogger)
        {
            // Create a new in-memory DbContext for isolated testing
            context = GetInMemoryDbContext();

            // Build an in-memory configuration that includes token settings
            var config = ConfigurationBuilder();

            // Create a mock logger to verify or suppress log output during tests
            mockLogger = new Mock<ILogger<AuthService>>();

            // Return a new instance of the AuthService using the test context and config
            return new AuthService(context, config, mockLogger.Object);
        }


        /// <summary>
        /// Creates a new instance of the in-memory FantasyGameTutorialDbContext for testing purposes.
        /// Each call generates a unique database to ensure test isolation.
        /// </summary>
        private static CharacterManagerDbContext GetInMemoryDbContext()
        {
            // Create a new in-memory database with a unique name for test isolation
            var options = new DbContextOptionsBuilder<CharacterManagerDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())  // unique DB for isolation
                .Options;

            // Return a new DbContext instance using the in-memory options
            return new CharacterManagerDbContext(options);
        }


        /// <summary>
        /// Builds an in-memory IConfiguration instance populated with required JWT settings.
        /// This is used for testing purposes to simulate configuration values without relying on external files.
        /// </summary>
        private static IConfiguration ConfigurationBuilder()
        {
            var inMemorySettings = new Dictionary<string, string>
            {
                // The JWT signing key must be long enough to support the HS512 algorithm (512 bits = 64 bytes)
                {"AppSettings:Token", "your-secret-token-your-secret-token-your-secret-token-your-secret-token"},

                // JWT issuer and audience values used to validate the token
                {"AppSettings:Issuer", "your-issuer"},
                {"AppSettings:Audience", "your-audience"}
            };

            // Build the configuration using the in-memory collection
            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings!)
                .Build();

            return configuration;
        }
    }
}
