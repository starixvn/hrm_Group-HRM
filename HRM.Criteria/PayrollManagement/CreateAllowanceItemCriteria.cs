namespace HRM.Criteria.PayrollManagement;

using Fanex.Data.Repository;

public class CreateAllowanceItemCriteria : CriteriaBase
{
    public int PayrollItemSettingId { get; set; }
    public string ItemName { get; set; }
    public string Amount { get; set; }
    public int CurrencyId { get; set; }
    public bool IsPercent { get; set; }
    public bool IsActive { get; set; }
    public int ModifiedBy { get; set; }

    public override string GetSettingKey()
    {
        return "AllowanceItem_Create";
    }

    public override bool IsValid() => !string.IsNullOrEmpty(ItemName)
        && !string.IsNullOrEmpty(Amount) 
        && PayrollItemSettingId > 0;
}