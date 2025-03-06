using System.Threading.Tasks;
using GO.UserControl.Client.Core.Enumerables;
using GO.UserControl.Client.Core.Session;
using HRM.Core.Constants;
using HRM.Core.Filters;
using HRM.Criteria.JobLevel;
using HRM.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HRM.Controllers;

public class JobLevelController(
    IUserControlSession userControlSession,
    IJobLevelService jobLevelService) : BaseController(userControlSession)
{
    private readonly IUserControlSession userControlSession = userControlSession;

    [HrmPermission(Policy.Read, FunctionKey.JobLevelManagement)]
    public IActionResult Index()
    {
        var hasPermission = userControlSession.HasPermissionOnFunction(FunctionKey.JobLevelManagement, Policy.Host);
        return View(hasPermission);
    }

    [HrmPermission(Policy.Read, FunctionKey.JobLevelManagement)]
    public async Task<IActionResult> GetJobLevels()
    {
        var jobLevels = await jobLevelService.GetJobLevel();
        return Json(jobLevels);
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.JobLevelManagement)]
    public async Task<IActionResult> Insert([FromBody] JobLevelInsertModel model)
    {
        var criteria = new InsertJobLevelCriteria
        {
            Name = model.Name,
            ModifiedBy = EmployeeSession.EmpId
        };

        var result = await jobLevelService.Insert(criteria);

        return Json(result);
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.JobLevelManagement)]
    public async Task<IActionResult> Update([FromBody] JobLevelUpdateModel model)
    {
        var criteria = new UpdateJobLevelCriteria
        {
            JobLevelId = model.Id,
            Name = model.Name,
            ModifiedBy = EmployeeSession.EmpId
        };
        var result = await jobLevelService.Update(criteria);

        return Json(result);
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.JobLevelManagement)]
    public async Task<IActionResult> Delete([FromBody] JobLevelDeleteModel model)
    {
        var result = await jobLevelService.Delete(model.Id, EmployeeSession.EmpId);

        return Json(result);
    }
}

public class JobLevelUpdateModel
{
    public int Id { get; set; }
    public string Name { get; set; }
}

public class JobLevelDeleteModel
{
    public int Id { get; set; }
}

public class JobLevelInsertModel
{
    public string Name { get; set; }
}