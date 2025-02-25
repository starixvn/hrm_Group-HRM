namespace Fanex.NexHR.Criteria.HospitalizationLeave
{
    using System;
    using Fanex.Data.Repository;

    public class UpdateTransactionCriteria : CriteriaBase
    {
        public long TransId { get; set; }
        public string Description { get; set; }
        public DateTime FromDate { get; set; }
        public decimal DeductDays { get; set; }
        public decimal CreditDays { get; set; }
        public DateTime ModifiedDate { get; set; }
        public int ModifiedBy { get; set; }

        public override string GetSettingKey() => "HospitalizationLeave_UpdateTransaction";

        public override bool IsValid() => TransId > 0 && ModifiedBy > 0;
    }
}
