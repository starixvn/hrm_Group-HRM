namespace Fanex.NexHR.Criteria.Message
{
    using Fanex.Data.Repository;

    public class EditMessageStatusCriteria : CriteriaBase
    {
        public int MessageId { get; set; }

        public int EmployeeId { get; set; }

        public override string GetSettingKey() => "Message_Read";

        public override bool IsValid() => MessageId > 0 && EmployeeId > 0;
    }
}
