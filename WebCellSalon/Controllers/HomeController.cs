using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebCellSalon.Data;
using WebCellSalon.Models;

namespace WebCellSalon.Controllers;

public class HomeController(AppDbContext dbContext) : Controller
{
    [HttpGet("/")]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var model = new HomeIndexViewModel
        {
            Devices = await dbContext.DeviceCatalog
                .AsNoTracking()
                .OrderByDescending(x => x.StockBalance)
                .ThenBy(x => x.ManufacturerName)
                .ThenBy(x => x.Model)
                .ToListAsync(cancellationToken),
            Tariffs = await dbContext.ActiveTariffs
                .AsNoTracking()
                .OrderBy(x => x.OperatorName)
                .ThenBy(x => x.MonthlyFee)
                .ToListAsync(cancellationToken)
        };

        return View(model);
    }

    [HttpGet("/Home/Error")]
    public IActionResult Error()
    {
        return View();
    }
}
