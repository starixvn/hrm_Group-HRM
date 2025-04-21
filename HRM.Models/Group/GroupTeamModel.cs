using System;

namespace HRM.Models.Group;

public class GroupTeamModel
{
    public int TeamId { get; set; }

    public string UnitCode { get; set; }

    public string DepartmentName { get; set; }

    public string DepartmentCode { get; set; }

    public string DepartmentPIC { get; set; }

    public string TeamName { get; set; }

    public string TeamCode { get; set; }

    public string TeamPIC { get; set; }

    public int NoOfMembers { get; set; }

    public int TotalLocal { get; set; }

    public int TotalExpat { get; set; }

    public string MailGroup { get; set; }

    public byte? LocalBasicAnnualLeaveBalance { get; set; }

    public byte? LocalWorkingDays { get; set; }

    public byte? LocalWorkingHours { get; set; }

    public byte? LocalOffDaysbyWeek { get; set; }

    public byte? LocalWorkingHoursbyWeek { get; set; }

    public byte? ExpatBasicAnnualLeaveBalance { get; set; }

    public byte? ExpatWorkingDays { get; set; }

    public byte? ExpatWorkingHours { get; set; }

    public byte? ExpatOffDaysbyWeek { get; set; }

    public byte? ExpatWorkingHoursbyWeek { get; set; }

    public DateTime? ModifiedDate { get; set; }
}