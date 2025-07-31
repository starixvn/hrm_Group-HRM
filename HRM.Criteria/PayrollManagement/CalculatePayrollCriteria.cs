namespace HRM.Criteria.PayrollManagement;

using System;
using Fanex.Data.Repository;

public class CalculatePayrollCriteria : CriteriaBase
{
    public int ModifiedBy { get; set; }
    public int CompanyId { get; set; }
    public DateTime Date { get; set; }
    public string PayrollListJson { get; set; }

    public override string GetSettingKey() => "Payroll_Calculate";

    public override bool IsValid() => CompanyId > 0 && Date != DateTime.MinValue;
}