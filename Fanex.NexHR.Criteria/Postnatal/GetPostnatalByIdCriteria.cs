namespace Fanex.NexHR.Criteria.Postnatal
{
    using Fanex.Data.Repository;

    public class GetPostnatalByIdCriteria : CriteriaBase
    {
        public int PostnatalId { get; set; }

        public override string GetSettingKey()
        {
            return "Postnatal_GetById";
        }

        public override bool IsValid()
        {
            return PostnatalId > 0;
        }
    }
}