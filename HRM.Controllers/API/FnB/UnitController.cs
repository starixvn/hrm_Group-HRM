using System;
using System.Threading.Tasks;
using HRM.Criteria.Fnb;
using HRM.Services.Interfaces.FnB;
using Microsoft.AspNetCore.Mvc;

namespace HRM.Controllers.API.FnB;

[ApiController]
[Route("api/fnb/[controller]")]
public class UnitController(IUnitService unitService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetUnits([FromQuery] DateTime lastModifiedDate)
    {
        var criteria = new GetUnitByLastModifiedDateCriteria()
        {
            LastModifiedDate = lastModifiedDate
        };

        var result = await unitService.GetUnitByLastModifiedDate(criteria);

        return new JsonResult(result);
    }
}