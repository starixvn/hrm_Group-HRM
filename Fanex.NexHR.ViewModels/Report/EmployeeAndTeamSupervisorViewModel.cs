namespace Fanex.NexHR.ViewModels
{
    using System.Collections.Generic;
    using Fanex.NexHR.Models;

    public class EmployeeAndTeamSupervisorViewModel
    {
        public int? EmployeeId { get; set; }

        public string TeamIds { get; set; }

        public int? ActionType { get; set; }

        public byte? IsLocal { get; set; }

        public IList<Fanex.NexHR.Models.Team> Teams { get; set; }
    }
}