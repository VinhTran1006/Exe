using Agriculture_Analyst.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

public class InventoryController : Controller
{
    private readonly IInventoryTransactionService _service;
    private readonly AgricultureAnalystDbContext _context;

    public InventoryController(
        IInventoryTransactionService service,
        AgricultureAnalystDbContext context)
    {
        _service = service;
        _context = context;
    }

    public IActionResult Index()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        return View(_service.GetUserTransactions(userId));
    }

    public IActionResult Create()
    {
        ViewBag.InventoryList = new SelectList(
            _context.Inventories.ToList(),
            "InvId",
            "InvName"
        );

        ViewBag.ItemList = new SelectList(
            _context.Items.ToList(),
            "ItemId",
            "ItemName"
        );

        return View();
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(InventoryTransaction model)
    {
        ModelState.Remove("Inventory");
        ModelState.Remove("Item");

        if (!ModelState.IsValid)
        {
            ViewBag.Items = _context.Items
    .Select(i => new SelectListItem
    {
        Value = i.ItemId.ToString(),
        Text = i.ItemName
    })
    .ToList();

            ViewBag.Inventories = _context.Inventories
                .Select(i => new SelectListItem
                {
                    Value = i.InvId.ToString(),
                    Text = i.InvName
                })
                .ToList();

            return View(model);
        }

        model.UserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        model.NgayGiaoDich = DateTime.Now;

        _service.Create(model);

        return RedirectToAction(nameof(Index));
    }

}
