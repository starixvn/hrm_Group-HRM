using System.Threading.Tasks;
using HRM.Core.Models;
using HRM.Models.LetsTalk;
using HRM.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HRM.Controllers.LetsTalk;

[ApiController]
[Route("api/letstalk/[controller]")]
public class LetsTalkController(
    ILetstalkService letstalkService, LetstalkSettings letstalkSettings) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> SendGetMessage()
    {
        var replyMarkup = new ReplyMarkup
        {
            Actions =
            [
                new() {
                    ActionType = ReplyActionType.PostBack,
                    Label = "Accept",
                    Data = $"{letstalkSettings.HRMCallbackUrl}api/letstalk/letstalk?accept=1"
                },
                new() {
                    ActionType = ReplyActionType.PostBack,
                    Label = "Reject",
                    Data = $"{letstalkSettings.HRMCallbackUrl}api/letstalk/letstalk?reject=1"
                },
            ]
        };
        await letstalkService.SendNotificationAsync("t870342617", "Henry test", replyMarkup);
        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> SendPostMessage()
    {
        var replyMarkup = new ReplyMarkup
        {
            Actions =
            [
                new() {
                    ActionType = ReplyActionType.PostBack,
                    Label = "Accept",
                    Data = $"{letstalkSettings.HRMCallbackUrl}api/letstalk/letstalk?accept=1"
                },
                new() {
                    ActionType = ReplyActionType.PostBack,
                    Label = "Reject",
                    Data = $"{letstalkSettings.HRMCallbackUrl}api/letstalk/letstalk?reject=1"
                },
            ]
        };
        await letstalkService.SendNotificationAsync("t870342617", "Henry test", replyMarkup);
        return Ok();
    }
}