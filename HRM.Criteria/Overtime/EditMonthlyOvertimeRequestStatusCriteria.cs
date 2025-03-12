namespace HRM.Criteria.Overtime
{
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
            var key = string.Empty;
            switch (command)
            {
                case RequestCommand.Accept:
                case RequestCommand.Approve:
                case RequestCommand.Process:
                    key = "Overtime_ApproveRequest";
                    break;

                case RequestCommand.NotAccept:
                case RequestCommand.Reject:
                case RequestCommand.Void:
                    key = "Overtime_RejectRequest";
                    break;

                default:
                    key = "Overtime_ResetRequest";
                    break;
            }

            return key;
        }
    }
}