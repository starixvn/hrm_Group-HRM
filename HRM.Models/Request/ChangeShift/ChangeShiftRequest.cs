namespace HRM.Models.Request.ChangeShift
{
    using System;

    public class ChangeShiftRequest : BaseRequest
    {
        public byte TypeId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }
}