namespace HRM.MailDeliver.Criteria;

using System;
using Fanex.Data.Repository;

public class InsertMailErrorCriteria : CriteriaBase
{
    public long MailId { get; set; }

    public int AppId { get; set; }

    public string Message { get; set; }

    public string StackTrace { get; set; }

    public DateTime ErrorTime { get; set; }

    public override string GetSettingKey()
    {
        return "MailRecorder_InsError";
    }

    public override bool IsValid()
    {
        return MailId > 0;
    }
}