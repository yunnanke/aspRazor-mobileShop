using Microsoft.EntityFrameworkCore;
using WebCellSalon.Data;
using WebCellSalon.Models;

namespace WebCellSalon.Services;

public class CustomerScopedDataService(AppDbContext dbContext)
{
    public async Task<List<CustomerOwnBonusView>> GetBonusesAsync(int clientId, CancellationToken cancellationToken = default)
    {
        return await ExecuteWithClientScopeAsync(
            clientId,
            () => dbContext.CustomerOwnBonuses.AsNoTracking().ToListAsync(cancellationToken));
    }

    public async Task<List<CustomerOwnContractView>> GetContractsAsync(int clientId, CancellationToken cancellationToken = default)
    {
        return await ExecuteWithClientScopeAsync(
            clientId,
            () => dbContext.CustomerOwnContracts.AsNoTracking().ToListAsync(cancellationToken));
    }

    public async Task<List<CustomerOwnRepairView>> GetRepairsAsync(int clientId, CancellationToken cancellationToken = default)
    {
        return await ExecuteWithClientScopeAsync(
            clientId,
            () => dbContext.CustomerOwnRepairs.AsNoTracking().ToListAsync(cancellationToken));
    }

    private async Task<List<T>> ExecuteWithClientScopeAsync<T>(int clientId, Func<Task<List<T>>> query)
    {
        await dbContext.Database.OpenConnectionAsync();
        try
        {
            var sql = $"SET SESSION app.current_client_id = {clientId}";
            await dbContext.Database.ExecuteSqlRawAsync(sql);
            return await query();
        }
        finally
        {
            await dbContext.Database.CloseConnectionAsync();
        }
    }
}
