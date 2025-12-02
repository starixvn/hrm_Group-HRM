namespace HRM.Models.PayrollManagement;

public class CalculateAllowanceRequest
{
    public int EmpId { get; set; }
    public decimal ActualPaidDays { get; set; }
    public decimal CompanyEvent { get; set; }
    public int StandardWorkingDays { get; set; }
    public bool IsNewHire { get; set; }
    public bool IsResigned { get; set; }
    public bool IsMaternity { get; set; }
    public decimal TotalUnpaidLeaveDays { get; set; }
    public decimal TotalWorkingDays { get; set; }
}
