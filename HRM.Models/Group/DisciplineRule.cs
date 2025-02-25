namespace HRM.Models.Group
{
    using System;
    using System.Xml.Serialization;
    using HRM.Core.XMLExtensions;

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