namespace HRM.Criteria.EmployeePayroll;

using System;
using Fanex.Data.Repository;

public class GetEmployeePayrollCriteria : CriteriaBase
{
    public DateTime Month { get; set; }

    public int CompanyId { get; set; }

    public override string GetSettingKey()
    {
        return "EmployeePayroll_Get";
    }

    public override bool IsValid()
    {
        return Month != default && CompanyId > 0;
    }
}