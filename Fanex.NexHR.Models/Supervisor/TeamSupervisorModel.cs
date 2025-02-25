namespace Fanex.NexHR.Models.Supervisor
{
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Core.Helpers;

    public class TeamSupervisorModel
    {
        public int TeamId { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public int ActionType { get; set; }

        public string Supervisors { get; set; }

        public string ActionName
        {
            get
            {
                if (ActionType > 0)
                {
                    return EnumHelper.GetStringValue((SupervisorActionType)ActionType);
                }

                return string.Empty;
            }
        }
    }
}