using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fanex.Data;
using GO.UserControl.Client.Core.Session;
using HRM.Core.Constants;
using HRM.Core.Enums;
using HRM.Core.Models;
using HRM.Core.Services;
using HRM.Criteria.PayrollManagement;
using HRM.Models.Payroll;
using HRM.Models.PayrollManagement;
using HRM.Services.Helper;
using HRM.Services.Implement.PayrollManagement;
using HRM.Services.Interfaces;
using HRM.ViewModels.SalaryStructure;
using Newtonsoft.Json;
using NSubstitute;
using Xunit;

namespace HRM.Services.UnitTests;

public class SMSPayrollManagementServiceTests
{
    private readonly IBaseService _baseService;
    private readonly IUserControlSession _session;
    private readonly ISalaryStructureService _salaryStructureService;
    private readonly IExchangeRateService _exchangeRateService;
    private readonly SMSPayrollManagementService _service;
    private const short TestCompanyId = 1;

    public SMSPayrollManagementServiceTests()
    {
        _baseService = Substitute.For<IBaseService>();
        _session = Substitute.For<IUserControlSession>();
        _salaryStructureService = Substitute.For<ISalaryStructureService>();
        _exchangeRateService = Substitute.For<IExchangeRateService>();
        _session.Get<short>(SessionKey.CurrentCompanyId).Returns(TestCompanyId);
        
        SetupDefaultMocks();
        
        _service = new SMSPayrollManagementService(_baseService, _session, _salaryStructureService, _exchangeRateService);
    }

    private void SetupDefaultMocks()
    {
        var defaultConfig = new BasicSalaryConfigViewModel
        {
            Config = "1",
            ConfigOptions = new List<BasicSalaryConfigOption>
            {
                new() { Value = "1", Label = "Basic Salary" },
                new() { Value = "2", Label = "Job Level Add-On" },
                new() { Value = "3", Label = "Special Subsidy" },
                new() { Value = "4", Label = "Management Allowance" }
            }
        };
        
        _salaryStructureService.GetBasicSalaryConfigAsync(Arg.Any<short>())
            .Returns(defaultConfig);
        
        _exchangeRateService.GetEffectiveExchangeRatesDictAsync()
            .Returns(new Dictionary<string, decimal> { { "USD", 1m }, { "VND", 0.00004m } });
        
        _exchangeRateService.GetRoundingRulesDictAsync()
            .Returns(new Dictionary<string, int> { { "USD", 2 }, { "VND", 0 } });
    }

    #region GetPayrollByMonthAsync Tests

    [Fact]
    public async Task GetPayrollByMonthAsync_ShouldCallBaseServiceWithCorrectCriteria()
    {
        // Arrange
        var criteria = new GetPayrollByMonthCriteria
        {
            CompanyId = TestCompanyId,
            Month = DateTime.Now,
            IsLocal = true
        };
        var expectedResult = new List<GeneratedPayrollDetail>();

        _baseService.GetDataAsync<GeneratedPayrollDetail>(criteria).Returns(expectedResult);

        // Act
        var result = await _service.GetPayrollByMonthAsync(criteria);

        // Assert
        await _baseService.Received(1).GetDataAsync<GeneratedPayrollDetail>(criteria);
        Assert.Equal(expectedResult, result);
    }

    #endregion

    #region GeneratePayroll Tests

    [Fact]
    public async Task GeneratePayroll_ShouldReturnSuccessfulCommandResult()
    {
        // Arrange
        var date = DateTime.Now;
        var employeeAttendants = new List<PayrollEmployeeAttendant> { CreateEmployeeAttendant() };
        var expectedCommandResult = new CommandResult { Code = 0 };

        _baseService.GetDataAsync<PayrollEmployeeAttendant>(Arg.Any<GetPayrollAttendantEmployeeCriteria>())
            .Returns(employeeAttendants);
        _baseService.ExecuteNonQueryAsync(Arg.Any<CalculatePayrollCriteria>())
            .Returns(expectedCommandResult);

        // Act
        var result = await _service.GeneratePayroll(date, 424);

        // Assert
        Assert.Equal(expectedCommandResult, result);
        await _baseService.Received(1).ExecuteNonQueryAsync(Arg.Is<CalculatePayrollCriteria>(c =>
            c.CompanyId == TestCompanyId &&
            c.Date == date &&
            !string.IsNullOrEmpty(c.PayrollListJson)));
    }

    [Fact]
    public async Task GeneratePayroll_ShouldProcessAllEmployeeAttendants()
    {
        // Arrange
        var date = DateTime.Now;
        var employeeAttendants = new List<PayrollEmployeeAttendant> 
        { 
            CreateEmployeeAttendant(),
            CreateEmployeeAttendant(basicSalary: 2000)
        };
        var expectedCommandResult = new CommandResult { Code = 0 };

        _baseService.GetDataAsync<PayrollEmployeeAttendant>(Arg.Any<GetPayrollAttendantEmployeeCriteria>())
            .Returns(employeeAttendants);
        _baseService.ExecuteNonQueryAsync(Arg.Any<CalculatePayrollCriteria>())
            .Returns(expectedCommandResult);

        // Act
        await _service.GeneratePayroll(date, 424);

        // Assert
        await _baseService.Received(1).ExecuteNonQueryAsync(Arg.Is<CalculatePayrollCriteria>(c =>
            JsonConvert.DeserializeObject<List<PayrollEmployeeResult>>(c.PayrollListJson).Count == employeeAttendants.Count));
    }

    [Fact]
    public async Task GeneratePayroll_ShouldUseCorrectCompanyIdFromSession()
    {
        // Arrange
        var date = DateTime.Now;
        var employeeAttendants = new List<PayrollEmployeeAttendant>();
        var expectedCommandResult = new CommandResult { Code = 0 };

        _baseService.GetDataAsync<PayrollEmployeeAttendant>(Arg.Any<GetPayrollAttendantEmployeeCriteria>())
            .Returns(employeeAttendants);
        _baseService.ExecuteNonQueryAsync(Arg.Any<CalculatePayrollCriteria>())
            .Returns(expectedCommandResult);

        // Act
        await _service.GeneratePayroll(date, 424);

        // Assert
        await _baseService.Received(1).GetDataAsync<PayrollEmployeeAttendant>(Arg.Is<GetPayrollAttendantEmployeeCriteria>(c =>
            c.CompanyId == TestCompanyId &&
            c.Date == date));
    }

    #endregion

    #region Payroll Calculation Logic Tests

    [Fact]
    public async Task GeneratePayroll_ShouldCalculateCorrectGrossSalary_WithBasicSalaryOnly()
    {
        // Arrange
        var date = DateTime.Now;
        var employeeAttendant = CreateEmployeeAttendant(
            basicSalary: 1000,
            jobLevelAddOn: 0,
            specialSubsidy: 0,
            isNewHire: false
        );

        var employeeAttendants = new List<PayrollEmployeeAttendant> { employeeAttendant };
        SetupMocks(employeeAttendants);

        // Act
        await _service.GeneratePayroll(date, 424);

        // Assert
        var result = GetPayrollResult();
        Assert.Equal(1000, PayrollHelper.DecryptSalary(result.GrossSalary), 0);
        Assert.Equal(EmploymentStatus.Normal, result.EmploymentStatus);
    }

    [Fact]
    public async Task GeneratePayroll_ShouldCalculateCorrectGrossSalary_WithMultipleConfigOptions()
    {
        // Arrange
        var date = DateTime.Now;
        _salaryStructureService.GetBasicSalaryConfigAsync(Arg.Any<short>())
            .Returns(new BasicSalaryConfigViewModel
            {
                Config = "1,2,3",
                ConfigOptions = new List<BasicSalaryConfigOption>
                {
                    new() { Value = "1", Label = "Basic Salary" },
                    new() { Value = "2", Label = "Job Level Add-On" },
                    new() { Value = "3", Label = "Special Subsidy" }
                }
            });

        var employeeAttendant = CreateEmployeeAttendant(
            basicSalary: 1000,
            jobLevelAddOn: 200,
            specialSubsidy: 300,
            isNewHire: false
        );

        var employeeAttendants = new List<PayrollEmployeeAttendant> { employeeAttendant };
        SetupMocks(employeeAttendants);

        // Act
        await _service.GeneratePayroll(date, 424);

        // Assert
        var result = GetPayrollResult();
        Assert.Equal(1500, PayrollHelper.DecryptSalary(result.GrossSalary), 0);
    }

    [Fact]
    public async Task GeneratePayroll_ShouldCalculateSpecialSubsidy_WithCurrencyConversion()
    {
        // Arrange
        var date = DateTime.Now;
        _exchangeRateService.GetEffectiveExchangeRatesDictAsync()
            .Returns(new Dictionary<string, decimal> 
            { 
                { "USD", 1m }, 
                { "VND", 0.00004m } // 1 VND = 0.00004 USD
            });

        var employeeAttendant = CreateEmployeeAttendant(
            basicSalary: 0,
            jobLevelAddOn: 0,
            specialSubsidy: 1000000, // 1,000,000 VND
            specialSubsidyCurrency: "VND",
            currencyCode: "USD",
            isNewHire: false
        );

        _salaryStructureService.GetBasicSalaryConfigAsync(Arg.Any<short>())
            .Returns(new BasicSalaryConfigViewModel
            {
                Config = "3",
                ConfigOptions = new List<BasicSalaryConfigOption>
                {
                    new() { Value = "3", Label = "Special Subsidy" }
                }
            });

        var employeeAttendants = new List<PayrollEmployeeAttendant> { employeeAttendant };
        SetupMocks(employeeAttendants);

        // Act
        await _service.GeneratePayroll(date, 424);

        // Assert
        var result = GetPayrollResult();
        // 1,000,000 VND * 0.00004 = 40 USD
        Assert.Equal(40, PayrollHelper.DecryptSalary(result.GrossSalary), 0);
    }

    [Fact]
    public async Task GeneratePayroll_ShouldCalculateNewHireSalary_WithProbationAndConfirmedDays()
    {
        // Arrange
        var date = DateTime.Now;
        var employeeAttendant = CreateEmployeeAttendant(
            basicSalary: 1000,
            jobLevelAddOn: 0,
            specialSubsidy: 0,
            workingDays: 22,
            totalWorkingDays: 10,
            newHireDays: 20,
            isNewHire: true
        );

        var employeeAttendants = new List<PayrollEmployeeAttendant> { employeeAttendant };
        SetupMocks(employeeAttendants);

        // Act
        await _service.GeneratePayroll(date, 424);

        // Assert
        var result = GetPayrollResult();
        // New hire: 20/22*1000*0.85 + 10/22*1000 = 773 + 455 = 1228
        var expectedSalary = (20m / 22m * 1000m * 0.85m) + (10m / 22m * 1000m);
        Assert.Equal(expectedSalary, PayrollHelper.DecryptSalary(result.GrossSalary), 0);
        Assert.Equal(EmploymentStatus.Confirmed, result.EmploymentStatus);
        Assert.Equal(30, result.PaidDays); // TotalWorkingDays + TotalNewHireWorkingDays
    }

    [Fact]
    public async Task GeneratePayroll_ShouldCalculateNewHireSalary_WithProbationOnly()
    {
        // Arrange
        var date = DateTime.Now;
        var employeeAttendant = CreateEmployeeAttendant(
            basicSalary: 1000,
            jobLevelAddOn: 0,
            specialSubsidy: 0,
            workingDays: 22,
            totalWorkingDays: 0,
            newHireDays: 20,
            isNewHire: true
        );

        var employeeAttendants = new List<PayrollEmployeeAttendant> { employeeAttendant };
        SetupMocks(employeeAttendants);

        // Act
        await _service.GeneratePayroll(date, 424);

        // Assert
        var result = GetPayrollResult();
        // New hire probation only: 20/22*1000*0.85 = 773
        var expectedSalary = 20m / 22m * 1000m * 0.85m;
        Assert.Equal(expectedSalary, PayrollHelper.DecryptSalary(result.GrossSalary), 0);
        Assert.Equal(EmploymentStatus.Probation, result.EmploymentStatus);
    }

    [Fact]
    public async Task GeneratePayroll_ShouldCalculatePaidDays_ForNormalEmployee()
    {
        // Arrange
        var date = DateTime.Now;
        var employeeAttendant = CreateEmployeeAttendant(
            basicSalary: 1000,
            workingDays: 22,
            totalWorkingDays: 20,
            unpaidLeaveDays: 5,
            isNewHire: false
        );

        var employeeAttendants = new List<PayrollEmployeeAttendant> { employeeAttendant };
        SetupMocks(employeeAttendants);

        // Act
        await _service.GeneratePayroll(date, 424);

        // Assert
        var result = GetPayrollResult();
        Assert.Equal(17, result.PaidDays); // 22 - 5 = 17
    }

    [Fact]
    public async Task GeneratePayroll_ShouldCalculatePaidDays_ForResignedEmployee()
    {
        // Arrange
        var date = DateTime.Now;
        var employeeAttendant = CreateEmployeeAttendant(
            basicSalary: 1000,
            workingDays: 22,
            totalWorkingDays: 14,
            unpaidLeaveDays: 2,
            isResigned: true,
            isNewHire: false
        );

        var employeeAttendants = new List<PayrollEmployeeAttendant> { employeeAttendant };
        SetupMocks(employeeAttendants);

        // Act
        await _service.GeneratePayroll(date, 424);

        // Assert
        var result = GetPayrollResult();
        Assert.Equal(14, result.PaidDays); // TotalWorkingDays for resigned
        Assert.Equal(EmploymentStatus.Resignation, result.EmploymentStatus);
    }

    [Fact]
    public async Task GeneratePayroll_ShouldCalculatePaidDays_ForMaternityEmployee()
    {
        // Arrange
        var date = DateTime.Now;
        var employeeAttendant = CreateEmployeeAttendant(
            basicSalary: 1000,
            workingDays: 22,
            totalWorkingDays: 15,
            unpaidLeaveDays: 1,
            isMaternity: true,
            isNewHire: false
        );

        var employeeAttendants = new List<PayrollEmployeeAttendant> { employeeAttendant };
        SetupMocks(employeeAttendants);

        // Act
        await _service.GeneratePayroll(date, 424);

        // Assert
        var result = GetPayrollResult();
        Assert.Equal(15, result.PaidDays); // TotalWorkingDays for maternity
        Assert.Equal(EmploymentStatus.Maternity, result.EmploymentStatus);
    }

    [Fact]
    public async Task GeneratePayroll_ShouldCalculatePaidDays_WithUnpaidLeaveExceedingLimit()
    {
        // Arrange
        var date = DateTime.Now;
        var employeeAttendant = CreateEmployeeAttendant(
            basicSalary: 1000,
            workingDays: 22,
            totalWorkingDays: 6,
            unpaidLeaveDays: 15, // > 10
            isNewHire: false
        );

        var employeeAttendants = new List<PayrollEmployeeAttendant> { employeeAttendant };
        SetupMocks(employeeAttendants);

        // Act
        await _service.GeneratePayroll(date, 424);

        // Assert
        var result = GetPayrollResult();
        Assert.Equal(6, result.PaidDays); // TotalWorkingDays when unpaid leave > 10
    }

    [Fact]
    public async Task GeneratePayroll_ShouldReturnZeroGrossSalary_WhenConfigIsNull()
    {
        // Arrange
        var date = DateTime.Now;
        _salaryStructureService.GetBasicSalaryConfigAsync(Arg.Any<short>())
            .Returns((BasicSalaryConfigViewModel)null);

        var employeeAttendant = CreateEmployeeAttendant(basicSalary: 1000);
        var employeeAttendants = new List<PayrollEmployeeAttendant> { employeeAttendant };
        SetupMocks(employeeAttendants);

        // Act
        await _service.GeneratePayroll(date, 424);

        // Assert
        var result = GetPayrollResult();
        Assert.Equal(0, PayrollHelper.DecryptSalary(result.GrossSalary), 0);
    }

    [Fact]
    public async Task GeneratePayroll_ShouldReturnZeroGrossSalary_WhenConfigIsEmpty()
    {
        // Arrange
        var date = DateTime.Now;
        _salaryStructureService.GetBasicSalaryConfigAsync(Arg.Any<short>())
            .Returns(new BasicSalaryConfigViewModel
            {
                Config = "",
                ConfigOptions = new List<BasicSalaryConfigOption>()
            });

        var employeeAttendant = CreateEmployeeAttendant(basicSalary: 1000);
        var employeeAttendants = new List<PayrollEmployeeAttendant> { employeeAttendant };
        SetupMocks(employeeAttendants);

        // Act
        await _service.GeneratePayroll(date, 424);

        // Assert
        var result = GetPayrollResult();
        Assert.Equal(0, PayrollHelper.DecryptSalary(result.GrossSalary), 0);
    }

    [Fact]
    public async Task GeneratePayroll_ShouldHandleInvalidEncryptedSalary()
    {
        // Arrange
        var date = DateTime.Now;
        var employeeAttendant = CreateEmployeeAttendant(
            basicSalary: 0, // Will use invalid encrypted value
            encryptedBasicSalary: "invalid"
        );

        var employeeAttendants = new List<PayrollEmployeeAttendant> { employeeAttendant };
        SetupMocks(employeeAttendants);

        // Act
        await _service.GeneratePayroll(date, 424);

        // Assert
        var result = GetPayrollResult();
        Assert.Equal(0, PayrollHelper.DecryptSalary(result.GrossSalary), 0);
    }

    [Fact]
    public async Task GeneratePayroll_ShouldCalculateTotalIncome_WithAllowances()
    {
        // Arrange
        var date = DateTime.Now;
        var employeeAttendant = CreateEmployeeAttendant(
            basicSalary: 1000,
            managementAllowance: 100,
            transportationAllowance: 50,
            mobilephoneAllowance: 25
        );

        var employeeAttendants = new List<PayrollEmployeeAttendant> { employeeAttendant };
        SetupMocks(employeeAttendants);

        // Act
        await _service.GeneratePayroll(date, 424);

        // Assert
        var result = GetPayrollResult();
        // TotalIncome = GrossSalary (1000) + Allowances (100 + 50 + 25) = 1175
        var expectedTotalIncome = 1175m;
        Assert.Equal(expectedTotalIncome, PayrollHelper.DecryptSalary(result.TotalIncome), 0);
    }

    #endregion

    #region Edge Cases and Error Handling

    [Fact]
    public async Task GeneratePayroll_WithEmptyEmployeeList_ShouldStillProcess()
    {
        // Arrange
        var date = DateTime.Now;
        var employeeAttendants = new List<PayrollEmployeeAttendant>();
        var expectedCommandResult = new CommandResult { Code = 0 };

        _baseService.GetDataAsync<PayrollEmployeeAttendant>(Arg.Any<GetPayrollAttendantEmployeeCriteria>())
            .Returns(employeeAttendants);
        _baseService.ExecuteNonQueryAsync(Arg.Any<CalculatePayrollCriteria>())
            .Returns(expectedCommandResult);

        // Act
        var result = await _service.GeneratePayroll(date, 424);

        // Assert
        Assert.Equal(expectedCommandResult, result);
        CalculatePayrollCriteria capturedCriteria = null;
        _baseService.ExecuteNonQueryAsync(Arg.Do<CalculatePayrollCriteria>(c => capturedCriteria = c))
            .Returns(expectedCommandResult);

        await _service.GeneratePayroll(date, 424);

        var payrollResults = JsonConvert.DeserializeObject<List<PayrollEmployeeResult>>(capturedCriteria.PayrollListJson);
        Assert.Empty(payrollResults);
    }

    [Fact]
    public async Task GeneratePayroll_WithMultipleEmployees_ShouldProcessAllCorrectly()
    {
        // Arrange
        var date = DateTime.Now;
        var employeeAttendants = new List<PayrollEmployeeAttendant>
        {
            new() {
                EmpId = 1,
                EncryptBasicSalary = EncryptSalary("1000"),
                EncryptJobLevelAddOn = EncryptSalary("0"),
                EncryptSpecialSubsidy = EncryptSalary("0"),
                CurrencyCode = "USD",
                SpecialSubsidyCurrencyCode = "USD",
                WorkingDays = 22,
                TotalUnpaidLeaveDays = 0,
                TotalNewHireWorkingDays = 0,
                TotalWorkingDays = 22,
                IsResigned = false,
                IsMaternity = false,
                IsNewHire = false,
                ConfirmedDate = DateTime.Now.AddYears(-1)
            },
            new() {
                EmpId = 2,
                EncryptBasicSalary = EncryptSalary("2000"),
                EncryptJobLevelAddOn = EncryptSalary("0"),
                EncryptSpecialSubsidy = EncryptSalary("0"),
                CurrencyCode = "USD",
                SpecialSubsidyCurrencyCode = "USD",
                WorkingDays = 22,
                TotalUnpaidLeaveDays = 5,
                TotalNewHireWorkingDays = 0,
                TotalWorkingDays = 22,
                IsResigned = false,
                IsMaternity = false,
                IsNewHire = false,
                ConfirmedDate = DateTime.Now.AddYears(-1)
            }
        };
        var expectedCommandResult = new CommandResult { Code = 0 };

        _baseService.GetDataAsync<PayrollEmployeeAttendant>(Arg.Any<GetPayrollAttendantEmployeeCriteria>())
            .Returns(employeeAttendants);
        _baseService.ExecuteNonQueryAsync(Arg.Any<CalculatePayrollCriteria>())
            .Returns(expectedCommandResult);

        // Act
        await _service.GeneratePayroll(date, 424);

        // Assert
        CalculatePayrollCriteria capturedCriteria = null;
        _baseService.ExecuteNonQueryAsync(Arg.Do<CalculatePayrollCriteria>(c => capturedCriteria = c))
            .Returns(expectedCommandResult);

        await _service.GeneratePayroll(date, 424);

        var payrollResults = JsonConvert.DeserializeObject<List<PayrollEmployeeResult>>(capturedCriteria.PayrollListJson);
        Assert.Equal(2, payrollResults.Count);
        Assert.Contains(payrollResults, r => r.EmpId == 1);
        Assert.Contains(payrollResults, r => r.EmpId == 2);
    }

    #endregion

    #region Helper Methods

    private PayrollEmployeeAttendant CreateEmployeeAttendant(
        decimal basicSalary = 1000,
        decimal jobLevelAddOn = 0,
        decimal specialSubsidy = 0,
        string currencyCode = "USD",
        string specialSubsidyCurrency = "USD",
        int workingDays = 22,
        decimal totalWorkingDays = 22,
        decimal unpaidLeaveDays = 0,
        decimal newHireDays = 0,
        bool isNewHire = false,
        bool isResigned = false,
        bool isMaternity = false,
        decimal managementAllowance = 0,
        decimal transportationAllowance = 0,
        decimal mobilephoneAllowance = 0,
        string encryptedBasicSalary = null)
    {
        return new PayrollEmployeeAttendant
        {
            EmpId = 1,
            EncryptBasicSalary = encryptedBasicSalary ?? EncryptSalary(basicSalary.ToString()),
            EncryptJobLevelAddOn = EncryptSalary(jobLevelAddOn.ToString()),
            EncryptSpecialSubsidy = EncryptSalary(specialSubsidy.ToString()),
            CurrencyCode = currencyCode,
            SpecialSubsidyCurrencyCode = specialSubsidyCurrency,
            WorkingDays = workingDays,
            TotalWorkingDays = totalWorkingDays,
            TotalUnpaidLeaveDays = unpaidLeaveDays,
            TotalNewHireWorkingDays = newHireDays,
            IsNewHire = isNewHire,
            IsResigned = isResigned,
            IsMaternity = isMaternity,
            ManagementAllowance = managementAllowance,
            TransportationAllowance = transportationAllowance,
            MobilephoneAllowance = mobilephoneAllowance,
            ConfirmedDate = isNewHire ? DateTime.Now : DateTime.Now.AddYears(-1)
        };
    }

    private void SetupMocks(List<PayrollEmployeeAttendant> employeeAttendants)
    {
        _baseService.GetDataAsync<PayrollEmployeeAttendant>(Arg.Any<GetPayrollAttendantEmployeeCriteria>())
            .Returns(employeeAttendants);
        _baseService.ExecuteNonQueryAsync(Arg.Any<CalculatePayrollCriteria>())
            .Returns(new CommandResult { Code = 0 });
    }

    private PayrollEmployeeResult GetPayrollResult()
    {
        var capturedCriteria = _baseService.ReceivedCalls()
            .First(c => c.GetMethodInfo().Name == "ExecuteNonQueryAsync")
            .GetArguments()
            .First() as CalculatePayrollCriteria;

        var payrollResults = JsonConvert.DeserializeObject<List<PayrollEmployeeResult>>(capturedCriteria.PayrollListJson);
        return payrollResults.First();
    }

    private static string EncryptSalary(string salary)
    {
        return Cryptography.EncryptPassword(salary);
    }

    #endregion
}