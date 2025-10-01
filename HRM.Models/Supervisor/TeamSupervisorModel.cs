namespace HRM.Models.Supervisor;

using Core.Enums;
using Core.Helpers;

public class TeamSupervisorModel
{
    public int TeamId { get; set; }

    public string Code { get; set; }

    public string Name { get; set; }

    public int ActionType { get; set; }

    public string Supervisors { get; set; }

    public string ActionName =>
        ActionType > 0 ? EnumHelper.GetDescription((SupervisorActionType)ActionType) : string.Empty;
}
