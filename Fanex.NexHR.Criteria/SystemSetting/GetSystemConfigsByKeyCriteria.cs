namespace Fanex.NexHR.Criteria.SystemSetting
{
    using Fanex.Data.Repository;

    public class GetSystemConfigsByKeyCriteria : CriteriaBase
    {
        public string Key { get; set; }

        public override string GetSettingKey() => "SystemConfigs_GetByKey";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(Key);
    }
}