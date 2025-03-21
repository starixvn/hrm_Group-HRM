using System;
using System.Threading.Tasks;
using HRM.Criteria.Fnb;
using HRM.Services.Interfaces.FnB;
using Microsoft.AspNetCore.Mvc;

namespace HRM.Controllers.API.FnB;

[ApiController]
[Route("api/fnb/[controller]")]
public class EmployeeController(IEmployeeService employeeService) : ControllerBase
{

    [HttpGet]
    public async Task<IActionResult> GetEmployees([FromQuery] DateTime lastModifiedDate)
    {
        var criteria = new GetEmployeeByLastModifiedDateCriteria
        {
            LastModifiedDate = lastModifiedDate
        };

        var result = await employeeService.GetEmployeesByLastModifiedDate(criteria);

        return new JsonResult(result);
    }

    [HttpGet("team")]
    public async Task<IActionResult> GetEmployeeGroups([FromQuery] DateTime lastModifiedDate)
    {
        var criteria = new GetEmployeeGroupByLastModifiedDateCriteria
        {
            LastModifiedDate = lastModifiedDate
        };
        var result = await employeeService.GetEmployeeGroupByLastModifiedDate(criteria);
        return new JsonResult(result);
    }

    [HttpGet("calendar")]
    public async Task<IActionResult> GetEmployeeCalendars([FromQuery] DateTime lastModifiedDate)
    {
        var criteria = new GetEmployeeCalendarByLastModifiedDateCriteria
        {
            LastModifiedDate = lastModifiedDate
        };
        var result = await employeeService.GetEmployeeCalendarByLastModifiedDate(criteria);
        return new JsonResult(result);
    }
}