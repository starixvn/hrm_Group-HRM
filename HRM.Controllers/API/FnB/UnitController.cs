using System.Threading.Tasks;
using HRM.Criteria.Fnb;
using HRM.Models.FnB;
using HRM.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using IUnitService = HRM.Services.Interfaces.FnB.IUnitService;

namespace HRM.Controllers.API.FnB;

[ApiController]
[Route("api/fnb/[controller]")]
public class UnitController(
    IUnitService unitService,
    ICompanyService companyService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetUnits([FromQuery] SyncFnBRequestModel request)
    {
        var criteria = new GetUnitByLastModifiedDateCriteria
        {
            LastModifiedDate = request.LastModifiedDate
        };

        var result = await unitService.GetUnitByLastModifiedDate(criteria);

        return new JsonResult(result);
    }
}