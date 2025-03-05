namespace HRM.Criteria.SystemSetting
{
    using Fanex.Data.Repository;

    public class GetSystemConfigsByKeyCriteria : CriteriaBase
    {
        public short CompanyId { get; set; }
        public string Key { get; set; }

        public override string GetSettingKey() => "SystemConfigs_GetByKey";

        public override bool IsValid() => CompanyId > 0 && !string.IsNullOrWhiteSpace(Key);
    }
}