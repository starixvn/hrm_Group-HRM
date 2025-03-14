namespace HRM.Criteria.Request;

using Fanex.Data;
using Fanex.Data.Repository;
using Core.Enums;

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
	    var key = RequestTypeId switch
	    {
		    RequestType.CheckIn or RequestType.CheckOut => "CheckInOutRequest_Reject",
		    RequestType.LateIn or RequestType.EarlyOut => "LateInEarlyOutRequest_Reject",
		    RequestType.ChangeShift => "ChangeShiftRequest_Reject",
		    RequestType.Leave => "LeaveRequest_Reject",
		    _ => "KeepInViewRequest_Reject"
	    };

	    return key;
    }

    public override bool IsValid() => RequestId > 0 && ActorId > 0;
}