using Agriculture_Analyst.Models;
using Agriculture_Analyst.Repositories.Interfaces;

namespace Agriculture_Analyst.Services.Interfaces
{
    public interface IPlantService
    {
        Task<List<Plant>> GetUserPlantsAsync(int userId);
        Task<Plant?> GetByIdAsync(int id, int userId);
        Task CreateAsync(Plant plant, int userId);
        Task UpdateAsync(Plant plant, int userId);
        Task DeleteAsync(int plantId, int userId);
    }
}
