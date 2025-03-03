namespace HRM.Controllers;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GO.UserControl.Client.Core.Enumerables;
using GO.UserControl.Client.Core.Session;
using Core;
using Core.Constants;
using Core.Enums;
using Core.Filters;
using HRM.Core.Services;
using Criteria.Lookup;
using Models;
using Resources;
using Services.ExportExcel;
using Services.Implement;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

public class JobTitleController(
    IUserControlSession userControlSession,
    ICommonDataService commonDataService,
    IJobTitleExportService jobTitleExportService,
    IBaseService baseService)
    : BaseExportController(userControlSession)
{
    private readonly IUserControlSession userControlSession = userControlSession;

    [HrmPermission(Policy.Read, FunctionKey.JobTitleManagement)]
    public ActionResult JobTitleManagement()
    {
        var hasPermission = userControlSession.HasPermissionOnFunction(FunctionKey.JobTitleManagement, Policy.Host);
        return View(hasPermission);
    }

    [HrmPermission(Policy.Host, FunctionKey.JobTitleManagement)]
    public Task<CustomJsonResult> GetJobCategory()
    {
        var jobCategories = GetJobCategories();
        return Task.FromResult(new CustomJsonResult(jobCategories));
    }

    [HrmPermission(Policy.Read, FunctionKey.JobTitleManagement)]
    public async Task<CustomJsonResult> GetJobTitles(string jobTitle)
    {
        IList<LookupData> jobTitles = await commonDataService.GetJobTitle(jobTitle);
        return new CustomJsonResult (jobTitles);
    }

    [HrmPermission(Policy.Host, FunctionKey.JobTitleManagement)]
    public async Task<FileContentResult> ExportExcel(string jobTitle)
    {
        IList<LookupData> jobTitles = await commonDataService.GetJobTitle(jobTitle);
        var bytes = jobTitleExportService.Export(jobTitles);

        return File(bytes, MimeType.XLSX, $"JobTitle_{DateTimeOffset.Now:yyyyMMdd}.xlsx");
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.JobTitleManagement)]
    public async Task<CustomJsonResult> Delete([FromBody]JobTitleDeleteModel model)
    {
        var criteria = new DeleteLookupDataCriteria
        {
            Id = model.Id
        };
        var result = await baseService.ExecuteNonQueryAsync(criteria);

        return new CustomJsonResult (result);
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.JobTitleManagement)]
    public async Task<CustomJsonResult> Insert([FromBody]JobTitleInsertModel model)
    {
        var criteria = new InsertLookupDataCriteria
        {
            LookupCode = Constants.JobTitle,
            Name = model.Name,
            CategoryId = model.CategoryId.ToString()
        };
        var result = await baseService.ExecuteNonQueryAsync(criteria);

        return new CustomJsonResult (result);
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.JobTitleManagement)]
    public async Task<CustomJsonResult> Update([FromBody] JobTitleUpdateModel model)
    {
        var criteria = new UpdateLookupDataCriteria
        {
            Id = model.Id,
            Name = model.Name,
            CategoryId = model.CategoryId.ToString()
        };
        var result = await baseService.ExecuteNonQueryAsync(criteria);

        return new CustomJsonResult (result);
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