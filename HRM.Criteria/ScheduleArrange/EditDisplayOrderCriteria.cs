namespace HRM.Criteria.ScheduleArrange;

using Fanex.Data.Repository;

public class EditDisplayOrderCriteria : CriteriaBase
{
    public int ActorId { get; set; }
    public string DisplayOrderXml { get; set; }

    public override string GetSettingKey() => "ScheduleArrangement_UpdateDisplayOrder";

    public override bool IsValid() => ActorId > 0;
}