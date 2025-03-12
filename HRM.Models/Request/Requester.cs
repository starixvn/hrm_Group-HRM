namespace HRM.Models.Request;

public class Requester
{
    public int EmpId { get; set; }
    public string Code { get; set; }
    public string Nickname { get; set; }
    public string FullName { get; set; }
    public string Team { get; set; }
    public string Section { get; set; }
    public string Department { get; set; }
    public bool IsTopManager { get; set; }
    public string DisplayName => (string.IsNullOrWhiteSpace(Nickname) || string.IsNullOrWhiteSpace(FullName)) ? string.Empty : Nickname + " (" + FullName + ")";
    public int WorkingHours { get; set; }
    public decimal SpecialOT { get; set; }
}