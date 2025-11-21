using System.Collections.Generic;
using System.Threading.Tasks;
using Fanex.Data;
using HRM.Core;
using HRM.Core.Services;
using HRM.Criteria.Group;
using HRM.Models.PayrollManagement;
using HRM.Services.Implement.PayrollManagement;
using NSubstitute;
using Xunit;

namespace HRM.Services.UnitTests.PayrollManagement;

public class CalculatePayrollServiceTests
{
    private readonly IBaseService _baseService;
    private readonly CalculatePayrollService _service;

    public CalculatePayrollServiceTests()
    {
        _baseService = Substitute.For<IBaseService>();
        _service = new CalculatePayrollService(_baseService);
    }

    [Fact]
    public async Task CalculateLocalMonthlySalary_WithSingleEmployee_ReturnsCorrectSalary()
    {
        // Arrange
        var empIds = new[] { 1 };
        var exchangeRates = new Dictionary<string, decimal> { { "USD", 1.0m } };
        var payrolls = new List<Payroll>
        {
            new()
            {
                EmpId = 1,
                EncryptBasicSalary = Cryptography.EncryptPassword("1000"),
                EncryptJobLevelAddOn = Cryptography.EncryptPassword("200"),
                EncryptSpecialSubsidy = Cryptography.EncryptPassword("100"),
                CurrencyCode = "USD",
            },
        };
        var allowances = new List<LocalPayrollAllowance>();

        _baseService
            .FetchMultipleAsync<Payroll, LocalPayrollAllowance>(Arg.Any<GetPayrollInfoCriteria>())
            .Returns((payrolls, allowances));

        // Act
        var result = await _service.CalculateLocalMonthlySalary(empIds, exchangeRates);

        // Assert
        Assert.Single(result);
        Assert.NotNull(result[1]);
        Assert.Equal(1300m, result[1].Total); // 1000 + 200 + 100
    }

    [Fact]
    public async Task CalculateLocalMonthlySalary_WithAllowances_IncludesAllowanceAmounts()
    {
        // Arrange
        var empIds = new[] { 1 };
        var exchangeRates = new Dictionary<string, decimal> { { "USD", 1.0m } };
        var payrolls = new List<Payroll>
        {
            new()
            {
                EmpId = 1,
                EncryptBasicSalary = Cryptography.EncryptPassword("1000"),
                EncryptJobLevelAddOn = Cryptography.EncryptPassword("200"),
                EncryptSpecialSubsidy = Cryptography.EncryptPassword("100"),
                CurrencyCode = "USD",
            },
        };
        var allowances = new List<LocalPayrollAllowance>
        {
            new()
            {
                EmpId = 1,
                EncryptAmount = Cryptography.EncryptPassword("300"),
                CurrencyCode = "USD",
            },
            new()
            {
                EmpId = 1,
                EncryptAmount = Cryptography.EncryptPassword("150"),
                CurrencyCode = "USD",
            },
        };

        _baseService
            .FetchMultipleAsync<Payroll, LocalPayrollAllowance>(Arg.Any<GetPayrollInfoCriteria>())
            .Returns((payrolls, allowances));

        // Act
        var result = await _service.CalculateLocalMonthlySalary(empIds, exchangeRates);

        // Assert
        Assert.Single(result);
        Assert.NotNull(result[1]);
        Assert.Equal(1750m, result[1].Total); // 1000 + 200 + 100 + 300 + 150
    }

    [Fact]
    public async Task CalculateLocalMonthlySalary_WithEmptyEncryptedValues_HandlesNullValues()
    {
        // Arrange
        var empIds = new[] { 1 };
        var exchangeRates = new Dictionary<string, decimal> { { "USD", 1.0m } };
        var payrolls = new List<Payroll>
        {
            new()
            {
                EmpId = 1,
                EncryptBasicSalary = Cryptography.EncryptPassword("1000"),
                EncryptJobLevelAddOn = null, // null value
                EncryptSpecialSubsidy = "", // empty value
                CurrencyCode = "USD",
            },
        };
        var allowances = new List<LocalPayrollAllowance>();

        _baseService
            .FetchMultipleAsync<Payroll, LocalPayrollAllowance>(Arg.Any<GetPayrollInfoCriteria>())
            .Returns((payrolls, allowances));

        // Act
        var result = await _service.CalculateLocalMonthlySalary(empIds, exchangeRates);

        // Assert
        Assert.Single(result);
        Assert.NotNull(result[1]);
        Assert.Equal(1000m, result[1].Total); // Only basic salary, null/empty values treated as 0
    }
}
