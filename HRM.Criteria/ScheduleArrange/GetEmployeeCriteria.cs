namespace HRM.Criteria.ScheduleArrange;

using Fanex.Data.Repository;

public class GetEmployeeCriteria : CriteriaBase
{
    public int ActorId { get; set; }
    public string TeamIds { get; set; }
    public string Query { get; set; }
    public bool IsIFrame { get; set; }

    public override string GetSettingKey() => "ScheduleArrangement_GetEmployee";

    public override bool IsValid() => true;
}