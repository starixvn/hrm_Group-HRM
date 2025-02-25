namespace HRM.ViewModels
{
    using System.Collections.Generic;
    using HRM.Models;
    using HRM.Models.Report;

    public class ShiftStatisticDetailViewModel : GetShiftStatisticDetailParam
    {
        public IEnumerable<Shift> Shifts { get; set; }
    }
}