using System;
using System.Threading.Tasks;
using HRM.Criteria.Fnb;
using HRM.Models.FnB;
using HRM.Services.Interfaces.FnB;
using Microsoft.AspNetCore.Mvc;

namespace HRM.Controllers.API.FnB;

[ApiController]
[Route("api/fnb/[controller]")]
public class SectionController(ISectionService sectionService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetSections([FromQuery] SyncFnBRequestModel request)
    {
        var criteria = new GetSectionByLastModifiedDateCriteria
        {
            CompanyId = request.CompanyId,
            LastModifiedDate = request.LastModifiedDate
        };

        var result = await sectionService.GetSectionsByLastModifiedDate(criteria);

        return new JsonResult(result);
    }
}