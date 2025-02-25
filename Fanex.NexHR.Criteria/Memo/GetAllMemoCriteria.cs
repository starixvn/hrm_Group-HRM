namespace Fanex.NexHR.Criteria.Memo
{
    using Fanex.Data.Repository;

    public class GetAllMemoCriteria : CriteriaBase
    {
        public int Year { get; set; }
        public int Category { get; set; }

        public override string GetSettingKey() => "Memo_GetAll";

        public override bool IsValid() => true;
    }
}