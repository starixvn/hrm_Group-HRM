namespace Fanex.NexHR.Criteria.Message
{
    using System;
    using Fanex.Data.Repository;

    public class GetMessageByCreatorCriteria : CriteriaBase
    {
        public int CreatedBy { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public int? IsDisabled { get; set; }

        public override string GetSettingKey()
        {
            return "Message_GetByCreator";
        }

        public override bool IsValid()
        {
            return CreatedBy > 0;
        }
    }
}
