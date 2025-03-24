using System;
using System.Threading.Tasks;
using HRM.Criteria.Fnb;
using HRM.Models.FnB;
using HRM.Services.Interfaces.FnB;
using Microsoft.AspNetCore.Mvc;

namespace HRM.Controllers.API.FnB;

[ApiController]
[Route("/api/fnb/[controller]")]
public class DepartmentController(IDepartmentService departmentService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetDepartments([FromQuery] SyncFnBRequestModel request)
    {
        var criteria = new GetDepartmentByLastModifiedDateCriteria
        {
            CompanyId = request.CompanyId,
            LastModifiedDate = request.LastModifiedDate
        };

        var result = await departmentService.GetDepartmentByLastModifiedDate(criteria);

        return new JsonResult(result);
    }
}