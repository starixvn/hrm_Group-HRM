using System;

namespace HRM.Models.PayrollManagement;

public class Payroll
{
    public int EmpId { get; set; }

    public string EncryptBasicSalary { get; set; }
    public string EncryptJobLevelAddOn { get; set; }
    public string CurrencyCode { get; set; }

    public string EncryptSpecialSubsidy { get; set; }
    public string SpecialSubsidyCurrencyCode { get; set; }
}

public class LocalPayrollAllowance
{
    public int EmpId { get; set; }
    public string Type { get; set; } = string.Empty;
    public bool Taxable { get; set; }
    public string ItemName { get; set; }
    public string EncryptAmount { get; set; }
    public string CurrencyCode { get; set; }
}

public class PayrollDetail
{
    public decimal BasicSalary { get; set; }
    public decimal JobLevelAddOn { get; set; }
    public decimal ManagementAllowance { get; set; }
    public decimal OverseasAllowance { get; set; }
    public decimal SkillSetAddOn { get; set; }
    public decimal Others { get; set; }
    public decimal Total => BasicSalary + JobLevelAddOn + ManagementAllowance + OverseasAllowance + SkillSetAddOn + Others;

    public decimal BasicSalaryDisplay => Math.Round(BasicSalary, 0);
    public decimal JobLevelAddOnDisplay => Math.Round(JobLevelAddOn, 0);
    public decimal ManagementAllowanceDisplay => Math.Round(ManagementAllowance, 0);
    public decimal OverseasAllowanceDisplay => Math.Round(OverseasAllowance, 0);
    public decimal SkillSetAddOnDisplay => Math.Round(SkillSetAddOn, 0);
    public decimal OthersDisplay => Math.Round(Others, 0);
    public decimal TotalDisplay { get; set; }
}

public class AllowanceItem
{
    public string ItemName { get; set; }
    public decimal Amount { get; set; }
}