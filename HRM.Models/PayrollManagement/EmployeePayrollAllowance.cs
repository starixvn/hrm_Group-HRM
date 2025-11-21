namespace HRM.Models.PayrollManagement;

public class EmployeePayrollAllowance
{
    public int AllowanceItemSettingId { get; set; }

    public string ItemName { get; set; }

    public string Amount { get; set; }

    public string CurrencyCode { get; set; }

    public bool IsPercent { get; set; }

    public string AllowanceName { get; set; }

    public bool Taxable { get; set; }

    public bool IsMultiSelect { get; set; }

}

