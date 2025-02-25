namespace HRM.AutoJobs.Criteria.MailDeliver
{
    using System;
    using Fanex.Data.Repository;

    public class UpdateMailStatusCommand : NonQueryCommand
    {
        public long MailId { get; set; }

        public bool Status { get; set; }

        public DateTime MailUpdatedDate { get; set; }

        public override string GetSettingKey() => "MailRecorder_UpdateMailStatus";

        public override bool IsValid() => MailId > 0;
    }
}