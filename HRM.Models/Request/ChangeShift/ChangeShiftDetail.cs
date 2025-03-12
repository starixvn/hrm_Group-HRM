namespace HRM.Models.Request.ChangeShift
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;
    using Core;
    using Core.XMLExtensions;
    using Newtonsoft.Json;

    [Serializable]
    [XMLName("Row")]
    public class ChangeShiftDetail
    {
        [JsonIgnore]
        [XmlIgnore]
        public long RequestID { get; set; }

        public DateTime OnDate { get; set; }

        public int FromShiftId { get; set; }

        [XmlIgnore]
        public string FromShift { get; set; }

        public int ToShiftId { get; set; }

        [XmlIgnore]
        public string ToShift { get; set; }

        public string Remark { get; set; }

        [XmlIgnore]
        public DateTime? ToDate { get; set; }

        [JsonIgnore]
        [XMLName("ToDate")]
        public string ToDateXml => ToDate.HasValue ? ToDate.Value.ToDateMonthYearFormatedDate() : string.Empty;

        [XmlIgnore]
        public IEnumerable<DateTime> DisableDates { get; set; } = new List<DateTime>();
    }
}