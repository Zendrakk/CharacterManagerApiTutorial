using System.ComponentModel.DataAnnotations;

namespace CharacterManagerApiTutorial.Models.Auth
{
    public class User
    {
        public Guid Id { get; set; }
        [StringLength(20, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 20 characters.")]
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = "user";
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiration { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
