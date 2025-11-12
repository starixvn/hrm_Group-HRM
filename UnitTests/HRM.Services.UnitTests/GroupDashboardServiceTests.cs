using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HRM.Models.Group;
using HRM.Models.Report.HeadcountReport;
using HRM.Services.Implement;
using HRM.Services.Interfaces;
using HRM.Services.Interfaces.Report.Headcount;
using NSubstitute;
using Xunit;

namespace HRM.Services.UnitTests;

public class GroupDashboardServiceTests
{
    private readonly IHeadcountReportService _mockHeadcountReportService;
    private readonly IMonthlyPayrollService _mockPayrollService;
    private readonly IEmployeeService _mockEmployeeService;
    private readonly GroupDashboardService _service;

    public GroupDashboardServiceTests()
    {
        _mockHeadcountReportService = Substitute.For<IHeadcountReportService>();
        _mockPayrollService = Substitute.For<IMonthlyPayrollService>();
        _mockEmployeeService = Substitute.For<IEmployeeService>();
        _service = new GroupDashboardService(
            _mockHeadcountReportService,
            _mockPayrollService,
            _mockEmployeeService);
    }

    [Fact]
    public async Task GetDashboardDataAsync_WithValidData_ReturnsCompleteDashboard()
    {
        // Arrange
        const int year = 2024;
        var reportData = CreateMockHeadcountReport();
        var employees = CreateMockEmployees();
        
        _mockHeadcountReportService.GetReportAsync(year).Returns(reportData);
        _mockEmployeeService.GetEmployeesInGroupAsync().Returns(employees);
        _mockEmployeeService.CalculateLengthOfService(
            Arg.Any<DateTime?>(), 
            Arg.Any<DateTime?>(), 
            Arg.Any<DateTime?>())
            .Returns((5.5, "5 years 6 months"));

        // Act
        var result = await _service.GetDashboardDataAsync(year);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(10, result.CurrentHeadcount);
        Assert.Equal(2, result.Recruitment);
        Assert.Equal(1, result.ProbationResign);
        Assert.Equal(1, result.ConfirmedResign);
        Assert.Equal(2, result.Companies);
        Assert.True(result.AvgAge > 0);
        Assert.True(result.AvgYrsTenure >= 0);
        Assert.NotNull(result.HeadcountByBU);
        Assert.NotNull(result.HeadcountByCountry);
        Assert.NotNull(result.BuCategory);
        Assert.NotNull(result.EmployeeByAge);
        Assert.NotNull(result.SalaryExpenseByBU);
        Assert.NotNull(result.SalaryExpenseByBUCategory);
        
        await _mockHeadcountReportService.Received(1).GetReportAsync(year);
        await _mockEmployeeService.Received(1).GetEmployeesInGroupAsync();
    }

    [Fact]
    public async Task GetDashboardDataAsync_WithNullGrandTotal_HandlesGracefully()
    {
        // Arrange
        const int year = 2024;
        var reportData = new HeadcountReportModel
        {
            ReportByUnit = [new UnitReport { IsGrandTotal = false, Label1 = "BU1" }],
            ReportByCompany = [],
            ReportByUnitCountry = []
        };
        
        _mockHeadcountReportService.GetReportAsync(year).Returns(reportData);
        _mockEmployeeService.GetEmployeesInGroupAsync().Returns(new List<GroupEmployeeModel>());

        // Act
        var result = await _service.GetDashboardDataAsync(year);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(0, result.CurrentHeadcount);
        Assert.Equal(0, result.Recruitment);
        Assert.Equal(0, result.AvgYrsTenure);
        Assert.Equal(0, result.AvgAge);
    }

    [Fact]
    public async Task GetDashboardDataAsync_CalculatesSalaryExpenseCorrectly()
    {
        // Arrange
        const int year = 2024;
        var reportData = CreateMockHeadcountReportWithSalary();
        var employees = CreateMockEmployees();
        
        _mockHeadcountReportService.GetReportAsync(year).Returns(reportData);
        _mockEmployeeService.GetEmployeesInGroupAsync().Returns(employees);
        _mockEmployeeService.CalculateLengthOfService(
            Arg.Any<DateTime?>(), 
            Arg.Any<DateTime?>(), 
            Arg.Any<DateTime?>())
            .Returns((3.0, "3 years 0 months"));

        // Act
        var result = await _service.GetDashboardDataAsync(year);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.SalaryExpenseByBU);
        Assert.NotNull(result.SalaryExpenseByBUCategory);
        
        await _mockPayrollService.Received().CalculateMonthlySalary(Arg.Any<IEnumerable<GroupEmployeeModel>>());
    }

    [Fact]
    public async Task GetDashboardDataAsync_WithEmployeesWithoutDOB_SkipsAgeCalculation()
    {
        // Arrange
        const int year = 2024;
        var reportData = CreateMockHeadcountReport();
        var employeesWithoutDOB = new List<GroupEmployeeModel>
        {
            new()
            {
                EmpId = 1,
                DOB = DateTime.MinValue,
                JoinedDate = new DateTime(2020, 1, 1)
            }
        };
        
        _mockHeadcountReportService.GetReportAsync(year).Returns(reportData);
        _mockEmployeeService.GetEmployeesInGroupAsync().Returns(employeesWithoutDOB);
        _mockEmployeeService.CalculateLengthOfService(
            Arg.Any<DateTime?>(), 
            Arg.Any<DateTime?>(), 
            Arg.Any<DateTime?>())
            .Returns((2.0, "2 years 0 months"));

        // Act
        var result = await _service.GetDashboardDataAsync(year);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(0, result.AvgAge);
        Assert.Empty(result.EmployeeByAge);
    }

    [Fact]
    public async Task GetDashboardDataAsync_WithMultipleBUs_CalculatesCorrectly()
    {
        // Arrange
        const int year = 2024;
        var reportData = new HeadcountReportModel
        {
            ReportByUnit =
            [
                new UnitReport
                {
                    IsGrandTotal = true,
                    Label1 = "Grand Total",
                    Details = new List<ReportModel>
                    {
                        new()
                        {
                            CurrentHeadCountEmpIds = new List<int> { 1, 2, 3 },
                            CurrentHeadCountEmpInfo = CreateMockEmployeeInfos(3),
                            NewHireEmpIds = new List<int> { 1 },
                            ProbationResignEmpIds = new List<int> { 2 },
                            ConfirmedResignEmpIds = new List<int> { 3 }
                        }
                    }
                },

                new UnitReport
                {
                    IsGrandTotal = false,
                    IsUnitCategory = false,
                    Label1 = "IT",
                    Details = new List<ReportModel>
                    {
                        new()
                        {
                            CurrentHeadCountEmpIds = new List<int> { 1, 2 },
                            CurrentHeadCountEmpInfo = CreateMockEmployeeInfos(2)
                        }
                    }
                },

                new UnitReport
                {
                    IsGrandTotal = false,
                    IsUnitCategory = false,
                    Label1 = "HR",
                    Details = new List<ReportModel>
                    {
                        new()
                        {
                            CurrentHeadCountEmpIds = new List<int> { 3 },
                            CurrentHeadCountEmpInfo = CreateMockEmployeeInfos(1)
                        }
                    }
                }
            ],
            ReportByCompany =
            [
                new UnitReport { IsGrandTotal = false, IsUnitCategory = false, Label1 = "Company A" },
                new UnitReport { IsGrandTotal = false, IsUnitCategory = false, Label1 = "Company B" }
            ],
            ReportByUnitCountry =
            [
                new UnitReport
                {
                    IsGrandTotal = true,
                    Label3 = "Vietnam",
                    Details = new List<ReportModel>
                    {
                        new() { CurrentHeadCountEmpIds = new List<int> { 1, 2, 3 } }
                    }
                }
            ]
        };
        
        var employees = CreateMockEmployees();
        _mockHeadcountReportService.GetReportAsync(year).Returns(reportData);
        _mockEmployeeService.GetEmployeesInGroupAsync().Returns(employees);
        _mockEmployeeService.CalculateLengthOfService(
            Arg.Any<DateTime?>(), 
            Arg.Any<DateTime?>(), 
            Arg.Any<DateTime?>())
            .Returns((4.0, "4 years 0 months"));

        // Act
        var result = await _service.GetDashboardDataAsync(year);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.CurrentHeadcount);
        Assert.Equal(2, result.Companies);
        Assert.Equal(2, result.HeadcountByBU.Count());
        Assert.Single(result.HeadcountByCountry);
    }

    [Fact]
    public async Task GetDashboardDataAsync_WithBUCategory_GroupsCorrectly()
    {
        // Arrange
        const int year = 2024;
        var reportData = new HeadcountReportModel
        {
            ReportByUnit =
            [
                new UnitReport
                {
                    IsGrandTotal = true,
                    Details = new List<ReportModel>
                    {
                        new() { CurrentHeadCountEmpIds = new List<int> { 1, 2 } }
                    }
                },

                new UnitReport
                {
                    IsGrandTotal = false,
                    IsUnitCategory = true,
                    Label1 = "Technology TOTAL (2)",
                    BUCount = 2,
                    Details = new List<ReportModel>
                    {
                        new()
                        {
                            CurrentHeadCountEmpIds = new List<int> { 1 },
                            CurrentHeadCountEmpInfo = CreateMockEmployeeInfos(1)
                        }
                    }
                },

                new UnitReport
                {
                    IsGrandTotal = false,
                    IsUnitCategory = true,
                    Label1 = "Technology TOTAL (1)",
                    BUCount = 1,
                    Details = new List<ReportModel>
                    {
                        new()
                        {
                            CurrentHeadCountEmpIds = new List<int> { 2 },
                            CurrentHeadCountEmpInfo = CreateMockEmployeeInfos(1)
                        }
                    }
                }
            ],
            ReportByCompany = [],
            ReportByUnitCountry = []
        };
        
        _mockHeadcountReportService.GetReportAsync(year).Returns(reportData);
        _mockEmployeeService.GetEmployeesInGroupAsync().Returns(CreateMockEmployees());
        _mockEmployeeService.CalculateLengthOfService(
            Arg.Any<DateTime?>(), 
            Arg.Any<DateTime?>(), 
            Arg.Any<DateTime?>())
            .Returns((2.0, "2 years 0 months"));

        // Act
        var result = await _service.GetDashboardDataAsync(year);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.BuCategory);
        var category = result.BuCategory.First();
        Assert.Equal("Technology", category.Category);
        Assert.Equal(3, category.BUCount);
    }

    private static HeadcountReportModel CreateMockHeadcountReport()
    {
        var lastMonthReport = new ReportModel
        {
            CurrentHeadCountEmpIds = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 },
            CurrentHeadCountEmpInfo = CreateMockEmployeeInfos(10),
            NewHireEmpIds = new List<int> { 1, 2 },
            ProbationResignEmpIds = new List<int> { 3 },
            ConfirmedResignEmpIds = new List<int> { 4 }
        };

        return new HeadcountReportModel
        {
            ReportByUnit =
            [
                new UnitReport
                {
                    IsGrandTotal = true,
                    Label1 = "Grand Total",
                    Details = new List<ReportModel> { lastMonthReport }
                },

                new UnitReport
                {
                    IsGrandTotal = false,
                    IsUnitCategory = false,
                    Label1 = "IT",
                    Details = new List<ReportModel>
                    {
                        new() { CurrentHeadCountEmpIds = new List<int> { 1, 2, 3 } }
                    }
                },

                new UnitReport
                {
                    IsGrandTotal = false,
                    IsUnitCategory = false,
                    Label1 = "HR",
                    Details = new List<ReportModel>
                    {
                        new() { CurrentHeadCountEmpIds = new List<int> { 4, 5 } }
                    }
                }
            ],
            ReportByCompany =
            [
                new UnitReport { IsGrandTotal = false, IsUnitCategory = false, Label1 = "Company A" },
                new UnitReport { IsGrandTotal = false, IsUnitCategory = false, Label1 = "Company B" }
            ],
            ReportByUnitCountry =
            [
                new UnitReport
                {
                    IsGrandTotal = true,
                    Label3 = "Vietnam",
                    Details = new List<ReportModel>
                    {
                        new() { CurrentHeadCountEmpIds = new List<int> { 1, 2, 3, 4, 5 } }
                    }
                }
            ]
        };
    }

    private static HeadcountReportModel CreateMockHeadcountReportWithSalary()
    {
        var employeesWithSalary = CreateMockEmployeeInfos(3);
        foreach (var emp in employeesWithSalary)
        {
            emp.Salary = 1000m;
        }

        var lastMonthReport = new ReportModel
        {
            CurrentHeadCountEmpIds = new List<int> { 1, 2, 3 },
            CurrentHeadCountEmpInfo = employeesWithSalary
        };

        return new HeadcountReportModel
        {
            ReportByUnit =
            [
                new UnitReport
                {
                    IsGrandTotal = true,
                    Details = new List<ReportModel> { lastMonthReport }
                },

                new UnitReport
                {
                    IsGrandTotal = false,
                    IsUnitCategory = false,
                    Label1 = "IT",
                    Details = new List<ReportModel>
                    {
                        new()
                        {
                            CurrentHeadCountEmpInfo = employeesWithSalary.Take(2).ToList()
                        }
                    }
                }
            ],
            ReportByCompany = [],
            ReportByUnitCountry = []
        };
    }

    private static List<GroupEmployeeModel> CreateMockEmployees()
    {
        var baseDate = new DateTime(1990, 1, 1);
        return
        [
            new GroupEmployeeModel
            {
                EmpId = 1,
                DOB = baseDate.AddYears(-30),
                JoinedDate = new DateTime(2020, 1, 1),
                MilitaryFromDate = null,
                MilitaryToDate = null
            },

            new GroupEmployeeModel
            {
                EmpId = 2,
                DOB = baseDate.AddYears(-25),
                JoinedDate = new DateTime(2021, 1, 1),
                MilitaryFromDate = null,
                MilitaryToDate = null
            },

            new GroupEmployeeModel
            {
                EmpId = 3,
                DOB = baseDate.AddYears(-35),
                JoinedDate = new DateTime(2019, 1, 1),
                MilitaryFromDate = new DateTime(2015, 1, 1),
                MilitaryToDate = new DateTime(2017, 1, 1)
            }
        ];
    }

    private static List<GroupEmployeeModel> CreateMockEmployeeInfos(int count)
    {
        var employees = new List<GroupEmployeeModel>();
        var baseDate = new DateTime(1990, 1, 1);
        
        for (var i = 1; i <= count; i++)
        {
            employees.Add(new GroupEmployeeModel
            {
                EmpId = i,
                DOB = baseDate.AddYears(-30 + i),
                JoinedDate = new DateTime(2020, 1, 1),
                Salary = 1000m * i
            });
        }
        
        return employees;
    }
}

