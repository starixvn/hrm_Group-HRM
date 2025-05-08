namespace HRM.Criteria.EmployeePayroll;

using System;
using Fanex.Data.Repository;

public class GetSalaryUploadHistoryCriteria : CriteriaBase
{
    public int CompanyId { get; set; }

    public override string GetSettingKey()
    {
        return "SalaryUploadHistory_Get";
    }

    public override bool IsValid()
    {
        return CompanyId > 0;
    }
}