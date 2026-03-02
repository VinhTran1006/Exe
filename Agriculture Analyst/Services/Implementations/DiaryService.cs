using Agriculture_Analyst.Models;
using Agriculture_Analyst.Repositories.Implementations;
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
        public async Task<List<CalendarDayDto>>
    GetCalendarAsync(int plantId, int month, int year)
        {
            var diaries =
                await _repo
                .GetByPlantAndMonthAsync(plantId, month, year);

            return diaries
                .Where(d => d.EntryDate.HasValue)
                .GroupBy(d => d.EntryDate.Value.Day)
                .Select(g => new CalendarDayDto
                {
                    Day = g.Key
                })
                .ToList();
        }

        public async Task<DiarySummaryDto>
            GetSummaryAsync(int plantId,
            int month,
            int year)
        {
            var diaries =
                await _repo.GetByPlantAndMonthAsync(
                    plantId, month, year);

            var workingDays =
                diaries.Select(d => d.EntryDate!.Value.Date)
                       .Distinct()
                       .Count();

            var weather =
                diaries.GroupBy(d => d.Weather)
                       .OrderByDescending(g => g.Count())
                       .FirstOrDefault()?.Key;

            return new DiarySummaryDto
            {
                TotalEntries = diaries.Count,
                WorkingDays = workingDays,
                MostWeather = weather
            };
        }
    }

}
