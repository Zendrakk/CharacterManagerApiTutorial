using CharacterManagerApiTutorial.Data;
using CharacterManagerApiTutorial.Models;
using CharacterManagerApiTutorial.Models.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace CharacterManagerApiTutorial.Services
{
    public class AuthService(CharacterManagerDbContext context, IConfiguration configuration, ILogger<AuthService> logger) : IAuthService
    {
        private readonly CharacterManagerDbContext _context = context;
        private readonly ILogger<AuthService> _logger = logger;


        /// <summary>
        /// Registers a new user account by validating input, checking for duplicates, hashing the password, and saving the user to the database.
        /// </summary>
        public async Task<Result<User>> RegisterAsync(UserDto request)
        {
            // Step 1: Check to see if object is null.
            if (request is null)
                return Result<User>.Failure("Request is null.");

            // Step 2: Normalize strings.
            request.Username = request.Username.Trim().ToLower();

            // Step 3: Check to see if either property is null or white space.
            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
                return Result<User>.Failure("Username and password are required.");

            // Step 4: Validate username length (Min = 3 characters, Max = 20 characters).
            if (request.Username.Length < 3 || request.Username.Length > 20)
            {
                return Result<User>.Failure("Username must be between 3 and 20 characters.");
            }

            // Step 5: Check if a user with the same username already exists in the database
            if (await _context.Users.AnyAsync(u => u.Username == request.Username))
                return Result<User>.Failure("Username already exists.");

            // Step 6: Create User object
            var user = new User();

            // Step 7: Hash the user's password using ASP.NET Core's PasswordHasher which includes salting and a secure hashing algorithm (PBKDF2 by default)
            var hashedPassword = new PasswordHasher<User>()
                .HashPassword(user, request.Password);

            // Step 8: Update properties.
            user.Username = request.Username;
            user.PasswordHash = hashedPassword;

            // Step 9: Persist to DB.
            try
            {
                _logger.LogInformation("Registering user: '{Username}'", user.Username);

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Successfully registered user: '{Username}'", user.Username);
                return Result<User>.Success(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while registering user '{Username}'.", user.Username);
                return Result<User>.Failure($"Failed to register user: {ex.Message}");
            }
        }


        /// <summary>
        /// Authenticates a user based on the provided credentials and returns a token response if successful.
        /// </summary>
        public async Task<Result<TokenResponseDto>> LoginAsync(UserDto request)
        {
            // Step 1: Check to see if object is null.
            if (request is null)
                return Result<TokenResponseDto>.Failure("Request is null.");

            // Step 2: Check to see if either property is null or white space.
            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
                return Result<TokenResponseDto>.Failure("Username and password are required.");

            // Step 3: Normalize strings.
            request.Username = request.Username.Trim().ToLower();

            // Step 4: Try to find the user by username
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);

            // Step 5: If user is not found or the password does not match, return a failure
            if (user is null || new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, request.Password) == PasswordVerificationResult.Failed)
                return Result<TokenResponseDto>.Failure("Invalid username or password.");

            // Step 6: Create a new access token and refresh token for the user
            var tokenResponse = await CreateTokenResponse(user);

            // Step 7: If refresh token saving fails, return an error
            if (tokenResponse is null)
                return Result<TokenResponseDto>.Failure("Issue with saving Refresh Token.");

            // Step 8: Return a successful result with the token response
            _logger.LogInformation("Successfully logged in user: '{Username}'", user.Username);
            return Result<TokenResponseDto>.Success(tokenResponse);
        }


        /// <summary>
        /// Validates the provided refresh token and, if valid, issues a new access and refresh token pair.
        /// </summary>
        public async Task<Result<TokenResponseDto>> RefreshTokensAsync(RefreshTokenRequestDto request)
        {
            // Step 1: Check for null/invalid input before hitting the database
            if (request is null || request.UserId == Guid.Empty || string.IsNullOrWhiteSpace(request.RefreshToken))
                return Result<TokenResponseDto>.Failure("Invalid request.");

            // Step 2: Validate the refresh token and ensure it matches the user and hasn't expired
            var user = await ValidateRefreshTokenAsync(request.UserId, request.RefreshToken);

            // Step 3: If validation fails, return a failure result
            if (user is null)
                return Result<TokenResponseDto>.Failure("Refresh token is invalid or expired.");

            // Step 4: Generate new access token and refresh token for the user
            var tokenResponse = await CreateTokenResponse(user);

            // Step 5: If token creation fails (e.g., refresh token couldn't be saved), return an error
            if (tokenResponse is null)
                return Result<TokenResponseDto>.Failure("Issue with saving Refresh Token.");

            // Step 6: Return a success result with the new token data
            _logger.LogInformation("Successfully refreshed token for user: '{userId}'", user.Id);
            return Result<TokenResponseDto>.Success(tokenResponse);
        }


        /// <summary>
        /// Logs out a user by revoking their refresh token and expiration.
        /// </summary>
        public async Task<Result<bool>> LogoutAsync(LogoutRequest request) 
        {
            // Check if the request or the refresh token is null/empty
            if (request is null || string.IsNullOrWhiteSpace(request.RefreshToken))
                return Result<bool>.Failure("Refresh token required.");

            // Look up the user in the database by the provided refresh token
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.RefreshToken == request.RefreshToken);
            
            // If no user is found with this refresh token, return a failure result
            if (user == null)
                return Result<bool>.Failure("User with this refresh token not found.");

            // Revoke the refresh token by clearing the token and its expiration
            user.RefreshToken = null;
            user.RefreshTokenExpiration = null;

            try
            {
                // Save changes to the database
                await _context.SaveChangesAsync();

                // Log successful logout
                _logger.LogInformation("Successfully logged out user ID: {Id}", user.Id);

                // Return a success result
                return Result<bool>.Success(true);
            }
            catch (Exception ex) 
            {
                // Log any exception that occurs during logout
                _logger.LogError(ex, "Exception occurred while logging out user ID: {Id}", user.Id);

                // Return a failure result containing the exception message
                return Result<bool>.Failure($"Failed to log out user with the following error: {ex.Message}");
            }
        }


        /// <summary>
        /// Creates a <see cref="TokenResponseDto"/> containing both an access token and a refresh token for the specified user.
        /// </summary>
        private async Task<TokenResponseDto?> CreateTokenResponse(User user)
        {
            var tokenResponseDto = new TokenResponseDto
            {
                AccessToken = CreateToken(user),  // Generate a signed JWT with user claims
                RefreshToken = await GenerateAndSaveRefreshTokenAsync(user)  // Generate and persist a secure refresh token
            };

            if (tokenResponseDto.RefreshToken == null)
                return null;

            return tokenResponseDto;
        }


        /// <summary>
        /// Generates a cryptographically secure, random 256-bit refresh token.
        /// The token is returned as a Base64-encoded string for safe storage and transmission.
        /// </summary>
        private string GenerateRefreshToken()
        {
            // Create a 32-byte array (256 bits) to hold the random data
            var randomNumber = new byte[32];

            // Use a cryptographically secure random number generator
            using var rng = RandomNumberGenerator.Create();

            // Fill the array with secure random bytes
            rng.GetBytes(randomNumber);

            // Convert the byte array to a Base64-encoded string for easy storage/transmission
            return Convert.ToBase64String(randomNumber);
        }


        /// <summary>
        /// Validates a refresh token for a specific user.
        /// Checks if the user exists, if the provided refresh token matches the stored one, and if the token has not expired.
        /// </summary>
        private async Task<User?> ValidateRefreshTokenAsync(Guid userId, string refreshToken)
        {
            var user = await _context.Users.FindAsync(userId);

            if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiration <= DateTime.UtcNow)
                return null;

            return user;
        }


        /// <summary>
        /// Generates a new refresh token, assigns it to the user, sets an expiration date,
        /// saves the changes to the database, and returns the token.
        /// </summary>
        private async Task<string?> GenerateAndSaveRefreshTokenAsync(User user)
        {
            var refreshToken = GenerateRefreshToken();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiration = DateTime.UtcNow.AddDays(7);

            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation("Refresh token updated for user: {UserId}", user.Id);
                return refreshToken;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save refresh token for user: {UserId}", user.Id);
                return null;
            }
        }


        /// <summary>
        /// Creates a signed JSON Web Token for the specified user.
        /// The token includes the user's ID, username, and role as claims, and is signed using a symmetric key from configuration.
        /// It is valid for 10 minutes and includes issuer and audience information.
        /// </summary>
        private string CreateToken(User user)
        {
            // Define the claims to include in the JWT
            var claims = new List<Claim>
            {
                new (ClaimTypes.Name, user.Username),
                new (ClaimTypes.NameIdentifier, user.Id.ToString()),
                new (ClaimTypes.Role, user.Role)
            };

            // Create a symmetric security key from the configured secret
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(configuration.GetValue<string>("AppSettings:Token")!));

            // Define the credentials for signing the token
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            // Build the token with issuer, audience, claims, expiration, and signing credentials
            var tokenDescriptor = new JwtSecurityToken(
                issuer: configuration.GetValue<string>("AppSettings:Issuer"),
                audience: configuration.GetValue<string>("AppSettings:Audience"),
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(10),
                signingCredentials: creds
            );

            // Generate the token string and return it
            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }
    }
}
