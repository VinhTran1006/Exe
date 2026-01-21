using Agriculture_Analyst.Models;

namespace Agriculture_Analyst.Repositories.Interfaces
{
    public interface IPlantRepository
    {
        Task<List<Plant>> GetByUserIdAsync(int userId);
        Task<Plant?> GetByIdAsync(int id);
        Task AddAsync(Plant plant);
        Task UpdateAsync(Plant plant);
        Task DeleteAsync(Plant plant);
    }
}
