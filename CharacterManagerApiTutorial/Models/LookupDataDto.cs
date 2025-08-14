namespace CharacterManagerApiTutorial.Models
{
    public class LookupDataDto
    {
        public List<RaceType> RaceTypes { get; set; } = [];
        public List<ClassType> ClassTypes { get; set; } = [];
        public List<FactionType> FactionTypes { get; set; } = [];
        public List<Realm> Realms { get; set; } = [];
    }
}
