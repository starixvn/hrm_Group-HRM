namespace Fanex.NexHR.Criteria.AnnualLeave
{
    using System;
    using Fanex.Data.Repository;

    public class DeleteAnnualLeaveTransactionCriteria : CriteriaBase
    {
        public long LeaveTransId { get; set; }

        public DateTime ModifiedDate { get; set; }

        public int ModifiedBy { get; set; }

        public override string GetSettingKey() => "AnnualLeave_DeleteTransaction";

        public override bool IsValid() => LeaveTransId > 0;
    }
}
