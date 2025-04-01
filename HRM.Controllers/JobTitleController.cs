using HRM.Controllers.Export;

namespace HRM.Controllers;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Constants;
using Core.Enums;
using Core.Filters;
using Criteria.Lookup;
using GO.UserControl.Client.Core.Enumerables;
using GO.UserControl.Client.Core.Session;
using HRM.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Resources;
using Services.ExportExcel;
using Services.Implement;

public class JobTitleController(
    IUserControlSession session,
    ICommonDataService commonDataService,
    IJobTitleExportService jobTitleExportService,
    IBaseService baseService) : BaseExportController(session)
{
    [HrmPermission(Policy.Read, FunctionKey.JobTitleManagement)]
    public IActionResult JobTitleManagement()
    {
        var hasPermission = session.HasPermissionOnFunction(FunctionKey.JobTitleManagement, Policy.Host);
        return View(hasPermission);
    }

    [HrmPermission(Policy.Host, FunctionKey.JobTitleManagement)]
    public IActionResult GetJobCategory()
    {
        var jobCategories = GetJobCategories();
        return Json(jobCategories);
    }

    [HrmPermission(Policy.Read, FunctionKey.JobTitleManagement)]
    public async Task<IActionResult> GetJobTitles([FromBody] JobTitleModel model)
    {
        var jobTitles = await commonDataService.GetJobTitle(model.JobTitle, CurrentCompanyId);
        return Json(jobTitles);
    }

    [HrmPermission(Policy.Host, FunctionKey.JobTitleManagement)]
    public async Task<IActionResult> ExportExcel(string jobTitle)
    {
        var jobTitles = await commonDataService.GetJobTitle(jobTitle, CurrentCompanyId);
        var bytes = jobTitleExportService.Export(jobTitles);

        return File(bytes, MimeType.XLSX, $"JobTitle_{DateTimeOffset.Now:yyyyMMdd}.xlsx");
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.JobTitleManagement)]
    public async Task<IActionResult> Delete([FromBody] JobTitleDeleteModel model)
    {
        var criteria = new DeleteLookupDataCriteria
        {
            Id = model.Id
        };
        var result = await baseService.ExecuteNonQueryAsync(criteria);

        return Json(result);
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.JobTitleManagement)]
    public async Task<IActionResult> Insert([FromBody] JobTitleInsertModel model)
    {
        var criteria = new InsertLookupDataCriteria
        {
            LookupCode = Constants.JobTitle,
            Name = model.Name,
            CategoryId = model.CategoryId.ToString(),
            CompanyId = CurrentCompanyId
        };
        var result = await baseService.ExecuteNonQueryAsync(criteria);

        return Json(result);
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.JobTitleManagement)]
    public async Task<IActionResult> Update([FromBody] JobTitleUpdateModel model)
    {
        var criteria = new UpdateLookupDataCriteria
        {
            Id = model.Id,
            Name = model.Name,
            CategoryId = model.CategoryId.ToString()
        };
        var result = await baseService.ExecuteNonQueryAsync(criteria);

        return Json(result);
    }

    private static IList<SelectListItem> GetJobCategories() =>
        new List<SelectListItem>
        {
            new() { Text = Labels.Worker, Value = ((int)JobTitleCategory.Worker).ToString() },
            new() { Text = Labels.Staff, Value = ((int)JobTitleCategory.Staff).ToString() },
            new() { Text = Labels.Executive, Value = ((int)JobTitleCategory.Executive).ToString() },
            new() { Text = Labels.ExecutiveWithPosition, Value = ((int)JobTitleCategory.ExecutiveWithPosition).ToString() },
            new() { Text = Labels.Management, Value = ((int)JobTitleCategory.Management).ToString() }
        };
}

public class JobTitleUpdateModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int CategoryId { get; set; }
}

public class JobTitleDeleteModel
{
    public int Id { get; set; }
}

public class JobTitleInsertModel
{
    public string Name { get; set; }
    public int CategoryId { get; set; }
}

public class JobTitleModel
{
    public string JobTitle { get; set; }
}