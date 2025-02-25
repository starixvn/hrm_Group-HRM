namespace Fanex.NexHR.Criteria.ScheduleArrange
{
    using Fanex.Data.Repository;

    public class EditRemarkCriteria : CriteriaBase
    {
        public int ActorId { get; set; }
        public int EmployeeId { get; set; }
        public string Remark { get; set; }

        public override string GetSettingKey() => "ScheduleArrangement_UpdateRemark";

        public override bool IsValid() => ActorId > 0 && EmployeeId > 0;
    }
}