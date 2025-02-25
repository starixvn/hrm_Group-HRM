namespace Fanex.NexHR.Criteria.Section
{
    using Fanex.Data.Repository;

    public class UpdateSectionCriteria : CriteriaBase
    {
        public int SectionId { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public bool IsActive { get; set; }

        public int DepartmentId { get; set; }

        public int ModifiedBy { get; set; }

        public override string GetSettingKey()
        {
            return "Section_Update";
        }

        public override bool IsValid()
        {
            return SectionId > 0 && !string.IsNullOrWhiteSpace(Code) && !string.IsNullOrWhiteSpace(Name);
        }
    }
}
