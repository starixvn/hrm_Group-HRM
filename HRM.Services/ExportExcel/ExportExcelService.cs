using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HRM.Core;
using HRM.Core.Constants;
using HRM.Core.Enums;
using HRM.Models;
using HRM.Models.HospitalizationLeave;
using HRM.Models.Request;
using HRM.Services.Implement.Request;
using Microsoft.AspNetCore.Hosting;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;

namespace HRM.Services.ExportExcel;

public class ExportExcelService(IWebHostEnvironment hostingEnvironment) : IExportExcelService
{
    private const string DefaultSheetName = "sheet1";
    private const int RowStartBodyLeaveSummary = 4;
    private const int RowStartBodyWorkingTimeRequest = 3;

    private const string LeaveSummaryFilePathExcelTemplate = "ExcelTemplates/LeaveSummaryTemplate.xls";
    private const string LeaveSummaryForJoinedDateFilePathExcelTemplate = "ExcelTemplates/LeaveSummaryForJoinedDateTemplate.xls";
    private const string EmployeeWFHTemplate = "ExcelTemplates/EmployeeWorkFromHome.xls";
    private const string WorkingTimeRequestFilePathExcelTemplate = "ExcelTemplates/WorkingTimeRequestTemplate.xls";
    private const string HospitalizationLeaveSummaryTemplatePath = "ExcelTemplates/HospitalizationLeaveSummary.xls";
    private const string SickLeaveSummaryTemplatePath = "ExcelTemplates/SickLeaveSummary.xls";

    public HSSFWorkbook ExportEmployeeWfh(IEnumerable<EmployeeWorkFromHome> employeeWfhList)
    {
        HSSFWorkbook workbook;

        var templatePath = Path.Combine(hostingEnvironment.ContentRootPath, EmployeeWFHTemplate);
        using (var template = new FileStream(templatePath, FileMode.Open, FileAccess.Read))
        {
            workbook = new HSSFWorkbook(template);
        }

        var templateSheet = workbook.GetSheet(DefaultSheetName);
        var sheet = workbook.CloneSheet(DefaultSheetName, "EmployeeWorkFromHome");

        //// render body

        var rowIndex = 4;
        var index = 0;

        foreach (var item in employeeWfhList)
        {
            var rowCopy = 4 + index % 2;
            sheet.CopyRow(templateSheet, rowCopy, rowIndex);
            var row = sheet.GetRow(rowIndex);

            row.GetCell(0).SetCellValue(index + 1);
            row.GetCell(1).SetCellValue(item.DepartmentName);
            row.GetCell(2).SetCellValue(item.TeamName);
            row.GetCell(3).SetCellValue(item.Code);
            row.GetCell(4).SetCellValue(item.FullName);
            row.GetCell(5).SetCellValue(item.Nickname);
            row.GetCell(6).SetCellValue(item.FromDate.ToDateMonthYearFormatedDate());
            row.GetCell(7).SetCellValue(item.ToDate.ToDateMonthYearFormatedDate());
            row.GetCell(8).SetCellValue(item.Remark);
            row.GetCell(9).SetCellValue(item.Modifier);
            row.GetCell(10).SetCellValue(item.ModifiedDate.ToFormatedDate());

            index++;
            rowIndex++;
        }

        workbook.RemoveSheetAt(0);

        return workbook;
    }

    public HSSFWorkbook ExportLeaveSummary(IEnumerable<LeaveSummary> leaveSummaryList, int year)
    {
        var workbook = LoadWorkbookFromTemplate(LeaveSummaryFilePathExcelTemplate);
        var (templateSheet, sheet) = PrepareWorksheetForExport(workbook, "LeaveSummary");

        //// render header
        var rowTakeLeave = sheet.GetRow(2);
        var cellTakeLeave = rowTakeLeave.GetCell(7);
        cellTakeLeave.SetCellValue("Leave in " + year);

        var rowAnnualBalance = sheet.GetRow(2);
        var cellAnnualBalance = rowAnnualBalance.GetCell(19);
        cellAnnualBalance.SetCellValue("Balance Of AL " + year);

        var cellKIVBalance = rowAnnualBalance.GetCell(20);
        cellKIVBalance.SetCellValue("Balance Of CL " + year);

        //// render body
        RenderLeaveSummaryRows(templateSheet, sheet, leaveSummaryList, (row, item, index) =>
        {
            row.GetCell(0).SetCellValue(index + 1);
            row.GetCell(1).SetCellValue(item.Department);
            row.GetCell(2).SetCellValue(item.Team);
            row.GetCell(3).SetCellValue(item.FullName);
            row.GetCell(4).SetCellValue(item.Nickname);
            row.GetCell(5).SetCellValue(item.CreditKIV);
            row.GetCell(6).SetCellValue(item.AnnualLeaveCurrentYear);
            row.GetCell(7).SetCellValue(item.Jan);
            row.GetCell(8).SetCellValue(item.Feb);
            row.GetCell(9).SetCellValue(item.Mar);
            row.GetCell(10).SetCellValue(item.Apr);
            row.GetCell(11).SetCellValue(item.May);
            row.GetCell(12).SetCellValue(item.Jun);
            row.GetCell(13).SetCellValue(item.Jul);
            row.GetCell(14).SetCellValue(item.Aug);
            row.GetCell(15).SetCellValue(item.Sep);
            row.GetCell(16).SetCellValue(item.Oct);
            row.GetCell(17).SetCellValue(item.Nov);
            row.GetCell(18).SetCellValue(item.Dec);
            row.GetCell(19).SetCellValue(item.LeaveBalance);
            row.GetCell(20).SetCellValue(item.KIVBalance);
        });

        FinalizeWorkbook(workbook);
        return workbook;
    }

    public HSSFWorkbook ExportLeaveSummaryForJoinedDate(IEnumerable<LeaveSummaryForJoinedDate> leaveSummaryList)
    {
        var workbook = LoadWorkbookFromTemplate(LeaveSummaryForJoinedDateFilePathExcelTemplate);
        var (templateSheet, sheet) = PrepareWorksheetForExport(workbook, "LeaveSummary");

        // render body
        RenderLeaveSummaryRows(templateSheet, sheet, leaveSummaryList, (row, item, index) =>
        {
            row.GetCell(0).SetCellValue(index + 1); // #
            row.GetCell(1).SetCellValue(item.Department); // Department
            row.GetCell(2).SetCellValue(item.Team); // Team
            row.GetCell(3).SetCellValue(item.FullName); // FullName
            row.GetCell(4).SetCellValue(item.Nickname); // Nickname
            row.GetCell(5).SetCellValue(item.FromDate == DateTimeOffset.MinValue.DateTime ? string.Empty : item.FromDate.ToString("dd MMM yyyy")); // Joined Date
            row.GetCell(6).SetCellValue(item.AnnualLeaveCurrentYear); // Annual Leave
            row.GetCell(7).SetCellValue(item.ExtraLeave); // Extra Leave
            row.GetCell(8).SetCellValue(item.DeductDays); // Taken Leave
            row.GetCell(9).SetCellValue(item.LeaveBalance); // Balance
        });

        FinalizeWorkbook(workbook);
        return workbook;
    }

    public HSSFWorkbook ExportWorkingTimeRequest(IList<RequestEntity> requestList, string title)
    {
        HSSFWorkbook workbook;
        var templatePath = Path.Combine(hostingEnvironment.ContentRootPath, WorkingTimeRequestFilePathExcelTemplate);
        using (var template = new FileStream(templatePath, FileMode.Open, FileAccess.Read))
        {
            workbook = new HSSFWorkbook(template);
        }

        var templateSheet = workbook.GetSheet(DefaultSheetName);
        var sheet = workbook.CloneSheet(DefaultSheetName, title);
        var rowIndex = RowStartBodyWorkingTimeRequest;
        var height = sheet.GetRow(RowStartBodyWorkingTimeRequest).Height;

        var rowTitle = sheet.GetRow(0);
        var cellTitle = rowTitle.GetCell(0);
        cellTitle.SetCellValue(title);
        foreach (var item in requestList)
        {
            var templateRowIndex = GetTemplateRowIndex(RowStartBodyWorkingTimeRequest, item.Requester.Enabled, item.Requester.IsDeleted);
            sheet.CopyRow(templateSheet, templateRowIndex, rowIndex);
            var row = sheet.GetRow(rowIndex);
            row.Height = height;
            RenderBodyWorkingTimeRequest(rowIndex, item, row);
            rowIndex++;
        }

        workbook.RemoveSheetAt(0);

        return workbook;
    }

    public HSSFWorkbook ExportHospitalizationLeaveSummary(IEnumerable<HospitalizationLeaveBalanceSummaryReport> employees, int year)
    {
        return ExportLeaveBalanceSummary(employees, year, HospitalizationLeaveSummaryTemplatePath);
    }

    public HSSFWorkbook ExportSickLeaveSummary(IEnumerable<HospitalizationLeaveBalanceSummaryReport> employees, int year)
    {
        return ExportLeaveBalanceSummary(employees, year, SickLeaveSummaryTemplatePath);
    }

    private static void RenderBodyWorkingTimeRequest(
        int rowIndex,
        RequestEntity item,
        IRow row)
    {
        var companyId = item.CompanyId;
        var isInNexCompany = Constants.NexCompanyIds.Contains(companyId);

        var createdDate = item.CreatedDate;
        if (isInNexCompany)
        {
            createdDate = item.OriginalCreatedDate ?? item.CreatedDate;
        }

        var cellNo = row.GetCell(0);
        var cellUnitCode = row.GetCell(1);
        var cellDepartment = row.GetCell(2);
        var cellTeam = row.GetCell(3);
        var cellName = row.GetCell(4);
        var cellNick = row.GetCell(5);
        var cellRequest = row.GetCell(6);
        var cellDetails = row.GetCell(7);
        var cellReason = row.GetCell(8);
        var cellRemarks = row.GetCell(9);
        var cellCreateDate = row.GetCell(10);
        var cellStatus = row.GetCell(11);
        cellNo.SetCellValue(rowIndex - RowStartBodyLeaveSummary + 2);
        cellUnitCode.SetCellValue(item.Requester.UnitCode);
        cellDepartment.SetCellValue(item.Requester.Team.Department.Name);
        cellTeam.SetCellValue(item.Requester.Team.Name);
        cellNick.SetCellValue(item.Requester.NickName);
        cellName.SetCellValue(item.Requester.FullName);
        cellRequest.SetCellValue(item.RequestType.Id == (byte)RequestType.Leave ? item.LeaveType.Name : item.RequestType.Name);
        cellDetails.SetCellValue(RequestService.ToRequestMessage(item, isNoFormat: true));
        cellReason.SetCellValue(RequestService.GetReasonRequest(item.RequestType.Id, item.ReasonId));
        cellRemarks.SetCellValue(item.Remark);
        cellCreateDate.SetCellValue(createdDate);
        cellStatus.SetCellValue(item.Status.Name);
    }

    public static void WriteRowExcelReport(ExcelTemplateWriter writer, int rowIndex, int rowStartBody, object record)
    {
        var rowStart = rowIndex % 2 != 0 ? rowStartBody : rowStartBody + 1;
        writer.WriteRow(rowStart, record);
    }

    public byte GetTemplateRowIndex(int startRow, bool enabled, bool isDeleted)
    {
        int normalRowIndex = startRow;
        int disabledRowIndex = startRow + 1;
        int deletedRowIndex = startRow + 2;

        if (isDeleted)
        {
            return (byte)deletedRowIndex;
        }

        return enabled ? (byte)normalRowIndex : (byte)disabledRowIndex;
    }

    private HSSFWorkbook LoadWorkbookFromTemplate(string templateFileName)
    {
        var templatePath = Path.Combine(hostingEnvironment.ContentRootPath, templateFileName);
        using var template = new FileStream(templatePath, FileMode.Open, FileAccess.Read);
        return new HSSFWorkbook(template);
    }

    private (ISheet templateSheet, ISheet sheet) PrepareWorksheetForExport(HSSFWorkbook workbook, string sheetName)
    {
        var templateSheet = workbook.GetSheet(DefaultSheetName);
        var sheet = workbook.CloneSheet(DefaultSheetName, sheetName);
        return (templateSheet, sheet);
    }

    private void RenderLeaveSummaryRows<T>(
        ISheet templateSheet, 
        ISheet sheet, 
        IEnumerable<T> items, 
        Action<IRow, T, int> setCellValues) where T : LeaveSummary
    {
        var rowIndex = RowStartBodyLeaveSummary;
        var height = sheet.GetRow(RowStartBodyLeaveSummary).Height;
        var index = 0;

        foreach (var item in items)
        {
            var templateRowIndex = GetTemplateRowIndex(RowStartBodyLeaveSummary, item.Enabled, item.IsDeleted);
            sheet.CopyRow(templateSheet, templateRowIndex, rowIndex);
            var row = sheet.GetRow(rowIndex);
            row.Height = height;

            setCellValues(row, item, index);

            index++;
            rowIndex++;
        }
    }

    private HSSFWorkbook ExportLeaveBalanceSummary(
        IEnumerable<HospitalizationLeaveBalanceSummaryReport> employees, 
        int year, 
        string templatePath)
    {
        var workbook = LoadWorkbookFromTemplate(templatePath);
        var (templateSheet, sheet) = PrepareWorksheetForExport(workbook, "Summary");

        // render header
        var rowTakeLeave = sheet.GetRow(2);
        var cellTakeLeave = rowTakeLeave.GetCell(7);
        cellTakeLeave.SetCellValue("Leave in " + year);

        var rowAnnualBalance = sheet.GetRow(2);
        var cellAnnualBalance = rowAnnualBalance.GetCell(18);
        cellAnnualBalance.SetCellValue("Balance Of " + year);

        // render body
        var rowIndex = 4;
        var height = sheet.GetRow(4).Height;
        var index = 0;

        foreach (var employee in employees)
        {
            var rowCopy = 4 + index % 2;
            sheet.CopyRow(templateSheet, rowCopy, rowIndex);
            var row = sheet.GetRow(rowIndex);
            row.Height = height;

            row.GetCell(0).SetCellValue(index + 1);
            row.GetCell(1).SetCellValue(employee.Department);
            row.GetCell(2).SetCellValue(employee.Team);
            row.GetCell(3).SetCellValue(employee.FullName);
            row.GetCell(4).SetCellValue(employee.Nickname);
            row.GetCell(5).SetCellValue(employee.Balance);
            row.GetCell(6).SetCellValue(employee.Jan);
            row.GetCell(7).SetCellValue(employee.Feb);
            row.GetCell(8).SetCellValue(employee.Mar);
            row.GetCell(9).SetCellValue(employee.Apr);
            row.GetCell(10).SetCellValue(employee.May);
            row.GetCell(11).SetCellValue(employee.Jun);
            row.GetCell(12).SetCellValue(employee.Jul);
            row.GetCell(13).SetCellValue(employee.Aug);
            row.GetCell(14).SetCellValue(employee.Sep);
            row.GetCell(15).SetCellValue(employee.Oct);
            row.GetCell(16).SetCellValue(employee.Nov);
            row.GetCell(17).SetCellValue(employee.Dec);
            row.GetCell(18).SetCellValue(employee.RemainingBalance);

            index++;
            rowIndex++;
        }

        FinalizeWorkbook(workbook);
        return workbook;
    }

    private static void FinalizeWorkbook(HSSFWorkbook workbook)
    {
        workbook.RemoveSheetAt(0);
    }
}