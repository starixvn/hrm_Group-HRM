namespace Fanex.NexHR.Criteria.Postnatal
{
    using Fanex.Data.Repository;

    public class GetPostnatalCriteria : CriteriaBase
    {
        public int Year { get; set; }

        public override string GetSettingKey()
        {
            return "Postnatal_Get";
        }

        public override bool IsValid()
        {
            return Year > 0;
        }
    }
}