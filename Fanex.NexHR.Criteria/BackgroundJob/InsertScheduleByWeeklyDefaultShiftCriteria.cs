namespace Fanex.NexHR.Criteria.BackgroundJob
{
    using System;
    using Fanex.Data.Repository;

    public class InsertScheduleByWeeklyDefaultShiftCriteria : CriteriaBase
    {
        public int? EmployeeId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        public override string GetSettingKey() => "Auto_EmployeeCalendar_InsertWeeklyDefaultShift";

        public override bool IsValid() => true;
    }
}