using System.Collections.Generic;
using HRM.Models.Report.ShiftStatistic;

namespace HRM.ViewModels.Report;

public class ShiftStatisticViewModel : GetShiftStatisticParam
{
    public IEnumerable<ShiftStatistic> ShiftStatistics { get; set; }
}