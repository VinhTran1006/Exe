using Agriculture_Analyst.Models.ViewModel;
using Agriculture_Analyst.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
namespace Agriculture_Analyst.Controllers
{
    [Authorize]

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

        private int GetUserId()
        {
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        }

        // =========================
        // INDEX – TRANG CHÍNH
        // =========================
        public async Task<IActionResult> Index(int plantId, DateTime? date)
        {
            var userId = GetUserId();
            var selectedDate = date ?? DateTime.Today;

            var plant = await _plantService.GetByIdAsync(plantId, userId);
            if (plant == null) return NotFound();

            var tasks = await _taskService.GetTasksByDateAsync(
                plantId, userId, selectedDate);

            var diaries = await _diaryService.GetByPlantAndDateAsync(
                plantId, selectedDate);

            var vm = new DiaryIndexViewModel
            {
                Plant = plant,
                Tasks = tasks,
                DiaryEntries = diaries,
                SelectedDate = selectedDate
            };

            return View(vm);
        }

        // =========================
        // HOÀN THÀNH TASK
        // =========================
        [HttpPost]
        public async Task<IActionResult> CompleteTask(int taskId)
        {
            await _taskService.CompleteTaskAsync(taskId, GetUserId());
            return Redirect(Request.Headers["Referer"].ToString());
        }

        // =========================
        // XÓA MỀM TASK
        // =========================
        [HttpPost]
        public async Task<IActionResult> DeleteTask(int taskId)
        {
            await _taskService.SoftDeleteAsync(taskId, GetUserId());
            return Redirect(Request.Headers["Referer"].ToString());
        }

        // =========================
        // TẠO TASK
        // =========================
        [HttpGet]
        public IActionResult CreateTask(int plantId)
        {
            return View(new CreatePlantTaskViewModel
            {
                PlantId = plantId
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTask(CreatePlantTaskViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await _taskService.CreateAsync(model, GetUserId());
            return RedirectToAction(nameof(Index), new { plantId = model.PlantId });
        }
    }



}
