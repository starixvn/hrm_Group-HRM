using HRM.Models.LetsTalk;

namespace HRM.Models;

public class EmailModel
{
    public string Subject { get; set; }

    public string Body { get; set; }

    public string SendTo { get; set; }

    public string SendCC { get; set; }

    public string SendToNickName { get; set; }

    public string SendToDisplayName { get; set; }

    public bool IsAllowSendRequester { get; set; }

    public string RequesterCode { get; set; }

    public string PICDisplayName { get; set; }

    public long RequestId { get; set; }

    public int Action { get; set; }

    public int? RequestType { get; set; }

    public string CallbackUrl { get; set; }

    public LetsTalkActor Approver { get; set; }

    public LetsTalkActor Verifier { get; set; }

    public LetsTalkActor Owner { get; set; }

}

public class LetsTalkActor
{
    public string LetsTalkId { get; set; }
    public ReplyMarkup ReplyMarkup { get; set; }
}
