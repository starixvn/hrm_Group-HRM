namespace Fanex.NexHR.Criteria.BackgroundJob
{
    using Fanex.Data.Repository;

    public class UpdateAnnualLeaveBalanceCriteria : CriteriaBase
    {
        public int Year { get; set; }
        public int? EmpId { get; set; }
        public bool IsDelete { get; set; }

        public override string GetSettingKey() => "Auto_AnnualLeaveBalance_Update";

        public override bool IsValid() => Year > 0;
    }
}