namespace HRM.Criteria.PayrollManagement;

using Fanex.Data.Repository;

public class GetAllowanceItemSettingCriteria : CriteriaBase
{
    public int? CompanyId { get; set; }

    public int? PayrollItemSettingId { get; set; }

    public override string GetSettingKey()
    {
        return "AllowanceItem_Get";
    }

    public override bool IsValid() => true;
}