namespace HRM.Criteria.Overtime;

using Fanex.Data;
using Fanex.Data.Repository;
using Core.Enums;

public class EditMonthlyOvertimeRequestStatusCriteria : CriteriaBase
{
    public int ActorId { get; set; }
    public int RoleId { get; set; }
    public string RequestIds { get; set; }

    [SpParam(Ignored = true)]
    public RequestCommand RequestCommand { get; set; }

    public override string GetSettingKey() => GetSpKey(RequestCommand);

    public override bool IsValid() => ActorId > 0 && !string.IsNullOrWhiteSpace(RequestIds);

    private static string GetSpKey(RequestCommand command)
    {
	    var key = command switch
	    {
		    RequestCommand.Accept or RequestCommand.Approve or RequestCommand.Process => "Overtime_ApproveRequest",
		    RequestCommand.NotAccept or RequestCommand.Reject or RequestCommand.Void => "Overtime_RejectRequest",
		    _ => "Overtime_ResetRequest"
	    };

	    return key;
    }
}