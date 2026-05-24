using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebCellSalon.Models;

[Table("sim_карты", Schema = "салон_сотовой_связи")]
public class SimCard
{
    [Key, Column("id_sim_карты")] public int Id { get; set; }
    [Column("iccid")] public string Iccid { get; set; } = string.Empty;
    [Column("номер_телефона")] public string PhoneNumber { get; set; } = string.Empty;
    [Column("id_тарифа")] public int TariffId { get; set; }
    [Column("статус")] public string? Status { get; set; }
    [Column("дата_активации")] public DateOnly? ActivationDate { get; set; }
    [Column("id_договора")] public int? ContractId { get; set; }
}

[Table("акции_и_скидки", Schema = "салон_сотовой_связи")]
public class Promotion
{
    [Key, Column("id_акции")] public int Id { get; set; }
    [Column("название_акции")] public string Name { get; set; } = string.Empty;
    [Column("тип_скидки")] public string DiscountType { get; set; } = string.Empty;
    [Column("размер_скидки")] public decimal DiscountAmount { get; set; }
    [Column("дата_начала")] public DateOnly StartDate { get; set; }
    [Column("дата_окончания")] public DateOnly EndDate { get; set; }
    [Column("описание")] public string? Description { get; set; }
    [Column("активна")] public bool? IsActive { get; set; }
}

[Table("мобильные_операторы", Schema = "салон_сотовой_связи")]
public class MobileOperator
{
    [Key, Column("id_оператора")] public int Id { get; set; }
    [Column("название_оператора")] public string Name { get; set; } = string.Empty;
    [Column("контактная_информация")] public string? ContactInfo { get; set; }
}

[Table("тарифные_планы", Schema = "салон_сотовой_связи")]
public class TariffPlan
{
    [Key, Column("id_тарифа")] public int Id { get; set; }
    [Column("id_оператора")] public int OperatorId { get; set; }
    [Column("название_тарифа")] public string Name { get; set; } = string.Empty;
    [Column("ежемесячная_плата")] public decimal MonthlyFee { get; set; }
    [Column("включённые_минуты")] public int? IncludedMinutes { get; set; }
    [Column("включённые_sms")] public int? IncludedSms { get; set; }
    [Column("включённый_интернет_гб")] public decimal? IncludedInternetGb { get; set; }
    [Column("описание")] public string? Description { get; set; }
}

[Table("бонусная_программа", Schema = "салон_сотовой_связи")]
public class BonusProgram
{
    [Key, Column("id_клиента")] public int ClientId { get; set; }
    [Column("количество_баллов")] public int? Points { get; set; }
    [Column("дата_последнего_начисления")] public DateOnly? LastAccrualDate { get; set; }
    [Column("уровень_клиента")] public string? ClientLevel { get; set; }
}

[Table("договоры", Schema = "салон_сотовой_связи")]
public class ContractEntity
{
    [Key, Column("id_договора")] public int Id { get; set; }
    [Column("id_клиента")] public int ClientId { get; set; }
    [Column("id_сотрудника")] public int EmployeeId { get; set; }
    [Column("тип_договора")] public string ContractType { get; set; } = string.Empty;
    [Column("дата_начала")] public DateOnly StartDate { get; set; }
    [Column("дата_окончания")] public DateOnly? EndDate { get; set; }
    [Column("детали_договора")] public string? Details { get; set; }
}

[Table("клиенты", Schema = "салон_сотовой_связи")]
public class Customer
{
    [Key, Column("id_клиента")] public int Id { get; set; }
    [Column("фамилия")] public string? LastName { get; set; }
    [Column("имя")] public string? FirstName { get; set; }
    [Column("отчество")] public string? MiddleName { get; set; }
    [Column("телефон")] public string Phone { get; set; } = string.Empty;
    [Column("email")] public string? Email { get; set; }
    [Column("паспортные_данные")] public string? PassportData { get; set; }
    [Column("дата_регистрации")] public DateOnly RegistrationDate { get; set; }

    [NotMapped]
    public string FullName => string.Join(' ', new[] { LastName, FirstName, MiddleName }.Where(x => !string.IsNullOrWhiteSpace(x)));
}

[Table("ремонт_устройств", Schema = "салон_сотовой_связи")]
public class DeviceRepair
{
    [Key, Column("id_ремонта")] public int Id { get; set; }
    [Column("id_клиента")] public int ClientId { get; set; }
    [Column("id_устройства")] public int? DeviceId { get; set; }
    [Column("серийный_номер")] public string? SerialNumber { get; set; }
    [Column("тип_неисправности")] public string FaultType { get; set; } = string.Empty;
    [Column("описание_проблемы")] public string? ProblemDescription { get; set; }
    [Column("статус")] public string? Status { get; set; }
    [Column("дата_приема")] public DateOnly AcceptanceDate { get; set; }
    [Column("дата_выдачи")] public DateOnly? IssueDate { get; set; }
    [Column("стоимость_ремонта")] public decimal? RepairCost { get; set; }
    [Column("id_сотрудника")] public int? EmployeeId { get; set; }
    [Column("примечания")] public string? Notes { get; set; }
}

[Table("должности", Schema = "салон_сотовой_связи")]
public class JobPosition
{
    [Key, Column("id_должности")] public int Id { get; set; }
    [Column("название_должности")] public string Name { get; set; } = string.Empty;
    [Column("базовая_ставка")] public decimal BaseRate { get; set; }
}

[Table("продажи", Schema = "салон_сотовой_связи")]
public class SaleEntity
{
    [Key, Column("id_продажи")] public int Id { get; set; }
    [Column("id_клиента")] public int ClientId { get; set; }
    [Column("id_сотрудника")] public int EmployeeId { get; set; }
    [Column("дата_продажи")] public DateOnly SaleDate { get; set; }
}

[Table("сотрудники", Schema = "салон_сотовой_связи")]
public class Employee
{
    [Key, Column("id_сотрудника")] public int Id { get; set; }
    [Column("фамилия")] public string? LastName { get; set; }
    [Column("имя")] public string? FirstName { get; set; }
    [Column("отчество")] public string? MiddleName { get; set; }
    [Column("телефон")] public string? Phone { get; set; }
    [Column("email")] public string? Email { get; set; }
    [Column("id_должности")] public int PositionId { get; set; }
    [Column("дата_приёма")] public DateOnly HireDate { get; set; }
    [Column("зарплата")] public decimal Salary { get; set; }

    [NotMapped]
    public string FullName => string.Join(' ', new[] { LastName, FirstName, MiddleName }.Where(x => !string.IsNullOrWhiteSpace(x)));
}

[Table("категории_устройств", Schema = "салон_сотовой_связи")]
public class DeviceCategory
{
    [Key, Column("id_категории")] public int Id { get; set; }
    [Column("название_категории")] public string Name { get; set; } = string.Empty;
    [Column("описание_категории")] public string? Description { get; set; }
}

[PrimaryKey(nameof(PurchaseId), nameof(DeviceId), nameof(SerialNumber))]
[Table("позиции_закупок", Schema = "салон_сотовой_связи")]
public class PurchaseLine
{
    [Column("id_закупки")] public int PurchaseId { get; set; }
    [Column("id_устройства")] public int DeviceId { get; set; }
    [Column("серийный_номер")] public string SerialNumber { get; set; } = string.Empty;
    [Column("цена_за_единицу")] public decimal UnitPrice { get; set; }
}

[PrimaryKey(nameof(SaleId), nameof(DeviceId), nameof(SerialNumber))]
[Table("позиции_продаж", Schema = "салон_сотовой_связи")]
public class SaleLine
{
    [Column("id_продажи")] public int SaleId { get; set; }
    [Column("id_устройства")] public int DeviceId { get; set; }
    [Column("серийный_номер")] public string SerialNumber { get; set; } = string.Empty;
}

[Table("производители_устройств", Schema = "салон_сотовой_связи")]
public class DeviceManufacturer
{
    [Key, Column("id_производителя")] public int Id { get; set; }
    [Column("название_производителя")] public string Name { get; set; } = string.Empty;
}

[Table("устройства", Schema = "салон_сотовой_связи")]
public class DeviceEntity
{
    [Key, Column("id_устройства")] public int Id { get; set; }
    [Column("id_производителя")] public int ManufacturerId { get; set; }
    [Column("id_категории")] public int CategoryId { get; set; }
    [Column("модель")] public string Model { get; set; } = string.Empty;
    [Column("характеристики")] public string? Specifications { get; set; }
    [Column("розничная_цена")] public decimal RetailPrice { get; set; }
}

[Table("гарантии", Schema = "салон_сотовой_связи")]
public class Warranty
{
    [Key, Column("id_гарантии")] public int Id { get; set; }
    [Column("серийный_номер")] public string SerialNumber { get; set; } = string.Empty;
    [Column("дата_начала_гарантии")] public DateOnly StartDate { get; set; }
    [Column("дата_окончания_гарантии")] public DateOnly? EndDate { get; set; }
    [Column("условия_гарантии")] public string? Terms { get; set; }
    [Column("активна")] public bool? IsActive { get; set; }
    [Column("id_устройства")] public int? DeviceId { get; set; }
}

[Table("журнал_входов", Schema = "салон_сотовой_связи")]
public class LoginJournal
{
    [Key, Column("id_входа")] public long Id { get; set; }
    [Column("тип_пользователя")] public string UserType { get; set; } = string.Empty;
    [Column("id_пользователя")] public int UserId { get; set; }
    [Column("время_входа")] public DateTime? LoginTime { get; set; }
    [Column("ip_адрес")] public string? IpAddress { get; set; }
    [Column("user_agent")] public string? UserAgent { get; set; }
    [Column("токен")] public string? Token { get; set; }
}

[PrimaryKey(nameof(ContractId), nameof(ServiceId), nameof(EmployeeId))]
[Table("заказы_на_услуги", Schema = "салон_сотовой_связи")]
public class ServiceOrder
{
    [Column("id_договора")] public int ContractId { get; set; }
    [Column("id_услуги")] public int ServiceId { get; set; }
    [Column("id_сотрудника")] public int EmployeeId { get; set; }
    [Column("дата_выполнения")] public DateOnly? CompletedDate { get; set; }
    [Column("статус")] public string? Status { get; set; }
    [Column("примечания")] public string? Notes { get; set; }
}

[Table("закупки", Schema = "салон_сотовой_связи")]
public class PurchaseEntity
{
    [Key, Column("id_закупки")] public int Id { get; set; }
    [Column("id_поставщика")] public int SupplierId { get; set; }
    [Column("id_сотрудника")] public int? EmployeeId { get; set; }
    [Column("дата_закупки")] public DateOnly PurchaseDate { get; set; }
}

[Table("обращения_клиентов", Schema = "салон_сотовой_связи")]
public class CustomerTicket
{
    [Key, Column("id_обращения")] public int Id { get; set; }
    [Column("id_клиента")] public int ClientId { get; set; }
    [Column("id_устройства")] public int? DeviceId { get; set; }
    [Column("id_сотрудника")] public int? EmployeeId { get; set; }
    [Column("тема")] public string Subject { get; set; } = string.Empty;
    [Column("описание")] public string Description { get; set; } = string.Empty;
    [Column("статус")] public string? Status { get; set; }
    [Column("приоритет")] public string? Priority { get; set; }
    [Column("дата_создания")] public DateTime? CreatedAt { get; set; }
    [Column("дата_решения")] public DateTime? ResolvedAt { get; set; }
    [Column("ответ")] public string? Answer { get; set; }
}

[Table("платежи", Schema = "салон_сотовой_связи")]
public class PaymentEntity
{
    [Key, Column("id_платежа")] public int Id { get; set; }
    [Column("id_договора")] public int ContractId { get; set; }
    [Column("id_способа_оплаты")] public int PaymentMethodId { get; set; }
    [Column("дата_платежа")] public DateOnly PaymentDate { get; set; }
}

[Table("поставщики", Schema = "салон_сотовой_связи")]
public class Supplier
{
    [Key, Column("id_поставщика")] public int Id { get; set; }
    [Column("название_компании")] public string CompanyName { get; set; } = string.Empty;
    [Column("адрес")] public string? Address { get; set; }
    [Column("телефон")] public string? Phone { get; set; }
    [Column("email")] public string? Email { get; set; }
}

[Table("сессии", Schema = "салон_сотовой_связи")]
public class UserSession
{
    [Key, Column("id_сессии")] public string Id { get; set; } = string.Empty;
    [Column("Тип_пользователя")] public string UserType { get; set; } = string.Empty;
    [Column("id_пользователя")] public int UserId { get; set; }
    [Column("Токен_обновления")] public string? RefreshToken { get; set; }
    [Column("токен")] public string? Token { get; set; }
    [Column("ip_адрес")] public string? IpAddress { get; set; }
    [Column("user_agent")] public string? UserAgent { get; set; }
    [Column("Время_создания")] public DateTime? CreatedAt { get; set; }
    [Column("Время_истечения")] public DateTime ExpiresAt { get; set; }
    [Column("Время_выхода")] public DateTime? LoggedOutAt { get; set; }
    [Column("Активна")] public bool? IsActive { get; set; }
}

[Table("способы_оплаты", Schema = "салон_сотовой_связи")]
public class PaymentMethod
{
    [Key, Column("id_способа_оплаты")] public int Id { get; set; }
    [Column("название_способа_оплаты")] public string Name { get; set; } = string.Empty;
}

[Table("услуги", Schema = "салон_сотовой_связи")]
public class ServiceEntity
{
    [Key, Column("id_услуги")] public int Id { get; set; }
    [Column("название_услуги")] public string Name { get; set; } = string.Empty;
    [Column("описание_услуги")] public string? Description { get; set; }
    [Column("стоимость")] public decimal Cost { get; set; }
}

[Table("учетные_записи_сотрудников", Schema = "салон_сотовой_связи")]
public class UserAccount
{
    [Key, Column("id_учетной_записи")] public int Id { get; set; }
    [Column("id_сотрудника")] public int? EmployeeId { get; set; } = null;
    [Column("Логин")] public string Login { get; set; } = string.Empty;
    [Column("Хеш_пароля")] public string PasswordHash { get; set; } = string.Empty;
    [Column("Роль")] public string Role { get; set; } = string.Empty;
    [Column("Двухфакторный_ключ")] public string? TwoFactorKey { get; set; } = null;
    [Column("Активен")] public bool? IsActive { get; set; }
    [Column("Попыток_входа")] public int? LoginAttempts { get; set; }
    [Column("Последний_вход")] public DateTime? LastLoginAt { get; set; }
    [Column("id_клиента")] public int? ClientId { get; set; } = null;
}
