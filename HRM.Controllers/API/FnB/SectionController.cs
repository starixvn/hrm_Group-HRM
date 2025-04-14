using System.Threading.Tasks;
using HRM.Criteria.Fnb;
using HRM.Models.FnB;
using HRM.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using ISectionService = HRM.Services.Interfaces.FnB.ISectionService;

namespace HRM.Controllers.API.FnB;

[ApiController]
[Route("api/fnb/[controller]")]
public class SectionController(
    ISectionService sectionService,
    ICompanyService companyService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetSections([FromQuery] SyncFnBRequestModel request)
    {
        var company = await companyService.GetCompanySiteByDomainAsync();
        var criteria = new GetSectionByLastModifiedDateCriteria
        {
            CompanyId = company.CompanyId,
            LastModifiedDate = request.LastModifiedDate
        };

        var result = await sectionService.GetSectionsByLastModifiedDate(criteria);

        return new JsonResult(result);
    }
}