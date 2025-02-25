namespace HRM.Models.API.Request
{
    using System;

    [Serializable]
    public class ChangeShiftDetail
    {
        public long RequestId { get; set; }
        public DateTime OnDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}