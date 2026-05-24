using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebCellSalon.Models;

public class HomeIndexViewModel
{
    public List<DeviceCatalogView> Devices { get; set; } = [];
    public List<ActiveTariffView> Tariffs { get; set; } = [];
}

public class LoginViewModel
{
    [Required(ErrorMessage = "Введите логин")]
    [Display(Name = "Логин")]
    public string Login { get; set; } = string.Empty;

    [Required(ErrorMessage = "Введите пароль")]
    [DataType(DataType.Password)]
    [Display(Name = "Пароль")]
    public string Password { get; set; } = string.Empty;
}

public class ProfilePageViewModel
{
    public Customer? Customer { get; set; }
    public CustomerOwnBonusView? Bonus { get; set; }
    public int ActiveContractsCount { get; set; }
    public CreateTicketViewModel TicketForm { get; set; } = new();
    public List<SelectListItem> DeviceOptions { get; set; } = [];
}

public class CreateTicketViewModel
{
    [Display(Name = "Устройство")]
    public int? DeviceId { get; set; }

    [Required(ErrorMessage = "Укажите тему обращения")]
    [Display(Name = "Тема")]
    public string Subject { get; set; } = string.Empty;

    [Required(ErrorMessage = "Опишите проблему")]
    [Display(Name = "Описание")]
    public string Description { get; set; } = string.Empty;
}

public class RegisterSaleViewModel
{
    [Required(ErrorMessage = "Выберите клиента")]
    [Display(Name = "Клиент")]
    public int ClientId { get; set; }

    [Required(ErrorMessage = "Укажите серийные номера")]
    [Display(Name = "Серийные номера")]
    public string SerialNumbers { get; set; } = string.Empty;

    [Required(ErrorMessage = "Выберите способ оплаты")]
    [Display(Name = "Способ оплаты")]
    public int PaymentMethodId { get; set; }

    public List<SelectListItem> ClientOptions { get; set; } = [];
    public List<SelectListItem> PaymentMethodOptions { get; set; } = [];
}

public class RegisterContractViewModel
{
    [Required(ErrorMessage = "Выберите клиента")]
    [Display(Name = "Клиент")]
    public int ClientId { get; set; }

    [Required(ErrorMessage = "Укажите ICCID SIM-карты")]
    [Display(Name = "ICCID SIM-карты")]
    public string SimIccid { get; set; } = string.Empty;

    [Required(ErrorMessage = "Укажите тип договора")]
    [Display(Name = "Тип договора")]
    public string ContractType { get; set; } = "мобильная связь";

    public List<SelectListItem> ClientOptions { get; set; } = [];
}

public class RegisterRepairViewModel
{
    [Required(ErrorMessage = "Выберите клиента")]
    [Display(Name = "Клиент")]
    public int ClientId { get; set; }

    [Display(Name = "Устройство")]
    public int? DeviceId { get; set; }

    [Display(Name = "Серийный номер")]
    public string? SerialNumber { get; set; }

    [Required(ErrorMessage = "Укажите тип неисправности")]
    [Display(Name = "Тип неисправности")]
    public string FaultType { get; set; } = string.Empty;

    [Display(Name = "Описание проблемы")]
    public string? ProblemDescription { get; set; }

    [Display(Name = "Примечания")]
    public string? Notes { get; set; }

    public List<SelectListItem> ClientOptions { get; set; } = [];
    public List<SelectListItem> DeviceOptions { get; set; } = [];
}

public class CloseRepairViewModel
{
    public int RepairId { get; set; }
    public string RepairTitle { get; set; } = string.Empty;

    [Required(ErrorMessage = "Укажите итоговую стоимость")]
    [Display(Name = "Итоговая стоимость")]
    public decimal FinalCost { get; set; }

    [Display(Name = "Примечания")]
    public string? Notes { get; set; }
}

public class EmployeeEditViewModel
{
    public int? Id { get; set; }

    [Display(Name = "Фамилия")]
    public string? LastName { get; set; }

    [Display(Name = "Имя")]
    public string? FirstName { get; set; }

    [Display(Name = "Отчество")]
    public string? MiddleName { get; set; }

    [Display(Name = "Телефон")]
    public string? Phone { get; set; }

    [Display(Name = "Email")]
    [EmailAddress(ErrorMessage = "Некорректный email")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Выберите должность")]
    [Display(Name = "Должность")]
    public int PositionId { get; set; }

    [Required(ErrorMessage = "Укажите дату приёма")]
    [Display(Name = "Дата приёма")]
    [DataType(DataType.Date)]
    public DateOnly HireDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);

    [Required(ErrorMessage = "Укажите зарплату")]
    [Display(Name = "Зарплата")]
    public decimal Salary { get; set; }

    public List<SelectListItem> PositionOptions { get; set; } = [];
}

public class AdminUsersPageViewModel
{
    public List<AccountListItemView> Accounts { get; set; } = [];
    public CreateUserViewModel CreateUserForm { get; set; } = new();
}

public class CreateUserViewModel
{
    [Display(Name = "ID сотрудника")]
    public int? EmployeeId { get; set; }

    [Display(Name = "ID клиента")]
    public int? ClientId { get; set; }

    [Required(ErrorMessage = "Введите логин")]
    [Display(Name = "Логин")]
    public string Login { get; set; } = string.Empty;

    [Required(ErrorMessage = "Введите пароль")]
    [DataType(DataType.Password)]
    [Display(Name = "Пароль")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Укажите роль")]
    [Display(Name = "Роль")]
    public string Role { get; set; } = "клиент";
}

public class ResetPasswordViewModel
{
    public int AccountId { get; set; }

    [Required(ErrorMessage = "Введите новый пароль")]
    [DataType(DataType.Password)]
    [Display(Name = "Новый пароль")]
    public string NewPassword { get; set; } = string.Empty;
}

public class EmployeeStatsPageViewModel
{
    public List<EmployeePerformanceView> Items { get; set; } = [];
    public decimal CurrentMonthSalesAmount { get; set; }
    public decimal CurrentMonthPurchasesAmount { get; set; }
}
