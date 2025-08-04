using CharacterManagerApiTutorial.Models;

namespace CharacterManagerApiTutorial.Services
{
    public interface ICharacterMetadataService
    {
        Task<Result<List<FactionType>>> GetFactionTypesAsync();
        Task<Result<List<RaceType>>> GetRaceTypesAsync();
        Task<Result<List<ClassType>>> GetClassTypesAsync();
        Task<Result<List<CharacterMappings>>> GetCharacterMappingsAsync();
        Task<Result<List<Realm>>> GetRealmsAsync();
    }
}
