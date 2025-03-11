namespace HRM.Criteria.ExtensionNumber
{
    using Fanex.Data.Repository;

    public class GetAllExtensionNumberCriteria : CriteriaBase
    {
        public short CompanyId { get; set; }

        public override string GetSettingKey() => "ExtensionNO_GetAll";

        public override bool IsValid() => true;
    }
}