namespace HRM.AutoJobs.Criteria;

using Fanex.Data.Repository;

public class PublicScheduleCommand : NonQueryCommand
{
    public int? EmployeeId { get; set; }

    public override string GetSettingKey() => "Auto_EmployeeCalendar_UpdateIsPublic";

    public override bool IsValid() => true;
}