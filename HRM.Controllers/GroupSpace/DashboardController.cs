using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GO.UserControl.Client.Core.Session;
using HRM.Services.Interfaces.Report.Headcount;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace HRM.Controllers.GroupSpace;

public class DashboardController(
    IHeadcountReportService service,
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
        var data = await service.GetReportAsync(year);
        var viewModel = new DashboardViewModel
        {
            CurrentHeadcount = data.ReportByUnit.Find(x => x.IsGrandTotal).Details.LastOrDefault()?.CurrentHeadCount ?? 0,
            Recruitment = data.ReportByUnit.Find(x => x.IsGrandTotal).Details.LastOrDefault()?.NewHire ?? 0,
            ProbationResign = data.ReportByUnit.Find(x => x.IsGrandTotal).Details.Sum(x => x.ProbationResign) ?? 0,
            ConfirmedResign = data.ReportByUnit.Find(x => x.IsGrandTotal).Details.Sum(x=> x.ConfirmedResign) ?? 0,
            Companies = data.ReportByCompany.Count(x => !x.IsGrandTotal && !x.IsUnitCategory),
            AvgAge = 35,
            AvgYrsTenure = 13,
            HeadcountByBU = data.ReportByUnit.Where(x=> !x.IsGrandTotal && !x.IsUnitCategory)
                .Select(x => new HeadcountModel { Label = x.Label1, Value = x.Details.LastOrDefault()?.CurrentHeadCount ?? 0 }),
            HeadcountByCountry = data.ReportByUnitCountry.Where(x => x.IsGrandTotal).Select(x => new HeadcountModel { Label = x.Label3, Value = x.Details.LastOrDefault()?.CurrentHeadCount ?? 0 }),
        };
        var settings = new JsonSerializerSettings
        {
            DateFormatString = "yyyy-MM-dd",
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        return Json(viewModel, settings);
    }
}


public class DashboardViewModel
{
    public int CurrentHeadcount { get; set; }
    public int Recruitment { get; set; }
    public int ProbationResign { get; set; }
    public int ConfirmedResign { get; set; }
    public int Companies { get; set; }
    public int AvgAge { get; set; }
    public int AvgYrsTenure { get; set; }
    public int Countries => HeadcountByCountry.Count();
    public int BusinessUnits => HeadcountByBU.Count();
    public IEnumerable<HeadcountModel> HeadcountByBU { get; set; }
    public IEnumerable<HeadcountModel> HeadcountByCountry { get; set; }
}

public class HeadcountModel 
{
    public string Label { get; set; }
    public int Value { get; set; }
}
