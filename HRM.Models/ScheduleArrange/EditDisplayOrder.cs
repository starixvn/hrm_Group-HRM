namespace HRM.Models.ScheduleArrange
{
    using System;
    using Core.XMLExtensions;

    [Serializable]
    [XMLName("Row")]
    public class EditDisplayOrder
    {
        public int EmployeeId { get; set; }
        public int DisplayOrder { get; set; }
    }
}