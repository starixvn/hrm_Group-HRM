namespace HRM.Models;

using Group;
using System;
using System.Collections.Generic;

public class Team : EntityBase<int>
{
    public Section.Section Section { get; set; } = new();

    public IEnumerable<TeamManager> Managers { get; set; } = new List<TeamManager>();

    public DateTime? CreatedDate { get; set; }

    public Employee Creator { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public Employee Modifier { get; set; }

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

    public Department Department { get; set; } = new();

    public string DepartmentCode { get; set; }
}