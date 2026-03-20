using Agriculture_Analyst.Models;
using Agriculture_Analyst.Models.ViewModel;

namespace Agriculture_Analyst.Repositories.Interfaces
{
    public interface IPlantRepository
    {
        Task<List<Plant>> GetByUserIdAsync(int userId);
        Task<Plant?> GetByIdAsync(int id);
        Task AddAsync(Plant plant);
        Task UpdateAsync(Plant plant);
        Task DeleteAsync(Plant plant);

        PlantReportViewModel GetPlantReport(int plantId);
    }
}
