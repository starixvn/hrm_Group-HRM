namespace Fanex.NexHR.Criteria.Country
{
    using Fanex.Data.Repository;

    public class GetAllCountryCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "Country_GetAll";

        public override bool IsValid() => true;
    }
}