using System;

namespace HRM.Models.FnB;

public class ShiftModel
{
    public int ShiftId { get; set; }

    public string Code { get; set; }

    public string Name { get; set; }

    public string BeginCheckInTime { get; set; }

    public string EndCheckInTime { get; set; }

    public string BeginCheckOutTime { get; set; }

    public string EndCheckOutTime { get; set; }

    public string IsInAfterDay { get; set; }

    public string IsOutAfterDay { get; set; }

    public int CompanyId { get; set; }

    public DateTime ModifiedDate { get; set; }

    public bool IsDisabled { get; set; }
}