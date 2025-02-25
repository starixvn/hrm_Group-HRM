namespace HRM.Criteria.Section
{
    using Fanex.Data.Repository;

    public class InsertSectionCriteria : CriteriaBase
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public bool IsActive { get; set; }

        public int DepartmentId { get; set; }

        public int ModifiedBy { get; set; }

        public override string GetSettingKey()
        {
            return "Section_Insert";
        }

        public override bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(Code)
                && !string.IsNullOrWhiteSpace(Name)
                && DepartmentId > 0;
        }
    }
}
