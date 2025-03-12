namespace HRM.Models.AnnualLeaveBalance;

using System;

public class AdjustAnnualLeaveBalance
{
    public string EmployeeIds { get; set; }
    public string Description { get; set; }
    public DateTime AffectedFrom { get; set; }
    public decimal DeductDays { get; set; }
    public decimal CreditDays { get; set; }
}