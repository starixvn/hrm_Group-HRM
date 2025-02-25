namespace Fanex.NexHR.Criteria.Message
{
    using Fanex.Data.Repository;

    public class GetMessageOfEmployeeByIdCriteria : CriteriaBase
    {
        public int MessageId { get; set; }

        public override string GetSettingKey() => "Message_GetDetailByEmployee";

        public override bool IsValid() => MessageId > 0;
    }
}