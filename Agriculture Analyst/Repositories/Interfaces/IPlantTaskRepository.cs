using Agriculture_Analyst.Models;

namespace Agriculture_Analyst.Repositories.Interfaces
{
    public interface IPlantTaskRepository
    {
        Task<List<PlantTask>> GetByPlantAsync(int plantId, int userId, DateTime date);
        Task<PlantTask?> GetByIdAsync(int taskId);
        Task AddAsync(PlantTask task);
        Task UpdateAsync(PlantTask task);
    }
}
