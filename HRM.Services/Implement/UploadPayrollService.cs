using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using HRM.Core.Models;
using HRM.Core.Services;
using HRM.Criteria.EmployeePayroll;
using HRM.Models.Payroll;
using HRM.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using OfficeOpenXml;

namespace HRM.Services.Implement;

public class UploadPayrollService(IBaseService baseService) : IUploadPayrollService
{
    private const string SheetExpat = "Payroll - Expat";
    private const string SheetGO = "Payroll - GO";
    private const string SheetOPS = "Payroll - OPS";
    private const string SheetExtraPayment = "Payroll - Extra payment";
    private const string ColumnIdStaff = "id staff";
    private const string ColumnTotalPersonalCost = "total personal cost";

    public async Task<CommandResult> ImportEmpPayrollAsync(UploadPayrollModel model)
    {
        try
        {
            var allPayrollData = await ProcessExcelFileAsync(model.File);
            var criteria = new ImportEmployeePayrollCriteria
            {
                EmployeeListJson = JsonConvert.SerializeObject(allPayrollData),
                Month = model.Month,
                PeriodType = model.PeriodType,
                CompanyId = model.CompanyId,
                ActorId = model.ActorId,
                SavedFileName = model.SavedFileName,
                FileName = model.File.FileName
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

    private static async Task<List<ImportPayrollModel>> ProcessExcelFileAsync(IFormFile file)
    {
        await using var stream = file.OpenReadStream();
        using var package = new ExcelPackage(stream);

        if (package.Workbook.Worksheets.Count == 0)
        {
            throw new InvalidOperationException("Excel file does not contain any worksheets");
        }

        var worksheets = new[] { SheetExpat, SheetGO, SheetOPS, SheetExtraPayment };
        var allPayrollData = new List<ImportPayrollModel>();

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
            if (columnIndexes.idStaffColumn != 0 && columnIndexes.totalPersonalCostColumn != 0)
            {
                var payrollData = ExtractPayrollData(worksheet, headerRow, columnIndexes, sheetName);

                ProcessPayrollData(allPayrollData, payrollData, sheetName);
            }
        }

        return allPayrollData;
    }

    private static void ProcessPayrollData(List<ImportPayrollModel> allPayrollData, List<ImportPayrollModel> payrollData, string sheetName)
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

    private static void ProcessExtraPayments(List<ImportPayrollModel> allPayrollData, List<ImportPayrollModel> extraPayments)
    {
        foreach (var extraPayment in extraPayments)
        {
            var existingPayroll = allPayrollData.Find(p => p.EmpCode == extraPayment.EmpCode);
            if (existingPayroll != null)
            {
                existingPayroll.ExtraPayment = extraPayment.TotalPersonalCost;
            }
            else
            {
                allPayrollData.Add(new ImportPayrollModel
                {
                    EmpCode = extraPayment.EmpCode,
                    ExtraPayment = extraPayment.TotalPersonalCost,
                    IsExtraPayment = true
                });
            }
        }
    }

    private static void ProcessRegular(List<ImportPayrollModel> allPayrollData, List<ImportPayrollModel> regularPayments)
    {
        foreach (var payroll in regularPayments)
        {
            var existingPayroll = allPayrollData.Find(p => p.EmpCode == payroll.EmpCode);
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
        for (var row = 1; row <= totalRows; row++)
        {
            var headerName = worksheet.Cells[row, 1].Value?.ToString()?.Trim().ToLower(CultureInfo.InvariantCulture);
            if (headerName is "no" or "no.")
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

        // Search from header row up to first row
        for (var row = headerRow; row >= 1; row--)
        {
            for (var col = 1; col <= totalColumns; col++)
            {
                var headerText = worksheet.Cells[row, col].Value?.ToString()?.Trim().ToLower(CultureInfo.InvariantCulture);

                if (headerText == ColumnIdStaff && idStaffColumn == 0)
                {
                    idStaffColumn = col;
                }

                if (headerText == ColumnTotalPersonalCost && totalPersonalCostColumn == 0)
                {
                    totalPersonalCostColumn = col;
                }

                if (idStaffColumn > 0 && totalPersonalCostColumn > 0)
                {
                    return (idStaffColumn, totalPersonalCostColumn);
                }
            }
        }

        return (idStaffColumn, totalPersonalCostColumn);
    }

    private static List<ImportPayrollModel> ExtractPayrollData(ExcelWorksheet worksheet, int headerRow, (int idStaffColumn, int totalPersonalCostColumn) columns, string sheetName)
    {
        var payrollData = new List<ImportPayrollModel>();
        var totalRows = worksheet.Dimension.End.Row;

        for (var row = headerRow + 1; row <= totalRows; row++)
        {
            var EmpCode = worksheet.Cells[row, columns.idStaffColumn].Value?.ToString();
            if (string.IsNullOrEmpty(EmpCode))
            {
                continue;
            }

            var totalPersonalCost = ParseCostCell(worksheet.Cells[row, columns.totalPersonalCostColumn].Value);

            payrollData.Add(new ImportPayrollModel
            {
                EmpCode = EmpCode,
                TotalPersonalCost = totalPersonalCost,
                IsExtraPayment = sheetName == SheetExtraPayment
            });
        }

        return payrollData;
    }

    private static decimal ParseCostCell(object costCell)
    {
        if (costCell == null)
        {
            return 0M;
        }

        _ = decimal.TryParse(costCell.ToString(), out var totalPersonalCost);
        return totalPersonalCost;
    }
}