namespace Fanex.NexHR.MailDeliver.Criteria
{
    using System;
    using Fanex.Data.Repository;

    public class UpdateMailStatusCriteria : CriteriaBase
    {
        public long MailId { get; set; }

        public bool Status { get; set; }

        public DateTime MailUpdatedDate { get; set; }

        public override string GetSettingKey()
        {
            return "MailRecorder_UpdateMailStatus";
        }

        public override bool IsValid()
        {
            return MailId > 0;
        }
    }
}