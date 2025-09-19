namespace HRM.Models.Report.MonthlyWorkingHours;

using System;

public class GetMonthlyWorkingHoursParam
{
    public int ActorId { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public string TeamIds { get; set; }
    public string Employee { get; set; }
    public byte? IsLocal { get; set; }
    public short CompanyId { get; set; }
    public string EmployeeStatusIds { get; set; }
}