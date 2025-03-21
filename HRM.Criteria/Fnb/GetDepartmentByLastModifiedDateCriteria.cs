using System;
using Fanex.Data.Repository;

namespace HRM.Criteria.Fnb;

public class GetDepartmentByLastModifiedDateCriteria : CriteriaBase
{
    public DateTime LastModifiedDate { get; set; }

    public override string GetSettingKey() => "FnB_Department_GetByLastModifiedDate";

    public override bool IsValid() => true;
}