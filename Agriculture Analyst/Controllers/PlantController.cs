using Agriculture_Analyst.Models;
using Agriculture_Analyst.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace Agriculture_Analyst.Controllers
{
    [Authorize]
    public class PlantController : Controller
    {
        private readonly IPlantService _service;

        public PlantController(IPlantService service)
        {
            _service = service;
        }

        private int GetUserId()
        {
            return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        }

        // LIST
        public async Task<IActionResult> Index()
        {
            int userId = GetUserId();
            var plants = await _service.GetUserPlantsAsync(userId);
            return View(plants);
        }

        // CREATE
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Plant plant)
        {
            if (!ModelState.IsValid)
                return View(plant);

            await _service.CreateAsync(plant, GetUserId());
            return RedirectToAction(nameof(Index));
        }

        // EDIT
        public async Task<IActionResult> Edit(int id)
        {
            var plant = await _service.GetByIdAsync(id, GetUserId());
            if (plant == null) return NotFound();

            return View(plant);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Plant plant)
        {
            if (!ModelState.IsValid)
                return View(plant);

            await _service.UpdateAsync(plant, GetUserId());
            return RedirectToAction(nameof(Index));
        }

        // DELETE
        public async Task<IActionResult> Delete(int id)
        {
            var plant = await _service.GetByIdAsync(id, GetUserId());
            if (plant == null) return NotFound();

            return View(plant);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _service.DeleteAsync(id, GetUserId());
            return RedirectToAction(nameof(Index));
        }
    }


}
