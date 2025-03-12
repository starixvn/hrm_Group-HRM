namespace HRM.Models.SyncData;

using System;
using System.Xml.Serialization;
using Core.XMLExtensions;

[Serializable]
[XMLName("Row")]
public class WorkingShiftSyncModel
{
    private const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss:fff";

    public string EmployeeID { get; set; }

    public string ShiftCode { get; set; }

    public DateTime ShiftDate { get; set; }

    [XmlIgnore]
    public DateTime? ShiftStartTime { get; set; }

    [XmlIgnore]
    public DateTime? ShiftEndTime { get; set; }

    [XmlIgnore]
    public DateTime? LastUpdated { get; set; }

    [XMLName("LastUpdated")]
    public string LastUpdatedXml => FormatDateTime(LastUpdated);

    public bool IsOffDay { get; set; }
    public bool IsPublicHoliday { get; set; }

    public bool IsInAfterDay { get; set; }

    public bool IsOutAfterDay { get; set; }

    public bool IsCrossDay { get; set; }

    public double TotalHours { get; set; }

    public string CheckInTime { get; set; }

    public string CheckOutTime { get; set; }

    private static string FormatDateTime(DateTime? date)
        => date.HasValue ? date.Value.ToString(DateTimeFormat) : string.Empty;
}