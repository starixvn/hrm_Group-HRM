namespace Fanex.NexHR.Criteria.Message
{
    using System;
    using Fanex.Data.Repository;

    public class DeleteMessageCriteria : CriteriaBase
    {
        public int MessageId { get; set; }

        public int ModifiedBy { get; set; }

        public DateTime ModifiedDate { get; set; }

        public override string GetSettingKey() => "Message_Delete";

        public override bool IsValid() => MessageId > 0;
    }
}