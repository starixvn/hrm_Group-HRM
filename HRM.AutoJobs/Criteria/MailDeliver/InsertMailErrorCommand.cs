namespace HRM.AutoJobs.Criteria.MailDeliver;

using System;
using Fanex.Data.Repository;

public class InsertMailErrorCommand : NonQueryCommand
{
    public long MailId { get; set; }

    public int AppId { get; set; }

    public string Message { get; set; }

    public string StackTrace { get; set; }

    public DateTime ErrorTime { get; set; }

    public override string GetSettingKey() => "MailRecorder_InsError";

    public override bool IsValid() => MailId > 0;
}