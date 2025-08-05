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
using HRM.Services.Implement.PayrollManagement;
using Newtonsoft.Json;
using NSubstitute;
using Xunit;

namespace HRM.Services.UnitTests;

public class SMSPayrollManagementServiceTests
{
    private readonly IBaseService _baseService;
    private readonly IUserControlSession _session;
    private readonly SMSPayrollManagementService _service;
    private const short TestCompanyId = 1;

    public SMSPayrollManagementServiceTests()
    {
        _baseService = Substitute.For<IBaseService>();
        _session = Substitute.For<IUserControlSession>();
        _session.Get<short>(SessionKey.CurrentCompanyId).Returns(TestCompanyId);
        _service = new SMSPayrollManagementService(_baseService, _session);
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
        var employeeAttendants = CreateSampleEmployeeAttendants();
        var expectedCommandResult = new CommandResult { Code = 0 };

        _baseService.GetDataAsync<PayrollEmployeeAttendant>(Arg.Any<GetPayrollAttendantEmployeeCriteria>())
            .Returns(employeeAttendants);
        _baseService.ExecuteNonQueryAsync(Arg.Any<CalculatePayrollCriteria>())
            .Returns(expectedCommandResult);

        // Act
        var result = await _service.GeneratePayroll(date);

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
        var employeeAttendants = CreateSampleEmployeeAttendants();
        var expectedCommandResult = new CommandResult { Code = 0 };

        _baseService.GetDataAsync<PayrollEmployeeAttendant>(Arg.Any<GetPayrollAttendantEmployeeCriteria>())
            .Returns(employeeAttendants);
        _baseService.ExecuteNonQueryAsync(Arg.Any<CalculatePayrollCriteria>())
            .Returns(expectedCommandResult);

        // Act
        await _service.GeneratePayroll(date);

        // Assert
        await _baseService.Received(1).ExecuteNonQueryAsync(Arg.Is<CalculatePayrollCriteria>(c =>
            JsonConvert.DeserializeObject<List<PayrollEmployeeResult>>(c.PayrollListJson).Count == employeeAttendants.Count()));
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
        await _service.GeneratePayroll(date);

        // Assert
        await _baseService.Received(1).GetDataAsync<PayrollEmployeeAttendant>(Arg.Is<GetPayrollAttendantEmployeeCriteria>(c =>
            c.CompanyId == TestCompanyId &&
            c.Date == date));
    }

    #endregion

    #region Payroll Calculation Logic Tests

    [Theory]
    [InlineData(20, 0, 0, false, false, EmploymentStatus.Normal, 1000)] // Normal employee - no unpaid leave
    [InlineData(20, 5, 0, false, false, EmploymentStatus.Normal, 773)] // Normal employee with unpaid leave ≤ 10: (22-5)/22*1000 = 773
    [InlineData(7, 15, 0, false, false, EmploymentStatus.Normal, 318)] // Normal employee with unpaid leave > 10: 7/22*1000 = 318
    [InlineData(14, 2, 0, true, false, EmploymentStatus.Resignation, 636)] // Resigned employee: 14/22*1000 = 636
    [InlineData(15, 1, 0, false, true, EmploymentStatus.Maternity, 682)] // Maternity employee: 15/22*1000 = 682
    [InlineData(0, 0, 20, false, false, EmploymentStatus.Probation, 773)] // New hire with probation days only: 20/22*1000*85/100 = 773
    [InlineData(10, 0, 10, false, false, EmploymentStatus.Confirmed, 841)] // New hire with confirmed days only: 10/22*1000*85/100 + 10/22*1000 = 387 + 455 = 842
    public async Task GeneratePayroll_ShouldCalculateCorrectEmploymentStatusAndSalary(decimal totalWorkingDays, decimal unpaidLeaveDays, decimal newHireDays,
       bool isResigned, bool isMaternity, EmploymentStatus expectedStatus, decimal expectedSalary)
    {
        // Arrange
        var date = DateTime.Now;
        var employeeAttendant = new PayrollEmployeeAttendant
        {
            EmpId = 1,
            Salary = EncryptSalary("1000"),
            WorkingDays = 22,
            TotalUnpaidLeaveDays = unpaidLeaveDays,
            TotalNewHireWorkingDays = newHireDays,
            TotalWorkingDays = totalWorkingDays,
            IsResigned = isResigned,
            IsMaternity = isMaternity
        };

        var employeeAttendants = new List<PayrollEmployeeAttendant> { employeeAttendant };
        var expectedCommandResult = new CommandResult { Code = 0 };

        _baseService.GetDataAsync<PayrollEmployeeAttendant>(Arg.Any<GetPayrollAttendantEmployeeCriteria>())
            .Returns(employeeAttendants);
        _baseService.ExecuteNonQueryAsync(Arg.Any<CalculatePayrollCriteria>())
            .Returns(expectedCommandResult);

        // Act
        await _service.GeneratePayroll(date);

        // Assert
        await _baseService.Received(1).ExecuteNonQueryAsync(Arg.Any<CalculatePayrollCriteria>());
        var capturedCriteria = _baseService.ReceivedCalls()
            .First(c => c.GetMethodInfo().Name == "ExecuteNonQueryAsync")
            .GetArguments()
            .First() as CalculatePayrollCriteria;

        var payrollResults = JsonConvert.DeserializeObject<List<PayrollEmployeeResult>>(capturedCriteria.PayrollListJson);
        var result = payrollResults.First();

        Assert.Equal(expectedStatus, result.EmploymentStatus);
        Assert.Equal(employeeAttendant.EmpId, result.EmpId);
        Assert.Equal(22, result.WorkingDays);
        Assert.Equal(unpaidLeaveDays, result.UnpaidLeave);
        var actualSalary = Math.Round(decimal.Parse(Cryptography.DecryptPassword(result.GrossSalary)));
        Assert.Equal(expectedSalary, actualSalary);
    }

    [Theory]
    [InlineData(21, 0, 22)] // No unpaid leave
    [InlineData(15, 5, 17)] // Unpaid leave ≤ 10: 22-5=17
    [InlineData(6, 15, 6)] // Unpaid leave > 10: 6
    [InlineData(14, 2, 14, true)] // Resigned employee
    [InlineData(15, 1, 15, false, true)] // Maternity employee
    [InlineData(15, 0, 20, false, false, 5)] // New hire with probation days
    public async Task GeneratePayroll_ShouldCalculateCorrectPaidDays(
         decimal totalWorkingDays, decimal unpaidLeaveDays, decimal expectedPaidDays,
        bool isResigned = false, bool isMaternity = false, decimal newHireDays = 0)
    {
        // Arrange
        var date = DateTime.Now;
        var employeeAttendant = new PayrollEmployeeAttendant
        {
            EmpId = 1,
            Salary = EncryptSalary("1000"),
            WorkingDays = 22,
            TotalUnpaidLeaveDays = unpaidLeaveDays,
            TotalNewHireWorkingDays = newHireDays,
            TotalWorkingDays = totalWorkingDays,
            IsResigned = isResigned,
            IsMaternity = isMaternity
        };

        var employeeAttendants = new List<PayrollEmployeeAttendant> { employeeAttendant };
        var expectedCommandResult = new CommandResult { Code = 0 };
        CalculatePayrollCriteria capturedCriteria = null;

        _baseService.GetDataAsync<PayrollEmployeeAttendant>(Arg.Any<GetPayrollAttendantEmployeeCriteria>())
            .Returns(employeeAttendants);
        _baseService.ExecuteNonQueryAsync(Arg.Do<CalculatePayrollCriteria>(c => capturedCriteria = c))
            .Returns(expectedCommandResult);

        // Act
        await _service.GeneratePayroll(date);

        // Assert
        await _baseService.Received(1).ExecuteNonQueryAsync(Arg.Any<CalculatePayrollCriteria>());
        var payrollResults = JsonConvert.DeserializeObject<List<PayrollEmployeeResult>>(capturedCriteria.PayrollListJson);
        var result = payrollResults.First();
        Assert.Equal(expectedPaidDays, result.PaidDays);
    }

    [Theory]
    [InlineData("prOHl0ZDB3w=", 1000)]
    [InlineData("", 0)]
    [InlineData(null, 0)]
    [InlineData("invalid", 0)]
    public async Task GeneratePayroll_ShouldHandleSalaryDecryptionCorrectly(string encryptedSalary, decimal expectedSalary)
    {
        // Arrange
        var date = DateTime.Now;
        var employeeAttendant = new PayrollEmployeeAttendant
        {
            EmpId = 1,
            Salary = encryptedSalary,
            WorkingDays = 22,
            TotalUnpaidLeaveDays = 0,
            TotalNewHireWorkingDays = 0,
            TotalWorkingDays = 22,
            IsResigned = false,
            IsMaternity = false
        };

        var employeeAttendants = new List<PayrollEmployeeAttendant> { employeeAttendant };
        var expectedCommandResult = new CommandResult { Code = 0 };

        _baseService.GetDataAsync<PayrollEmployeeAttendant>(Arg.Any<GetPayrollAttendantEmployeeCriteria>())
            .Returns(employeeAttendants);
        _baseService.ExecuteNonQueryAsync(Arg.Any<CalculatePayrollCriteria>())
            .Returns(expectedCommandResult);

        // Act
        await _service.GeneratePayroll(date);

        // Assert
        CalculatePayrollCriteria capturedCriteria = null;
        _baseService.ExecuteNonQueryAsync(Arg.Do<CalculatePayrollCriteria>(c => capturedCriteria = c))
            .Returns(expectedCommandResult);

        await _service.GeneratePayroll(date);

        var payrollResults = JsonConvert.DeserializeObject<List<PayrollEmployeeResult>>(capturedCriteria.PayrollListJson);
        var result = payrollResults.First();
        Assert.Equal(expectedSalary, decimal.Parse(Cryptography.DecryptPassword(result.GrossSalary)));
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
        var result = await _service.GeneratePayroll(date);

        // Assert
        Assert.Equal(expectedCommandResult, result);
        CalculatePayrollCriteria capturedCriteria = null;
        _baseService.ExecuteNonQueryAsync(Arg.Do<CalculatePayrollCriteria>(c => capturedCriteria = c))
            .Returns(expectedCommandResult);

        await _service.GeneratePayroll(date);

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
                Salary = EncryptSalary("1000"),
                WorkingDays = 22,
                TotalUnpaidLeaveDays = 0,
                TotalNewHireWorkingDays = 0,
                TotalWorkingDays = 22,
                IsResigned = false,
                IsMaternity = false
            },
            new() {
                EmpId = 2,
                Salary = EncryptSalary("2000"),
                WorkingDays = 22,
                TotalUnpaidLeaveDays = 5,
                TotalNewHireWorkingDays = 0,
                TotalWorkingDays = 22,
                IsResigned = false,
                IsMaternity = false
            }
        };
        var expectedCommandResult = new CommandResult { Code = 0 };

        _baseService.GetDataAsync<PayrollEmployeeAttendant>(Arg.Any<GetPayrollAttendantEmployeeCriteria>())
            .Returns(employeeAttendants);
        _baseService.ExecuteNonQueryAsync(Arg.Any<CalculatePayrollCriteria>())
            .Returns(expectedCommandResult);

        // Act
        await _service.GeneratePayroll(date);

        // Assert
        CalculatePayrollCriteria capturedCriteria = null;
        _baseService.ExecuteNonQueryAsync(Arg.Do<CalculatePayrollCriteria>(c => capturedCriteria = c))
            .Returns(expectedCommandResult);

        await _service.GeneratePayroll(date);

        var payrollResults = JsonConvert.DeserializeObject<List<PayrollEmployeeResult>>(capturedCriteria.PayrollListJson);
        Assert.Equal(2, payrollResults.Count);
        Assert.Contains(payrollResults, r => r.EmpId == 1);
        Assert.Contains(payrollResults, r => r.EmpId == 2);
    }

    #endregion

    #region Helper Methods

    private static IEnumerable<PayrollEmployeeAttendant> CreateSampleEmployeeAttendants()
    {
        return new List<PayrollEmployeeAttendant>
        {
            new() {
                EmpId = 1,
                Salary = EncryptSalary("1000"),
                WorkingDays = 22,
                TotalUnpaidLeaveDays = 0,
                TotalNewHireWorkingDays = 0,
                TotalWorkingDays = 22,
                IsResigned = false,
                IsMaternity = false
            }
        };
    }

    private static string EncryptSalary(string salary)
    {
        return Cryptography.EncryptPassword(salary);
    }

    #endregion
}