using CharacterManagerApiTutorial.Models;

namespace CharacterManagerApiTutorial.Services
{
    public interface ICharacterService
    {
        Task<Result<List<CharacterDisplayDto>>> GetCharactersAsync(Guid userGuid);
        Task<Result<Character>> GetCharacterByIdAsync(int id, Guid userGuid);
        Task<Result<Character>> CreateCharacterAsync(CharacterDto newCharacterDto, Guid userGuid);
        Task<Result> UpdateCharacterAsync(int id, CharacterDto updatedCharacterDto, Guid userGuid);
        Task<Result<int>> DeleteCharacterAsync(int id, Guid userGuid);
    }
}
