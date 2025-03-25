using System.Threading.Tasks;
using HRM.Criteria.Fnb;
using HRM.Models.FnB;
using HRM.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using IDepartmentService = HRM.Services.Interfaces.FnB.IDepartmentService;

namespace HRM.Controllers.API.FnB;

[ApiController]
[Route("/api/fnb/[controller]")]
public class DepartmentController(IDepartmentService departmentService,
    ICompanyService companyService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetDepartments([FromQuery] SyncFnBRequestModel request)
    {
        var company = await companyService.GetCompanySiteByDomainAsync();
        var criteria = new GetDepartmentByLastModifiedDateCriteria
        {
            CompanyId = company.CompanyId,
            LastModifiedDate = request.LastModifiedDate
        };

        var result = await departmentService.GetDepartmentByLastModifiedDate(criteria);

        return new JsonResult(result);
    }
}