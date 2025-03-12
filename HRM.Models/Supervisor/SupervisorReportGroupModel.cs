namespace HRM.Models.Supervisor;

using System.Collections.Generic;

public class SupervisorReportGroupModel
{
    public int EmployeeId { get; set; }

    public List<SupervisorReportModel> SupervisorAction { get; set; }
}