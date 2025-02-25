namespace Fanex.NexHR.Criteria.Request
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Core.Enums;

    public class VoidRequestCriteria : CriteriaBase
    {
        public long RequestId { get; set; }
        public int ActorId { get; set; }

        [SpParam(Ignored = true)]
        public RequestType RequestTypeId { get; set; }

        public string Comment { get; set; }

        public override string GetSettingKey()
        {
            string key;
            switch (RequestTypeId)
            {
                case RequestType.CheckIn:
                case RequestType.CheckOut:
                    key = "CheckInOutRequest_VoidBySA";
                    break;

                case RequestType.LateIn:
                case RequestType.EarlyOut:
                    key = "LateInEarlyOutRequest_VoidBySA";
                    break;

                case RequestType.ChangeShift:
                    key = "ChangeShiftRequest_VoidBySA";
                    break;

                case RequestType.Leave:
                    key = "LeaveRequest_VoidBySA";
                    break;

                default:
                    key = "KeepInViewRequest_VoidBySA";
                    break;
            }

            return key;
        }

        public override bool IsValid() => RequestId > 0 && ActorId > 0;
    }
}