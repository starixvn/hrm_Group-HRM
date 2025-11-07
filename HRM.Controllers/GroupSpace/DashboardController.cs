using System.Threading.Tasks;
using GO.UserControl.Client.Core.Enumerables;
using GO.UserControl.Client.Core.Session;
using HRM.Core.Constants;
using HRM.Core.Filters;
using HRM.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace HRM.Controllers.GroupSpace;

[HrmPermission(Policy.Read, FunctionKey.Dashboard)]
public class DashboardController(
    IGroupDashboardService groupDashboardService,
    IUserControlSession session)
    : BaseController(session)
{
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> GetDashboardData(int year)
    {
        var viewModel = await groupDashboardService.GetDashboardDataAsync(year);

        var settings = new JsonSerializerSettings
        {
            DateFormatString = "yyyy-MM-dd",
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        return Json(viewModel, settings);
    }
}
