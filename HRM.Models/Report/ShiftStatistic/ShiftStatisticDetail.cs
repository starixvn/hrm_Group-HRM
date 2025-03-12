using System;

namespace HRM.Models.Report.ShiftStatistic;

public class ShiftStatisticDetail
{
    public DateTime WorkDate { get; set; }
    public string Nickname { get; set; }
    public string FullName { get; set; }
    public string Team { get; set; }
    public string Shift { get; set; }
}