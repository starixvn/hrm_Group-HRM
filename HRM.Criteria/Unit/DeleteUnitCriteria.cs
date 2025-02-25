namespace HRM.Criteria.Unit
{
    using Fanex.Data.Repository;
    
    public class DeleteUnitCriteria : CriteriaBase
    {
        public int UnitId { get; set; }

        public int ModifiedBy { get; set; }

        public override string GetSettingKey() => "Unit_Delete";

        public override bool IsValid() => UnitId > 0;
    }
}