namespace HRM.Criteria.Message
{
    using Fanex.Data.Repository;

    public class GetMessageByIdCriteria : CriteriaBase
    {
        public int MessageId { get; set; }

        public override string GetSettingKey() => "Message_GetById";

        public override bool IsValid() => MessageId > 0;
    }
}