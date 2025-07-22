using System.Threading.Tasks;
using HRM.Models.Request;
using HRM.Services.Interfaces.Request;
using Microsoft.AspNetCore.Mvc;
using HRM.Core.Enums;
using HRM.Core.Models;

namespace HRM.Controllers.API.Request;

[ApiController]
[Route("api/[controller]")]
public class RequestController(IRequestService requestService) : ControllerBase
{
    [HttpPost("Process")]
    public async Task<IActionResult> ProcessRequest([FromQuery] ProcessRequestParam request, [FromQuery] EmployeeSession employee)
    {
        var commandResult = request.Command switch
        {
            RequestCommand.Accept or RequestCommand.Approve or RequestCommand.Process => await requestService
                .ApproveRequest(employee.EmpId, request),
            RequestCommand.Reset => await requestService.ResetRequest(employee.EmpId, request),
            RequestCommand.NotAccept or RequestCommand.Reject or RequestCommand.Void => await requestService
                .RejectRequest(employee.EmpId, request),
            _ => await requestService.VoidRequest(employee.EmpId, request)
        };

        if (commandResult.IsSuccessful)
        {
            await requestService.SendMailProcessRequest(request, employee, request.Command);
        }

        return new JsonResult(commandResult);
    }
}