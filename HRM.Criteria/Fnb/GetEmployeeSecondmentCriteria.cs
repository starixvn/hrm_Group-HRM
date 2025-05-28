using System;
using Fanex.Data.Repository;

namespace HRM.Criteria.Fnb;

public class GetEmployeeSecondmentCriteria : CriteriaBase
{
    public int CompanyId { get; set; }

    public override string GetSettingKey() => "FnB_Employee_GetSecondmentCompany";

    public override bool IsValid() => true;
}