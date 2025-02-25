namespace HRM.Models.Group
{
    using System;
    using HRM.Core.XMLExtensions;

    [Serializable]
    [XMLName("Row")]
    public class EditTeamSupervisor
    {
        public int OwnerId { get; set; }
        public ushort RequestType { get; set; }
        public ushort DisplayOrder { get; set; }
    }
}