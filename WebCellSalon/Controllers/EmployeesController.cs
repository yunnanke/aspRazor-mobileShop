using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebCellSalon.Data;
using WebCellSalon.Models;

namespace WebCellSalon.Controllers;

[Authorize(Roles = "администратор")]
public class EmployeesController(AppDbContext dbContext) : Controller
{
    [HttpGet("/Employees")]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var employees = await dbContext.Employees.AsNoTracking()
            .OrderBy(x => x.LastName).ThenBy(x => x.FirstName)
            .ToListAsync(cancellationToken);

        return View(employees);
    }

    [HttpGet("/Employees/Create")]
    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        return View("Edit", await BuildEmployeeViewModel(new EmployeeEditViewModel(), cancellationToken));
    }

    [ValidateAntiForgeryToken]
    [HttpPost("/Employees/Create")]
    public async Task<IActionResult> Create(EmployeeEditViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View("Edit", await BuildEmployeeViewModel(model, cancellationToken));
        }

        dbContext.Employees.Add(new Employee
        {
            LastName = model.LastName,
            FirstName = model.FirstName,
            MiddleName = model.MiddleName,
            Phone = model.Phone,
            Email = model.Email,
            PositionId = model.PositionId,
            HireDate = model.HireDate,
            Salary = model.Salary
        });

        await dbContext.SaveChangesAsync(cancellationToken);
        TempData["SuccessMessage"] = "Сотрудник добавлен.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("/Employees/Edit/{id:int}")]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var employee = await dbContext.Employees.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (employee is null)
        {
            return NotFound();
        }

        var model = new EmployeeEditViewModel
        {
            Id = employee.Id,
            LastName = employee.LastName,
            FirstName = employee.FirstName,
            MiddleName = employee.MiddleName,
            Phone = employee.Phone,
            Email = employee.Email,
            PositionId = employee.PositionId,
            HireDate = employee.HireDate,
            Salary = employee.Salary
        };

        return View(await BuildEmployeeViewModel(model, cancellationToken));
    }

    [ValidateAntiForgeryToken]
    [HttpPost("/Employees/Edit/{id:int}")]
    public async Task<IActionResult> Edit(int id, EmployeeEditViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(await BuildEmployeeViewModel(model, cancellationToken));
        }

        var employee = await dbContext.Employees.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (employee is null)
        {
            return NotFound();
        }

        employee.LastName = model.LastName;
        employee.FirstName = model.FirstName;
        employee.MiddleName = model.MiddleName;
        employee.Phone = model.Phone;
        employee.Email = model.Email;
        employee.PositionId = model.PositionId;
        employee.HireDate = model.HireDate;
        employee.Salary = model.Salary;

        await dbContext.SaveChangesAsync(cancellationToken);
        TempData["SuccessMessage"] = "Данные сотрудника обновлены.";
        return RedirectToAction(nameof(Index));
    }

    [ValidateAntiForgeryToken]
    [HttpPost("/Employees/Delete/{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var employee = await dbContext.Employees.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (employee is not null)
        {
            dbContext.Employees.Remove(employee);
            await dbContext.SaveChangesAsync(cancellationToken);
            TempData["SuccessMessage"] = "Сотрудник удалён.";
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet("/EmployeeStats")]
    public async Task<IActionResult> Stats(CancellationToken cancellationToken)
    {
        var model = new EmployeeStatsPageViewModel
        {
            Items = await dbContext.EmployeePerformance.AsNoTracking()
                .OrderByDescending(x => x.SalesCount)
                .ThenByDescending(x => x.RepairsCount)
                .ToListAsync(cancellationToken),
            CurrentMonthSalesAmount = await GetCurrentMonthSalesAmount(cancellationToken),
            CurrentMonthPurchasesAmount = await GetCurrentMonthPurchaseAmount(cancellationToken)
        };

        return View(model);
    }

    private async Task<EmployeeEditViewModel> BuildEmployeeViewModel(EmployeeEditViewModel model, CancellationToken cancellationToken)
    {
        model.PositionOptions = await dbContext.JobPositions.AsNoTracking()
            .OrderBy(x => x.Name)
            .Select(x => new SelectListItem(x.Name, x.Id.ToString()))
            .ToListAsync(cancellationToken);

        return model;
    }

    private async Task<decimal> GetCurrentMonthSalesAmount(CancellationToken cancellationToken)
    {
        var fromDate = DateOnly.FromDateTime(new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1));
        var total = await (
            from sale in dbContext.Sales
            join line in dbContext.SaleLines on sale.Id equals line.SaleId
            join device in dbContext.Devices on line.DeviceId equals device.Id
            where sale.SaleDate >= fromDate
            select (decimal?)device.RetailPrice).SumAsync(cancellationToken);

        return total ?? 0m;
    }

    private async Task<decimal> GetCurrentMonthPurchaseAmount(CancellationToken cancellationToken)
    {
        var fromDate = DateOnly.FromDateTime(new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1));
        var total = await (
            from purchase in dbContext.Purchases
            join line in dbContext.PurchaseLines on purchase.Id equals line.PurchaseId
            where purchase.PurchaseDate >= fromDate
            select (decimal?)line.UnitPrice).SumAsync(cancellationToken);

        return total ?? 0m;
    }
}
