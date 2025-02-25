namespace Fanex.NexHR.Criteria.SickLeave
{
    using Fanex.Data.Repository;

    public class GetTransactionByIdCriteria : CriteriaBase
    {
        public long TransId { get; set; }

        public override string GetSettingKey() => "SickLeave_GetTransById";

        public override bool IsValid() => TransId > 0;
    }
}