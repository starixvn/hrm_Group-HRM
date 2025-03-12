using System;

namespace HRM.Models.Report.ShiftStatistic;

public class ShiftStatistic
{
    public int ShiftId { get; set; }
    public string Code { get; set; }
    public DateTime WorkDate { get; set; }
    public int Total { get; set; }
}