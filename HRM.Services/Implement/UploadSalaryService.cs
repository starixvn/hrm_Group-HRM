using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;
using HRM.Core.Models;
using HRM.Core.Services;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using HRM.Services.Interfaces;
using HRM.Models.Payroll;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using HRM.Criteria.EmployeePayroll;

namespace HRM.Services.Implement;

public class UploadSalaryService(
    IWebHostEnvironment hostingEnvironment,
    IBaseService baseService) : IUploadSalaryService
{
    private const string SheetExpat = "Payroll - Expat";
    private const string SheetGO = "Payroll - GO";
    private const string SheetOPS = "Payroll - OPS";
    private const string SheetExtraPayment = "Payroll - Extra payment";
    private const string ColumnIdStaff = "id staff";
    private const string ColumnTotalPersonalCost = "total personal cost";

    public async Task<CommandResult> ImportEmpSalaryAsync(IFormFile file)
    {
        try
        {
            var allPayrollData = await ProcessExcelFileAsync(file);
            var criteria = new ImportEmployeePayrollCriteria
            {
                EmployeeListJson = JsonConvert.SerializeObject(allPayrollData),
                Month = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1),
                PeriodType = 1,
                CompanyId = 3
            };
            return await baseService.ExecuteNonQueryAsync(criteria);
        }
        catch (Exception ex)
        {
            return new CommandResult 
            {
                Message = ex.Message
            };
        }
    }

    private async Task<List<UploadSalaryModel>> ProcessExcelFileAsync(IFormFile file)
    {
        var templatePath = Path.Combine(hostingEnvironment.ContentRootPath, "ExcelTemplates/Payroll.xlsx");
        // await using var stream = file.OpenReadStream();
        // using var package = new ExcelPackage(stream);
        using var package = new ExcelPackage(new FileInfo(templatePath));
        if (package.Workbook.Worksheets.Count == 0)
        {
            throw new InvalidOperationException("Excel file does not contain any worksheets");
        }

        var worksheets = new[] { SheetExpat, SheetGO, SheetOPS, SheetExtraPayment };
        var allPayrollData = new List<UploadSalaryModel>();

        foreach (var sheetName in worksheets)
        {
            var worksheet = package.Workbook.Worksheets[sheetName];
            if (worksheet == null) 
            {
                continue;
            }

            var headerRow = FindHeaderRow(worksheet);
            if (headerRow == 0) 
            {
                continue;
            }

            var columnIndexes = GetColumnIndexes(worksheet, headerRow);
            var payrollData = ExtractPayrollData(worksheet, headerRow, columnIndexes, sheetName);
            
            ProcessPayrollData(allPayrollData, payrollData, sheetName);
        }

        return allPayrollData;
    }


    private static void ProcessPayrollData(List<UploadSalaryModel> allPayrollData, List<UploadSalaryModel> payrollData, string sheetName)
    {
        if (sheetName == SheetExtraPayment)
        {
            ProcessExtraPayments(allPayrollData, payrollData);
        }
        else
        {
            ProcessRegular(allPayrollData, payrollData);
        }
    }

    private static void ProcessExtraPayments(List<UploadSalaryModel> allPayrollData, List<UploadSalaryModel> extraPayments)
    {
        foreach (var extraPayment in extraPayments)
        {
            var existingPayroll = allPayrollData.FirstOrDefault(p => p.EmpId == extraPayment.EmpId);
            if (existingPayroll != null)
            {
                existingPayroll.ExtraPayment = extraPayment.TotalPersonalCost;
            }
            else
            {
                allPayrollData.Add(new UploadSalaryModel
                {
                    EmpId = extraPayment.EmpId,
                    ExtraPayment = extraPayment.TotalPersonalCost,
                    IsExtraPayment = true
                });
            }
        }
    }

    private static void ProcessRegular(List<UploadSalaryModel> allPayrollData, List<UploadSalaryModel> regularPayments)
    {
        foreach (var payroll in regularPayments)
        {
            var existingPayroll = allPayrollData.FirstOrDefault(p => p.EmpId == payroll.EmpId);
            if (existingPayroll != null)
            {
                existingPayroll.TotalPersonalCost = payroll.TotalPersonalCost;
            }
            else
            {
                allPayrollData.Add(payroll);
            }
        }
    }

    private static int FindHeaderRow(ExcelWorksheet worksheet)
    {
        var totalRows = worksheet.Dimension.End.Row;
        for (int row = 1; row <= totalRows; row++)
        {
            var headerName = worksheet.Cells[row, 1].Value?.ToString()?.Trim().ToLower();
            if (headerName == "no" || headerName == "no.")
            {
                return row;
            }
        }
        return 0;
    }

    private static (int idStaffColumn, int totalPersonalCostColumn) GetColumnIndexes(ExcelWorksheet worksheet, int headerRow)
    {
        var idStaffColumn = 0;
        var totalPersonalCostColumn = 0;
        var totalColumns = worksheet.Dimension.End.Column;

        for (int col = 1; col <= totalColumns; col++)
        {
            var headerText = worksheet.Cells[headerRow, col].Value?.ToString()?.Trim().ToLower();
            switch (headerText)
            {
                case ColumnIdStaff:
                    idStaffColumn = col;
                    break;
                case ColumnTotalPersonalCost:
                    totalPersonalCostColumn = col;
                    break;
            }
        }

        return (idStaffColumn, totalPersonalCostColumn);
    }

    private static List<UploadSalaryModel> ExtractPayrollData(ExcelWorksheet worksheet, int headerRow, (int idStaffColumn, int totalPersonalCostColumn) columns, string sheetName)
    {
        var payrollData = new List<UploadSalaryModel>();
        var totalRows = worksheet.Dimension.End.Row;

        for (int row = headerRow + 1; row <= totalRows; row++)
        {
            var empId = worksheet.Cells[row, columns.idStaffColumn].Value?.ToString();
            if (string.IsNullOrEmpty(empId)) continue;

            var totalPersonalCost = ParseCostCell(worksheet.Cells[row, columns.totalPersonalCostColumn].Value);
            
            payrollData.Add(new UploadSalaryModel
            {
                EmpId = empId,
                TotalPersonalCost = totalPersonalCost,
                IsExtraPayment = sheetName == SheetExtraPayment
            });
        }

        return payrollData;
    }

    private static decimal ParseCostCell(object costCell)
    {
        if (costCell == null) return 0M;
        decimal.TryParse(costCell.ToString(), out var totalPersonalCost);
        return totalPersonalCost;
    }
}