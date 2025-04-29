namespace HRM.Criteria.SystemSetting;

using Fanex.Data.Repository;

public class GetSystemSettingCriteria : CriteriaBase
{
    public short CompanyId { get; set; }

    public override string GetSettingKey() => "SystemSetting_Get";

    public override bool IsValid() => true;
}