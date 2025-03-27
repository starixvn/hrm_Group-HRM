using System;
using System.Xml.Serialization;
using HRM.Core.XMLExtensions;

namespace HRM.Models.SyncData;

[Serializable]
[XMLName("Row")]
public class OWPShift
{
    private const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss:fff";

    public string ShiftCode { get; set; }
    public string CheckInTime { get; set; }
    public string CheckOutTime { get; set; }
    public bool IsInAfterDay { get; set; }
    public bool IsOutAfterDay { get; set; }
    public bool IsCrossDay { get; set; }
    public double TotalHours { get; set; }

    [XmlIgnore]
    public DateTime? LastUpdated { get; set; }

    public string ModifiedDate => LastUpdated.HasValue ? LastUpdated.Value.ToString(DateTimeFormat) : string.Empty;
}