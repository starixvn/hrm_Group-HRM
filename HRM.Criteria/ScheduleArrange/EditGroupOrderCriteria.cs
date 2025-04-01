namespace HRM.Criteria.ScheduleArrange;

using Fanex.Data.Repository;

public class EditTeamOrderCriteria : CriteriaBase
{
    public int ActorId { get; set; }
    public string DisplayOrderXml { get; set; }

    public override string GetSettingKey() => "ScheduleArrangement_UpdateTeamOrder";

    public override bool IsValid() => ActorId > 0;
}