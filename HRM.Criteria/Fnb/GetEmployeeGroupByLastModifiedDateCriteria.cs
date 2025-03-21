using System;
using Fanex.Data.Repository;

namespace HRM.Criteria.Fnb;

public class GetEmployeeGroupByLastModifiedDateCriteria : CriteriaBase
{
    public DateTime LastModifiedDate { get; set; }

    public override string GetSettingKey() => "FnB_EmployeeGroup_GetByLastModifiedDate";

    public override bool IsValid() => true;
}