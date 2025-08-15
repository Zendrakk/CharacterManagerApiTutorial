using CharacterManagerApiTutorial.Models;

namespace CharacterManagerApiTutorial.Services
{
    public interface ICharacterService
    {
        Task<Result<List<CharacterDto>>> GetCharactersAsync(Guid userGuid);
        Task<Result<Character>> GetCharacterByIdAsync(int id, Guid userGuid);
        Task<Result<Character>> CreateCharacterAsync(Character newCharacter, Guid userGuid);
        Task<Result> UpdateCharacterAsync(int id, Character updatedCharacter, Guid userGuid);
        Task<Result<int>> DeleteCharacterAsync(int id, Guid userGuid);
    }
}
