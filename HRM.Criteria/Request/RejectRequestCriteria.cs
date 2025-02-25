namespace HRM.Criteria.Request
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using HRM.Core.Enums;

    public class RejectRequestCriteria : CriteriaBase
    {
        public long RequestId { get; set; }
        public int ActorId { get; set; }
        public int RoleId { get; set; }
        public string Comment { get; set; }

        [SpParam(Ignored = true)]
        public RequestType RequestTypeId { get; set; }

        public override string GetSettingKey()
        {
            var key = string.Empty;
            switch (RequestTypeId)
            {
                case RequestType.CheckIn:
                case RequestType.CheckOut:
                    key = "CheckInOutRequest_Reject";
                    break;

                case RequestType.LateIn:
                case RequestType.EarlyOut:
                    key = "LateInEarlyOutRequest_Reject";
                    break;

                case RequestType.ChangeShift:
                    key = "ChangeShiftRequest_Reject";
                    break;

                case RequestType.Leave:
                    key = "LeaveRequest_Reject";
                    break;

                default:
                    key = "KeepInViewRequest_Reject";
                    break;
            }

            return key;
        }

        public override bool IsValid() => RequestId > 0 && ActorId > 0;
    }
}