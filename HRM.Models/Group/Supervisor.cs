namespace HRM.Models.Group;

public class Supervisor
{
    public int OwnerId { get; set; }
    public string Nickname { get; set; }
    public string FullName { get; set; }
    public string CompanyName { get; set; }
    public int CompanyId { get; set; }
    public bool CanRemove { get; set; }
    public string CompanyName { get; set; }
}