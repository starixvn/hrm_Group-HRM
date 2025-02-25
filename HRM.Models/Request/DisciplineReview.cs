namespace HRM.Models.Request
{
    using System;
    using HRM.Core.XMLExtensions;

    [Serializable]
    [XMLName("Row")]
    public class DisciplineReview
    {
        public DateTime WorkDate { get; set; }
        public int EmployeeId { get; set; }
        public short DisciplineTypeId { get; set; }
        public bool IsIgnored { get; set; }
        public bool IsConfirmed => !IsIgnored;
    }
}