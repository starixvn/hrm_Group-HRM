namespace Fanex.NexHR.Criteria.ScheduleArrange
{
    using System;
    using Fanex.Data.Repository;

    public class EditWeeklyDefaultShiftCriteria : CriteriaBase
    {
        public int ActorId { get; set; }
        public int EmployeeId { get; set; }
        public DateTime FromDate { get; set; }
        public int? MonShiftId { get; set; }
        public int? TueShiftId { get; set; }
        public int? WedShiftId { get; set; }
        public int? ThuShiftId { get; set; }
        public int? FriShiftId { get; set; }
        public int? SatShiftId { get; set; }
        public int? SunShiftId { get; set; }
        public bool IsUsedWeeklyDefaultShift { get; set; }

        public override string GetSettingKey() => "ScheduleArrangement_UpdateWeeklyDefaultShift";

        public override bool IsValid() => ActorId > 0;
    }
}