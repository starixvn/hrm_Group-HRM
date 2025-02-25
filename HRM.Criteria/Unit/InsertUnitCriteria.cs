namespace HRM.Criteria.Unit
{
    using Fanex.Data.Repository;

    public class InsertUnitCriteria : CriteriaBase
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public bool IsActive { get; set; }
        
        public int CreatedBy { get; set; }

        public override string GetSettingKey()
        {
            return "Unit_Insert";
        }

        public override bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(Code) && !string.IsNullOrWhiteSpace(Name);
        }
    }
}
