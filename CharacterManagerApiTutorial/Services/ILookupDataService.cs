using CharacterManagerApiTutorial.Models;

namespace CharacterManagerApiTutorial.Services
{
    public interface ILookupDataService
    {
        Task<Result<LookupDataDto>> GetLookupDataAsync();
    }
}
