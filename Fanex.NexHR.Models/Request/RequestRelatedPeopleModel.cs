namespace Fanex.NexHR.Models.Request
{
    using System.Collections.Generic;
    using Fanex.NexHR.Models.Supervisor;

    public class RequestRelatedPeopleModel
    {
        public SupervisorNameModel Requester { get; set; }

        public SupervisorNameModel Verifier { get; set; }

        public SupervisorNameModel Approver { get; set; }

        public IList<SupervisorNameModel> ListHR { get; set; }

        public EmailSettingsModel EmailSetting { get; set; }
    }
}