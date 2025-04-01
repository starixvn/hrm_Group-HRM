namespace HRM.Models.ScheduleArrange;

public class SumEmployeeWorkingOnShift
{
    private const int TotalDays = 7;

    public string Code { get; set; }
    public string CheckIn { get; set; }
    public string CheckOut { get; set; }
    public bool IsInAfterDay { get; set; }
    public bool IsOutAfterDay { get; set; }
    public int[] Weekdays { get; set; } = new int[TotalDays];
}