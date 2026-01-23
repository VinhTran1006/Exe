using Agriculture_Analyst.Models;
using Microsoft.AspNetCore.Mvc;

public class ItemController : Controller
{
    private readonly IItemService _service;

    public ItemController(IItemService service)
    {
        _service = service;
    }

    public IActionResult Index()
    {
        return View(_service.GetAll());
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Create(Item item)
    {
        if (ModelState.IsValid)
        {
            _service.Add(item);
            return RedirectToAction(nameof(Index));
        }
        return View(item);
    }

    // Bạn có thể làm thêm Edit/Delete tương tự
}