using System.ComponentModel.DataAnnotations;

namespace CharacterManagerApiTutorial.Models
{
    public class CharacterDisplayDto
    {
        public int Id { get; set; }
        [StringLength(15, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 15 characters.")]
        public string Name { get; set; } = string.Empty;
        [Range(1, 50, ErrorMessage = "Level must be between 1 and 50.")]
        public int Level { get; set; }
        public int RealmId { get; set; }
        public string RealmName { get; set; } = string.Empty;
        public int RaceId { get; set; }
        public string RaceName { get; set; } = string.Empty;
        public int ClassId { get; set; }
        public string ClassName { get; set; } = string.Empty;
        public int FactionId { get; set; }
        public string FactionName { get; set; } = string.Empty;
    }
}
