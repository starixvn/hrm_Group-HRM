namespace HRM.ViewModels.Overtime
{
    using System.Collections.Generic;
    using HRM.Models.Overtime;
    using HRM.Models.Supervisor;

    public class EditMonthlyRequestViewModel
    {
        public int VerifierId { get; set; }
        public int ApproverId { get; set; }
        public IEnumerable<SupervisorNameModel> Verifiers { get; set; } = new List<SupervisorNameModel>();
        public IEnumerable<SupervisorNameModel> Approvers { get; set; } = new List<SupervisorNameModel>();
        public MonthlyOvertimeRequest MonthlyRequest { get; set; }
    }
}