namespace HRM.AutoJobs.Criteria.MailDeliver;

using System;
using Fanex.Data.Repository;

public class InsertMailRecordCommand : NonQueryCommand
{
    public long MailId { get; set; }

    public int AppId { get; set; }

    public string MailTo { get; set; }

    public string MailCc { get; set; }

    public string MailBcc { get; set; }

    public string Subject { get; set; }

    public string Body { get; set; }

    public DateTime MailDate { get; set; }

    public string Attachments { get; set; }

    public bool IsEncrypt { get; set; }

    public override string GetSettingKey() => "MailRecorder_InsRecord";

    public override bool IsValid() => !string.IsNullOrWhiteSpace(MailTo) && !string.IsNullOrWhiteSpace(Subject);
}