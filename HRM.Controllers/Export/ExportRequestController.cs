using HRM.Core.Models;
using HRM.Criteria.Request;
using HRM.Models.Request;
using HRM.Resources;
using HRM.Services;
using HRM.ViewModels.Request;
using System.Collections.Generic;
using System;
using GO.UserControl.Client.Core.Session;
using HRM.Controllers;
using HRM.Core;
using Microsoft.AspNetCore.Mvc;
using NPOI.HSSF.UserModel;

namespace HRM.Controllers.Export;

public class ExportRequestController(IUserControlSession userControlSession,
    IRequestService requestService,
    IExportExcelService exportExcelService) : BaseExportController(userControlSession)
{
    public IActionResult VoidRequestWorkingTimeRequest(RequestViewModel request)
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

        var requestList = requestService.GetRequests(criteria);
        var hssfworkbook = exportExcelService.ExportWorkingTimeRequest(requestList, Labels.VoidRequest);
        var fileName = "EmployeeVoidRequest_" + DateTimeOffset.Now.ToString("yyyyMMdd") + ".xls";

        return ExcelFile(hssfworkbook, fileName);
    }
}