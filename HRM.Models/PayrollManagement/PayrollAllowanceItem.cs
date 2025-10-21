using System.Collections.Generic;

namespace HRM.Models.PayrollManagement;

public class PayrollSetting
{
    public int PayrollItemSettingId { get; set; }

    public string Type { get; set; }

    public bool Taxable { get; set; }

    public int Category { get; set; }

    public bool IsMultiSelect { get; set; }

    public List<PayrollAllowanceItem> PayrollAllowanceItems { get; set; } = new();
}

public class PayrollAllowanceItem
{
    public int AllowanceItemSettingId { get; set; }

    public string ItemName { get; set; }

    public string Amount { get; set; }

    public bool IsPercent { get; set; }
}

