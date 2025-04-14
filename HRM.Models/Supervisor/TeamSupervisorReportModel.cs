namespace HRM.Models.Supervisor;

using System.Collections.Generic;

public class TeamSupervisorReportModel
{
    public int TeamId { get; set; }

    public string Code { get; set; }

    public string Name { get; set; }

    public string DepartmentCode { get; set; }
    public string DepartmentName { get; set; }

    public IList<SupervisorModel> Supervisors { get; set; } = new List<SupervisorModel>();
}