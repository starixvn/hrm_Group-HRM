namespace HRM.Criteria.EmployeePayroll;

using System;
using Fanex.Data.Repository;

public class ConfirmUploadPayrollCriteria : CriteriaBase
{
    public DateTime Month { get; set; }

    public int CompanyId { get; set; }

    public short PeriodType { get; set; }

    public override string GetSettingKey()
    {
        return "EmployeePayroll_ConfirmUpload";
    }

    public override bool IsValid()
    {
        return Month != default && CompanyId > 0;
    }
}