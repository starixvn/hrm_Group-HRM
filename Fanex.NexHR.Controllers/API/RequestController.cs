namespace Fanex.NexHR.Controllers.API
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Http;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Core.JWT;
    using Fanex.NexHR.Core.Models;
    using Fanex.NexHR.Models.API.Request;
    using Fanex.NexHR.Services;

    [Authorize]
    [JwtAuthentication]
    public class RequestController : ApiController
    {
        private readonly IHRMAuthService authService;
        private readonly IRequestService requestService;

        public RequestController(
            IHRMAuthService authService,
            IRequestService requestService)
        {
            this.authService = authService;
            this.requestService = requestService;
        }

        [HttpPost]
        public IHttpActionResult ProcessRequest(ProcessRequestParam param)
        {
            EmployeeSession employee = authService.GetByUserId(param.UserId);
            RoleSystem roleId = RoleSystem.Verifier;
            RequestCommand requestCommand = RequestCommand.Accept;
            switch (param.StatusId)
            {
                case (byte)CurrentRequestStatus.Verified:
                    requestCommand = RequestCommand.Accept;
                    break;

                case (byte)CurrentRequestStatus.NotVerified:
                    requestCommand = RequestCommand.NotAccept;
                    break;

                case (byte)CurrentRequestStatus.Approved:
                    requestCommand = RequestCommand.Approve;
                    roleId = RoleSystem.Approver;
                    break;

                case (byte)CurrentRequestStatus.Rejected:
                    requestCommand = RequestCommand.Reject;
                    roleId = RoleSystem.Approver;
                    break;

                default:
                    // nothing
                    break;
            }

            var disciplines = new List<Models.Request.DisciplineReview>();
            if (param.Disciplines != null && param.Disciplines.Any())
            {
                disciplines = param.Disciplines.Select(x => new Models.Request.DisciplineReview
                {
                    WorkDate = x.WorkDate,
                    EmployeeId = param.EmpId,
                    DisciplineTypeId = x.TypeId,
                    IsIgnored = x.IsIgnore
                }).ToList();
            }

            var model = new Models.Request.ProcessRequestParam
            {
                RequestId = param.RequestId,
                EmployeeId = param.EmpId,
                RequestTypeId = (RequestType)param.RequestTypeId,
                RoleId = (byte)roleId,
                Command = requestCommand,
                Comment = param.Comment,
                Disciplines = disciplines
            };

            CommandResult commandResult = ProcessRequest(employee, model);

            return Ok(commandResult);
        }

        private CommandResult ProcessRequest(EmployeeSession employee, Models.Request.ProcessRequestParam model)
        {
            var commandResult = new CommandResult();

            switch (model.Command)
            {
                case RequestCommand.Accept:
                case RequestCommand.Approve:
                    commandResult = requestService.ApproveRequest(employee.EmpId, model);
                    break;

                case RequestCommand.NotAccept:
                case RequestCommand.Reject:
                    commandResult = requestService.RejectRequest(employee.EmpId, model);
                    break;

                default:
                    // nothing
                    break;
            }

            if (commandResult.IsSuccessful)
            {
                requestService.SendMailProcessRequest(model, employee, model.Command);
            }

            return commandResult;
        }
    }
}