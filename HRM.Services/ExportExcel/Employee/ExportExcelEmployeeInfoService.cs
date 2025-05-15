namespace HRM.Services.ExportExcel.Employee;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Core;
using Core.Enums;
using Implement;
using Interfaces;
using Microsoft.AspNetCore.Hosting;
using Models;
using Models.EmployeeInformation;
using Models.Group;
using Models.Supervisor;
using OfficeOpenXml;
using Resources;

public class ExportExcelEmployeeInfoService(
    IEmployeeService employeeService,
    ICommonDataService commonDataService,
    IWebHostEnvironment hostingEnvironment) : IExportExcelEmployeeInfoService
{
    private const string EmployeeBasicInfoTemplatePath = "ExcelTemplates/EmployeeBasicInfoTemplate.xls";
    private const string EmployeeFullInfoTemplatePath = "ExcelTemplates/EmployeeFullProfileInfoTemplate.xls";
    private const string GroupEmployeeListTemplatePath = "ExcelTemplates/GroupEmployeeList.xlsx";

    private const byte TotalColumns = 23;
    private const byte RowStartBody = 3;
    private const byte MinimumWidth = 5;
    private const byte MaximumWidth = 50;

    private enum BasicInfo
    {
        RowStartHeader = 2,
        RowStartBody = 4,
        TotalColumns = 22
    }

    public async Task<byte[]> ExportEmployeeBasicInfo(IList<Employee> employees)
    {
        var templatePath = Path.Combine(hostingEnvironment.ContentRootPath, EmployeeBasicInfoTemplatePath);
        const string sheetName = "Employee Basic Info";
        var writer = new ExcelTemplateWriter(templatePath, sheetName);
        var enableGroupDate = await commonDataService.IsEnableGroupDateAsync();
        const byte colGroupDate = 8;
        const byte colLengthOfServiceGroupDate = 12;

        writer.CopyRows(0, 2);
        writer.CopyRows((byte)BasicInfo.RowStartHeader, 2);

        FillContentRowEmployeeBasicInfoReport(employees, ref writer);
        writer.SyncColumnsWidth((byte)BasicInfo.TotalColumns);
        writer.CreateFreezePane(0, (byte)BasicInfo.RowStartBody);
        if (!enableGroupDate)
        {
            writer.RemoveColumn(colGroupDate);
            writer.RemoveColumn(colLengthOfServiceGroupDate);
        }
        return writer.WriteContentToBytes();
    }

    #region Employee Basic Info

    private void FillContentRowEmployeeBasicInfoReport(IList<Employee> employees, ref ExcelTemplateWriter writer)
    {
        var rowIndex = 1;

        foreach (var employee in employees)
        {
            FillRowEmployeeBasicInfoReport(employee, ref writer, ref rowIndex);
        }
    }

    private void FillRowEmployeeBasicInfoReport(Employee employee, ref ExcelTemplateWriter writer, ref int rowIndex)
    {
        var record = CreateRecordEmployeeBasicInfoReport(employee, rowIndex);

        var writeRowNumber = rowIndex % 2 == 0 ? 5 : 4;

        writer.WriteRow(writeRowNumber, record);

        rowIndex++;
    }

    private object CreateRecordEmployeeBasicInfoReport(Employee employee, int rowIndex)
    {
        var record = new
        {
            Count = rowIndex,
            employee.UnitCode,
            Department = employee.Team.Department.Name,
            Team = employee.Team.Name,
            employee.Code,
            employee.FullName,
            employee.NickName,
            GroupDate = employee.JoinedDate,
            employee.CompanyDate,
            employee.ConfirmedDate,
            employee.ResignedDate,
            employee.ResignedReason,
            LengthOfServiceByGroupDate = employeeService.CalculateLengthOfService(employee.JoinedDate, employee.MilitaryFromDate, employee.MilitaryToDate),
            LengthOfServiceByCompanyDate = employeeService.CalculateLengthOfService(employee.CompanyDate, employee.MilitaryFromDate, employee.MilitaryToDate),
            DoB = employee.DateOfBirth.ToDateMonthYearFormatedDate(),
            Gender = employee.Gender == 0 ? "F" : "M",
            CellPhone = employee.Phone,
            employee.Email,
            employee.Address,
            employee.CurrentAddress,
            Category = employee.IsLocal ? Labels.Local : Labels.Expat
        };

        return record;
    }

    #endregion Employee Basic Info

    #region Employee Full Profile Information

    public async Task<byte[]> ExportEmployeeFullProfile(IEnumerable<EmployeeFullProfile> profiles)
    {
        var templatePath = Path.Combine(hostingEnvironment.ContentRootPath, EmployeeFullInfoTemplatePath);
        const string sheetName = "Employee Full Information";
        var writer = new ExcelTemplateWriter(templatePath, sheetName);
        var enableGroupDate = await commonDataService.IsEnableGroupDateAsync();
        const byte colGroupDate = 10;
        const byte colLengthOfServiceGroupDate = 14;

        writer.CopyRows(0, 2);
        writer.CopyRows(2, 2);

        WriteFullProfileBody(writer, profiles);
        writer.SyncColumnsWidth(57);
        writer.CreateFreezePane(0, 4);
        if (!enableGroupDate)
        {
            writer.RemoveColumn(colGroupDate);
            writer.RemoveColumn(colLengthOfServiceGroupDate);
        }
        return writer.WriteContentToBytes();
    }

    private void WriteFullProfileBody(ExcelTemplateWriter writer, IEnumerable<EmployeeFullProfile> profiles)
    {
        var currentRow = 3;
        foreach (var p in profiles)
        {
            var writeRowNumber = currentRow % 2 == 0 ? 5 : 4;

            var record = CreateRecordEmployeeFullProfile(p, currentRow);
            writer.WriteRow(writeRowNumber, record);
            currentRow++;
        }
    }

    private object CreateRecordEmployeeFullProfile(EmployeeFullProfile p, int currentRow)
    {
        var gender = "Unknown";
        if (p.Gender.HasValue)
        {
            gender = p.Gender == 0 ? "Female" : "Male";
        }

        var record = new
        {
            Index = currentRow - 2,
            p.UnitCode,
            Department = p.DepartmentName,
            p.TeamName,
            p.Code,
            p.EmployeeCard,
            p.FullName,
            p.Nickname,
            Gender = gender,
            GroupDate = FormatDate(p.JoinedDate),
            CompanyDate = FormatDate(p.CompanyDate),
            ConfirmedDate = FormatDate(p.ConfirmedDate),
            ResignedDate = FormatDate(p.ResignedDate),
            p.ResignedReason,
            LengthOfServiceByGroupDate = employeeService.CalculateLengthOfService(p.JoinedDate, p.MilitaryFromDate, p.MilitaryToDate),
            LengthOfServiceByCompanyDate = employeeService.CalculateLengthOfService(p.CompanyDate, p.MilitaryFromDate, p.MilitaryToDate),
            p.JobTitle,
            p.JobLevel,
            p.ManagementLevel,
            TeamDate = FormatDate(p.TeamDate),
            DOB = FormatDate(p.DateOfBirth),
            p.PlaceOfBirth,
            p.Ethnic,
            p.Native,
            p.Religion,
            p.MarriageName,
            p.Nationality,
            p.CardNo,
            DateOfIDCard = FormatDate(p.DateOfIdCard),
            p.PlaceOfIdCard,
            p.PassportNo,
            PassportExp = FormatDate(p.PassportExp),
            p.Qualification,
            p.Major,
            p.Address,
            p.CurrentAddress,
            p.Phone,
            p.HomePhone,
            p.Email,
            p.PickupRemark,
            p.Skype,
            p.LetsTalk,
            CanOT = p.CanOT ? Labels.Yes : Labels.No,
            p.SpecialOT,
            p.WorkingDays,
            p.WorkingHours,
            p.OffDaysbyWeek,
            p.AnnualLeaveBalance,
            p.KeepInViewBalance,
            Category = p.IsLocal ? Labels.Local : Labels.Expat,
            p.Roles2String,
            SpecialUser = p.IsTopManager ? Labels.Yes : Labels.No,
            VerifiedBy = GetSupervisors(p.Supervisors, (short)RoleSystem.Verifier),
            ApprovedBy = GetSupervisors(p.Supervisors, (short)RoleSystem.Approver),
            DisciplineManager = GetSupervisors(p.Supervisors, (short)RoleSystem.DisciplineManager),
            ShiftArrangement = GetSupervisors(p.Supervisors, (short)RoleSystem.ShiftManager),
            OnBehalfRequest = GetSupervisors(p.Supervisors, (short)RoleSystem.OnBehalfRequest)
        };

        return record;
    }

    private static string GetSupervisors(IList<SupervisorNameModel> supervisors, short roleId)
    {
        var nicknames = string.Empty;
        if (supervisors != null && supervisors.Any())
        {
            nicknames = string.Join(", ", supervisors.Where(x => x.ActionType == roleId).Select(x => x.NickName));
        }

        return nicknames;
    }

    private static string FormatDate(DateTime? date, string format = "dd/MM/yyyy") => date?.ToString(format);

    #endregion Employee Full Profile Information

    public async Task<byte[]> ExportGroupEmployeesAsync(IEnumerable<GroupEmployeeModel> employees)
    {
        var templatePath = Path.Combine(hostingEnvironment.ContentRootPath, GroupEmployeeListTemplatePath);

        await using var templateStream = new FileStream(templatePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, useAsync: true);
        using var templatePackage = new ExcelPackage(templateStream);
        using var xlPackage = new ExcelPackage();

        var template = templatePackage.Workbook.Worksheets.FirstOrDefault();
        if (template == null)
        {
            throw new InvalidOperationException("The template Excel file does not contain any worksheet.");
        }

        var worksheet = xlPackage.Workbook.Worksheets.Add("Group Employee List");
        var rowIndex = 1;
        var colIndex = 1;

        // title
        template.Cells[rowIndex, colIndex, rowIndex, TotalColumns].Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, TotalColumns]);
        rowIndex++;

        // header
        template.Cells[rowIndex, colIndex, rowIndex, TotalColumns].Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, TotalColumns]);
        rowIndex++;

        // data
        var rowNum = 1;
        foreach (var employee in employees)
        {
            colIndex = 1;
            template.Cells[RowStartBody, 1, RowStartBody, TotalColumns].Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, TotalColumns]);

            worksheet.Cells[rowIndex, colIndex++].Value = rowNum++;
            worksheet.Cells[rowIndex, colIndex++].Value = employee.CtrId;
            worksheet.Cells[rowIndex, colIndex++].Value = employee.EmployeeCode;
            worksheet.Cells[rowIndex, colIndex++].Value = employee.FullName;
            worksheet.Cells[rowIndex, colIndex++].Value = employee.NickName;
            worksheet.Cells[rowIndex, colIndex++].Value = employee.UnitId;
            worksheet.Cells[rowIndex, colIndex++].Value = employee.UnitCode;
            worksheet.Cells[rowIndex, colIndex++].Value = employee.UnitPIC;
            worksheet.Cells[rowIndex, colIndex++].Value = employee.DepartmentName;
            worksheet.Cells[rowIndex, colIndex++].Value = employee.DepartmentPIC;
            worksheet.Cells[rowIndex, colIndex++].Value = employee.TeamName;
            worksheet.Cells[rowIndex, colIndex++].Value = employee.TeamPIC;
            worksheet.Cells[rowIndex, colIndex++].Value = employee.JobLevel;
            worksheet.Cells[rowIndex, colIndex++].Value = employee.ManagementLevel;
            worksheet.Cells[rowIndex, colIndex++].Value = employee.JobTitle;
            worksheet.Cells[rowIndex, colIndex++].Value = employee.JoinedDate.HasValue ? employee.JoinedDate.ToDateMonthYearFormatedDate() : null;
            worksheet.Cells[rowIndex, colIndex++].Value = employee.Company;
            worksheet.Cells[rowIndex, colIndex++].Value = employee.CompanyDate.HasValue ? employee.CompanyDate.ToDateMonthYearFormatedDate() : null;
            worksheet.Cells[rowIndex, colIndex++].Value = employee.ResignedDate.HasValue ? employee.ResignedDate.ToDateMonthYearFormatedDate() : null;
            worksheet.Cells[rowIndex, colIndex++].Value = employee.IsLocal;
            worksheet.Cells[rowIndex, colIndex++].Value = employee.IsSpecial;
            worksheet.Cells[rowIndex, colIndex++].Value = employee.Enabled;
            worksheet.Cells[rowIndex, colIndex].Value = employee.IsDeleted;

            rowIndex++;
        }

        worksheet.Cells.AutoFitColumns(MinimumWidth, MaximumWidth);
        worksheet.View.FreezePanes(RowStartBody, 1);

        await using var ms = new MemoryStream();
        await xlPackage.SaveAsAsync(ms);
        return ms.ToArray();
    }
}