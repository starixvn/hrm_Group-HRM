namespace HRM.Models.Request.ChangeShift
{
    using System;

    public class GetWorkingCalendarForRequestParam
    {
        public int EmployeeId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public byte TypeId { get; set; }
        public long? RequestId { get; set; }
    }
}