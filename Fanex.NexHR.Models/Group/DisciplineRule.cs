namespace Fanex.NexHR.Models.Group
{
    using System;
    using System.Xml.Serialization;
    using Fanex.NexHR.Core.XMLExtensions;

    [Serializable]
    [XMLName("Row")]
    public class DisciplineRule
    {
        public short DisciplineTypeId { get; set; }

        [XmlIgnore]
        public string Name { get; set; }

        public bool IsApplyForLocal { get; set; }
        public bool IsApplyForExpat { get; set; }
    }
}