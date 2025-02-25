namespace Fanex.NexHR.Criteria.ScheduleArrange
{
    using System;
    using Fanex.Data.Repository;

    public class SyncWorkingShiftCommand : CriteriaBase
    {
        public int ActorId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string EmployeeIds { get; set; }
        public string WorkingShiftXml { get; set; }

        public override string GetSettingKey() => "ScheduleArrangement_SyncWorkingShift";

        public override bool IsValid() => ActorId > 0 && !string.IsNullOrWhiteSpace(EmployeeIds) && !string.IsNullOrWhiteSpace(WorkingShiftXml);
    }
}