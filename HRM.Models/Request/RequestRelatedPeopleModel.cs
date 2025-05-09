namespace HRM.Models.Request;

using Supervisor;

public class RequestRelatedPeopleModel
{
    public SupervisorNameModel Requester { get; set; }

    public SupervisorNameModel Verifier { get; set; }

    public SupervisorNameModel Approver { get; set; }

    public EmailSettingsModel EmailSetting { get; set; } = new();
}