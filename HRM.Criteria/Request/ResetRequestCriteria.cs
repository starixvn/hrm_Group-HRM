namespace HRM.Criteria.Request;

using Fanex.Data;
using Fanex.Data.Repository;
using Core.Enums;

public class ResetRequestCriteria : CriteriaBase
{
    public long RequestId { get; set; }
    public int ActorId { get; set; }
    public int RoleId { get; set; }

    [SpParam(Ignored = true)]
    public RequestType RequestTypeId { get; set; }

    public override string GetSettingKey()
    {
        var key = string.Empty;
        switch (RequestTypeId)
        {
            case RequestType.CheckIn:
            case RequestType.CheckOut:
                key = "CheckInOutRequest_Reset";
                break;

            case RequestType.LateIn:
            case RequestType.EarlyOut:
                key = "LateInEarlyOutRequest_Reset";
                break;

            case RequestType.ChangeShift:
                key = "ChangeShiftRequest_Reset";
                break;

            case RequestType.Leave:
                key = "LeaveRequest_Reset";
                break;

            default:
                key = "KeepInViewRequest_Reset";
                break;
        }

        return key;
    }

    public override bool IsValid() => RequestId > 0 && ActorId > 0;
}