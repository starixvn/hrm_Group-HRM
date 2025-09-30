namespace HRM.Models.Report.Overtime;

public class GetYearlyOvertimeParam
{
    public short CompanyId { get; set; }
    public int ActorId { get; set; }
    public int Year { get; set; }
    public string Employee { get; set; }
    public string TeamIds { get; set; }
    public byte Calendar { get; set; }
    public string EmployeeStatusIds { get; set; }
    public byte? IsLocal { get; set; }
}