using Fanex.Data.Repository;

namespace HRM.Criteria.SystemSetting;

public class GetMailConfigByEmpIdCriteria : CriteriaBase
{
    public int EmpId { get; set; }
    public override string GetSettingKey()
    {
        return "MailConfig_ByEmpId_Get";
    }
    public override bool IsValid()
    {
        return EmpId > 0;
    }
}