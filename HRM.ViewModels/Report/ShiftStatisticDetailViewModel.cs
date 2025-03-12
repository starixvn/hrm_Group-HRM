using System.Collections.Generic;
using HRM.Models.Report.ShiftStatistic;
using HRM.Models.Shifts;

namespace HRM.ViewModels.Report;

public class ShiftStatisticDetailViewModel : GetShiftStatisticDetailParam
{
    public IEnumerable<Shift> Shifts { get; set; }
}