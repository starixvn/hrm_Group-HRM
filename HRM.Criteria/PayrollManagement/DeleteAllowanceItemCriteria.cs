namespace HRM.Criteria.PayrollManagement;

using Fanex.Data.Repository;

public class DeleteAllowanceItemCriteria : CriteriaBase
{
    public int AllowanceItemSettingId { get; set; }
    public int ModifiedBy { get; set; }

    public override string GetSettingKey()
    {
        return "AllowanceItem_Delete";
    }

    public override bool IsValid() => true;
}