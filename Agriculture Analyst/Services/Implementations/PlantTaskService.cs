using Agriculture_Analyst.Models;
using Agriculture_Analyst.Models.ViewModel;
using Agriculture_Analyst.Repositories.Interfaces;
using Agriculture_Analyst.Services.Interfaces;

namespace Agriculture_Analyst.Services.Implementations
{
    public class PlantTaskService : IPlantTaskService
    {
        private readonly IPlantTaskRepository _taskRepo;
        private readonly IDiaryRepository _diaryRepo;

        public PlantTaskService(
            IPlantTaskRepository taskRepo,
            IDiaryRepository diaryRepo)
        {
            _taskRepo = taskRepo;
            _diaryRepo = diaryRepo;
        }

        // =========================
        // GET TASK THEO NGÀY
        // =========================
        public async Task<List<PlantTask>> GetTasksByDateAsync(
            int plantId,
            int userId,
            DateTime date)
        {
            return await _taskRepo.GetByPlantAsync(plantId, userId, date);
        }

        // =========================
        // CREATE TASK
        // =========================
        public async Task CreateAsync(
            CreatePlantTaskViewModel model,
            int userId)
        {
            var task = new PlantTask
            {
                PlantId = model.PlantId,
                UserId = userId,
                Title = model.Title,
                Note = model.Note,
                CreatedDate = DateTime.Now,
                NextDate = model.NextDate,
                IsRecurring = model.IsRecurring,
                RepeatIntervalDays = model.RepeatIntervalDays,
                IsCompleted = false,
                IsDeleted = false
            };

            await _taskRepo.AddAsync(task);
        }

        // =========================
        // COMPLETE TASK
        // =========================
        public async Task CompleteTaskAsync(int taskId, int userId)
        {
            var task = await _taskRepo.GetByIdAsync(taskId);

            if (task == null || task.UserId != userId)
                throw new UnauthorizedAccessException();

            // 1️⃣ AUTO CREATE DIARY
            var diary = new DiaryEntry
            {
                PlantId = task.PlantId,
                Activity = task.Title,
                Description = task.Note,
                EntryDate = DateTime.Now
            };

            await _diaryRepo.AddAsync(diary);

            // 2️⃣ UPDATE TASK
            if (task.IsRecurring && task.RepeatIntervalDays.HasValue)
            {
                task.NextDate = task.NextDate.AddDays(task.RepeatIntervalDays.Value);
            }
            else
            {
                task.IsCompleted = true;
            }

            await _taskRepo.UpdateAsync(task);
        }

        // =========================
        // SOFT DELETE
        // =========================
        public async Task SoftDeleteAsync(int taskId, int userId)
        {
            var task = await _taskRepo.GetByIdAsync(taskId);

            if (task == null || task.UserId != userId)
                throw new UnauthorizedAccessException();

            task.IsDeleted = true;
            task.DeletedAt = DateTime.Now;

            await _taskRepo.UpdateAsync(task);
        }
    }

}
