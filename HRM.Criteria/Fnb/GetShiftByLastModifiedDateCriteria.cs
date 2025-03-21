using System;
using Fanex.Data.Repository;

namespace HRM.Criteria.Fnb;

public class GetShiftByLastModifiedDateCriteria : CriteriaBase
{
    public DateTime LastModifiedDate { get; set; }

    public override string GetSettingKey() => "FnB_Shift_GetByLastModifiedDate";

    public override bool IsValid() => true;
}