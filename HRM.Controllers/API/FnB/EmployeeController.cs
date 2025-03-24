using System;
using System.Threading.Tasks;
using HRM.Criteria.Fnb;
using HRM.Models.FnB;
using HRM.Services.Interfaces.FnB;
using Microsoft.AspNetCore.Mvc;

namespace HRM.Controllers.API.FnB;

[ApiController]
[Route("api/fnb/[controller]")]
public class EmployeeController(IEmployeeService employeeService) : ControllerBase
{

    [HttpGet]
    public async Task<IActionResult> GetEmployees([FromQuery] SyncFnBRequestModel request)
    {
        var criteria = new GetEmployeeByLastModifiedDateCriteria
        {
            CompanyId = request.CompanyId,
            LastModifiedDate = request.LastModifiedDate
        };

        var result = await employeeService.GetEmployeesByLastModifiedDate(criteria);

        return new JsonResult(result);
    }

    [HttpGet("team")]
    public async Task<IActionResult> GetEmployeeGroups([FromQuery] SyncFnBRequestModel request)
    {
        var criteria = new GetEmployeeGroupByLastModifiedDateCriteria
        {
            CompanyId = request.CompanyId,
            LastModifiedDate = request.LastModifiedDate
        };
        var result = await employeeService.GetEmployeeGroupByLastModifiedDate(criteria);
        return new JsonResult(result);
    }

    [HttpGet("calendar")]
    public async Task<IActionResult> GetEmployeeCalendars([FromQuery] SyncFnBRequestModel request)
    {
        var criteria = new GetEmployeeCalendarByLastModifiedDateCriteria
        {
            CompanyId = request.CompanyId,
            LastModifiedDate = request.LastModifiedDate
        };
        var result = await employeeService.GetEmployeeCalendarByLastModifiedDate(criteria);
        return new JsonResult(result);
    }
}