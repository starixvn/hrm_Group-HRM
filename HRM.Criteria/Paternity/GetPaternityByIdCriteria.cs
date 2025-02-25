namespace HRM.Criteria.Paternity
{
    using Fanex.Data.Repository;

    public class GetPaternityByIdCriteria : CriteriaBase
    {
        public int PaternityId { get; set; }

        public override string GetSettingKey()
        {
            return "Paternity_GetById";
        }

        public override bool IsValid()
        {
            return PaternityId > 0;
        }
    }
}