using System;
using Fanex.Data.Repository;

namespace HRM.Criteria.Fnb;

public class GetEmployeeByLastModifiedDateCriteria : CriteriaBase
{
    public DateTime LastModifiedDate { get; set; }

    public override string GetSettingKey() => "FnB_Employee_GetByLastModifiedDate";

    public override bool IsValid() => true;
}