namespace HRM.Models.Request
{
    using HRM.Models.Supervisor;
    using System.Collections.Generic;

    public class VerifierAndApproverForRequest
    {
        public IList<SupervisorNameModel> Verifiers { get; set; }

        public IList<SupervisorNameModel> Approvers { get; set; }
    }
}
