namespace HRM.ViewModels
{
    using System.Collections.Generic;
    using HRM.Models.Report;

    public class ShiftStatisticViewModel : GetShiftStatisticParam
    {
        public IEnumerable<ShiftStatistic> ShiftStatistics { get; set; }
    }
}