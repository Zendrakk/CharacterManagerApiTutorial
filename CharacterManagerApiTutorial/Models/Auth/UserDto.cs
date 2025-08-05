using System.ComponentModel.DataAnnotations;

namespace CharacterManagerApiTutorial.Models.Auth
{
    public class UserDto
    {
        [StringLength(20, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 20 characters.")]
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
