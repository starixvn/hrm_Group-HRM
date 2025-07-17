namespace HRM.Models;
using System;


public class Team : EntityBase<int>
{
    public DateTime? CreatedDate { get; set; }

    public Employee Creator { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public Employee Modifier { get; set; }

    public int NoOfMembers { get; set; }

    public int TotalLocal { get; set; }

    public int TotalExpat { get; set; }

    public string MailGroup { get; set; }

    public bool IsOutsource { get; set; }

    public byte? LocalBasicAnnualLeaveBalance { get; set; }

    public byte? LocalWorkingDays { get; set; }

    public byte? LocalWorkingHours { get; set; }

    public byte? LocalOffDaysbyWeek { get; set; }

    public byte? LocalWorkingHoursbyWeek { get; set; }

    public byte? LocalYearsOfServiceForExtraLeave { get; set; }

    public byte? ExpatBasicAnnualLeaveBalance { get; set; }

    public byte? ExpatWorkingDays { get; set; }

    public byte? ExpatWorkingHours { get; set; }

    public byte? ExpatOffDaysbyWeek { get; set; }

    public byte? ExpatWorkingHoursbyWeek { get; set; }

    public byte? ExpatYearsOfServiceForExtraLeave { get; set; }

    public Department Department { get; set; } = new();

    public int? PICEmpId { get; set; }

    public string PICNickName { get; set; }

    public int PICCompanyId { get; set; }

    public string PICCompanyName { get; set; }

    public int GroupTeamId { get; set; }

    public string AccountableArea { get; set; }

    public string UnitCode { get; set; }
}