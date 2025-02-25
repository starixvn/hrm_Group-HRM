namespace HRM.Criteria.SystemSetting
{
    using Fanex.Data.Repository;

    public class GetSystemSettingCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "SystemSetting_Get";

        public override bool IsValid() => true;
    }
}