namespace HRM.Models.Request
{
    using System;
    using HRM.Core.Enums;
    using HRM.Resources;

    public class KeepInViewRequest : BaseRequest
    {
        public byte KeepInViewTypeId { get; set; }

        public DateTime OnDate { get; set; }

        public bool IsHalfDay { get; set; }

        public DateTime FromTime { get; set; }
        public DateTime ToTime { get; set; }

        public string KeepInViewTypeName
        {
            get
            {
                return KeepInViewTypeId == (int)KeepInViewTypes.KeepInViewByDay
                    ? Labels.ByDay
                    : Labels.ByHour;
            }
        }

        public string ShiftName { get; set; }

        public int ShiftId { get; set; }
    }
}