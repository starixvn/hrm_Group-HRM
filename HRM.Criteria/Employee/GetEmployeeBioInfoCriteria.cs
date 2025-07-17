using System;
using Fanex.Data.Repository;

namespace HRM.Criteria.Employee;

public class GetEmployeeBioInfoCriteria : CriteriaBase
{
    public int EmpId { get; set; }

    public int CompanyId { get; set; }

    public DateTime CurrentDate { get; set; }

    public override string GetSettingKey() => "EmployeeBioInfo_Get";

    public override bool IsValid() => EmpId > 0 && CompanyId > 0;
}