using HRM.Criteria.Request;
using HRM.Resources;
using HRM.ViewModels.Request;
using System;
using System.Threading.Tasks;
using GO.UserControl.Client.Core.Session;
using HRM.Core;
using HRM.Services.Interfaces.Request;
using Microsoft.AspNetCore.Mvc;
using HRM.Core.Constants;
using HRM.Core.Enums;
using HRM.Core.Filters;
using GO.UserControl.Client.Core.Enumerables;
using HRM.Services.ExportExcel;
using NPOI.HSSF.UserModel;

namespace HRM.Controllers.Export;

public class ExportRequestController(IUserControlSession userControlSession,
    IRequestService requestService,
    IExportExcelService exportExcelService) : BaseExportController(userControlSession)
{
    public async Task<IActionResult> VoidRequestWorkingTimeRequest(RequestViewModel request)
    {
        request.ToDate = request.ToDate.ToEndOfDay();
        int? employeeId = null;
        if (request.EmployeeId != -1 && !string.IsNullOrWhiteSpace(request.Employee))
        {
            employeeId = request.EmployeeId;
        }

        bool? isLocal = null;
        var criteria = new GetRequestCriteria
        {
            EmpId = employeeId,
            FromDate = request.FromDate,
            ToDate = request.ToDate,
            TeamIds = request.TeamIds,
            Status = request.StatusIds,
            RequestType = request.RequestType,
            IsQueryByCreatedDate = !request.IsFilterByAffectedDate,
            RequestViewType = request.ViewType,
            SupervisorId = EmployeeSession.EmpId,
            IsLocal = request.IsLocal.HasValue ? (request.IsLocal.Value == 1) : isLocal
        };

        var requestList = await requestService.GetRequests(criteria);
        var hssfWorkbook = exportExcelService.ExportWorkingTimeRequest(requestList, Labels.VoidRequest);
        var fileName = "EmployeeVoidRequest_" + DateTimeOffset.Now.ToString("yyyyMMdd") + ".xls";

        return ExcelFile(hssfWorkbook, fileName);
    }

    [HrmPermission(Policy.Host, FunctionKey.VerifyRequest)]
    [HrmPermission(Policy.Host, FunctionKey.ApproveRequest)]
    public async Task<IActionResult> MemberWorkingTimeRequest(RequestViewModel model)
    {
        model.ViewType = RequestViewType.PersonalSpace;
        return await WorkingTimeRequest(model);
    }

    [HrmPermission(Policy.Host, FunctionKey.ProcessRequest)]
    public async Task<IActionResult> HRViewWorkingTimeRequest(RequestViewModel model)
    {
        model.ToDate = model.ToDate.ToEndOfDay();
        model.ViewType = RequestViewType.HRSpace;

        var criteria = new GetRequestCriteria
        {
            SupervisorId = EmployeeSession.EmpId,
            EmpId = model.EmployeeId == -1 ? null : new int?(model.EmployeeId),
            FromDate = model.FromDate,
            ToDate = model.ToDate,
            TeamIds = model.TeamIds,
            Status = model.StatusIds,
            RequestType = model.RequestType,
            IsQueryByCreatedDate = !model.IsFilterByAffectedDate,
            RequestViewType = model.ViewType,
            IsLocal = model.IsLocal.HasValue ? (model.IsLocal.Value == 1) : (bool?)null
        };

        var requestList = await requestService.GetRequests(criteria);
        var hssfworkbook = exportExcelService.ExportWorkingTimeRequest(requestList, model.PageTitle);
        var fileName = "EmployeeRequest_" + DateTimeOffset.Now.ToString("yyyyMMdd") + ".xls";

        return ExcelFile(hssfworkbook, fileName);
    }


    public async Task<IActionResult> DailyViewWorkingTimeRequest(RequestViewModel model)
    {
        model.ViewType = RequestViewType.CompanySpace;
        return await WorkingTimeRequest(model);
    }

    private async Task<IActionResult> WorkingTimeRequest(RequestViewModel model)
    {
        model.ToDate = model.ToDate.ToEndOfDay();
        model.ViewType = RequestViewType.CompanySpace;
        var criteria = new GetRequestCriteria
        {
            EmpId = model.EmployeeId == -1 ? null : new int?(model.EmployeeId),
            FromDate = model.FromDate,
            ToDate = model.ToDate,
            TeamIds = model.TeamIds,
            Status = model.StatusIds,
            RequestType = model.RequestType,
            IsQueryByCreatedDate = !model.IsFilterByAffectedDate,
            RequestViewType = model.ViewType,
            SupervisorId = EmployeeSession.EmpId,
            IsLocal = model.IsLocal.HasValue ? (model.IsLocal.Value == 1) : (bool?)null
        };

        var requestList = await requestService.GetRequests(criteria);
        var hssfWorkbook = exportExcelService.ExportWorkingTimeRequest(requestList, model.PageTitle);
        var fileName = "DailyRequest_" + DateTimeOffset.Now.ToString("yyyyMMdd") + ".xls";

        return ExcelFile(hssfWorkbook, fileName);
    }
}