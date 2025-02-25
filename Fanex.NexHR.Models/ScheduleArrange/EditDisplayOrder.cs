namespace Fanex.NexHR.Models.ScheduleArrange
{
    using System;
    using Fanex.NexHR.Core.XMLExtensions;

    [Serializable]
    [XMLName("Row")]
    public class EditDisplayOrder
    {
        public int EmployeeId { get; set; }
        public int DisplayOrder { get; set; }
    }
}