using Agriculture_Analyst.Models;
using Agriculture_Analyst.Models.ViewModel;

namespace Agriculture_Analyst.Services.Interfaces
{
    public interface IPlantTaskService
    {
        Task<List<PlantTask>> GetTasksByDateAsync(
            int plantId,
            int userId,
            DateTime date);

        Task CreateAsync(
            CreatePlantTaskViewModel model,
            int userId);

        Task CompleteTaskAsync(
            int taskId,
            int userId);

        Task SoftDeleteAsync(
            int taskId,
            int userId);
    }

}
