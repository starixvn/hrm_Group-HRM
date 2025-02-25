using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace HRM.Controllers.API
{
    using System.Collections.Generic;
    using System.Linq;
    using HRM.Core.Enums;
    using HRM.Core.JWT;
    using HRM.Core.Models;
    using HRM.Models.API.Request;
    using HRM.Services;

    [Authorize]
    [JwtAuthentication]
    public class RequestController : ControllerBase
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
        public IActionResult ProcessRequest(ProcessRequestParam param)
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