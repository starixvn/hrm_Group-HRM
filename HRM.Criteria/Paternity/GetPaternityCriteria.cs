namespace HRM.Criteria.Paternity
{
    using Fanex.Data.Repository;

    public class GetPaternityCriteria : CriteriaBase
    {
        public int Year { get; set; }

        public override string GetSettingKey()
        {
            return "Paternity_Get";
        }

        public override bool IsValid()
        {
            return Year > 0;
        }
    }
}