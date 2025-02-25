namespace HRM.Criteria.ExtensionNumber
{
    using Fanex.Data.Repository;

    public class GetAllExtensionNumberCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "ExtensionNO_GetAll";

        public override bool IsValid() => true;
    }
}