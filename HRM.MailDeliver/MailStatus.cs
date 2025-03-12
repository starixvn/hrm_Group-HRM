namespace HRM.MailDeliver;

public class MailStatus
{
    public bool IsSucessfull { get; set; }

    public string ErrorMessage { get; set; }

    public string StackTrace { get; set; }
}