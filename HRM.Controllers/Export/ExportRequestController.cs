using Microsoft.AspNetCore.Mvc;
namespace HRM.Controllers
{
    using System;
    using System.Collections.Generic;
    using HRM.Core;
    using HRM.Core.Constants;
    using HRM.Core.Enums;
    using HRM.Core.Filters;
    using HRM.Criteria.Request;
    using HRM.Models.Request;
    using HRM.Resources;
    using HRM.Services;
    using HRM.ViewModels.Request;
    using GO.UserControl.Client.Core.Enumerables;
    using GO.UserControl.Client.Core.Session;
    using NPOI.HSSF.UserModel;

    public class ExportRequestController : BaseExportController
    {
        private readonly IExportExcelService exportExcelService;
        private readonly IRequestService requestService;

        public ExportRequestController(
            IUserControlSession userControlSession,
            IExportExcelService exportExcelService,
            IRequestService requestService) : base(userControlSession)
        {
            this.exportExcelService = exportExcelService;
            this.requestService = requestService;
        }

        [HrmPermission(Policy.Host, FunctionKey.VerifyRequest)]
        [HrmPermission(Policy.Host, FunctionKey.ApproveRequest)]
        public ActionResult MemberWorkingTimeRequest(RequestViewModel model)
        {
            model.ViewType = RequestViewType.PersonalSpace;
            return WorkingTimeRequest(model);
        }

        [HrmPermission(Policy.Host, FunctionKey.ProcessRequest)]
        public ActionResult HRViewWorkingTimeRequest(RequestViewModel model)
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

            IList<RequestEntity> requestList = requestService.GetRequests(criteria);
            HSSFWorkbook hssfworkbook = exportExcelService.ExportWorkingTimeRequest(requestList, model.PageTitle);
            string fileName = "EmployeeRequest_" + DateTimeOffset.Now.ToString("yyyyMMdd") + ".xls";

            return ExcelFile(hssfworkbook, fileName);
        }

        public ActionResult VoidRequestWorkingTimeRequest(RequestViewModel request)
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

            IList<RequestEntity> requestList = requestService.GetRequests(criteria);
            HSSFWorkbook hssfworkbook = exportExcelService.ExportWorkingTimeRequest(requestList, Labels.VoidRequest);
            string fileName = "EmployeeVoidRequest_" + DateTimeOffset.Now.ToString("yyyyMMdd") + ".xls";

            return ExcelFile(hssfworkbook, fileName);
        }

        public ActionResult DailyViewWorkingTimeRequest(RequestViewModel model)
        {
            model.ViewType = RequestViewType.CompanySpace;
            return WorkingTimeRequest(model);
        }

        private ActionResult WorkingTimeRequest(RequestViewModel model)
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

            IList<RequestEntity> requestList = requestService.GetRequests(criteria);
            HSSFWorkbook hssfworkbook = exportExcelService.ExportWorkingTimeRequest(requestList, model.PageTitle);
            string fileName = "DailyRequest_" + DateTimeOffset.Now.ToString("yyyyMMdd") + ".xls";

            return ExcelFile(hssfworkbook, fileName);
        }
    }
}