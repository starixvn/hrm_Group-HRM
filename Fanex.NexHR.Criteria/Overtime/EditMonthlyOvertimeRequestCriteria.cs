namespace Fanex.NexHR.Criteria.Overtime
{
    using System;
    using Fanex.Data.Repository;

    public class EditMonthlyOvertimeRequestCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }
        public DateTime Month { get; set; }
        public int VerifierId { get; set; }
        public int ApproverId { get; set; }
        public int ActorId { get; set; }
        public string DailyDetailsXml { get; set; }

        public override string GetSettingKey() => "Overtime_UpdateMonthlyRequest";

        public override bool IsValid() => EmployeeId > 0 && VerifierId > 0 && ApproverId > 0 && ActorId > 0;
    }
}