namespace HRM.Criteria.ScheduleArrange;

using System;
using Fanex.Data.Repository;

public class EditWeeklyScheduleCriteria : CriteriaBase
{
    public int ActorId { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public string WeeklyScheduleXml { get; set; }
    public string DefaultShiftXml { get; set; }

    public override string GetSettingKey() => "ScheduleArrangement_UpdateWeekly";

    public override bool IsValid() => ActorId > 0;
}