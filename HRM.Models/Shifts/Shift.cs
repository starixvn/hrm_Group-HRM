namespace HRM.Models.Shifts;

public class Shift : EntityBase<int>
{
    public string BeginCheckInTime { get; set; }

    public string EndCheckInTime { get; set; }

    public string BeginCheckOutTime { get; set; }

    public string EndCheckOutTime { get; set; }

    public int BreakTimeMins { get; set; }

    public string BeginBreakTime { get; set; }

    public string EndBreakTime { get; set; }

    public bool CrossDay { get; set; }

    public bool IsInAfterDay { get; set; }

    public bool IsOutAfterDay { get; set; }

    public bool IsDisabled { get; set; }

    public bool IsUsed { get; set; }
}