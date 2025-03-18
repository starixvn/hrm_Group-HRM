namespace HRM.Criteria.Request;

using Core.Enums;
using Fanex.Data;
using Fanex.Data.Repository;

public class VoidRequestCriteria : CriteriaBase
{
    public long RequestId { get; set; }
    public int ActorId { get; set; }

    [SpParam(Ignored = true)]
    public RequestType RequestTypeId { get; set; }

    public string Comment { get; set; }

    public override string GetSettingKey()
    {
        var key = RequestTypeId switch
        {
            RequestType.CheckIn or RequestType.CheckOut => "CheckInOutRequest_VoidBySA",
            RequestType.LateIn or RequestType.EarlyOut => "LateInEarlyOutRequest_VoidBySA",
            RequestType.ChangeShift => "ChangeShiftRequest_VoidBySA",
            RequestType.Leave => "LeaveRequest_VoidBySA",
            _ => "KeepInViewRequest_VoidBySA"
        };

        return key;
    }

    public override bool IsValid() => RequestId > 0 && ActorId > 0;
}