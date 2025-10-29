namespace HRM.Models.PayrollManagement;

public class PayrollAllowanceCreateRequest
{
    public int PayrollItemSettingId { get; set; }

    public string ItemName { get; set; }

    public string Amount { get; set; }

    public bool IsPercent { get; set; }

    public bool IsActive { get; set; }
}

