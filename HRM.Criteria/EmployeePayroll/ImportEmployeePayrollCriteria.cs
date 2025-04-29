namespace HRM.Criteria.EmployeePayroll;

using System;
using Fanex.Data.Repository;

public class ImportEmployeePayrollCriteria : CriteriaBase
{
    public string EmployeeListJson { get; set; }

    public DateTime Month { get; set; }

    public short PeriodType { get; set; }

    public int ActorId { get; set; }

    public int CompanyId { get; set; }

    public override string GetSettingKey()
    {
        return "EmployeePayroll_Import";
    }

    public override bool IsValid()
    {
        return !string.IsNullOrEmpty(EmployeeListJson) && Month != default && CompanyId > 0;
    }
}