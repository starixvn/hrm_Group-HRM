using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using GO.UserControl.Client.Core.Enumerables;
using GO.UserControl.Client.Core.Session;
using HRM.Core;
using HRM.Core.Constants;
using HRM.Core.Filters;
using HRM.Core.Models;
using HRM.Core.Services;
using HRM.Criteria.EmployeeWorkFromHome;
using HRM.Models;
using HRM.Services.Interfaces;
using HRM.ViewModels.EmployeeWorkFromHome;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace HRM.Controllers;

public class EmployeeWorkFromHomeController : BaseController
{
    private readonly IUserControlSession userControlSession;
    private readonly IBaseService baseService;
    private readonly IAttachFileService attachFileService;
    private readonly string uploadFolderPath;


    public EmployeeWorkFromHomeController(
        IUserControlSession userControlSession,
        IBaseService baseService,
        IAttachFileService attachFileService) : base(userControlSession)
    {
        this.baseService = baseService;
        this.attachFileService = attachFileService;
        this.userControlSession = userControlSession;
        uploadFolderPath = $"{CurrentCompanyId}/WHF";
    }


    [HrmPermission(Policy.Read, FunctionKey.EmployeeManagement)]
    public ActionResult Index()
    {
        var currentDateTime = DateTimeOffset.Now.DateTime.ToClientTimeZone();
        var model = new EmployeeWorkFromHomeFilterViewModel
        {
            FromDateFilter = currentDateTime,
            ToDateFilter = currentDateTime
        };

        ViewBag.IsHost = userControlSession.HasPermissionOnFunction(FunctionKey.EmployeeManagement, Policy.Host);
        return View(model);
    }

    [HrmPermission(Policy.Read, FunctionKey.EmployeeManagement)]
    public async Task<IActionResult> GetEmployeeWorkFromHomeGrid(GetEmployeeWorkFromHomeCriteria criteria)
    {
        var employees = await baseService.GetDataAsync<EmployeeWorkFromHome>(criteria);
        foreach (var employee in employees)
        {
            employee.AttachFiles = await attachFileService.GetInfoAttachFileAsync(employee.EmployeeWorkFromHomeId, uploadFolderPath);
        }

        ViewBag.IsHost = userControlSession.HasPermissionOnFunction(FunctionKey.EmployeeManagement, Policy.Host);
        return PartialView("_EmployeeWorkFromHomeGrid", employees);
    }

    [HrmPermission(Policy.Host, FunctionKey.EmployeeManagement)]
    public IActionResult Create()
    {
        var currentDateTime = DateTimeOffset.Now.DateTime.ToClientTimeZone();
        var model = new EmployeeWorkFromHomeViewModel
        {
            FromDate = currentDateTime,
            ToDate = currentDateTime
        };
        return PartialView("_Create", model);
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.EmployeeManagement)]
    public async Task<IActionResult> Create(string requestModel, IList<IFormFile> files)
    {
        var model = JsonSerializer.Deserialize<EmployeeWorkFromHomeViewModel>(requestModel, JsonOptions);
        var result = new CommandResult();

        if (model != null && ModelState.IsValid)
        {
            var criteria = new InsertEmployeeWorkFromHomeCriteria
            {
                EmpId = model.EmployeeId,
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                CreatedBy = EmployeeSession.EmpId,
                Remark = model.Remark
            };

            result = await baseService.ExecuteNonQueryAsync(criteria);
            if (result.IsSuccessful)
            {
                await attachFileService.UploadFileAsync(files, criteria.RequestId, uploadFolderPath);
            }
        }
        else
        {
            result.Message = ModelState.Values.FirstOrDefault(v => v.Errors.Any())?.Errors.FirstOrDefault()?.ErrorMessage;
        }

        return new JsonResult(result, new JsonSerializerSettings { ContractResolver = new DefaultContractResolver() });
    }

    [HrmPermission(Policy.Read, FunctionKey.EmployeeManagement)]
    public async Task<IActionResult> Edit(int id)
    {
        var criteria = new GetEmployeeWorkFromHomeByIdCriteria { EmployeeWorkFromHomeId = id };
        var employeeWorkFromHome = await baseService.GetDataEntityAsync<EmployeeWorkFromHome>(criteria);

        var model = new EmployeeWorkFromHomeViewModel
        {
            EmployeeWorkFromHomeId = employeeWorkFromHome.EmployeeWorkFromHomeId,
            EmployeeId = employeeWorkFromHome.EmpId,
            Employee = $"{employeeWorkFromHome.Nickname} ({employeeWorkFromHome.FullName})",
            FromDate = employeeWorkFromHome.FromDate,
            ToDate = employeeWorkFromHome.ToDate,
            Remark = employeeWorkFromHome.Remark,
            Files = (await attachFileService.GetInfoAttachFileAsync(id, $"{CurrentCompanyId}/requests")).Select(x => x.FileName).ToList()
        };
        return PartialView("_Edit", model);
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.EmployeeManagement)]
    public async Task<IActionResult> Edit(string requestModel, IList<IFormFile> files, string filesRemove)
    {
        var model = JsonSerializer.Deserialize<EmployeeWorkFromHomeViewModel>(requestModel, JsonOptions);
        var result = new CommandResult();

        if (model != null && ModelState.IsValid)
        {
            var criteria = new UpdateEmployeeWorkFromHomeCriteria
            {
                EmployeeWorkFromHomeId = model.EmployeeWorkFromHomeId,
                EmpId = model.EmployeeId,
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                ModifiedBy = EmployeeSession.EmpId,
                Remark = model.Remark
            };

            result = await baseService.ExecuteNonQueryAsync(criteria);
            if (result.IsSuccessful)
            {
                if (!string.IsNullOrWhiteSpace(filesRemove))
                {
                    await attachFileService.DeleteFileAsync(JsonSerializer.Deserialize<List<string>>(filesRemove), criteria.EmployeeWorkFromHomeId, uploadFolderPath);
                }

                await attachFileService.UploadFileAsync(files, criteria.EmployeeWorkFromHomeId, uploadFolderPath);
            }
        }
        else
        {
            result.Message = ModelState.Values.FirstOrDefault(v => v.Errors.Any())?.Errors.FirstOrDefault()?.ErrorMessage;
        }

        return new JsonResult(result, new JsonSerializerSettings { ContractResolver = new DefaultContractResolver() });
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.EmployeeManagement)]
    public async Task<IActionResult> Delete(int id)
    {
        var criteria = new DeleteEmployeeWorkFromHomeCriteria { EmployeeWorkFromHomeId = id };
        var result = await baseService.ExecuteNonQueryAsync(criteria);

        return new JsonResult(result, new JsonSerializerSettings { ContractResolver = new DefaultContractResolver() });
    }

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        NumberHandling = JsonNumberHandling.AllowReadingFromString,
        Converters = { new DateTimeConverter() }
    };
}
