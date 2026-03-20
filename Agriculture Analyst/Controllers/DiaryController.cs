using Agriculture_Analyst.Models.ViewModel;
using Agriculture_Analyst.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
namespace Agriculture_Analyst.Controllers
{
    [Authorize]

    // Controllers/DiaryController.cs
    [Authorize]
    public class DiaryController : Controller
    {
        private readonly IPlantService _plantService;
        private readonly IPlantTaskService _taskService;
        private readonly IDiaryService _diaryService;

        public DiaryController(
            IPlantService plantService,
            IPlantTaskService taskService,
            IDiaryService diaryService)
        {
            _plantService = plantService;
            _taskService = taskService;
            _diaryService = diaryService;
        }

        private int GetUserId() =>
            int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        // ── Index ──────────────────────────────────────────────────
        public async Task<IActionResult> Index(
            int plantId, int? day, int? month, int? year)
        {
            var userId = GetUserId();
            var selectedDate = (day.HasValue && month.HasValue && year.HasValue)
                ? new DateTime(year.Value, month.Value, day.Value)
                : DateTime.Today;

            var plant = await _plantService.GetByIdAsync(plantId, userId);
            if (plant == null) return NotFound();

            var tasks = await _taskService.GetTasksByDateAsync(plantId, userId, selectedDate);
            var diaries = await _diaryService.GetByPlantAndDateAsync(plantId, selectedDate);

            var vm = new DiaryIndexViewModel
            {
                Plant = plant,
                Tasks = tasks,
                DiaryEntries = diaries,
                SelectedDate = selectedDate
            };
            return View(vm);
        }

        // ── Calendar View ──────────────────────────────────────────
        public async Task<IActionResult> Calendar(int plantId)
        {
            var userId = GetUserId();
            var plant = await _plantService.GetByIdAsync(plantId, userId);
            if (plant == null) return NotFound();
            ViewBag.PlantId = plantId;
            ViewBag.PlantName = plant.PlantName;
            ViewBag.PlantType = plant.PlantType;
            ViewBag.StartDate = plant.StartDate?.ToString("dd/MM/yyyy") ?? "--";
            return View();
        }

        // ── API: Calendar days ─────────────────────────────────────
        [HttpGet]
        public async Task<IActionResult> GetCalendar(
            int plantId, int month, int year)
        {
            var data = await _diaryService.GetCalendarAsync(plantId, month, year);
            return Json(data);
        }

        // ── API: Summary tháng ─────────────────────────────────────
        [HttpGet]
        public async Task<IActionResult> GetSummary(
            int plantId, int month, int year)
        {
            var data = await _diaryService.GetSummaryAsync(plantId, month, year);
            return Json(data);
        }

        // ── API: Heatmap năm ───────────────────────────────────────
        [HttpGet]
        public async Task<IActionResult> GetYearHeatmap(int plantId, int year)
        {
            var data = await _diaryService.GetYearHeatmapAsync(plantId, year);
            return Json(data);
        }

        // ── Complete Task ──────────────────────────────────────────
        [HttpPost]
        public async Task<IActionResult> CompleteTask(int taskId)
        {
            await _taskService.CompleteTaskAsync(taskId, GetUserId());
            return Redirect(Request.Headers["Referer"].ToString());
        }

        // ── Delete Task ────────────────────────────────────────────
        [HttpPost]
        public async Task<IActionResult> DeleteTask(int taskId)
        {
            await _taskService.SoftDeleteAsync(taskId, GetUserId());
            return Redirect(Request.Headers["Referer"].ToString());
        }

        // ── Create Task ────────────────────────────────────────────
        [HttpGet]
        public IActionResult CreateTask(int plantId) =>
            View(new CreatePlantTaskViewModel { PlantId = plantId });

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTask(CreatePlantTaskViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            await _taskService.CreateAsync(model, GetUserId());
            return RedirectToAction(nameof(Index), new { plantId = model.PlantId });
        }
    }



}
