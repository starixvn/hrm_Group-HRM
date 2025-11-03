using GO.UserControl.Client.Core.Enumerables;
using GO.UserControl.Client.Core.Models.User;
using GO.UserControl.Client.Core.Services;
using HRM.Controllers.GroupSpace;
using HRM.Core.Constants;
using HRM.Core.Models;
using HRM.Core.Services;
using HRM.Models;
using HRM.Services.Implement;
using HRM.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Xunit;

namespace HRM.Controllers.UnitTests;

public class EmployeeTransferControllerTest : BaseControllerTest
{
    private readonly IBaseService _baseService;
    private readonly EmployeeTransferController _controller;

    public EmployeeTransferControllerTest()
    {
        var employeeService = Substitute.For<IEmployeeService>();
        var userService = Substitute.For<IUserService>();
        var commonDataService = Substitute.For<ICommonDataService>();
        var companyService = Substitute.For<ICompanyService>();
        var jobLevelService = Substitute.For<IJobLevelService>();
        var managementLevelService = Substitute.For<IManagementLevelService>();
        _baseService = Substitute.For<IBaseService>();

        _controller = new EmployeeTransferController(
            _session,
            employeeService,
            userService,
            commonDataService,
            companyService,
            jobLevelService,
            managementLevelService,
            _baseService);
    }

    [Fact]
    public async Task TransferEmployee_WithValidModel_ReturnsSuccessfulResult()
    {
        // Arrange
        var model = new EmployeeTransfer
        {
            Code = "EMP001",
            NickName = "John",
            TargetCompanyId = 2,
            TransferOutCompanyId = 1,
            UserId = 100
        };

        var checkResult = new CommandResult { Code = 0 };
        var editResult = new CommandResult { Code = 0 };
        var companySites = new List<Models.CompanySite>
        {
            new() { CompanyId = 1, SiteId = 1 },
            new() { CompanyId = 2, SiteId = 2 }
        };

        var employeeService = Substitute.For<IEmployeeService>();
        var companyService = Substitute.For<ICompanyService>();
        var userService = Substitute.For<IUserService>();

        employeeService.CheckExistAsync(model.Code, model.NickName).Returns(checkResult);
        employeeService.TransferEmployeeAsync(model).Returns(123);
        employeeService.EditAsync(Arg.Any<int>(), Arg.Any<EditEmployeeParam>(), model.TargetCompanyId).Returns(editResult);
        employeeService.UpdateUserInfoAsync(model.UserId, Arg.Any<EditEmployeeParam>()).Returns(new GO.UserControl.Client.Core.Models.CommandResult());

        companyService.GetAllCompanySiteAsync().Returns(companySites);
        userService.UpdateUserSiteAsync(Arg.Any<UpdateUserSiteParam>()).Returns(new GO.UserControl.Client.Core.Models.CommandResult());
        userService.AssignRoleAsync(Arg.Any<AssignRoleParam>()).Returns(new GO.UserControl.Client.Core.Models.CommandResult());

        var controller = new EmployeeTransferController(
            _session,
            employeeService,
            userService,
            Substitute.For<ICommonDataService>(),
            companyService,
            Substitute.For<IJobLevelService>(),
            Substitute.For<IManagementLevelService>(),
            _baseService);

        SetupPermission(FunctionKey.EmployeeTransfer_All, Policy.Host, true);

        // Act
        var result = await controller.TransferEmployee(model);

        // Assert
        var jsonResult = Assert.IsType<JsonResult>(result);
        var commandResult = Assert.IsType<CommandResult>(jsonResult.Value);
        Assert.True(commandResult.IsSuccessful);

        await employeeService.Received(1).CheckExistAsync(model.Code, model.NickName);
        await employeeService.Received(1).TransferEmployeeAsync(model);
        await employeeService.Received(1).EditAsync(Arg.Any<int>(), Arg.Any<EditEmployeeParam>(), model.TargetCompanyId);
        await employeeService.Received(1).UpdateUserInfoAsync(model.UserId, Arg.Any<EditEmployeeParam>());
    }

    [Fact]
    public async Task TransferEmployee_WithCheckExistFailure_ReturnsFailureResult()
    {
        // Arrange
        var model = new EmployeeTransfer
        {
            Code = "EMP001",
            NickName = "John",
            TargetCompanyId = 2,
            TransferOutCompanyId = 1,
            UserId = 100
        };

        var checkResult = new CommandResult { Code = 1, Message = "Employee already exists" };

        var employeeService = Substitute.For<IEmployeeService>();
        employeeService.CheckExistAsync(model.Code, model.NickName).Returns(checkResult);

        var controller = new EmployeeTransferController(
            _session,
            employeeService,
            Substitute.For<IUserService>(),
            Substitute.For<ICommonDataService>(),
            Substitute.For<ICompanyService>(),
            Substitute.For<IJobLevelService>(),
            Substitute.For<IManagementLevelService>(),
            _baseService);

        SetupPermission(FunctionKey.EmployeeTransfer_All, Policy.Host, true);

        // Act
        var result = await controller.TransferEmployee(model);

        // Assert
        var jsonResult = Assert.IsType<JsonResult>(result);
        var commandResult = Assert.IsType<CommandResult>(jsonResult.Value);
        Assert.False(commandResult.IsSuccessful);
        Assert.Equal("Employee already exists", commandResult.Message);

        await employeeService.Received(1).CheckExistAsync(model.Code, model.NickName);
        await employeeService.DidNotReceive().TransferEmployeeAsync(Arg.Any<EmployeeTransfer>());
    }

    [Fact]
    public async Task TransferEmployee_WithEditFailure_DoesNotUpdateUserInfo()
    {
        // Arrange
        var model = new EmployeeTransfer
        {
            Code = "EMP001",
            NickName = "John",
            TargetCompanyId = 2,
            TransferOutCompanyId = 1,
            UserId = 100
        };

        var checkResult = new CommandResult { Code = 0 };
        var editResult = new CommandResult { Code = 1, Message = "Edit failed" };

        var employeeService = Substitute.For<IEmployeeService>();
        employeeService.CheckExistAsync(model.Code, model.NickName).Returns(checkResult);
        employeeService.TransferEmployeeAsync(model).Returns(123);
        employeeService.EditAsync(Arg.Any<int>(), Arg.Any<EditEmployeeParam>(), model.TargetCompanyId).Returns(editResult);

        var controller = new EmployeeTransferController(
            _session,
            employeeService,
            Substitute.For<IUserService>(),
            Substitute.For<ICommonDataService>(),
            Substitute.For<ICompanyService>(),
            Substitute.For<IJobLevelService>(),
            Substitute.For<IManagementLevelService>(),
            _baseService);

        SetupPermission(FunctionKey.EmployeeTransfer_All, Policy.Host, true);

        // Act
        var result = await controller.TransferEmployee(model);

        // Assert
        var jsonResult = Assert.IsType<JsonResult>(result);
        var commandResult = Assert.IsType<CommandResult>(jsonResult.Value);
        Assert.False(commandResult.IsSuccessful);

        await employeeService.Received(1).EditAsync(Arg.Any<int>(), Arg.Any<EditEmployeeParam>(), model.TargetCompanyId);
        await employeeService.DidNotReceive().UpdateUserInfoAsync(Arg.Any<int>(), Arg.Any<EditEmployeeParam>());
    }

    [Fact]
    public async Task GetInfoEmployeeTransfer_WithValidId_ReturnsEmployeeData()
    {
        // Arrange
        var employee = new EmployeeTransfer { Id = 1, FullName = "John Doe" };
        var countries = new List<Country> { new() { Id = 1, Name = "Vietnam" } };
        var jobLevels = new List<JobLevel> { new() { Id = 1, Name = "Senior" } };
        var managementLevels = new List<ManagementLevel> { new() { Id = 1, Name = "Manager" } };
        var subCompanies = new List<SubCompany> { new() { SubCompanyId = 1, Name = "SubCompany1" } };
        var jobTitles = new List<LookupData> { new() { Id = 1, Name = "Developer" } };

        var employeeService = Substitute.For<IEmployeeService>();
        var commonDataService = Substitute.For<ICommonDataService>();
        var jobLevelService = Substitute.For<IJobLevelService>();
        var managementLevelService = Substitute.For<IManagementLevelService>();
        var companyService = Substitute.For<ICompanyService>();

        employeeService.GetEmployeeTransferByIdAsync(1, 1).Returns(employee);
        employeeService.CalculateLengthOfService(Arg.Any<DateTime?>(), Arg.Any<DateTime?>(), Arg.Any<DateTime?>()).Returns("2 years");
        commonDataService.GetAllCountry().Returns(countries);
        commonDataService.GetLookupDataByCodeAsync(Arg.Any<string>(), Arg.Any<int>()).Returns(jobTitles);
        commonDataService.GetSystemSettingAsync(Arg.Any<short>()).Returns(new SystemSetting { EnableSickLeaveBalance = true });
        jobLevelService.GetJobLevel().Returns(jobLevels);
        managementLevelService.GetManagementLevel().Returns(managementLevels);
        companyService.GetSubCompanies(Arg.Any<short>()).Returns(subCompanies);

        var controller = new EmployeeTransferController(
            _session,
            employeeService,
            Substitute.For<IUserService>(),
            commonDataService,
            companyService,
            jobLevelService,
            managementLevelService,
            _baseService);

        SetupPermission(FunctionKey.EmployeeTransfer_All, Policy.Host, true);

        // Act
        var result = await controller.GetInfoEmployeeTransfer(1, 2, 1);

        // Assert
        var jsonResult = Assert.IsType<JsonResult>(result);
        Assert.Equal(employee, jsonResult.Value);
    }
}

