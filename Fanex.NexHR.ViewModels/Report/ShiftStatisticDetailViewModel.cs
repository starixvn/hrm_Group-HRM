namespace Fanex.NexHR.ViewModels
{
    using System.Collections.Generic;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Report;

    public class ShiftStatisticDetailViewModel : GetShiftStatisticDetailParam
    {
        public IEnumerable<Shift> Shifts { get; set; }
    }
}