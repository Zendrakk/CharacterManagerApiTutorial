using System.ComponentModel.DataAnnotations;

namespace CharacterManagerApiTutorial.Models
{
    public class Character
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public int Level { get; set; }
        public int FactionId { get; set; }
        public int RaceId { get; set; }
        public int ClassId { get; set; }
        public int RealmId { get; set; }
        public Guid UserId { get; set; }
    }
}
