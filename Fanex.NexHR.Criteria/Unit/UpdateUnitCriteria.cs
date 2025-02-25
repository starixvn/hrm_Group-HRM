namespace Fanex.NexHR.Criteria.Unit
{
    using Fanex.Data.Repository;

    public class UpdateUnitCriteria : CriteriaBase
    {
        public int UnitId { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public bool IsActive { get; set; }

        public int ModifiedBy { get; set; }

        public override string GetSettingKey()
        {
            return "Unit_Update";
        }

        public override bool IsValid()
        {
            return UnitId > 0 && !string.IsNullOrWhiteSpace(Code) && !string.IsNullOrWhiteSpace(Name);
        }
    }
}
