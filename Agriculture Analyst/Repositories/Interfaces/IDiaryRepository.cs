using Agriculture_Analyst.Models;

namespace Agriculture_Analyst.Repositories.Interfaces
{
    public interface IDiaryRepository
    {
        
        Task AddAsync(DiaryEntry diary);

        Task<List<DiaryEntry>> GetByPlantAndDateAsync(
            int plantId,
            DateTime date);
    }

}
