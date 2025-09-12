namespace HRM.Services.Implement;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Criteria.Department;
using GO.UserControl.Client.Core.Enumerables;
using GO.UserControl.Client.Core.Session;
using HRM.Core.Constants;
using HRM.Core.Models;
using HRM.Core.Services;
using HRM.Repositories.Interfaces;
using Interfaces;
using Microsoft.AspNetCore.Hosting;
using Models;
using OfficeOpenXml;

public class DepartmentService(
    IUserControlSession session,
    IDepartmentRepository departmentRepository,
    IBaseService baseService,
    IWebHostEnvironment hostingEnvironment) : IDepartmentService
{
    private const string DepartmentTemplatePath = "ExcelTemplates/Departments.xlsx";

    public async Task<CommandResult> DeleteAsync(int actorId, int id)
    {
        var criteria = new DeleteDepartmentCriteria
        {
            DepartmentId = id,
            ModifiedBy = actorId
        };
        return await baseService.ExecuteNonQueryAsync(criteria);
    }

    public async Task<int> GetNextGroupDeptIdAsync()
    {
        var criteria = new GetNextGroupDeptIdCriteria();
        return await baseService.GetDataEntityAsync<int>(criteria);
    }

    public async Task<IList<Department>> GetDepartmentAsync(bool? isActive = true, int? unitId = null)
    {
        var criteria = new GetDepartmentByActiveCriteria
        {
            IsActive = isActive,
            UnitId = unitId
        };
        var departments = await departmentRepository.GetByActiveAsync(criteria);

        return departments ?? [];
    }

    public IList<Department> GetDepartmentEmployeeBelongsTo(IList<Department> departments, int empId)
    {
        if (session.HasPermissionOnFunction(FunctionKey.DepartmentManagement_ViewAll, Policy.Read))
        {
            return departments;
        }
        List<int> unitIds = [];

        if (session.HasPermissionOnFunction(FunctionKey.DepartmentManagement_BUView, Policy.Read))
        {
            unitIds.AddRange(departments
                .Where(x => x.Unit.PICEmpId == empId ||
                            (!string.IsNullOrWhiteSpace(x.Unit.SupporterEmpId) &&
                             x.Unit.SupporterEmpId.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList().Exists(id => id == empId.ToString())))
                .Select(x => x.Unit.Id)
                .ToList());
        }

        if (session.HasPermissionOnFunction(FunctionKey.DepartmentManagement_DeptView, Policy.Read))
        {
            unitIds.AddRange(departments
                .Where(x => x.PICEmpId == empId ||
                            (!string.IsNullOrWhiteSpace(x.SupporterEmpId) &&
                             x.SupporterEmpId.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList().Exists(id => id == empId.ToString())))
                .Select(x => x.Unit.Id)
                .ToList());
        }

        var distinctUnitIds = unitIds.Distinct().ToList();
        return departments.Where(x => distinctUnitIds.Contains(x.Unit.Id)).ToList();
    }

    public async Task<Department> GetByIdAsync(int departmentId)
    {
        var criteria = new GetDepartmentByIdCriteria
        {
            Id = departmentId
        };
        return await departmentRepository.GetByIdAsync(criteria);
    }

    public async Task<byte[]> GenerateExcelAsync(IEnumerable<Department> departments)
    {
        const int totalColumns = 9;
        const int rowStartBody = 3;
        var templatePath = Path.Combine(hostingEnvironment.ContentRootPath, DepartmentTemplatePath);

        await using var templateStream = new FileStream(templatePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, useAsync: true);
        using var templatePackage = new ExcelPackage(templateStream);
        using var xlPackage = new ExcelPackage();

        var template = templatePackage.Workbook.Worksheets.FirstOrDefault();
        if (template == null)
        {
            throw new InvalidOperationException("The template Excel file does not contain any worksheet.");
        }

        var worksheet = xlPackage.Workbook.Worksheets.Add("Department List");
        var rowIndex = 1;
        var colIndex = 1;

        // title
        template.Cells[rowIndex, colIndex, rowIndex, totalColumns].Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, totalColumns]);
        rowIndex++;

        // header
        template.Cells[rowIndex, colIndex, rowIndex, totalColumns].Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, totalColumns]);
        rowIndex++;

        // data
        var rowNum = 1;
        foreach (var department in departments)
        {
            colIndex = 1;
            template.Cells[rowStartBody, 1, rowStartBody, totalColumns].Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, totalColumns]);

            worksheet.Cells[rowIndex, colIndex++].Value = rowNum++;
            worksheet.Cells[rowIndex, colIndex++].Value = department.GroupDeptId.ToString();
            worksheet.Cells[rowIndex, colIndex++].Value = department.Code;
            worksheet.Cells[rowIndex, colIndex++].Value = department.Unit?.Code;
            worksheet.Cells[rowIndex, colIndex++].Value = department.Name;
            worksheet.Cells[rowIndex, colIndex++].Value = department.AccountableArea;
            worksheet.Cells[rowIndex, colIndex++].Value = department.PICCtrId is not null ? department.PICCtrId.ToString().PadLeft(6, '0') : string.Empty;
            worksheet.Cells[rowIndex, colIndex++].Value = department.PICEmployeeName;
            worksheet.Cells[rowIndex, colIndex].Value = department.IsActive ? Constants.Yes : Constants.No;

            rowIndex++;
        }

        worksheet.Cells.AutoFitColumns();
        worksheet.View.FreezePanes(rowStartBody, 1);

        await using var ms = new MemoryStream();
        await xlPackage.SaveAsAsync(ms);
        return ms.ToArray();
    }
}