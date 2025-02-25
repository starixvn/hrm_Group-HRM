namespace Fanex.NexHR.Models.ScheduleArrange
{
    using System;
    using Fanex.NexHR.Core.XMLExtensions;

    [Serializable]
    [XMLName("Row")]
    public class EditTeamOrder
    {
        public int TeamId { get; set; }
        public int DisplayOrder { get; set; }
    }
}