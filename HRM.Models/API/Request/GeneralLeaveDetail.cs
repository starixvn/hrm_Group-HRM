namespace HRM.Models.API.Request
{
    using System;

    [Serializable]
    public class GeneralLeaveDetail
    {
        public long RequestId { get; set; }
        public DateTime OnDate { get; set; }
        public short LeaveTypeId { get; set; }
        public string LeaveTypeCode { get; set; }
        public string LeaveTypeName { get; set; }
        public decimal LeaveDay { get; set; }
        public bool IsOffDay { get; set; }
        public bool IsHoliday { get; set; }
    }
}