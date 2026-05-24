using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebCellSalon.Data;
using WebCellSalon.Models;
using WebCellSalon.Services;

namespace WebCellSalon.Controllers;

[Authorize(Roles = "консультант,администратор")]
public class SalesController(AppDbContext dbContext, CurrentUserAccessor currentUser) : Controller
{
    [HttpGet("/RegisterSale")]
    public async Task<IActionResult> RegisterSale(CancellationToken cancellationToken)
    {
        return View(await BuildRegisterSaleViewModel(new RegisterSaleViewModel(), cancellationToken));
    }

    [ValidateAntiForgeryToken]
    [HttpPost("/RegisterSale")]
    public async Task<IActionResult> RegisterSale(RegisterSaleViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(await BuildRegisterSaleViewModel(model, cancellationToken));
        }

        const string sql = """SELECT салон_сотовой_связи.f_register_sale({0}, {1}, {2}, {3})""";
        await dbContext.Database.ExecuteSqlRawAsync(
            sql,
            [model.ClientId, currentUser.EmployeeId ?? 0, model.SerialNumbers, model.PaymentMethodId],
            cancellationToken);

        TempData["SuccessMessage"] = "Продажа успешно оформлена.";
        return RedirectToAction(nameof(RegisterSale));
    }

    [HttpGet("/RegisterContract")]
    public async Task<IActionResult> RegisterContract(CancellationToken cancellationToken)
    {
        return View(await BuildRegisterContractViewModel(new RegisterContractViewModel(), cancellationToken));
    }

    [ValidateAntiForgeryToken]
    [HttpPost("/RegisterContract")]
    public async Task<IActionResult> RegisterContract(RegisterContractViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(await BuildRegisterContractViewModel(model, cancellationToken));
        }

        const string sql = """SELECT салон_сотовой_связи.f_create_contract_with_sim({0}, {1}, {2}, {3})""";
        await dbContext.Database.ExecuteSqlRawAsync(
            sql,
            [model.ClientId, currentUser.EmployeeId ?? 0, model.SimIccid, model.ContractType],
            cancellationToken);

        TempData["SuccessMessage"] = "Договор и SIM-карта оформлены.";
        return RedirectToAction(nameof(RegisterContract));
    }

    [HttpGet("/RegisterRepair")]
    public async Task<IActionResult> RegisterRepair(CancellationToken cancellationToken)
    {
        return View(await BuildRegisterRepairViewModel(new RegisterRepairViewModel(), cancellationToken));
    }

    [ValidateAntiForgeryToken]
    [HttpPost("/RegisterRepair")]
    public async Task<IActionResult> RegisterRepair(RegisterRepairViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(await BuildRegisterRepairViewModel(model, cancellationToken));
        }

        var entity = new DeviceRepair
        {
            ClientId = model.ClientId,
            DeviceId = model.DeviceId,
            SerialNumber = model.SerialNumber,
            FaultType = model.FaultType,
            ProblemDescription = model.ProblemDescription,
            Notes = model.Notes,
            Status = "принят",
            AcceptanceDate = DateOnly.FromDateTime(DateTime.Today),
            EmployeeId = currentUser.EmployeeId
        };

        dbContext.DeviceRepairs.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);

        TempData["SuccessMessage"] = "Ремонт зарегистрирован.";
        return RedirectToAction(nameof(RegisterRepair));
    }

    [HttpGet("/CloseRepair/{id:int}")]
    public async Task<IActionResult> CloseRepair(int id, CancellationToken cancellationToken)
    {
        var repair = await dbContext.DeviceRepairs.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (repair is null)
        {
            return NotFound();
        }

        var model = new CloseRepairViewModel
        {
            RepairId = repair.Id,
            RepairTitle = $"{repair.SerialNumber ?? "без серийного номера"} / {repair.FaultType}",
            FinalCost = repair.RepairCost ?? 0m,
            Notes = repair.Notes
        };

        return View(model);
    }

    [ValidateAntiForgeryToken]
    [HttpPost("/CloseRepair/{id:int}")]
    public async Task<IActionResult> CloseRepair(int id, CloseRepairViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            model.RepairId = id;
            return View(model);
        }

        const string sql = """SELECT салон_сотовой_связи.f_close_repair({0}, {1}, {2})""";
        await dbContext.Database.ExecuteSqlRawAsync(sql, [id, model.FinalCost, model.Notes ?? string.Empty], cancellationToken);

        TempData["SuccessMessage"] = "Ремонт закрыт.";
        return RedirectToAction(nameof(RegisterRepair));
    }

    private async Task<RegisterSaleViewModel> BuildRegisterSaleViewModel(RegisterSaleViewModel model, CancellationToken cancellationToken)
    {
        model.ClientOptions = await dbContext.Customers.AsNoTracking()
            .OrderBy(x => x.LastName).ThenBy(x => x.FirstName)
            .Select(x => new SelectListItem($"{x.FullName} ({x.Phone})", x.Id.ToString()))
            .ToListAsync(cancellationToken);

        model.PaymentMethodOptions = await dbContext.PaymentMethods.AsNoTracking()
            .OrderBy(x => x.Name)
            .Select(x => new SelectListItem(x.Name, x.Id.ToString()))
            .ToListAsync(cancellationToken);

        return model;
    }

    private async Task<RegisterContractViewModel> BuildRegisterContractViewModel(RegisterContractViewModel model, CancellationToken cancellationToken)
    {
        model.ClientOptions = await dbContext.Customers.AsNoTracking()
            .OrderBy(x => x.LastName).ThenBy(x => x.FirstName)
            .Select(x => new SelectListItem($"{x.FullName} ({x.Phone})", x.Id.ToString()))
            .ToListAsync(cancellationToken);

        return model;
    }

    private async Task<RegisterRepairViewModel> BuildRegisterRepairViewModel(RegisterRepairViewModel model, CancellationToken cancellationToken)
    {
        model.ClientOptions = await dbContext.Customers.AsNoTracking()
            .OrderBy(x => x.LastName).ThenBy(x => x.FirstName)
            .Select(x => new SelectListItem($"{x.FullName} ({x.Phone})", x.Id.ToString()))
            .ToListAsync(cancellationToken);

        model.DeviceOptions = await dbContext.Devices.AsNoTracking()
            .OrderBy(x => x.Model)
            .Select(x => new SelectListItem(x.Model, x.Id.ToString()))
            .ToListAsync(cancellationToken);

        return model;
    }
}
