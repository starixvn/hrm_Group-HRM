using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GO.UserControl.Client.Core.Session;
using HRM.Core;
using HRM.Core.Services;
using HRM.Criteria.Report.Headcount;
using HRM.Models.Report.HeadcountReport;
using HRM.Services.Implement.Report.Headcount;
using NSubstitute;
using Xunit;

namespace HRM.Services.UnitTests.Report.Headcount;

public class CalculateHeadcountServiceTests
{
    private readonly IBaseService _mockBaseService;
    private readonly CalculateHeadcountService _service;

    public CalculateHeadcountServiceTests()
    {
        _mockBaseService = Substitute.For<IBaseService>();
        var _userControlSession = Substitute.For<IUserControlSession>();

        _service = new CalculateHeadcountService(_mockBaseService);
        _userControlSession.Get<float>(Arg.Any<string>()).Returns(7);
        DateTimeExtensions.Configure(_userControlSession);
    }

    [Fact]
    public async Task GetEmployeesInfos_ShouldReturnEmployeeList()
    {
        // Arrange
        var mockEmployees = CreateMockEmployeeInfos();
        _mockBaseService.GetDataAsync<EmployeeInfo>(Arg.Any<GetHeadcountReportCriteria>())
            .Returns(mockEmployees);
        var mockWFH = CreateMockEmployeeWFHs();
        _mockBaseService.GetDataAsync<EmployeeWFH>(Arg.Any<GetEmployeeWFHCriteria>())
            .Returns(mockWFH);

        // Act
        var result = await _service.GetReportAsync(2024);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(6, result.ReportByUnit.Count);
        Assert.Equal(3, result.ReportByUnit.Count(x=> !x.IsGrandTotal && !x.IsUnitCategory));
        Assert.Equal(5, result.ReportByCompany.Count);
        Assert.Equal(4, result.ReportByDetails.Count);
        Assert.Equal(13, result.ReportByUnitCountry.Count);
        Assert.Equal(7, result.ReportByUnitCountryLocal.Count);
        Assert.Equal(6, result.ReportByUnitCountryExpat.Count);
    }

    [Fact]
    public void Helper_AddUnitReport_ShouldAddValidReports()
    {
        // Arrange
        var list = new List<UnitReport>();
        var validReport = new UnitReport
        {
            Label1 = "Test Unit",
            Details = new List<ReportModel>
            {
                new() { Month = 1, NewHire = 5, CurrentHeadCount = 10 }
            }
        };

        // Act
        list.AddUnitReport(validReport);

        // Assert
        Assert.Single(list);
        Assert.Equal("Test Unit", list[0].Label1);
    }

    [Fact]
    public void Helper_GroupByAndOrderByKey_ShouldGroupAndOrderCorrectly()
    {
        // Arrange
        var employees = CreateMockEmployeeInfos();

        // Act
        var groupedByUnit = employees.GroupByAndOrderByKey(x => x.Unit).ToList();

        // Assert
        Assert.Equal(3, groupedByUnit.Count);
    }

    private static List<EmployeeInfo> CreateMockEmployeeInfos()
    {
        return
        [
            new EmployeeInfo
            {
                UserId = 1,
                EmpId = 1,
                Unit = "IT",
                UnitCategory = "Technology",
                Nickname = "John",
                Company = "Company A",
                Country = "Vietnam",
                CountryCode = "VN",
                JoinedDate = new DateTime(2024, 1, 15),
                ConfirmedDate = new DateTime(2024, 4, 15),
                ResignedDate = null,
                IsLocal = true
            },

            new EmployeeInfo
            {
                UserId = 2,
                EmpId = 2,
                Unit = "HR",
                UnitCategory = "Administration",
                Nickname = "Jane",
                Company = "Company B",
                Country = "Singapore",
                CountryCode = "SG",
                JoinedDate = new DateTime(2024, 2, 1),
                ConfirmedDate = new DateTime(2024, 5, 1),
                ResignedDate = new DateTime(2024, 6, 30),
                IsLocal = false
            },

            new EmployeeInfo
            {
                UserId = 3,
                EmpId = 3,
                Unit = "Finance",
                UnitCategory = "Administration",
                Nickname = "Bob",
                Company = "Company A",
                Country = "Vietnam",
                CountryCode = "VN",
                JoinedDate = new DateTime(2023, 12, 1),
                ConfirmedDate = new DateTime(2024, 3, 1),
                ResignedDate = null,
                IsLocal = true
            }
        ];
    }

    private static List<EmployeeWFH> CreateMockEmployeeWFHs()
    {
        return
        [
            new EmployeeWFH
            {
                EmpId = 1,
                FromDate = new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero),
                ToDate = new DateTimeOffset(2024, 12, 31, 0, 0, 0, TimeSpan.Zero)
            },

            new EmployeeWFH
            {
                EmpId = 2,
                FromDate = new DateTimeOffset(2024, 3, 1, 0, 0, 0, TimeSpan.Zero),
                ToDate = new DateTimeOffset(2024, 6, 30, 0, 0, 0, TimeSpan.Zero)
            }
        ];
    }
}