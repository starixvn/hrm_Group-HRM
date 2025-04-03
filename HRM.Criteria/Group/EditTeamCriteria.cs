namespace HRM.Criteria.Group;

using Fanex.Data.Repository;

public class EditTeamCriteria : CriteriaBase
{
    public int TeamId { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public int DepartmentId { get; set; }
    public string MailGroup { get; set; }
    public int PICEmpId { get; set; }
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
    public int ModifiedBy { get; set; }

    public override string GetSettingKey()
    {
        return "Group_Update";
    }

    public override bool IsValid()
    {
        return TeamId > 0 &&
               !string.IsNullOrWhiteSpace(Code) &&
               !string.IsNullOrWhiteSpace(Name) &&
               PICEmpId > 0;
    }
}