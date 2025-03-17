using System;
using System.Threading.Tasks;
using HRM.Criteria.Fnb;
using HRM.Services.Interfaces.FnB;
using Microsoft.AspNetCore.Mvc;

namespace HRM.Controllers.API.FnB;

[ApiController]
[Route("api/fnb/employee/[controller]")]
public class EmployeeController(IEmployeeService fnBService) : ControllerBase
{

    [HttpGet]
    public async Task<IActionResult> GetEmployeeByLastModifiedDate([FromQuery] DateTime lastModifiedDate)
    {
        var criteria = new GetEmployeeByLastModifiedDateCriteria
        {
            LastModifiedDate = lastModifiedDate
        };

        var result = await fnBService.GetEmployeesByLastModifiedDate(criteria);

        return new JsonResult(result);
    }

    [HttpGet("group")]
    public async Task<IActionResult> GetEmployeeGroupByLastModifiedDate([FromQuery] DateTime lastModifiedDate)
    {
        var criteria = new GetEmployeeGroupByLastModifiedDateCriteria
        {
            LastModifiedDate = lastModifiedDate
        };
        var result = await fnBService.GetEmployeeGroupByLastModifiedDate(criteria);
        return new JsonResult(result);
    }
}