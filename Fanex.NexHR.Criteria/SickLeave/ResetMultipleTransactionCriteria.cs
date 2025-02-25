namespace Fanex.NexHR.Criteria.SickLeave
{
    using Fanex.Data.Repository;

    public class ResetMultipleTransactionCriteria : CriteriaBase
    {
        public int ActorId { get; set; }
        public int Year { get; set; }
        public decimal ResetDays { get; set; }
        public string Description { get; set; }

        public override string GetSettingKey() => "SickLeave_ResetMultipleTransactions";

        public override bool IsValid() => ActorId > 0;
    }
}