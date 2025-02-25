namespace Fanex.NexHR.Criteria.Section
{
    using Fanex.Data.Repository;

    public class DeleteSectionCriteria : CriteriaBase
    {
        public int sectionId { get; set; }

        public int ModifiedBy { get; set; }

        public override string GetSettingKey()
        {
            return "Section_Delete";
        }

        public override bool IsValid()
        {
            return sectionId > 0;
        }
    }
}