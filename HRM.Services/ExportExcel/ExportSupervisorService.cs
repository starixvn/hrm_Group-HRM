using System.Collections.Generic;
using System.IO;
using System.Linq;
using HRM.Core.Enums;
using HRM.Models.Supervisor;
using HRM.Resources;
using Microsoft.AspNetCore.Hosting;
using OfficeOpenXml;

namespace HRM.Services.ExportExcel;

public interface IExportSupervisorService
{
    byte[] ExportEmployeeSupervisor(IList<SupervisorReportModel> supervisors);

    byte[] ExportTeamSupervisor(IEnumerable<TeamSupervisorReportModel> supervisors);
}

#pragma warning disable S109 // Magic numbers should not be used
public class ExportSupervisorService(IWebHostEnvironment hostingEnvironment) : IExportSupervisorService
{
    private const byte HeaderRowIndex = 2;
    private const byte BodyRowIndex = 3;
    private const byte OddRowIndex = 3;
    private const byte EvenRowIndex = 4;
    private const byte MinimumWidth = 10;
    private const byte MaximumWidth = 50;

    public byte[] ExportEmployeeSupervisor(IList<SupervisorReportModel> supervisors)
    {
        var templatePath = Path.Combine(hostingEnvironment.ContentRootPath, "ExcelTemplates/EmployeeSupervisor.xlsx");
        using var templatePackage = new ExcelPackage(new FileInfo(templatePath));
        using var xlPackage = new ExcelPackage();
        const byte totalColumns = 13;
        var templateWorksheet = templatePackage.Workbook.Worksheets[0];
        var worksheet = xlPackage.Workbook.Worksheets.Add(Labels.EmployeeSupervisor);
        var rowIndex = 1;
        var colIndex = 1;

        // title & header
        templateWorksheet.Cells[rowIndex, colIndex, HeaderRowIndex, totalColumns].Copy(worksheet.Cells[rowIndex, colIndex, HeaderRowIndex, totalColumns]);

        // data
        rowIndex = BodyRowIndex;
        var recordIndex = 1;
        foreach (var supervisor in supervisors)
        {
            colIndex = 1;
            if (recordIndex % 2 != 0)
            {
                templateWorksheet.Cells[OddRowIndex, 1, OddRowIndex, totalColumns].Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, totalColumns]);
            }
            else
            {
                templateWorksheet.Cells[EvenRowIndex, 1, EvenRowIndex, totalColumns].Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, totalColumns]);
            }

            worksheet.Cells[rowIndex, colIndex++].Value = recordIndex++;
            worksheet.Cells[rowIndex, colIndex++].Value = supervisor.Department;
            worksheet.Cells[rowIndex, colIndex++].Value = supervisor.Team.Code;
            worksheet.Cells[rowIndex, colIndex++].Value = supervisor.Code;
            worksheet.Cells[rowIndex, colIndex++].Value = supervisor.FullName;
            worksheet.Cells[rowIndex, colIndex++].Value = supervisor.NickName;
            worksheet.Cells[rowIndex, colIndex++].Value = supervisor.Roles2String;
            worksheet.Cells[rowIndex, colIndex++].Value = GetSupervisorByActionType(supervisor.Supervisors, SupervisorActionType.Verifier);
            worksheet.Cells[rowIndex, colIndex++].Value = GetSupervisorByActionType(supervisor.Supervisors, SupervisorActionType.Approver);
            worksheet.Cells[rowIndex, colIndex++].Value = GetSupervisorByActionType(supervisor.Supervisors, SupervisorActionType.ShiftArrangement);
            worksheet.Cells[rowIndex, colIndex++].Value = GetSupervisorByActionType(supervisor.Supervisors, SupervisorActionType.OnBehalfRequest);
            worksheet.Cells[rowIndex, colIndex].Value = GetSupervisorByActionType(supervisor.Supervisors, SupervisorActionType.DisciplineManager);

            rowIndex++;
        }

        worksheet.Cells.AutoFitColumns(MinimumWidth, MaximumWidth);
        worksheet.View.FreezePanes(BodyRowIndex, 1);

        return xlPackage.GetAsByteArray();
    }

    public byte[] ExportTeamSupervisor(IEnumerable<TeamSupervisorReportModel> supervisors)
    {
        var templatePath = Path.Combine(hostingEnvironment.ContentRootPath, "ExcelTemplates/TeamSupervisor.xlsx");
        using var templatePackage = new ExcelPackage(new FileInfo(templatePath));
        using var xlPackage = new ExcelPackage();
        const byte totalColumns = 8;
        var templateWorksheet = templatePackage.Workbook.Worksheets[0];
        var worksheet = xlPackage.Workbook.Worksheets.Add(Labels.TeamSupervisor);
        var rowIndex = 1;
        var colIndex = 1;

        // title & header
        templateWorksheet.Cells[rowIndex, colIndex, HeaderRowIndex, totalColumns].Copy(worksheet.Cells[rowIndex, colIndex, HeaderRowIndex, totalColumns]);

        // data
        rowIndex = BodyRowIndex;
        var recordIndex = 1;
        foreach (var supervisor in supervisors)
        {
            colIndex = 1;
            if (recordIndex % 2 != 0)
            {
                templateWorksheet.Cells[OddRowIndex, 1, OddRowIndex, totalColumns].Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, totalColumns]);
            }
            else
            {
                templateWorksheet.Cells[EvenRowIndex, 1, EvenRowIndex, totalColumns].Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, totalColumns]);
            }

            worksheet.Cells[rowIndex, colIndex++].Value = recordIndex++;
            worksheet.Cells[rowIndex, colIndex++].Value = supervisor.DepartmentName;
            worksheet.Cells[rowIndex, colIndex++].Value = supervisor.Name;
            worksheet.Cells[rowIndex, colIndex++].Value = GetSupervisorByActionType(supervisor.Supervisors, SupervisorActionType.Verifier);
            worksheet.Cells[rowIndex, colIndex++].Value = GetSupervisorByActionType(supervisor.Supervisors, SupervisorActionType.Approver);
            worksheet.Cells[rowIndex, colIndex++].Value = GetSupervisorByActionType(supervisor.Supervisors, SupervisorActionType.ShiftArrangement);
            worksheet.Cells[rowIndex, colIndex++].Value = GetSupervisorByActionType(supervisor.Supervisors, SupervisorActionType.OnBehalfRequest);
            worksheet.Cells[rowIndex, colIndex].Value = GetSupervisorByActionType(supervisor.Supervisors, SupervisorActionType.DisciplineManager);

            rowIndex++;
        }

        worksheet.Cells.AutoFitColumns(MinimumWidth, MaximumWidth);
        worksheet.View.FreezePanes(BodyRowIndex, 1);

        return xlPackage.GetAsByteArray();
    }

    private static string GetSupervisorByActionType(IList<SupervisorModel> supervisors, SupervisorActionType actionType)
    {
        return string.Join(", ", supervisors.Where(x => x.ActionType == (byte)actionType).Select(x => x.NickName));
    }
}