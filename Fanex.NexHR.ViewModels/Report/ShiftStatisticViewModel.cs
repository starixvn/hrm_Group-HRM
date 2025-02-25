namespace Fanex.NexHR.ViewModels
{
    using System.Collections.Generic;
    using Fanex.NexHR.Models.Report;

    public class ShiftStatisticViewModel : GetShiftStatisticParam
    {
        public IEnumerable<ShiftStatistic> ShiftStatistics { get; set; }
    }
}