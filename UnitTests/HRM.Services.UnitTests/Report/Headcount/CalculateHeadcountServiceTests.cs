using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GO.UserControl.Client.Core.Session;
using HRM.Core;
using HRM.Models.Report.HeadcountReport;
using HRM.Repositories.HeadcountReport;
using HRM.Services.Implement.Report.Headcount;
using NSubstitute;
using Xunit;

namespace HRM.Services.UnitTests.Report.Headcount;

public class CalculateHeadcountServiceTests
{
    private readonly IHeadcountReportRepository _mockDataProvider;
    private readonly CalculateHeadcountService _service;

    public CalculateHeadcountServiceTests()
    {
        _mockDataProvider = Substitute.For<IHeadcountReportRepository>();
        _service = new CalculateHeadcountService(_mockDataProvider);
        
        // Setup session for DateTimeExtensions
        var userControlSession = Substitute.For<IUserControlSession>();
        userControlSession.Get<float>(Arg.Any<string>()).Returns(7);
        DateTimeExtensions.Configure(userControlSession);
    }

    [Fact]
    public async Task GetReportAsync_WithValidYear_ReturnsCompleteReport()
    {
        // Arrange
        var mockData = CreateMockHeadcountData();
        _mockDataProvider.GetHeadcountDataAsync(2024).Returns(mockData);

        // Act
        var result = await _service.GetReportAsync(2024);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.ReportByUnit);
        Assert.NotNull(result.ReportByCompany);
        Assert.NotNull(result.ReportByDetails);
        Assert.NotNull(result.ReportByUnitCountry);
        Assert.NotNull(result.ReportByUnitCountryLocal);
        Assert.NotNull(result.ReportByUnitCountryExpat);
        
        // Verify data provider was called
        await _mockDataProvider.Received(1).GetHeadcountDataAsync(2024);
    }

    [Fact]
    public async Task GetReportAsync_WithDifferentYear_CallsDataProviderWithCorrectYear()
    {
        // Arrange
        var mockData = CreateMockHeadcountData();
        _mockDataProvider.GetHeadcountDataAsync(2023).Returns(mockData);

        // Act
        await _service.GetReportAsync(2023);

        // Assert
        await _mockDataProvider.Received(1).GetHeadcountDataAsync(2023);
    }

    [Fact]
    public void UnitReportCalculator_Calculate_ReturnsUnitReports()
    {
        // Arrange
        var data = CreateMockHeadcountData();
        var calculator = new UnitReportCalculator(data);

        // Act
        var result = calculator.Calculate();

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.Contains(result, r => r.Label1.Contains("Technology"));
        Assert.Contains(result, r => r.Label1.Contains("Administration"));
        Assert.Contains(result, r => r.IsGrandTotal);
    }

    [Fact]
    public void CompanyReportCalculator_Calculate_ReturnsCompanyReports()
    {
        // Arrange
        var data = CreateMockHeadcountData();
        var calculator = new CompanyReportCalculator(data);

        // Act
        var result = calculator.Calculate();

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.Contains(result, r => r.Label1 == "Vietnam");
        Assert.Contains(result, r => r.Label1 == "Singapore");
        Assert.Contains(result, r => r.IsGrandTotal);
    }

    [Fact]
    public void DetailsReportCalculator_Calculate_ReturnsDetailedReports()
    {
        // Arrange
        var data = CreateMockHeadcountData();
        var calculator = new DetailsReportCalculator(data);

        // Act
        var result = calculator.Calculate();

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.Contains(result, r => r.Label1 == "IT");
        Assert.Contains(result, r => r.Label1 == "HR");
    }

    [Fact]
    public void UnitCountryReportCalculator_Calculate_WithDifferentParameters_ReturnsDifferentResults()
    {
        // Arrange
        var data = CreateMockHeadcountData();
        var calculator = new UnitCountryReportCalculator(data);

        // Act
        var resultAll = calculator.Calculate(null);
        var resultLocal = calculator.Calculate(true);
        var resultExpat = calculator.Calculate(false);

        // Assert
        Assert.NotNull(resultAll);
        Assert.NotNull(resultLocal);
        Assert.NotNull(resultExpat);
        
        // Results should be different based on isLocal parameter
        Assert.NotEqual(resultAll.Count, resultLocal.Count);
        Assert.NotEqual(resultAll.Count, resultExpat.Count);
    }

    private static HeadcountData CreateMockHeadcountData()
    {
        return new HeadcountData
        {
            Year = 2024,
            EmployeeInfos = CreateMockEmployeeInfos(),
            AllEmployeeWFHs = CreateMockEmployeeWFHs(),
        };
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