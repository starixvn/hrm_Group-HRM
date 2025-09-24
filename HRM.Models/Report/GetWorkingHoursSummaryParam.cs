namespace HRM.Models.Report;

using System;

public class GetWorkingHoursSummaryParam
{
    public short CompanyId { get; set; }
    public int ActorId { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public byte? IsLocal { get; set; }
    public string TeamIds { get; set; }
    public string Employee { get; set; }
    public string EmployeeStatusIds { get; set; }
}