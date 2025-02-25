namespace Fanex.NexHR.Models.ScheduleArrange
{
    using Newtonsoft.Json;

    public class WeeklyDefaultShift
    {
        public int EmployeeId { get; set; }

        [JsonIgnore]
        public int MonShiftId { get; set; }
        public ShiftBase MonShift { get; set; }

        [JsonIgnore]
        public int TueShiftId { get; set; }
        public ShiftBase TueShift { get; set; }

        [JsonIgnore]
        public int WedShiftId { get; set; }
        public ShiftBase WedShift { get; set; }

        [JsonIgnore]
        public int ThuShiftId { get; set; }
        public ShiftBase ThuShift { get; set; }

        [JsonIgnore]
        public int FriShiftId { get; set; }
        public ShiftBase FriShift { get; set; }

        [JsonIgnore]
        public int SatShiftId { get; set; }
        public ShiftBase SatShift { get; set; }

        [JsonIgnore]
        public int SunShiftId { get; set; }
        public ShiftBase SunShift { get; set; }
    }
}