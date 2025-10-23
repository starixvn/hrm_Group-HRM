namespace HRM.Models.PayrollManagement;

public class PayrollAllowanceEditRequest
{
    public int AllowanceItemSettingId { get; set; }

    public string ItemName { get; set; }

    public string Amount { get; set; }

    public bool IsPercent { get; set; }

    public bool IsActive { get; set; }
}

