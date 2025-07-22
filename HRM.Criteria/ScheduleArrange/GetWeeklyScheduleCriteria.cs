namespace HRM.Criteria.ScheduleArrange;

using System;
using Fanex.Data.Repository;

public class GetWeeklyScheduleCriteria : CriteriaBase
{
    public DateTime FromTime { get; set; }
    public DateTime ToTime { get; set; }
    public int ActorId { get; set; }
    public int? EmpId { get; set; }
    public string TeamIds { get; set; }
    public bool IsIFrame { get; set; }
    public bool? IsLocal { get; set; }
    public short CompanyId { get; set; }
    public short CurrentCompanyId { get; set; }
    public bool IsEditScheduleUnlimited { get; set; }

    public override string GetSettingKey() => "ScheduleArrangement_GetWeekly";

    public override bool IsValid() => true;
}