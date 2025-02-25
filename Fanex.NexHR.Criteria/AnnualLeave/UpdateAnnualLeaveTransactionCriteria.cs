namespace Fanex.NexHR.Criteria.AnnualLeave
{
    using System;
    using Fanex.Data.Repository;

    public class UpdateAnnualLeaveTransactionCriteria : CriteriaBase
    {
        public long LeaveTransId { get; set; }

        public string Description { get; set; }

        public DateTime FromDate { get; set; }

        public decimal DeductDays { get; set; }

        public decimal CreditDays { get; set; }

        public DateTime ModifiedDate { get; set; }

        public int ModifiedBy { get; set; }

        public override string GetSettingKey() => "AnnualLeave_UpdateTransaction";

        public override bool IsValid() => LeaveTransId > 0;
    }
}
