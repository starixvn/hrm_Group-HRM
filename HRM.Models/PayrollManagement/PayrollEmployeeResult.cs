using System;
using HRM.Core.Enums;

namespace HRM.Models.PayrollManagement;

public class PayrollEmployeeResult
{
    public int EmpId { get; set; }
    public int WorkingDays { get; set; }
    public decimal PaidDays { get; set; }
    public decimal UnpaidLeave { get; set; }
    public string BasicSalary { get; set; }
    public string JobLevelAddOn { get; set; }
    public string SpecialSubsidy { get; set; }
    public string GrossSalary { get; set; }
    public DateTime? CompanyDate { get; set; }
    public DateTime? ConfirmedDate { get; set; }
    public EmploymentStatus EmploymentStatus { get; set; }
    public DateTime GeneratedDate { get; set; }
    // Allowance
    public string ManagementAllowance { get; set; }
    public string TransportationAllowance { get; set; }
    public string MobilephoneAllowance { get; set; }
    //TotalIncome
    public string TotalIncome { get; set; }
}