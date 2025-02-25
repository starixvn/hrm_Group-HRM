namespace HRM.ViewModels.Request
{
    using System;

    public class ChangeShiftDetailViewModel
    {
        public DateTime OnDate { get; set; }
        public string Schedule { get; set; }
        public int FromShiftId { get; set; }
        public int ToShiftId { get; set; }
        public string Remark { get; set; }
    }
}