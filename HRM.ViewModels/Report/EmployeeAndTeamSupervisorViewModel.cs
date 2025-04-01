using System.Collections.Generic;

namespace HRM.ViewModels.Report;

public class EmployeeAndTeamSupervisorViewModel
{
    public int? EmployeeId { get; set; }

    public string TeamIds { get; set; }

    public int? ActionType { get; set; }

    public byte? IsLocal { get; set; }

    public IList<HRM.Models.Team> Teams { get; set; }
}