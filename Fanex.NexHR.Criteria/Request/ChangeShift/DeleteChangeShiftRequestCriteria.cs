namespace Fanex.NexHR.Criteria.Request.ChangeShift
{
    using Fanex.Data.Repository;

    public class DeleteChangeShiftRequestCriteria : CriteriaBase
    {
        public long RequestId { get; set; }
        public int ActorId { get; set; }

        public override string GetSettingKey() => "ChangeShiftRequest_Delete";

        public override bool IsValid() => RequestId > 0 && ActorId > 0;
    }
}