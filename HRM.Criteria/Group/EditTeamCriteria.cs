namespace HRM.Criteria.Group;

using Fanex.Data.Repository;

public class EditTeamCriteria : CriteriaBase
{
    public int TeamId { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public int GroupTeamId { get; set; }
    public int DepartmentId { get; set; }
    public int? PICEmpId { get; set; }
    public string AccountableArea { get; set; }
    public int ModifiedBy { get; set; }
    public string MailGroup { get; set; }
    public int LocalBasicAnnualLeaveBalance { get; set; }
    public int LocalWorkingDays { get; set; }
    public int LocalWorkingHours { get; set; }
    public int LocalOffDaysbyWeek { get; set; }
    public byte? LocalWorkingHoursbyWeek { get; set; }
    public byte? LocalYearsOfServiceForExtraLeave { get; set; }
    public int ExpatBasicAnnualLeaveBalance { get; set; }
    public int ExpatWorkingDays { get; set; }
    public int ExpatWorkingHours { get; set; }
    public int ExpatOffDaysbyWeek { get; set; }
    public byte? ExpatWorkingHoursbyWeek { get; set; }
    public byte? ExpatYearsOfServiceForExtraLeave { get; set; }
    public bool IsOutsource { get; set; }

    public override string GetSettingKey()
    {
        return "Group_Update";
    }

    public override bool IsValid()
    {
        return TeamId > 0 &&
               !string.IsNullOrWhiteSpace(Code) &&
               !string.IsNullOrWhiteSpace(Name);
    }
}