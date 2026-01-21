using Agriculture_Analyst.Models;
using Agriculture_Analyst.Repositories.Interfaces;
using Agriculture_Analyst.Services.Interfaces;

namespace Agriculture_Analyst.Services.Implementations
{
    public class DiaryService : IDiaryService
    {
        private readonly IDiaryRepository _repo;

        public DiaryService(IDiaryRepository repo)
        {
            _repo = repo;
        }

        // =========================
        // LẤY DIARY THEO NGÀY
        // =========================
        public async Task<List<DiaryEntry>> GetByPlantAndDateAsync(
            int plantId,
            DateTime date)
        {
            return await _repo.GetByPlantAndDateAsync(plantId, date);
        }

        // =========================
        // TẠO DIARY TỪ TASK
        // =========================
        public async Task CreateFromTaskAsync(PlantTask task)
        {
            var diary = new DiaryEntry
            {
                PlantId = task.PlantId,
                Activity = task.Title,
                Description = task.Note,
                EntryDate = DateTime.Now
            };

            await _repo.AddAsync(diary);
        }
    }

}
