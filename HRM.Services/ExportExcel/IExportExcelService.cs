using System.Collections.Generic;
using HRM.Models;
using HRM.Models.HospitalizationLeave;
using HRM.Models.Request;
using NPOI.HSSF.UserModel;

namespace HRM.Services.ExportExcel;

public interface IExportExcelService
{
    HSSFWorkbook ExportLeaveSummary(IEnumerable<LeaveSummary> leaveSummaryList, int year);
    HSSFWorkbook ExportEmployeeWfh(IEnumerable<EmployeeWorkFromHome> employeeWfhList);

    HSSFWorkbook ExportWorkingTimeRequest(IList<RequestEntity> requestList, string title);

    HSSFWorkbook ExportHospitalizationLeaveSummary(IEnumerable<HospitalizationLeaveBalanceSummaryReport> employees, int year);

    HSSFWorkbook ExportSickLeaveSummary(IEnumerable<HospitalizationLeaveBalanceSummaryReport> employees, int year);
}