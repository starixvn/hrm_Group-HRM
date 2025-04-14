using System;
using System.Threading.Tasks;
using GO.UserControl.Client.Core.Enumerables;
using GO.UserControl.Client.Core.Session;
using HRM.Core;
using HRM.Core.Constants;
using HRM.Core.Enums;
using HRM.Core.Filters;
using HRM.Criteria.Request;
using HRM.Resources;
using HRM.Services.ExportExcel;
using HRM.Services.Interfaces.Request;
using HRM.ViewModels.Request;
using Microsoft.AspNetCore.Mvc;

namespace HRM.Controllers.Export;

public class ExportRequestController(
    IUserControlSession session,
    IRequestService requestService,
    IExportExcelService exportExcelService) : BaseExportController(session)
{
    public async Task<IActionResult> VoidRequestWorkingTimeRequest(RequestViewModel request)
    {
        request.ToDate = request.ToDate.ToEndOfDay();
        int? employeeId = null;
        if (request.EmployeeId != -1 && !string.IsNullOrWhiteSpace(request.Employee))
        {
            employeeId = request.EmployeeId;
        }

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
            SupervisorId = session.UserId,
            IsLocal = request.IsLocal.HasValue ? request.IsLocal.Value == 1 : null,
            CompanyId = CurrentCompanyId
        };

        var requests = await requestService.GetRequestAsync(criteria);
        var hssfWorkbook = exportExcelService.ExportWorkingTimeRequest(requests, Labels.VoidRequest);
        var fileName = $"EmployeeVoidRequest_{DateTimeOffset.Now.DateTime.ToClientTimeZone():yyyyMMdd}.xls";

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
            EmpId = model.EmployeeId != -1 && !string.IsNullOrWhiteSpace(model.Employee) ? model.EmployeeId : null,
            FromDate = model.FromDate,
            ToDate = model.ToDate,
            TeamIds = model.TeamIds,
            Status = model.StatusIds,
            RequestType = model.RequestType,
            IsQueryByCreatedDate = !model.IsFilterByAffectedDate,
            RequestViewType = model.ViewType,
            SupervisorId = session.UserId,
            IsLocal = model.IsLocal.HasValue ? model.IsLocal.Value == 1 : null,
            LeaveTypeId = model.LeaveType,
            CompanyId = CurrentCompanyId
        };

        var requests = await requestService.GetRequestAsync(criteria);
        var hssfWorkbook = exportExcelService.ExportWorkingTimeRequest(requests, model.PageTitle);
        var fileName = $"EmployeeRequest_{DateTimeOffset.Now.DateTime.ToClientTimeZone():yyyyMMdd}.xls";

        return ExcelFile(hssfWorkbook, fileName);
    }

    public async Task<IActionResult> DailyViewWorkingTimeRequest(RequestViewModel model)
    {
        model.ViewType = RequestViewType.CompanySpace;
        return await WorkingTimeRequest(model);
    }

    private async Task<IActionResult> WorkingTimeRequest(RequestViewModel model)
    {
        model.ToDate = model.ToDate.ToEndOfDay();
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
            SupervisorId = session.UserId,
            IsLocal = model.IsLocal.HasValue ? model.IsLocal.Value == 1 : null,
            CompanyId = CurrentCompanyId
        };

        var requests = await requestService.GetRequestAsync(criteria);
        var hssfWorkbook = exportExcelService.ExportWorkingTimeRequest(requests, model.PageTitle);
        var fileName = $"DailyRequest_{DateTimeOffset.Now.DateTime.ToClientTimeZone():yyyyMMdd}.xls";

        return ExcelFile(hssfWorkbook, fileName);
    }
}