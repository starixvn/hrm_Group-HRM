using System.Threading.Tasks;
using HRM.Models.LetsTalk;
using HRM.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HRM.Controllers.LetsTalk;

[ApiController]
[Route("api/letstalk/[controller]")]
public class LetsTalkController(
    ILetstalkService letstalkService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> SendMessage()
    {
        var replyMarkup = new ReplyMarkup
        {
            Actions =
            [
                new() {
                    ActionType = ReplyActionType.PostBack,
                    Label = "Accept",
                    Data = "callback_accept"
                },
                new() {
                    ActionType = ReplyActionType.PostBack,
                    Label = "Reject",
                    Data = "callback_reject"
                },
                new() {
                    ActionType = ReplyActionType.Uri,
                    Label = "View details",
                    Uri = "https://www.google.com"
                }
            ]
        };
        await letstalkService.SendNotificationAsync("t870342617", "test", replyMarkup);
        return Ok();
    }
}