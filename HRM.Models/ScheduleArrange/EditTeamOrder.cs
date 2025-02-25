namespace HRM.Models.ScheduleArrange
{
    using System;
    using HRM.Core.XMLExtensions;

    [Serializable]
    [XMLName("Row")]
    public class EditTeamOrder
    {
        public int TeamId { get; set; }
        public int DisplayOrder { get; set; }
    }
}