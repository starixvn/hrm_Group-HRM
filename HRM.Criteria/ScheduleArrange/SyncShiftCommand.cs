namespace HRM.Criteria.ScheduleArrange;

using Fanex.Data.Repository;

public class SyncShiftCommand : CriteriaBase
{
    public short CompanyId { get; set; }
    public int ActorId { get; set; }
    public string ShiftXml { get; set; }

    public override string GetSettingKey() => "ScheduleArrangement_SyncShift";

    public override bool IsValid() => ActorId > 0 && !string.IsNullOrWhiteSpace(ShiftXml);
}