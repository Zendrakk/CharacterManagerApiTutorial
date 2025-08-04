namespace CharacterManagerApiTutorial.Models.Auth
{
    public class TokenResponseDto
    {
        public required string AccessToken { get; set; }
        public string? RefreshToken { get; set; }
    }
}
