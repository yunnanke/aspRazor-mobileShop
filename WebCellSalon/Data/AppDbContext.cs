using Microsoft.EntityFrameworkCore;
using WebCellSalon.Models;

namespace WebCellSalon.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<SimCard> SimCards => Set<SimCard>();
    public DbSet<Promotion> Promotions => Set<Promotion>();
    public DbSet<MobileOperator> MobileOperators => Set<MobileOperator>();
    public DbSet<TariffPlan> TariffPlans => Set<TariffPlan>();
    public DbSet<BonusProgram> BonusPrograms => Set<BonusProgram>();
    public DbSet<ContractEntity> Contracts => Set<ContractEntity>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<DeviceRepair> DeviceRepairs => Set<DeviceRepair>();
    public DbSet<JobPosition> JobPositions => Set<JobPosition>();
    public DbSet<SaleEntity> Sales => Set<SaleEntity>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<DeviceCategory> DeviceCategories => Set<DeviceCategory>();
    public DbSet<PurchaseLine> PurchaseLines => Set<PurchaseLine>();
    public DbSet<SaleLine> SaleLines => Set<SaleLine>();
    public DbSet<DeviceManufacturer> DeviceManufacturers => Set<DeviceManufacturer>();
    public DbSet<DeviceEntity> Devices => Set<DeviceEntity>();
    public DbSet<Warranty> Warranties => Set<Warranty>();
    public DbSet<LoginJournal> LoginJournals => Set<LoginJournal>();
    public DbSet<ServiceOrder> ServiceOrders => Set<ServiceOrder>();
    public DbSet<PurchaseEntity> Purchases => Set<PurchaseEntity>();
    public DbSet<CustomerTicket> CustomerTickets => Set<CustomerTicket>();
    public DbSet<PaymentEntity> Payments => Set<PaymentEntity>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<UserSession> Sessions => Set<UserSession>();
    public DbSet<PaymentMethod> PaymentMethods => Set<PaymentMethod>();
    public DbSet<ServiceEntity> Services => Set<ServiceEntity>();
    public DbSet<UserAccount> UserAccounts => Set<UserAccount>();

    public DbSet<ActiveTariffView> ActiveTariffs => Set<ActiveTariffView>();
    public DbSet<CustomerOwnBonusView> CustomerOwnBonuses => Set<CustomerOwnBonusView>();
    public DbSet<CustomerOwnContractView> CustomerOwnContracts => Set<CustomerOwnContractView>();
    public DbSet<CustomerOwnProfileView> CustomerOwnProfiles => Set<CustomerOwnProfileView>();
    public DbSet<CustomerOwnRepairView> CustomerOwnRepairs => Set<CustomerOwnRepairView>();
    public DbSet<EmployeePerformanceView> EmployeePerformance => Set<EmployeePerformanceView>();
    public DbSet<DeviceCatalogView> DeviceCatalog => Set<DeviceCatalogView>();
    public DbSet<CustomerPurchaseView> CustomerPurchases => Set<CustomerPurchaseView>();
    public DbSet<AccountListItemView> AccountListItems => Set<AccountListItemView>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PurchaseLine>().HasKey(x => new { x.PurchaseId, x.DeviceId, x.SerialNumber });
        modelBuilder.Entity<SaleLine>().HasKey(x => new { x.SaleId, x.DeviceId, x.SerialNumber });
        modelBuilder.Entity<ServiceOrder>().HasKey(x => new { x.ContractId, x.ServiceId, x.EmployeeId });

        modelBuilder.Entity<ActiveTariffView>().HasNoKey().ToView("v_active_tariffs_with_operator", "салон_сотовой_связи");
        modelBuilder.Entity<CustomerOwnBonusView>().HasNoKey().ToView("v_customer_own_bonus", "салон_сотовой_связи");
        modelBuilder.Entity<CustomerOwnContractView>().HasNoKey().ToView("v_customer_own_contracts", "салон_сотовой_связи");
        modelBuilder.Entity<CustomerOwnProfileView>().HasNoKey().ToView("v_customer_own_profile", "салон_сотовой_связи");
        modelBuilder.Entity<CustomerOwnRepairView>().HasNoKey().ToView("v_customer_own_repairs", "салон_сотовой_связи");
        modelBuilder.Entity<EmployeePerformanceView>().HasNoKey().ToView("v_employee_performance", "салон_сотовой_связи");
        modelBuilder.Entity<DeviceCatalogView>().HasNoKey().ToView("v_full_device_info", "салон_сотовой_связи");
        modelBuilder.Entity<CustomerPurchaseView>().HasNoKey();
        modelBuilder.Entity<AccountListItemView>().HasNoKey();

        base.OnModelCreating(modelBuilder);
    }
}
