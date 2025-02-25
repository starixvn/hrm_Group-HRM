namespace Fanex.NexHR.Criteria.Calendar
{
    using System;
    using Fanex.Data.Repository;

    public class GetEmployeeCalendarByMonthCriteria : CriteriaBase
    {
        public int EmpId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        public override string GetSettingKey() => "EmployeeCalendar_GetMonthlyByEmployeeId";

        public override bool IsValid() => EmpId > 0;
    }
}