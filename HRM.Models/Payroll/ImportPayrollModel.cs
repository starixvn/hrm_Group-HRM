namespace HRM.Models.Payroll;

public class ImportPayrollModel
{
    public string EmpId { get; set; }
    public decimal TotalPersonalCost { get; set; }
    public decimal ExtraPayment { get; set; }
    public bool IsExtraPayment { get; set; }
}
