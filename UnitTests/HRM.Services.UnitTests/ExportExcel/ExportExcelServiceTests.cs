using System;
using System.Collections.Generic;
using System.IO;
using HRM.Controllers.UnitTests;
using HRM.Core.Enums;
using HRM.Models;
using HRM.Models.HospitalizationLeave;
using HRM.Models.Request;
using HRM.Services.ExportExcel;
using Microsoft.AspNetCore.Hosting;
using NPOI.HSSF.UserModel;
using NSubstitute;
using Xunit;

namespace HRM.Services.UnitTests.ExportExcel;

public class ExportExcelServiceTests : BaseControllerTest
{
    private readonly string _tempDirectory;
    private readonly ExportExcelService _service;

    public ExportExcelServiceTests()
    {
        _tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(_tempDirectory);
        Directory.CreateDirectory(Path.Combine(_tempDirectory, "ExcelTemplates"));

        var _hostingEnvironment = Substitute.For<IWebHostEnvironment>();
        _hostingEnvironment.ContentRootPath.Returns(_tempDirectory);
        CreateExcelTemplates();

        _service = new ExportExcelService(_hostingEnvironment);
    }

    [Fact]
    public void ExportEmployeeWfh_WithData_ReturnsWorkbook()
    {
        // Arrange
        var employeeWfhList = new List<EmployeeWorkFromHome>
        {
            new()
            {
                Code = "EMP001",
                FullName = "John Doe",
                Nickname = "John",
                DepartmentName = "IT",
                TeamName = "Development",
                FromDate = new DateTime(2024, 1, 1),
                ToDate = new DateTime(2024, 1, 31),
                Remark = "Test",
                IsCountInHeadcount = true,
                Modifier = "Admin",
                ModifiedDate = new DateTime(2024, 1, 1)
            }
        };

        // Act
        var result = _service.ExportEmployeeWfh(employeeWfhList);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.NumberOfSheets);
        var sheet = result.GetSheet("EmployeeWorkFromHome");
        Assert.NotNull(sheet);
    }

    [Fact]
    public void ExportEmployeeWfh_WithEmptyList_ReturnsWorkbook()
    {
        // Arrange
        var employeeWfhList = new List<EmployeeWorkFromHome>();

        // Act
        var result = _service.ExportEmployeeWfh(employeeWfhList);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public void ExportLeaveSummary_WithData_ReturnsWorkbook()
    {
        // Arrange
        var leaveSummaryList = new List<LeaveSummary>
        {
            new()
            {
                Code = "EMP001",
                FullName = "John Doe",
                Nickname = "John",
                Department = "IT",
                Team = "Development",
                CreditKIV = 5,
                AnnualLeaveCurrentYear = 12,
                LeaveBalance = 10,
                KIVBalance = 3,
                KIVMinuteBalance = 120,
                Enabled = true,
                IsDeleted = false,
                Jan = 1,
                Feb = 2
            }
        };
        const int year = 2024;

        // Act
        var result = _service.ExportLeaveSummary(leaveSummaryList, year);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.NumberOfSheets);
        var sheet = result.GetSheet("LeaveSummary");
        Assert.NotNull(sheet);
    }

    [Fact]
    public void ExportLeaveSummaryForJoinedDate_WithData_ReturnsWorkbook()
    {
        // Arrange
        var leaveSummaryList = new List<LeaveSummaryForJoinedDate>
        {
            new()
            {
                Code = "EMP001",
                FullName = "John Doe",
                Nickname = "John",
                Department = "IT",
                Team = "Development",
                AnnualLeaveCurrentYear = 12,
                ExtraLeave = 2,
                DeductDays = 5,
                LeaveBalance = 9,
                FromDate = new DateTime(2023, 1, 1),
                Enabled = true,
                IsDeleted = false
            }
        };

        // Act
        var result = _service.ExportLeaveSummaryForJoinedDate(leaveSummaryList);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.NumberOfSheets);
    }

    [Fact]
    public void ExportWorkingTimeRequest_WithData_ReturnsWorkbook()
    {
        // Arrange
        var requestList = new List<RequestEntity>
        {
            new()
            {
                Id = 1,
                CompanyId = 1,
                Requester = new Employee
                {
                    UnitCode = "UNIT001",
                    Team = new Team
                    {
                        Name = "Development",
                        Department = new Department { Name = "IT" }
                    },
                    NickName = "John",
                    FullName = "John Doe",
                    Enabled = true,
                    IsDeleted = false
                },
                RequestType = new RequestTypeModel { Id = (byte)RequestType.Leave },
                LeaveType = new LeaveType { Id = 1 },
                Status = new RequestStatus { Id = (byte)CurrentRequestStatus.Approved },
                CreatedDate = new DateTime(2024, 1, 1),
                Remark = "Test"
            }
        };
        const string title = "Test Report";

        // Act
        var result = _service.ExportWorkingTimeRequest(requestList, title);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.NumberOfSheets);
    }

    [Fact]
    public void ExportHospitalizationLeaveSummary_WithData_ReturnsWorkbook()
    {
        // Arrange
        var employees = new List<HospitalizationLeaveBalanceSummaryReport>
        {
            new()
            {
                Code = "EMP001",
                FullName = "John Doe",
                Nickname = "John",
                Department = "IT",
                Team = "Development",
                Balance = 10,
                RemainingBalance = 8,
                Jan = 1,
                Feb = 1
            }
        };
        const int year = 2024;

        // Act
        var result = _service.ExportHospitalizationLeaveSummary(employees, year);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.NumberOfSheets);
    }

    [Fact]
    public void ExportSickLeaveSummary_WithData_ReturnsWorkbook()
    {
        // Arrange
        var employees = new List<HospitalizationLeaveBalanceSummaryReport>
        {
            new()
            {
                Code = "EMP001",
                FullName = "John Doe",
                Nickname = "John",
                Department = "IT",
                Team = "Development",
                Balance = 10,
                RemainingBalance = 8,
                Jan = 1
            }
        };
        const int year = 2024;

        // Act
        var result = _service.ExportSickLeaveSummary(employees, year);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.NumberOfSheets);
    }

    [Fact]
    public void GetTemplateRowIndex_WithDifferentStates_ReturnsCorrectIndex()
    {
        // Arrange
        const int startRow = 4;

        // Act & Assert
        var normal = _service.GetTemplateRowIndex(startRow, true, false);
        Assert.Equal(startRow, normal);

        var disabled = _service.GetTemplateRowIndex(startRow, false, false);
        Assert.Equal(startRow + 1, disabled);

        var deleted = _service.GetTemplateRowIndex(startRow, true, true);
        Assert.Equal(startRow + 2, deleted);
    }

    private void CreateExcelTemplates()
    {
        var templatesPath = Path.Combine(_tempDirectory, "ExcelTemplates");

        CreateTemplateFile(Path.Combine(templatesPath, "EmployeeWorkFromHome.xls"), 5);
        CreateTemplateFile(Path.Combine(templatesPath, "LeaveSummaryTemplate.xls"), 5);
        CreateTemplateFile(Path.Combine(templatesPath, "LeaveSummaryForJoinedDateTemplate.xls"), 5);
        CreateTemplateFile(Path.Combine(templatesPath, "WorkingTimeRequestTemplate.xls"), 4);
        CreateTemplateFile(Path.Combine(templatesPath, "HospitalizationLeaveSummary.xls"), 5);
        CreateTemplateFile(Path.Combine(templatesPath, "SickLeaveSummary.xls"), 5);
    }

    private static void CreateTemplateFile(string filePath, int rows)
    {
        using var workbook = new HSSFWorkbook();
        var sheet = workbook.CreateSheet("sheet1");

        // Create rows with cells, ensuring row 2 and row 4 exist for templates that need them
        for (var i = 0; i < Math.Max(rows, 5); i++)
        {
            var row = sheet.CreateRow(i);
            for (var j = 0; j < 22; j++)
            {
                var cell = row.CreateCell(j);
                if (i == 0)
                {
                    cell.SetCellValue("Header");
                }
            }
            // Set row height for rows that might be accessed
            if (i is >= 2 and <= 4)
            {
                row.Height = 400;
            }
        }

        using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
        workbook.Write(fileStream);
    }
}