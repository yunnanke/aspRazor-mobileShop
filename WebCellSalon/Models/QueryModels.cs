using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebCellSalon.Models;

[Keyless]
[Table("v_active_tariffs_with_operator", Schema = "салон_сотовой_связи")]
public class ActiveTariffView
{
    [Column("id_тарифа")] public int TariffId { get; set; }
    [Column("название_тарифа")] public string Name { get; set; } = string.Empty;
    [Column("название_оператора")] public string OperatorName { get; set; } = string.Empty;
    [Column("ежемесячная_плата")] public decimal MonthlyFee { get; set; }
    [Column("включённые_минуты")] public int? IncludedMinutes { get; set; }
    [Column("включённый_интернет_гб")] public decimal? IncludedInternetGb { get; set; }
    [Column("описание")] public string? Description { get; set; }
}

[Keyless]
[Table("v_customer_own_bonus", Schema = "салон_сотовой_связи")]
public class CustomerOwnBonusView
{
    [Column("количество_баллов")] public int? Points { get; set; }
    [Column("уровень_клиента")] public string? Level { get; set; }
    [Column("дата_последнего_начисления")] public DateOnly? LastAccrualDate { get; set; }
}

[Keyless]
[Table("v_customer_own_contracts", Schema = "салон_сотовой_связи")]
public class CustomerOwnContractView
{
    [Column("id_договора")] public int ContractId { get; set; }
    [Column("тип_договора")] public string ContractType { get; set; } = string.Empty;
    [Column("дата_начала")] public DateOnly StartDate { get; set; }
    [Column("дата_окончания")] public DateOnly? EndDate { get; set; }
}

[Keyless]
[Table("v_customer_own_profile", Schema = "салон_сотовой_связи")]
public class CustomerOwnProfileView
{
    [Column("id_клиента")] public int? ClientId { get; set; }
    [Column("фио")] public string? FullName { get; set; }
    [Column("телефон")] public string? Phone { get; set; }
    [Column("email")] public string? Email { get; set; }
    [Column("дата_регистрации")] public DateOnly? RegistrationDate { get; set; }
    [Column("уровень_клиента")] public string? Level { get; set; }
    [Column("количество_баллов")] public int? Points { get; set; }
    [Column("дата_последнего_начисления")] public DateOnly? LastAccrualDate { get; set; }
    [Column("active_contracts_count")] public long? ActiveContractsCount { get; set; }
}

[Keyless]
[Table("v_customer_own_repairs", Schema = "салон_сотовой_связи")]
public class CustomerOwnRepairView
{
    [Column("id_ремонта")] public int RepairId { get; set; }
    [Column("серийный_номер")] public string? SerialNumber { get; set; }
    [Column("тип_неисправности")] public string FaultType { get; set; } = string.Empty;
    [Column("статус")] public string? Status { get; set; }
    [Column("дата_приема")] public DateOnly AcceptanceDate { get; set; }
    [Column("стоимость_ремонта")] public decimal? RepairCost { get; set; }
}

[Keyless]
[Table("v_employee_performance", Schema = "салон_сотовой_связи")]
public class EmployeePerformanceView
{
    [Column("id_сотрудника")] public int EmployeeId { get; set; }
    [Column("фио")] public string FullName { get; set; } = string.Empty;
    [Column("название_должности")] public string PositionName { get; set; } = string.Empty;
    [Column("количество_продаж")] public long SalesCount { get; set; }
    [Column("количество_ремонтов")] public long RepairsCount { get; set; }
    [Column("зарплата")] public decimal Salary { get; set; }
}

[Keyless]
[Table("v_full_device_info", Schema = "салон_сотовой_связи")]
public class DeviceCatalogView
{
    [Column("id_устройства")] public int DeviceId { get; set; }
    [Column("модель")] public string Model { get; set; } = string.Empty;
    [Column("характеристики")] public string? Specifications { get; set; }
    [Column("розничная_цена")] public decimal RetailPrice { get; set; }
    [Column("название_производителя")] public string ManufacturerName { get; set; } = string.Empty;
    [Column("название_категории")] public string CategoryName { get; set; } = string.Empty;
    [Column("остаток_на_складе")] public long StockBalance { get; set; }
}

[Keyless]
public class CustomerPurchaseView
{
    public int SaleId { get; set; }
    public DateOnly SaleDate { get; set; }
    public string Model { get; set; } = string.Empty;
    public string SerialNumber { get; set; } = string.Empty;
    public decimal RetailPrice { get; set; }
    public string SellerName { get; set; } = string.Empty;
}

[Keyless]
public class AccountListItemView
{
    public int AccountId { get; set; }
    public string Login { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public int? EmployeeId { get; set; }
    public int? ClientId { get; set; }
    public string OwnerName { get; set; } = string.Empty;
    public DateTime? LastLoginAt { get; set; }
}
