namespace HRM.Services.ExportExcel.Employee;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Core;
using Core.Constants;
using Core.Enums;
using GO.UserControl.Client.Core.Session;
using Core.Helpers;
using Implement;
using Interfaces;
using Microsoft.AspNetCore.Hosting;
using Models;
using Models.EmployeeInformation;
using Models.Group;
using Models.Supervisor;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Resources;
using HRM.Models.Employees;

public class ExportExcelEmployeeInfoService(
    IUserControlSession session,
    IEmployeeService employeeService,
    ICommonDataService commonDataService,
    IWebHostEnvironment hostingEnvironment,
    ICompanyService companyService) : IExportExcelEmployeeInfoService
{
    private const string EmployeeBasicInfoTemplatePath = "ExcelTemplates/EmployeeBasicInfoTemplate.xls";
    private const string EmployeeFullInfoTemplatePath = "ExcelTemplates/EmployeeFullProfileInfoTemplate.xls";
    private const string GroupEmployeeListTemplatePath = "ExcelTemplates/GroupEmployeeList.xlsx";
    private const string ProfitCenterReportTemplatePath = "ExcelTemplates/ProfitCenterReport.xlsx";
    private const string GroupEmployeeTransferListTemplatePath = "ExcelTemplates/GroupEmployeeTransferList.xlsx";

    private const byte RowStartBody = 3;
    private const byte MinimumWidth = 5;
    private const byte MaximumWidth = 50;
    private const byte RowStartBodyProfitCenter = 4;
    private const byte ColumnsSalary = 20; // Number of columns before salary columns
    private const byte SalaryColumnsCount = 7; // Number of salary columns

    private enum BasicInfo
    {
        RowStartHeader = 2,
        RowStartBody = 4,
        TotalColumns = 23
    }

    public async Task<byte[]> ExportEmployeeBasicInfo(IList<Employee> employees)
    {
        var templatePath = Path.Combine(hostingEnvironment.ContentRootPath, EmployeeBasicInfoTemplatePath);
        const string sheetName = "Employee Basic Info";
        var writer = new ExcelTemplateWriter(templatePath, sheetName);
        var enableGroupDate = await commonDataService.IsEnableGroupDateAsync();
        const byte colGroupDate = 7;
        const byte colLengthOfServiceGroupDate = 11;
        byte colSubCompany = 21;

        writer.CopyRows(0, 2);
        writer.CopyRows((byte)BasicInfo.RowStartHeader, 2);

        FillContentRowEmployeeBasicInfoReport(employees, ref writer);
        writer.SyncColumnsWidth((byte)BasicInfo.TotalColumns);
        writer.CreateFreezePane(0, (byte)BasicInfo.RowStartBody);
        if (!enableGroupDate)
        {
            writer.RemoveColumn(colGroupDate);
            writer.RemoveColumn(colLengthOfServiceGroupDate);
            colSubCompany -= 2; // sub company column is backed to 2 cells if group date cols are removed
        }
        var companyId = session.Get<short>(SessionKey.CurrentCompanyId);
        var subCompanies = await companyService.GetSubCompanies(companyId);
        var isShowSubCompany = subCompanies != null && subCompanies.Any();

        if (!isShowSubCompany)
        {
            writer.RemoveColumn(colSubCompany);
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
            LengthOfServiceByGroupDate = employeeService.CalculateLengthOfService(employee.JoinedDate, employee.MilitaryFromDate, employee.MilitaryToDate).LengthOfServiceText,
            LengthOfServiceByCompanyDate = employeeService.CalculateLengthOfService(employee.CompanyDate, employee.MilitaryFromDate, employee.MilitaryToDate).LengthOfServiceText,
            DoB = employee.DateOfBirth.ToDateMonthYearFormatedDate(),
            Gender = employee.Gender == 0 ? "F" : "M",
            CellPhone = employee.Phone,
            employee.Email,
            employee.Address,
            employee.CurrentAddress,
            Category = employee.IsLocal ? Labels.Local : Labels.Expat,
            CompanyName = string.IsNullOrEmpty(employee.SubCompanyName) ? string.Empty : employee.SubCompanyName
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
        const byte colGroupDate = 9;
        const byte colLengthOfServiceGroupDate = 13;

        writer.CopyRows(0, 2);
        writer.CopyRows(2, 2);

        WriteFullProfileBody(writer, profiles);
        writer.SyncColumnsWidth(56);
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
            LengthOfServiceByGroupDate = employeeService.CalculateLengthOfService(p.JoinedDate, p.MilitaryFromDate, p.MilitaryToDate).LengthOfServiceText,
            LengthOfServiceByCompanyDate = employeeService.CalculateLengthOfService(p.CompanyDate, p.MilitaryFromDate, p.MilitaryToDate).LengthOfServiceText,
            p.JobTitle,
            p.JobLevel,
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

    #endregion Employee Full Profile Information

    public async Task<byte[]> ExportGroupEmployeesTransferAsync(IEnumerable<EmployeeTransferModel> employees)
    {
        var templatePath = Path.Combine(hostingEnvironment.ContentRootPath, GroupEmployeeTransferListTemplatePath);

        await using var templateStream = new FileStream(templatePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, useAsync: true);
        using var templatePackage = new ExcelPackage(templateStream);
        using var xlPackage = new ExcelPackage();

        var template = templatePackage.Workbook.Worksheets.FirstOrDefault();
        if (template == null)
        {
            throw new InvalidOperationException("The template Excel file does not contain any worksheet.");
        }

        var rowIndex = 4;
        var colIndex = 1;

        var templateSheet = templatePackage.Workbook.Worksheets[0];
        var worksheet = xlPackage.Workbook.Worksheets.Add("Sheet 1", templateSheet);
        var totalColumns = 16;
        const int transferRowStartBody = 4;

        // data
        var rowNum = 1;
        foreach (var employee in employees)
        {
            colIndex = 1;
            template.Cells[transferRowStartBody, 1, transferRowStartBody, totalColumns].Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, totalColumns]);

            worksheet.Cells[rowIndex, colIndex++].Value = rowNum++;
            worksheet.Cells[rowIndex, colIndex++].Value = employee.CtrIdFormatted;
            worksheet.Cells[rowIndex, colIndex++].Value = employee.FullName;
            worksheet.Cells[rowIndex, colIndex++].Value = employee.FromNickName;
            worksheet.Cells[rowIndex, colIndex++].Value = employee.FromBUName;
            worksheet.Cells[rowIndex, colIndex++].Value = employee.FromDeptName;
            worksheet.Cells[rowIndex, colIndex++].Value = employee.FromTeamName;
            worksheet.Cells[rowIndex, colIndex++].Value = employee.FromCompanyName;
            worksheet.Cells[rowIndex, colIndex++].Value = employee.ToNickName;
            worksheet.Cells[rowIndex, colIndex++].Value = employee.ToBUName;
            worksheet.Cells[rowIndex, colIndex++].Value = employee.ToDeptName;
            worksheet.Cells[rowIndex, colIndex++].Value = employee.ToTeamName;
            worksheet.Cells[rowIndex, colIndex++].Value = employee.ToCompanyName;
            worksheet.Cells[rowIndex, colIndex++].Value = employee.Remark;
            worksheet.Cells[rowIndex, colIndex++].Value = employee.TransferredDatetime;
            worksheet.Cells[rowIndex, colIndex++].Value = employee.TransferredByNickName;

            rowIndex++;
        }

        worksheet.Cells.AutoFitColumns(MinimumWidth, MaximumWidth);
        worksheet.View.FreezePanes(RowStartBody + 1, 1);

        await using var ms = new MemoryStream();
        await xlPackage.SaveAsAsync(ms);
        return ms.ToArray();
    }

    public async Task<byte[]> ExportGroupEmployeesAsync(IEnumerable<GroupEmployeeModel> employees, bool showSalaryCurrency)
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
        const int totalColumns = 30;

        // title
        template.Cells[rowIndex, colIndex, rowIndex, totalColumns].Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, totalColumns]);
        rowIndex++;

        // header
        template.Cells[rowIndex, colIndex, rowIndex, totalColumns].Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, totalColumns]);
        rowIndex++;

        // data
        var rowNum = 1;
        foreach (var employee in employees)
        {
            colIndex = 1;
            template.Cells[RowStartBody, 1, RowStartBody, totalColumns].Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, totalColumns]);

            worksheet.Cells[rowIndex, colIndex++].Value = rowNum++;
            worksheet.Cells[rowIndex, colIndex++].Value = employee.CtrIdFormatted;
            worksheet.Cells[rowIndex, colIndex++].Value = employee.EmployeeCode;
            worksheet.Cells[rowIndex, colIndex++].Value = employee.FullName;
            worksheet.Cells[rowIndex, colIndex++].Value = employee.NickName;
            worksheet.Cells[rowIndex, colIndex++].Value = FormatDate(employee.DOB);
            worksheet.Cells[rowIndex, colIndex++].Value = employee.Gender.GetDisplayName();
            worksheet.Cells[rowIndex, colIndex++].Value = employee.CountryName;
            worksheet.Cells[rowIndex, colIndex++].Value = employee.GroupUnitId;
            worksheet.Cells[rowIndex, colIndex++].Value = employee.UnitCode;
            worksheet.Cells[rowIndex, colIndex++].Value = employee.UnitPIC;
            worksheet.Cells[rowIndex, colIndex++].Value = employee.DepartmentName;
            worksheet.Cells[rowIndex, colIndex++].Value = employee.DepartmentPIC;
            worksheet.Cells[rowIndex, colIndex++].Value = employee.TeamName;
            worksheet.Cells[rowIndex, colIndex++].Value = employee.TeamPIC;
            worksheet.Cells[rowIndex, colIndex++].Value = employee.JobLevel;
            worksheet.Cells[rowIndex, colIndex++].Value = employee.ManagementLevel;
            worksheet.Cells[rowIndex, colIndex++].Value = FormatDate(employee.JoinedDate);
            worksheet.Cells[rowIndex, colIndex++].Value = FormatSalary(employee.Salary);
            worksheet.Cells[rowIndex, colIndex++].Value = employee.Company;
            worksheet.Cells[rowIndex, colIndex++].Value = employee.SubCompany;
            worksheet.Cells[rowIndex, colIndex++].Value = FormatDate(employee.CompanyDate);
            worksheet.Cells[rowIndex, colIndex++].Value = FormatDate(employee.ResignedDate);
            worksheet.Cells[rowIndex, colIndex++].Value = employee.ResignedReason;
            worksheet.Cells[rowIndex, colIndex++].Value = FormatBoolean(employee.IsLocal);
            worksheet.Cells[rowIndex, colIndex++].Value = FormatBoolean(employee.IsSpecial);
            worksheet.Cells[rowIndex, colIndex++].Value = FormatBoolean(employee.Enabled);
            worksheet.Cells[rowIndex, colIndex].Value = FormatBoolean(employee.IsDeleted);

            rowIndex++;
        }

        worksheet.Cells.AutoFitColumns(MinimumWidth, MaximumWidth);
        worksheet.View.FreezePanes(RowStartBody, 1);

        if (showSalaryCurrency)
        {
            xlPackage.Encryption.IsEncrypted = true;
            xlPackage.Encryption.Password = employeeService.GetTotalCostPassword();
        }
        else
        {
            const int salaryColumn = 18;
            const int numberOfColumn = 1;
            worksheet?.DeleteColumn(salaryColumn, numberOfColumn);
        }

        await using var ms = new MemoryStream();
        await xlPackage.SaveAsAsync(ms);
        return ms.ToArray();
    }

    public async Task<byte[]> ExportProfitCenterReportAsync(IEnumerable<GroupEmployeeModel> employees, bool showSalaryCurrency)
    {
        var templatePath = Path.Combine(hostingEnvironment.ContentRootPath, ProfitCenterReportTemplatePath);
        using var xlPackage = new ExcelPackage(new FileInfo(templatePath));

        var worksheet = xlPackage.Workbook.Worksheets.FirstOrDefault();
        FillEmployeeData(employees, worksheet, showSalaryCurrency);
        if (showSalaryCurrency)
        {
            xlPackage.Encryption.IsEncrypted = true;
            xlPackage.Encryption.Password = employeeService.GetTotalCostPassword();
        }
        else
        {
            worksheet?.DeleteColumn(ColumnsSalary, SalaryColumnsCount);
        }
        AddTotalRow(worksheet);

        await using var ms = new MemoryStream();
        await xlPackage.SaveAsAsync(ms);
        return ms.ToArray();
    }

    private static void FillEmployeeData(IEnumerable<GroupEmployeeModel> employees, ExcelWorksheet worksheet, bool showSalaryCurrency)
    {
        var rowIndex = 4;
        var rowNum = 1;

        foreach (var employee in employees)
        {
            var paymaster = DeterminePaymaster(employee);
            FillEmployeeRow(employee, worksheet, rowIndex, rowNum++, paymaster, showSalaryCurrency);
            rowIndex++;
        }
    }

    private static string DeterminePaymaster(GroupEmployeeModel employee)
    {
        if (Constants.NexCompanyIds.Contains(employee.CompanyId))
        {
            return "Nexcel";
        }

        return !string.IsNullOrWhiteSpace(employee.SubCompany) ? employee.SubCompany : employee.Company;
    }

    private static void FillEmployeeRow(GroupEmployeeModel employee, ExcelWorksheet worksheet, int rowIndex, int rowNum, string paymaster, bool showSalaryCurrency)
    {
        var colIndex = 1;

        worksheet.Cells[rowIndex, colIndex++].Value = rowNum;
        worksheet.Cells[rowIndex, colIndex++].Value = employee.CtrIdFormatted;
        worksheet.Cells[rowIndex, colIndex++].Value = $"{employee.DepartmentCode}-{employee.TeamCode}";
        worksheet.Cells[rowIndex, colIndex++].Value = employee.GroupUnitId;
        worksheet.Cells[rowIndex, colIndex++].Value = employee.UnitCode;
        worksheet.Cells[rowIndex, colIndex++].Value = employee.DepartmentName;
        worksheet.Cells[rowIndex, colIndex++].Value = employee.DepartmentCode;
        worksheet.Cells[rowIndex, colIndex++].Value = employee.DepartmentPIC;
        worksheet.Cells[rowIndex, colIndex++].Value = employee.TeamName;
        worksheet.Cells[rowIndex, colIndex++].Value = employee.TeamCode;
        worksheet.Cells[rowIndex, colIndex++].Value = employee.AccountableArea;
        worksheet.Cells[rowIndex, colIndex++].Value = employee.TeamPIC;
        worksheet.Cells[rowIndex, colIndex++].Value = employee.EmployeeCode;
        worksheet.Cells[rowIndex, colIndex++].Value = employee.NickName;
        worksheet.Cells[rowIndex, colIndex++].Value = employee.JobLevel;
        worksheet.Cells[rowIndex, colIndex++].Value = employee.ManagementLevel;
        worksheet.Cells[rowIndex, colIndex++].Value = paymaster;
        worksheet.Cells[rowIndex, colIndex++].Value = FormatDate(employee.JoinedDate);
        worksheet.Cells[rowIndex, colIndex++].Value = employee.Location;

        if (showSalaryCurrency)
        {
            SetCellValueRight(worksheet, rowIndex, colIndex++, FormatSalary(employee.Salary));
            SetCellValueRight(worksheet, rowIndex, colIndex++, FormatSalary(employee.BasicSalary));
            SetCellValueRight(worksheet, rowIndex, colIndex++, FormatSalary(employee.JobLevelAddOn));
            SetCellValueRight(worksheet, rowIndex, colIndex++, FormatSalary(employee.ManagementAllowance));
            SetCellValueRight(worksheet, rowIndex, colIndex++, FormatSalary(employee.OverseasAllowance));
            SetCellValueRight(worksheet, rowIndex, colIndex++, FormatSalary(employee.SkillSetAddOn));
            SetCellValueRight(worksheet, rowIndex, colIndex, FormatSalary(employee.Others));
        }
    }

    private static void AddTotalRow(ExcelWorksheet worksheet)
    {
        var rowEndData = worksheet.Dimension?.End.Row ?? 0;
        worksheet.Cells[2, 1].Value = "Total:";
        worksheet.Cells[2, 1].Style.Font.Bold = true;
        worksheet.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

        if (rowEndData >= RowStartBodyProfitCenter)
        {
            worksheet.Cells[2, 2].Formula = $"SUBTOTAL(103, A{RowStartBodyProfitCenter}:A{rowEndData})";
        }
        else
        {
            worksheet.Cells[2, 2].Value = 0;
        }
        
        worksheet.Cells[2, 2].Style.Font.Bold = true;
        worksheet.Cells[2, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

        worksheet.Cells[2, 1, 2, 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
        worksheet.Cells[2, 1, 2, 2].Style.Fill.BackgroundColor.SetColor(Color.Yellow);
        worksheet.Column(1).AutoFit();
    }

    private static void SetCellValueRight(ExcelWorksheet worksheet, int row, int col, string value)
    {
        worksheet.Cells[row, col].Value = value;
        worksheet.Cells[row, col].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
    }

    private static string FormatDate(DateTime? date) => date?.ToDateMonthYearFormatedDate();

    private static string FormatBoolean(bool value) => value ? Constants.Yes : Constants.No;

    private static string FormatSalary(decimal value) => value == 0 ? "-" : $"{value:N0}";
}