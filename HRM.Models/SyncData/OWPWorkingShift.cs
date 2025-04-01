namespace HRM.Models.SyncData;

using System;
using System.Xml.Serialization;
using Core.XMLExtensions;

[Serializable]
[XMLName("Row")]
public class OWPWorkingShift
{
    private const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss:fff";

    public string EmployeeID { get; set; }
    public string ShiftCode { get; set; }
    public DateTime ShiftDate { get; set; }
    public bool IsOffDay { get; set; }
    public bool IsPublicHoliday { get; set; }
    public string ModifiedDate => FormatDateTime(LastUpdated);

    [XmlIgnore]
    public DateTime? ShiftStartTime { get; set; }

    [XmlIgnore]
    public DateTime? ShiftEndTime { get; set; }

    [XmlIgnore]
    public DateTime? LastUpdated { get; set; }

    [XmlIgnore]
    public bool IsInAfterDay { get; set; }

    [XmlIgnore]
    public bool IsOutAfterDay { get; set; }

    [XmlIgnore]
    public bool IsCrossDay { get; set; }

    [XmlIgnore]
    public double TotalHours { get; set; }

    [XmlIgnore]
    public string CheckInTime { get; set; }

    [XmlIgnore]
    public string CheckOutTime { get; set; }

    private static string FormatDateTime(DateTime? date)
        => date.HasValue ? date.Value.ToString(DateTimeFormat) : string.Empty;
}