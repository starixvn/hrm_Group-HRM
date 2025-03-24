using System;
using System.Threading.Tasks;
using HRM.Criteria.Fnb;
using HRM.Models.FnB;
using HRM.Services.Interfaces.FnB;
using Microsoft.AspNetCore.Mvc;

namespace HRM.Controllers.API.FnB;

[ApiController]
[Route("api/fnb/[controller]")]
public class UnitController(IUnitService unitService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetUnits([FromQuery] SyncFnBRequestModel request)
    {
        var criteria = new GetUnitByLastModifiedDateCriteria
        {
            CompanyId = request.CompanyId,
            LastModifiedDate = request.LastModifiedDate
        };

        var result = await unitService.GetUnitByLastModifiedDate(criteria);

        return new JsonResult(result);
    }
}