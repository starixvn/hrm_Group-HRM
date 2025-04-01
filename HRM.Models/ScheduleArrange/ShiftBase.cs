namespace HRM.Models.ScheduleArrange;

using Newtonsoft.Json;

public class ShiftBase : EntityBase<int>
{
    [JsonIgnore]
    public string CheckIn { get; set; }

    [JsonIgnore]
    public string CheckOut { get; set; }

    [JsonIgnore]
    public bool IsInAfterDay { get; set; }

    [JsonIgnore]
    public bool IsOutAfterDay { get; set; }

    [JsonIgnore]
    public string BackgroundColor { get; set; }

    [JsonIgnore]
    public bool IsShift { get; set; }
}