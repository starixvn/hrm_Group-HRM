namespace Fanex.NexHR.Criteria.Message
{
    using System;
    using Fanex.Data.Repository;

    public class EditMessageCriteria : CriteriaBase
    {
        public int MessageId { get; set; }

        public string Subject { get; set; }

        public string Contents { get; set; }

        public int Category { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public string EmployeeIds { get; set; }

        public string TeamIds { get; set; }

        public int ModifiedBy { get; set; }

        public override string GetSettingKey() => "Message_Update";

        public override bool IsValid() => MessageId > 0;
    }
}