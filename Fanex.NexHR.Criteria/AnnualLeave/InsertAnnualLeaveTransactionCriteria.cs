namespace Fanex.NexHR.Criteria.AnnualLeave
{
    using System;
    using Fanex.Data.Repository;

    public class InsertAnnualLeaveTransactionCriteria : CriteriaBase
    {
        public int EmpId { get; set; }

        public int? RequestId { get; set; }

        public string Description { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public decimal DeductDays { get; set; }

        public decimal CreditDays { get; set; }

        public DateTime CreatedDate { get; set; }

        public int CreatedBy { get; set; }

        public override string GetSettingKey() => "AnnualLeave_InsertTransaction";

        public override bool IsValid() => EmpId > 0;
    }
}
