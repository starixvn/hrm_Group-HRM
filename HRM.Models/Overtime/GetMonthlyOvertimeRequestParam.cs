namespace HRM.Models.Overtime;

using System;

public class GetMonthlyOvertimeRequestParam
{
    public int ActorId { get; set; }
    public DateTime? Month { get; set; }
    public int? EmployeeId { get; set; }
    public string TeamIds { get; set; }
    public string StatusIds { get; set; }
    public bool IsProcessor { get; set; }
    public short OvertimeType { get; set; }
    public byte? IsLocal { get; set; }
    public int CompanyId { get; set; }
    public bool IsShowCrossCompany { get; set; }
    public string EmployeeStatusIds { get; set; }
}