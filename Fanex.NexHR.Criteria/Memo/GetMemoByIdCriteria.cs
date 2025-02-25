namespace Fanex.NexHR.Criteria.Memo
{
    using Fanex.Data.Repository;

    public class GetMemoByIdCriteria : CriteriaBase
    {
        public int Id { get; set; }

        public override string GetSettingKey() => "Memo_GetById";

        public override bool IsValid() => Id > 0;
    }
}