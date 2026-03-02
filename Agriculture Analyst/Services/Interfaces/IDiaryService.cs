using Agriculture_Analyst.Models;

namespace Agriculture_Analyst.Services.Interfaces
{
    public interface IDiaryService
    {
        Task<List<DiaryEntry>> GetByPlantAndDateAsync(
            int plantId,
            DateTime date);

        Task CreateFromTaskAsync(
            PlantTask task);
        Task<List<CalendarDayDto>>
        GetCalendarAsync(int plantId, int month, int year);

        Task<DiarySummaryDto>
            GetSummaryAsync(int plantId, int month, int year);
    }
}
