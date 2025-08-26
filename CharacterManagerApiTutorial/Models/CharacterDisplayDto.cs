using System.ComponentModel.DataAnnotations;

namespace CharacterManagerApiTutorial.Models
{
    public class CharacterDisplayDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
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
