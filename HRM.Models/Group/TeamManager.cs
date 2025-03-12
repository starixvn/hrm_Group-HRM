namespace HRM.Models.Group;

public class TeamManager
{
    public int TeamId { get; set; }

    public int EmpId { get; set; }
    public string Nickname { get; set; }
    public string FullName { get; set; }

    public string DisplayName => string.IsNullOrWhiteSpace(Nickname) ? FullName : Nickname + " (" + FullName + ")";

    public byte RoleId { get; set; }
}