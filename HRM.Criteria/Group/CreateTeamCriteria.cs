namespace HRM.Criteria.Group;

using Fanex.Data.Repository;

public class CreateTeamCriteria : CriteriaBase
{
    public string Code { get; set; }
    public string Name { get; set; }
    public int SectionId { get; set; }
    public string MailGroup { get; set; }
    public int HeadId { get; set; }
    public string ManagerIds { get; set; }
    public string LeaderIds { get; set; }
    public string SupervisorIds { get; set; }
    public byte LocalBasicAnnualLeaveBalance { get; set; }
    public byte LocalWorkingDays { get; set; }
    public byte LocalWorkingHours { get; set; }
    public byte LocalOffDaysbyWeek { get; set; }
    public byte? LocalWorkingHoursbyWeek { get; set; }
    public byte ExpatBasicAnnualLeaveBalance { get; set; }
    public byte ExpatWorkingDays { get; set; }
    public byte ExpatWorkingHours { get; set; }
    public byte ExpatOffDaysbyWeek { get; set; }
    public byte? ExpatWorkingHoursbyWeek { get; set; }
    public int CreatedBy { get; set; }

    public override string GetSettingKey()
    {
        return "Group_Insert";
    }

    public override bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(Code)
               && !string.IsNullOrWhiteSpace(Name)
               && HeadId > 0;
    }
}