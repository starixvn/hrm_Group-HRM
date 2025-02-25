namespace Fanex.NexHR.Criteria.SickLeave
{
    using System;
    using Fanex.Data.Repository;

    public class DeleteTransactionCriteria : CriteriaBase
    {
        public long TransId { get; set; }
        public DateTime ModifiedDate { get; set; }
        public int ModifiedBy { get; set; }

        public override string GetSettingKey() => "SickLeave_DeleteTransaction";

        public override bool IsValid() => TransId > 0 && ModifiedBy > 0;
    }
}