using System;
using System.Threading.Tasks;
using HRM.Criteria.Fnb;
using HRM.Services.Interfaces.FnB;
using Microsoft.AspNetCore.Mvc;

namespace HRM.Controllers.API.FnB;

[ApiController]
[Route("api/fnb/[controller]")]
public class SectionController(ISectionService sectionService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetSections([FromQuery] DateTime lastModifiedDate)
    {
        var criteria = new GetSectionByLastModifiedDateCriteria
        {
            LastModifiedDate = lastModifiedDate
        };

        var result = await sectionService.GetSectionsByLastModifiedDate(criteria);

        return new JsonResult(result);
    }
}