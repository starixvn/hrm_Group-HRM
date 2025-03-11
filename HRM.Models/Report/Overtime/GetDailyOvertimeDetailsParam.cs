namespace HRM.Models.Report.Overtime;

using System;

public class GetDailyOvertimeDetailsParam
{
    public short CompanyId { get; set; }
    public int ActorId { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public string Employee { get; set; }
    public string TeamIds { get; set; }
    public byte? IsLocal { get; set; }
    public byte? EmployeeStatusId { get; set; }
}