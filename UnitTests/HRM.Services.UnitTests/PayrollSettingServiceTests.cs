using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fanex.Data;
using GO.UserControl.Client.Core.Session;
using HRM.Core.Constants;
using HRM.Core.Models;
using HRM.Core.Services;
using HRM.Criteria.PayrollManagement;
using HRM.Models.PayrollManagement;
using HRM.Services.Implement;
using HRM.Services.Interfaces;
using NSubstitute;
using Xunit;

namespace HRM.Services.UnitTests;

public class PayrollSettingServiceTests
{
    private readonly IBaseService _baseService;
    private readonly IUserControlSession _session;
    private readonly IPayrollManagementService _payrollManagementService;
    private readonly IObjectDbFactory _objectDbFactory;
    private readonly IObjectDb _objectDb;
    private readonly IMultiResultReader _multiResultReader;
    private readonly PayrollSettingService _service;
    private const short TestCompanyId = 1;
    private readonly List<PayrollSetting> _mockData;

    public PayrollSettingServiceTests()
    {
        _baseService = Substitute.For<IBaseService>();
        _session = Substitute.For<IUserControlSession>();
        _payrollManagementService = Substitute.For<IPayrollManagementService>();
        _objectDbFactory = Substitute.For<IObjectDbFactory>();
        _objectDb = Substitute.For<IObjectDb>();
        _multiResultReader = Substitute.For<IMultiResultReader>();
        _session.Get<short>(SessionKey.CurrentCompanyId).Returns(TestCompanyId);
        _service = new PayrollSettingService(_payrollManagementService, _baseService, _objectDbFactory);
        _mockData =
        [
            new()
            {
                PayrollItemSettingId = 1,
                Type = "Allowance",
                PayrollAllowanceItems =
                [
                    new() { AllowanceItemSettingId = 10, ItemName = "Lunch", Amount = "100", IsPercent = false },
                    new() { AllowanceItemSettingId = 20, ItemName = "Transport", Amount = "200", IsPercent = false },
                ]
            },
            new()
            {
                PayrollItemSettingId = 2,
                Type = "Bonus",
                PayrollAllowanceItems =
                [
                    new() { AllowanceItemSettingId = 30, ItemName = "Performance", Amount = "10", IsPercent = true },
                ]
            }
        ];
    }

    [Fact]
    public async Task GetPayrollItemSettings_WithCompanyId_Returns2Items()
    {
        // Arrange
        var currentCompanyId = _session.Get<short>(SessionKey.CurrentCompanyId);
        var criteria = new GetPayrollItemSettingsCriteria { CompanyId = currentCompanyId };

        _objectDbFactory.CreateInstance(Arg.Any<string>()).Returns(_objectDb);

        _objectDb.QueryMultipleAsync(criteria).Returns(Task.FromResult(_multiResultReader));

        _multiResultReader.Read(
            Arg.Any<Func<PayrollSetting, PayrollAllowanceItem, PayrollSetting>>(),
            Arg.Any<string>()
        ).Returns(_mockData);

        // Act
        var result = (await _service.GetPayrollItemSettings(criteria)).ToList();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal("Allowance", result[0].Type);
        Assert.Equal(2, result[0].PayrollAllowanceItems.Count);
    }

    [Fact]
    public async Task CreateAllowanceItem_ShouldCallBaseServiceOnce_AndReturnCommandResult()
    {
        // Arrange
        var criteria = new CreateAllowanceItemCriteria
        {
            PayrollItemSettingId = 1,
            Amount = "encrypted-amount",
            IsActive = true,
            IsPercent = false,
            ItemName = "Meal",
            ModifiedBy = 1001
        };

        var expectedResult = new CommandResult
        {
            Code = 0,
            Message = "Created successfully",
            CompanyId = 1,
            OutputId = 999
        };

        _baseService.ExecuteNonQueryAsync(criteria).Returns(expectedResult);

        // Act
        var result = await _service.CreateAllowanceItem(criteria);

        // Assert
        await _baseService.Received(1).ExecuteNonQueryAsync(criteria);
        Assert.True(result.IsSuccessful);
        Assert.Equal(0, result.Code);
        Assert.Equal("Created successfully", result.Message);
        Assert.Equal(999, result.OutputId);
    }

    [Fact]
    public async Task EditAllowanceItem_ShouldCallBaseServiceOnce_AndReturnCommandResult()
    {
        // Arrange
        var criteria = new EditAllowanceItemCriteria
        {
            AllowanceItemSettingId = 10,
            Amount = "encrypted-amount",
            IsActive = true,
            IsPercent = false,
            ItemName = "Meal",
            ModifiedBy = 1001
        };

        var expected = new CommandResult
        {
            Code = 0,
            Message = "Edited successfully",
            OutputId = 10
        };

        _baseService.ExecuteNonQueryAsync(criteria).Returns(Task.FromResult(expected));

        // Act
        var result = await _service.EditAllowanceItem(criteria);

        // Assert
        await _baseService.Received(1).ExecuteNonQueryAsync(criteria);
        Assert.True(result.IsSuccessful);
        Assert.Equal("Edited successfully", result.Message);
        Assert.Equal(0, result.Code);
    }

    [Fact]
    public async Task DeleteAllowanceItem_ShouldCallBaseServiceOnce_AndReturnCommandResult()
    {
        // Arrange
        var criteria = new DeleteAllowanceItemCriteria
        {
            AllowanceItemSettingId = 20
        };

        var expected = new CommandResult
        {
            Code = 0,
            Message = "Deleted successfully",
            OutputId = 20
        };

        _baseService.ExecuteNonQueryAsync(criteria).Returns(Task.FromResult(expected));

        // Act
        var result = await _service.DeleteAllowanceItem(criteria);

        // Assert
        await _baseService.Received(1).ExecuteNonQueryAsync(criteria);
        Assert.True(result.IsSuccessful);
        Assert.Equal(0, result.Code);
        Assert.Equal("Deleted successfully", result.Message);
    }
}