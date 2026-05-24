using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebCellSalon.Data;
using WebCellSalon.Models;
using WebCellSalon.Services;

namespace WebCellSalon.Controllers;

[Authorize(Roles = "администратор")]
public class AdminController(AppDbContext dbContext, PasswordHashService passwordHashService) : Controller
{
    [HttpGet("/Admin/Users")]
    public async Task<IActionResult> Users(CancellationToken cancellationToken)
    {
        var model = new AdminUsersPageViewModel
        {
            Accounts = await BuildAccountList(cancellationToken)
        };

        return View(model);
    }

    [ValidateAntiForgeryToken]
    [HttpPost("/Admin/Users/Create")]
    public async Task<IActionResult> CreateUser(CreateUserViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            var page = new AdminUsersPageViewModel
            {
                Accounts = await BuildAccountList(cancellationToken),
                CreateUserForm = model
            };
            return View("Users", page);
        }

        // Проверка: должен быть указан либо сотрудник, либо клиент
        if (model.EmployeeId == null && model.ClientId == null)
        {
            ModelState.AddModelError("", "Укажите либо сотрудника, либо клиента");
            var page = new AdminUsersPageViewModel
            {
                Accounts = await BuildAccountList(cancellationToken),
                CreateUserForm = model
            };
            return View("Users", page);
        }

        // Подготовка параметров (для сотрудника clientId = 0, для клиента employeeId = 0)
        int employeeId = model.EmployeeId ?? 0;
        int clientId = model.ClientId ?? 0;
        var passwordHash = passwordHashService.CreateHash(model.Password);

        // Вызов функции БД
        var sql = @"
        SELECT * FROM салон_сотовой_связи.создать_учетку(
            {0}, {1}, {2}, {3}, {4}
        )";

        try
        {
            var result = await dbContext.Database
                .SqlQueryRaw<CreateAccountResult>(sql, employeeId, clientId, model.Login, passwordHash, model.Role)
                .FirstOrDefaultAsync(cancellationToken);

            if (result != null && result.успех)
            {
                TempData["SuccessMessage"] = result.сообщение;
            }
            else
            {
                TempData["ErrorMessage"] = result?.сообщение ?? "Ошибка при создании учётной записи";
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            TempData["ErrorMessage"] = $"Ошибка БД: {ex.InnerException?.Message ?? ex.Message}";
        }

        return RedirectToAction(nameof(Users));
    }

    // Класс для результата функции (добавьте в файл Models/CreateAccountResult.cs)
    public class CreateAccountResult
    {
        public bool успех { get; set; }
        public string сообщение { get; set; } = string.Empty;
        public int? id_учетки { get; set; }
    }

    [ValidateAntiForgeryToken]
    [HttpPost("/Admin/Users/ToggleActive/{id:int}")]
    public async Task<IActionResult> ToggleActive(int id, CancellationToken cancellationToken)
    {
        var account = await dbContext.UserAccounts.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (account is null)
        {
            return NotFound();
        }

        account.IsActive = !(account.IsActive ?? false);
        await dbContext.SaveChangesAsync(cancellationToken);
        TempData["SuccessMessage"] = "Статус учётной записи обновлён.";
        return RedirectToAction(nameof(Users));
    }

    [ValidateAntiForgeryToken]
    [HttpPost("/Admin/Users/ResetPassword/{id:int}")]
    public async Task<IActionResult> ResetPassword(int id, string newPassword, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(newPassword))
        {
            TempData["ErrorMessage"] = "Новый пароль не может быть пустым.";
            return RedirectToAction(nameof(Users));
        }

        var account = await dbContext.UserAccounts.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (account is null)
        {
            return NotFound();
        }

        account.PasswordHash = passwordHashService.CreateHash(newPassword);
        account.LoginAttempts = 0;
        account.IsActive = true;
        await dbContext.SaveChangesAsync(cancellationToken);

        TempData["SuccessMessage"] = "Пароль успешно сброшен.";
        return RedirectToAction(nameof(Users));
    }

    private async Task<List<AccountListItemView>> BuildAccountList(CancellationToken cancellationToken)
    {
        var accounts = await dbContext.UserAccounts.AsNoTracking().OrderBy(x => x.Login).ToListAsync(cancellationToken);
        var employees = await dbContext.Employees.AsNoTracking().ToDictionaryAsync(x => x.Id, cancellationToken);
        var customers = await dbContext.Customers.AsNoTracking().ToDictionaryAsync(x => x.Id, cancellationToken);

        return accounts.Select(account =>
        {
            string owner = "-";
            if (account.EmployeeId.HasValue && employees.TryGetValue(account.EmployeeId.Value, out var employee))
            {
                owner = employee.FullName;
            }
            else if (account.ClientId.HasValue && customers.TryGetValue(account.ClientId.Value, out var customer))
            {
                owner = customer.FullName;
            }

            return new AccountListItemView
            {
                AccountId = account.Id,
                Login = account.Login,
                Role = account.Role,
                IsActive = account.IsActive ?? false,
                EmployeeId = account.EmployeeId,
                ClientId = account.ClientId,
                OwnerName = owner,
                LastLoginAt = account.LastLoginAt
            };
        }).ToList();
    }
}
