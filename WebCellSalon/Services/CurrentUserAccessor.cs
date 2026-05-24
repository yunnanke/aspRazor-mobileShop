using System.Security.Claims;

namespace WebCellSalon.Services;

public class CurrentUserAccessor(IHttpContextAccessor httpContextAccessor)
{
    public ClaimsPrincipal Principal => httpContextAccessor.HttpContext?.User ?? new ClaimsPrincipal();

    public bool IsAuthenticated => Principal.Identity?.IsAuthenticated == true;
    public string? Login => Principal.Identity?.Name;
    public string? Role => Principal.FindFirstValue(ClaimTypes.Role);
    public string? UserType => Principal.FindFirstValue("UserType");
    public int? ClientId => ParseIntClaim("ClientId");
    public int? EmployeeId => ParseIntClaim("EmployeeId");
    public int? AccountId => ParseIntClaim("AccountId");
    public string? DisplayName => Principal.FindFirstValue("DisplayName");

    private int? ParseIntClaim(string name)
    {
        return int.TryParse(Principal.FindFirstValue(name), out var value) ? value : null;
    }
}
