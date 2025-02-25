namespace Fanex.NexHR.Criteria.Message
{
    using Fanex.Data.Repository;

    public class DeleteMessageByEmployeeCriteria : CriteriaBase
    {
        public int MessageId { get; set; }

        public int EmployeeId { get; set; }

        public override string GetSettingKey() => "Message_EmployeeDelete";

        public override bool IsValid() => MessageId > 0 && EmployeeId > 0;
    }
}