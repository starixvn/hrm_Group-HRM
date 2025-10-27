using System.Collections.Generic;
using System.Threading.Tasks;
using Fanex.Data;
using HRM.Core.Services;
using HRM.Criteria.Group;
using HRM.Models.PayrollManagement;
using HRM.Services.Implement.PayrollManagement;
using NSubstitute;
using Xunit;

namespace HRM.Services.UnitTests.PayrollManagement;

public class LocalPayrollServiceTests
{
    private readonly IBaseService _baseService;
    private readonly LocalPayrollService _service;

    public LocalPayrollServiceTests()
    {
        _baseService = Substitute.For<IBaseService>();
        _service = new LocalPayrollService(_baseService);
    }

    [Fact]
    public async Task CalculateMonthlySalary_WithSingleEmployee_ReturnsCorrectSalary()
    {
        // Arrange
        var empIds = new[] { 1 };
        var payrolls = new List<Payroll>
        {
            new()
            {
                EmpId = 1,
                EncryptBasicSalary = Cryptography.EncryptPassword("1000"),
                EncryptJobLevelAddOn = Cryptography.EncryptPassword("200"),
                EncryptSpecialSubsidy = Cryptography.EncryptPassword("100")
            }
        };
        var allowances = new List<LocalPayrollAllowance>();

        _baseService.FetchMultipleAsync<Payroll, LocalPayrollAllowance>(Arg.Any<GetPayrollInfoCriteria>())
            .Returns((payrolls, allowances));

        // Act
        var result = await _service.CalculateMonthlySalary(empIds);

        // Assert
        Assert.Single(result);
        Assert.Equal(1300m, result[1]); // 1000 + 200 + 100
    }

    [Fact]
    public async Task CalculateMonthlySalary_WithAllowances_IncludesAllowanceAmounts()
    {
        // Arrange
        var empIds = new[] { 1 };
        var payrolls = new List<Payroll>
        {
            new()
            {
                EmpId = 1,
                EncryptBasicSalary = Cryptography.EncryptPassword("1000"),
                EncryptJobLevelAddOn = Cryptography.EncryptPassword("200"),
                EncryptSpecialSubsidy = Cryptography.EncryptPassword("100")
            }
        };
        var allowances = new List<LocalPayrollAllowance>
        {
            new() { EmpId = 1, EncryptAmount = Cryptography.EncryptPassword("300") },
            new() { EmpId = 1, EncryptAmount = Cryptography.EncryptPassword("150") }
        };

        _baseService.FetchMultipleAsync<Payroll, LocalPayrollAllowance>(Arg.Any<GetPayrollInfoCriteria>())
            .Returns((payrolls, allowances));

        // Act
        var result = await _service.CalculateMonthlySalary(empIds);

        // Assert
        Assert.Single(result);
        Assert.Equal(1750m, result[1]); // 1000 + 200 + 100 + 300 + 150
    }

    [Fact]
    public async Task CalculateMonthlySalary_WithEmptyEncryptedValues_HandlesNullValues()
    {
        // Arrange
        var empIds = new[] { 1 };
        var payrolls = new List<Payroll>
        {
            new()
            {
                EmpId = 1,
                EncryptBasicSalary = Cryptography.EncryptPassword("1000"),
                EncryptJobLevelAddOn = null, // null value
                EncryptSpecialSubsidy = "" // empty value
            }
        };
        var allowances = new List<LocalPayrollAllowance>();

        _baseService.FetchMultipleAsync<Payroll, LocalPayrollAllowance>(Arg.Any<GetPayrollInfoCriteria>())
            .Returns((payrolls, allowances));

        // Act
        var result = await _service.CalculateMonthlySalary(empIds);

        // Assert
        Assert.Single(result);
        Assert.Equal(1000m, result[1]); // Only basic salary, null/empty values treated as 0
    }
}