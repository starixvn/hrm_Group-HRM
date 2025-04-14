using System.Threading.Tasks;
using HRM.Criteria.Fnb;
using HRM.Models.FnB;
using HRM.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using IEmployeeService = HRM.Services.Interfaces.FnB.IEmployeeService;

namespace HRM.Controllers.API.FnB;

[ApiController]
[Route("api/fnb/[controller]")]
public class EmployeeController(
    IEmployeeService employeeService,
    ICompanyService companyService) : ControllerBase
{

    [HttpGet]
    public async Task<IActionResult> GetEmployees([FromQuery] SyncFnBRequestModel request)
    {
        var company = await companyService.GetCompanySiteByDomainAsync();
        var criteria = new GetEmployeeByLastModifiedDateCriteria
        {
            CompanyId = company.CompanyId,
            LastModifiedDate = request.LastModifiedDate
        };

        var result = await employeeService.GetEmployeesByLastModifiedDate(criteria);

        return new JsonResult(result);
    }

    [HttpGet("team")]
    public async Task<IActionResult> GetEmployeeTeams([FromQuery] SyncFnBRequestModel request)
    {
        var company = await companyService.GetCompanySiteByDomainAsync();
        var criteria = new GetEmployeeTeamByLastModifiedDateCriteria
        {
            CompanyId = company.CompanyId,
            LastModifiedDate = request.LastModifiedDate
        };
        var result = await employeeService.GetEmployeeTeamByLastModifiedDate(criteria);
        return new JsonResult(result);
    }

    [HttpGet("calendar")]
    public async Task<IActionResult> GetEmployeeCalendars([FromQuery] SyncFnBRequestModel request)
    {
        var company = await companyService.GetCompanySiteByDomainAsync();
        var criteria = new GetEmployeeCalendarByLastModifiedDateCriteria
        {
            CompanyId = company.CompanyId,
            LastModifiedDate = request.LastModifiedDate
        };
        var result = await employeeService.GetEmployeeCalendarByLastModifiedDate(criteria);
        return new JsonResult(result);
    }
}