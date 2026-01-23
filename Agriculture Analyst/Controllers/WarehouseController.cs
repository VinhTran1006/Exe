using Agriculture_Analyst.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

public class WarehouseController : Controller
{
    private readonly IInventoryService _service;

    public WarehouseController(IInventoryService service)
    {
        _service = service;
    }

    public IActionResult Index()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        return View(_service.GetByUser(userId));
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Create(Inventory inv)
    {
        // Xóa validate User vì sẽ tự gán
        ModelState.Remove("User");

        if (ModelState.IsValid)
        {
            inv.UserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            inv.CreatedAt = DateTime.Now;

            _service.Add(inv);
            return RedirectToAction(nameof(Index));
        }
        return View(inv);
    }
}