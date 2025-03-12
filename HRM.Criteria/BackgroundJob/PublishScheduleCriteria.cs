namespace HRM.Criteria.BackgroundJob;

using Fanex.Data.Repository;

public class PublishScheduleCriteria : CriteriaBase
{
    public int? EmployeeId { get; set; }

    public override string GetSettingKey() => "Auto_EmployeeCalendar_UpdateIsPublic";

    public override bool IsValid() => true;
}