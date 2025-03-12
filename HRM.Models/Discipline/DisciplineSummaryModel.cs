namespace HRM.Models.Discipline;

public class DisciplineSummaryModel
{
    public EmployeeInfoModel Employee { get; set; }

    public int MissingCheckInOut { get; set; }
    public int WorkingUnderTime { get; set; }
    public int LateIn { get; set; }
    public int EarlyOut { get; set; }
    public int Absent { get; set; }
    public int UnpaidLeave { get; set; }
    public int UnpaidLeaveAbsent { get; set; }
    public int ChangeShift { get; set; }
    public int UnplannedLeave { get; set; }
    public int WeekendLeave { get; set; }
    public int SevenDaysLeave { get; set; }

    public float OtherDisciplineScore { get; set; }
    public float SystemSubtractScore { get; set; }
    public float ApproverSubtractScore { get; set; }

    public int Permission { get; set; }
}