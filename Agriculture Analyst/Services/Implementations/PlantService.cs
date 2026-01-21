using Agriculture_Analyst.Models;
using Agriculture_Analyst.Repositories.Interfaces;
using Agriculture_Analyst.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace Agriculture_Analyst.Services.Implementations
{
    public class PlantService : IPlantService
    {
        private readonly IPlantRepository _repo;

        public PlantService(IPlantRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<Plant>> GetUserPlantsAsync(int userId)
        {
            return await _repo.GetByUserIdAsync(userId);
        }

        public async Task<Plant?> GetByIdAsync(int id, int userId)
        {
            var plant = await _repo.GetByIdAsync(id);
            if (plant == null || plant.UserId != userId)
                return null;

            return plant;
        }

        public async Task CreateAsync(Plant plant, int userId)
        {
            plant.UserId = userId;
            plant.Status ??= "Đang trồng";
            await _repo.AddAsync(plant);
        }

        public async Task UpdateAsync(Plant plant, int userId)
        {
            var existing = await _repo.GetByIdAsync(plant.PlantId);
            if (existing == null || existing.UserId != userId)
                throw new UnauthorizedAccessException();

            existing.PlantName = plant.PlantName;
            existing.PlantType = plant.PlantType;
            existing.Area = plant.Area;
            existing.StartDate = plant.StartDate;
            existing.Status = plant.Status;

            await _repo.UpdateAsync(existing);
        }

        public async Task DeleteAsync(int plantId, int userId)
        {
            var plant = await _repo.GetByIdAsync(plantId);
            if (plant == null || plant.UserId != userId)
                throw new UnauthorizedAccessException();

            await _repo.DeleteAsync(plant);
        }
    }



}
