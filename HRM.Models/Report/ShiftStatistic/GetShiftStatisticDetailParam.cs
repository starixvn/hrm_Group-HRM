using System;

namespace HRM.Models.Report.ShiftStatistic;

public class GetShiftStatisticDetailParam
{
    public int SupervisorId { get; set; }
    public int? EmployeeId { get; set; }
    public int? ShiftId { get; set; }
    public DateTime WorkDate { get; set; } = DateTimeOffset.Now.Date;
    public string TeamIds { get; set; }
    public bool IsShowWholeCompany { get; set; }
    public byte? IsLocal { get; set; }
    public short CompanyId { get; set; }
}