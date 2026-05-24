using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebCellSalon.Data;
using WebCellSalon.Models;
using WebCellSalon.Services;

namespace WebCellSalon.Controllers;

[Authorize(Roles = "клиент")]
public class ProfileController(
    AppDbContext dbContext,
    CurrentUserAccessor currentUser,
    CustomerScopedDataService customerScopedDataService) : Controller
{
    [HttpGet("/Profile")]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        return View(await BuildProfilePageViewModel(cancellationToken));
    }

    [ValidateAntiForgeryToken]
    [HttpPost("/Profile/CreateTicket")]
    public async Task<IActionResult> CreateTicket(
        [Bind(Prefix = nameof(ProfilePageViewModel.TicketForm))] CreateTicketViewModel model,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            var pageModel = await BuildProfilePageViewModel(cancellationToken);
            pageModel.TicketForm = model;
            return View("Index", pageModel);
        }

        var ticket = new CustomerTicket
        {
            ClientId = currentUser.ClientId!.Value,
            DeviceId = model.DeviceId,
            Subject = model.Subject,
            Description = model.Description,
            Status = "новое",
            CreatedAt = DateTime.UtcNow
        };
        
        dbContext.CustomerTickets.Add(ticket);
        await dbContext.SaveChangesAsync(cancellationToken);

        TempData["SuccessMessage"] = "Обращение успешно создано!";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("/MyRepairs")]
    public async Task<IActionResult> MyRepairs(CancellationToken cancellationToken)
    {
        var items = await customerScopedDataService.GetRepairsAsync(currentUser.ClientId!.Value, cancellationToken);
        return View(items);
    }

    [HttpGet("/MyContracts")]
    public async Task<IActionResult> MyContracts(CancellationToken cancellationToken)
    {
        var items = await customerScopedDataService.GetContractsAsync(currentUser.ClientId!.Value, cancellationToken);
        return View(items);
    }

    [HttpGet("/MyPurchases")]
    public async Task<IActionResult> MyPurchases(CancellationToken cancellationToken)
    {
        var clientId = currentUser.ClientId!.Value;
        const string sql = """
            SELECT
                p."id_продажи" AS "SaleId",
                p."дата_продажи" AS "SaleDate",
                u."модель" AS "Model",
                pp."серийный_номер" AS "SerialNumber",
                u."розничная_цена" AS "RetailPrice",
                trim(concat(coalesce(s."фамилия", ''), ' ', coalesce(s."имя", ''), ' ', coalesce(s."отчество", ''))) AS "SellerName"
            FROM салон_сотовой_связи."продажи" p
            JOIN салон_сотовой_связи."позиции_продаж" pp ON pp."id_продажи" = p."id_продажи"
            JOIN салон_сотовой_связи."устройства" u ON u."id_устройства" = pp."id_устройства"
            JOIN салон_сотовой_связи."сотрудники" s ON s."id_сотрудника" = p."id_сотрудника"
            WHERE p."id_клиента" = {0}
            ORDER BY p."дата_продажи" DESC, p."id_продажи" DESC
            """;

        var items = await dbContext.CustomerPurchases
            .FromSqlRaw(sql, clientId)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return View(items);
    }

    private async Task<ProfilePageViewModel> BuildProfilePageViewModel(CancellationToken cancellationToken)
    {
        var clientId = currentUser.ClientId!.Value;
        await dbContext.Database.ExecuteSqlRawAsync($"SET SESSION app.current_client_id = {clientId}");

        var customer = await dbContext.Customers.AsNoTracking()
            .FirstAsync(x => x.Id == clientId, cancellationToken);

        var bonus = (await customerScopedDataService.GetBonusesAsync(clientId, cancellationToken)).FirstOrDefault();

        var contractsCount = await dbContext.Contracts.AsNoTracking()
            .CountAsync(x => x.ClientId == clientId && (x.EndDate == null || x.EndDate >= DateOnly.FromDateTime(DateTime.Today)), cancellationToken);

        return new ProfilePageViewModel
        {
            Customer = customer,
            Bonus = bonus,
            ActiveContractsCount = contractsCount,
            DeviceOptions = await dbContext.Devices.AsNoTracking()
                .OrderBy(x => x.Model)
                .Select(x => new SelectListItem(x.Model, x.Id.ToString()))
                .ToListAsync(cancellationToken)
        };
    }
}
