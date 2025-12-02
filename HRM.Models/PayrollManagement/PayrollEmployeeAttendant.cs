using System;

namespace HRM.Models.PayrollManagement;

public class PayrollEmployeeAttendant
{
    public int EmpId { get; set; }
    public decimal TotalUnpaidLeaveDays { get; set; }
    public decimal TotalCompanyEventDays { get; set; }
    public decimal TotalNewHireWorkingDays { get; set; }
    public decimal TotalWorkingDays { get; set; }
    public int WorkingDays { get; set; }
    public bool IsResigned { get; set; }
    public bool IsMaternity { get; set; }
    public bool IsNewHire { get; set; }
    public DateTime? CompanyDate { get; set; }
    public DateTime? JoinedDate { get; set; }
    public DateTime? ConfirmedDate { get; set; }
    public decimal ActualPaidDays { get; set; }
    public string EncryptBasicSalary { get; set; }
    public string EncryptJobLevelAddOn { get; set; }
    public string CurrencyCode { get; set; }
    public string EncryptSpecialSubsidy { get; set; }

    public string SpecialSubsidyCurrencyCode { get; set; }

    // Payroll Calculation (not rounded)
    public decimal SpecialSubsidy { get; set; }
    public decimal GrossSalary { get; set; }
    public decimal GrossSalaryPerDay => WorkingDays > 0 ? GrossSalary / WorkingDays : 0;
    public decimal ManagementAllowance { get; set; }
    public decimal TransportationAllowance { get; set; }
    public decimal MobilephoneAllowance { get; set; }
    public decimal TotalIncome => GrossSalary + ManagementAllowance + TransportationAllowance + MobilephoneAllowance;
}