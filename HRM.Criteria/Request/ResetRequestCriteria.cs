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
	    var key = RequestTypeId switch
	    {
		    RequestType.CheckIn or RequestType.CheckOut => "CheckInOutRequest_Reset",
		    RequestType.LateIn or RequestType.EarlyOut => "LateInEarlyOutRequest_Reset",
		    RequestType.ChangeShift => "ChangeShiftRequest_Reset",
		    RequestType.Leave => "LeaveRequest_Reset",
		    _ => "KeepInViewRequest_Reset"
	    };

	    return key;
    }

    public override bool IsValid() => RequestId > 0 && ActorId > 0;
}