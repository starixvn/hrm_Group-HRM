namespace Fanex.NexHR.Criteria.API.Request
{
    using Fanex.Data.Repository;

    public class GetPendingRequestByUserIdCriteria : CriteriaBase
    {
        public int UserId { get; set; }

        public override string GetSettingKey() => "API_GO_GetPendingRequestByUserId";

        public override bool IsValid() => UserId > 0;
    }
}