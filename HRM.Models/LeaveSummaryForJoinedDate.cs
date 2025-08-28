using System;

namespace HRM.Models;

public class LeaveSummaryForJoinedDate : LeaveSummary
{
    public float ExtraLeave { get; set; }
    public float DeductDays { get; set; }
    public DateTime FromDate { get; set; }
}