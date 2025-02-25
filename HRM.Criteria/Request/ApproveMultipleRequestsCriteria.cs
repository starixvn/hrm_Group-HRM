namespace HRM.Criteria.Request
{
    using Fanex.Data.Repository;
    using HRM.Core.Enums;

    public class ApproveMultipleRequestsCriteria : CriteriaBase
    {
        public string Requests { get; set; }
        public int ActorId { get; set; }
        public byte RoleId { get; set; }
        public string Comment { get; set; }
        public bool IsIgnoredViolate { get; set; }
        public bool IsConfirmed { get; set; }
        public RequestCommand Command { get; set; }

        public override string GetSettingKey() => "WorkingTimeRequest_ApproveMultipleRequest";

        public override bool IsValid() => ActorId > 0;
    }
}