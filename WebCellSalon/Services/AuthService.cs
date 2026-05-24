using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using WebCellSalon.Data;
using WebCellSalon.Models;

namespace WebCellSalon.Services;

public class AuthService(AppDbContext dbContext)
{
    public async Task<LoginResult> LoginAsync(string login, string password, CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT
                "успех" AS "Success",
                "id_пользователя" AS "UserId",
                "роль" AS "Role",
                "сообщение" AS "Message",
                "тип_пользователя" AS "UserType"
            FROM салон_сотовой_связи."проверить_пароль"(@login, @password)
            """;

        var result = await dbContext.Database.SqlQueryRaw<LoginResultRow>(
                sql,
                new NpgsqlParameter("login", login),
                new NpgsqlParameter("password", password))
            .FirstOrDefaultAsync(cancellationToken);

        if (result is null)
        {
            return LoginResult.Failed("Функция проверки пароля не вернула результат.");
        }

        if (!result.Success)
        {
            return LoginResult.Failed(result.Message ?? "Ошибка входа.");
        }

        var account = await dbContext.UserAccounts
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Login == login, cancellationToken);

        if (account is null)
        {
            return LoginResult.Failed("Учётная запись найдена функцией БД, но не загружена приложением.");
        }

        string displayName;
        if (account.ClientId.HasValue)
        {
            var customer = await dbContext.Customers.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == account.ClientId.Value, cancellationToken);
            displayName = customer?.FullName ?? login;
        }
        else
        {
            var employee = await dbContext.Employees.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == account.EmployeeId, cancellationToken);
            displayName = employee?.FullName ?? login;
        }

        return LoginResult.Ok(account, displayName, result.Role ?? account.Role, result.UserType ?? "сотрудник");
    }

    public async Task SignInAsync(HttpContext httpContext, LoginResult loginResult)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, loginResult.Account!.Login),
            new(ClaimTypes.Role, loginResult.Role!),
            new("AccountId", loginResult.Account.Id.ToString()),
            new("UserType", loginResult.UserType!),
            new("DisplayName", loginResult.DisplayName!)
        };

        if (loginResult.Account.ClientId.HasValue)
        {
            claims.Add(new Claim("ClientId", loginResult.Account.ClientId.Value.ToString()));
        }

        if (loginResult.Account.EmployeeId.HasValue)
        {
            claims.Add(new Claim("EmployeeId", loginResult.Account.EmployeeId.Value.ToString()));
        }

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await httpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(12)
            });
    }

    public Task SignOutAsync(HttpContext httpContext)
    {
        return httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    }
}

public sealed class LoginResult
{
    public bool Success { get; private init; }
    public string? Message { get; private init; }
    public UserAccount? Account { get; private init; }
    public string? DisplayName { get; private init; }
    public string? Role { get; private init; }
    public string? UserType { get; private init; }

    public static LoginResult Failed(string message) => new() { Success = false, Message = message };

    public static LoginResult Ok(UserAccount account, string displayName, string role, string userType) => new()
    {
        Success = true,
        Account = account,
        DisplayName = displayName,
        Role = role,
        UserType = userType
    };
}

public class LoginResultRow
{
    public bool Success { get; set; }
    public int? UserId { get; set; }
    public string? Role { get; set; }
    public string? Message { get; set; }
    public string? UserType { get; set; }
}
