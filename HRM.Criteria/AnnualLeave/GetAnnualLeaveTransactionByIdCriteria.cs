namespace HRM.Criteria.AnnualLeave
{
    using Fanex.Data.Repository;

    public class GetAnnualLeaveTransactionByIdCriteria : CriteriaBase
    {
        public long LeaveTransId { get; set; }

        public override string GetSettingKey() => "AnnualLeave_GetByID";

        public override bool IsValid() => LeaveTransId > 0;
    }
}