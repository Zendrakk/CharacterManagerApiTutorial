using System.ComponentModel.DataAnnotations;

namespace CharacterManagerApiTutorial.Models
{
    public class CharacterDto
    {
        public int Id { get; set; }
        [Required]
        [RegularExpression("^[A-Za-z]+$", ErrorMessage = "Name can only contain letters.")]
        [StringLength(15, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 15 characters.")]
        public required string Name { get; set; }
        [Range(1, 50, ErrorMessage = "Level must be between 1 and 50.")]
        public int Level { get; set; }
        public int FactionId { get; set; }
        public int RaceId { get; set; }
        public int ClassId { get; set; }
        public int RealmId { get; set; }
    }
}
