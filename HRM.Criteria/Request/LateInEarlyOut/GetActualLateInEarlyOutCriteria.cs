namespace HRM.Criteria.Request.LateInEarlyOut;

using System;
using Fanex.Data.Repository;

public class GetActualLateInEarlyOutCriteria : CriteriaBase
{
    public int EmployeeId { get; set; }

    public DateTime WorkDate { get; set; }

    public override string GetSettingKey() => "TimeSheet_GetLateInEarlyOut";

    public override bool IsValid() => EmployeeId > 0;
}