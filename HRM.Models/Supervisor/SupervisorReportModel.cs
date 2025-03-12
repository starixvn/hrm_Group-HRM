namespace HRM.Models.Supervisor;

using System.Collections.Generic;
using System.Linq;
using Group;
using GO.UserControl.Client.Core.Models.Role;

public class SupervisorReportModel
{
    public int Id { get; set; }

    public string NickName { get; set; }

    public string FullName { get; set; }

    public string Code { get; set; }

    public List<Role> Roles { get; set; }

    public string Roles2String
    {
        get
        {
            return (Roles == null || !Roles.Any()) ? string.Empty : string.Join(", ", Roles.Select(i => i.Name));
        }
    }

    public TeamModel Team { get; set; }

    public IList<SupervisorModel> Supervisors { get; set; } = new List<SupervisorModel>();

    public string Department { get; set; }

    public string Section { get; set; }
}