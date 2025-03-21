using System;
using Fanex.Data.Repository;

namespace HRM.Criteria.Fnb;

public class GetEmployeeCalendarByLastModifiedDateCriteria : CriteriaBase
{
    public DateTime LastModifiedDate { get; set; }

    public override string GetSettingKey() => "FnB_EmployeeCalendar_GetByLastModifiedDate";

    public override bool IsValid() => true;
}