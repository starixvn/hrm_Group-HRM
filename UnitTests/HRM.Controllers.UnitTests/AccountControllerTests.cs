using System.Collections.Generic;
using System.Threading.Tasks;
using GO.UserControl.Client.Core.Enumerables;
using GO.UserControl.Client.Core.Models.User;
using GO.UserControl.Client.Core.Services;
using HRM.Controllers;
using HRM.Core.Constants;
using HRM.Core.Enums;
using HRM.Core.Models;
using HRM.Core.Services;
using HRM.Models;
using HRM.Models.Log;
using HRM.Models.PayrollManagement;
using HRM.Resources;
using HRM.Services.Implement;
using HRM.Services.Implement.ActionLog;
using HRM.Services.Interfaces;
using HRM.ViewModels.Employee;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Xunit;

namespace HRM.Controllers.UnitTests;

public class AccountControllerTests : BaseControllerTest
{
    private readonly IEmployeeService _employeeService;
    private readonly IEmployeeLogService _employeeLogService;
    private readonly ICryptoService _cryptoService;
    private readonly IHRMAuthService _authService;
    private readonly ICommonDataService _commonDataService;
    private readonly IUserService _userService;
    private readonly IMinIOService _minIOService;
    private readonly AccountController _controller;

    public AccountControllerTests()
    {
        _employeeService = Substitute.For<IEmployeeService>();
        _employeeLogService = Substitute.For<IEmployeeLogService>();
        _cryptoService = Substitute.For<ICryptoService>();
        _authService = Substitute.For<IHRMAuthService>();
        _commonDataService = Substitute.For<ICommonDataService>();
        _userService = Substitute.For<IUserService>();
        _minIOService = Substitute.For<IMinIOService>();

        _controller = new AccountController(
            _session,
            _employeeService,
            _employeeLogService,
            _cryptoService,
            _authService,
            _commonDataService,
            _userService,
            _minIOService
        );
    }

    #region Index Tests

    [Fact]
    public async Task Index_WithValidEmployee_ShouldReturnViewWithViewModel()
    {
        // Arrange
        var employee = new Employee
        {
            Id = 1,
            Code = "EMP001",
            FullName = "Test User",
            NickName = "Test",
        };
        var payrollAllowances = new List<EmployeePayrollAllowance>
        {
            new() { AllowanceItemSettingId = 1, ItemName = "Test Allowance" },
        };
        var logs = new List<LogObjectDetail>
        {
            new() { UserId = 1, FullName = "Test User" },
        };

        _employeeService.GetEmployeeByIdAsync(1).Returns(employee);
        _employeeService.GetEmployeePayrollAllowanceByEmpIdAsync(1).Returns(payrollAllowances);
        _employeeLogService.GetLogDetailByIdAsync(1).Returns(logs);
        _commonDataService.IsEnableGroupDateAsync().Returns(true);
        _minIOService
            .GetFilePath(Bucket.Avatar, Arg.Any<string>(), "default-avatar.jpg")
            .Returns("https://cdn.example.com/avatar.jpg");

        // Act
        var result = await _controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<AccountViewModel>(viewResult.Model);
        Assert.Equal(employee, model.Employee);
        Assert.Equal(payrollAllowances, model.EmployeePayrollAllowances);
        Assert.Equal(logs, model.Logs);
        Assert.True((bool)_controller.ViewBag.EnableGroupDate);
        Assert.Equal("https://cdn.example.com/avatar.jpg", _controller.ViewBag.Avatar);
    }

    [Fact]
    public async Task Index_WithNullEmployee_ShouldReturnViewWithNewEmployee()
    {
        // Arrange
        _employeeService.GetEmployeeByIdAsync(1).Returns((Employee)null);
        _employeeService
            .GetEmployeePayrollAllowanceByEmpIdAsync(1)
            .Returns(new List<EmployeePayrollAllowance>());
        _employeeLogService.GetLogDetailByIdAsync(1).Returns((List<LogObjectDetail>)null);
        _commonDataService.IsEnableGroupDateAsync().Returns(false);
        _minIOService
            .GetFilePath(Bucket.Avatar, Arg.Any<string>(), "default-avatar.jpg")
            .Returns("default-avatar.jpg");

        // Act
        var result = await _controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<AccountViewModel>(viewResult.Model);
        Assert.NotNull(model.Employee);
        Assert.NotNull(model.Logs);
        Assert.False((bool)_controller.ViewBag.EnableGroupDate);
    }

    [Fact]
    public async Task Index_WithNullLogs_ShouldReturnViewWithEmptyLogList()
    {
        // Arrange
        var employee = new Employee { Id = 1, Code = "EMP001" };
        _employeeService.GetEmployeeByIdAsync(1).Returns(employee);
        _employeeService
            .GetEmployeePayrollAllowanceByEmpIdAsync(1)
            .Returns(new List<EmployeePayrollAllowance>());
        _employeeLogService.GetLogDetailByIdAsync(1).Returns((List<LogObjectDetail>)null);
        _commonDataService.IsEnableGroupDateAsync().Returns(false);
        _minIOService
            .GetFilePath(Bucket.Avatar, Arg.Any<string>(), "default-avatar.jpg")
            .Returns("default-avatar.jpg");

        // Act
        var result = await _controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<AccountViewModel>(viewResult.Model);
        Assert.NotNull(model.Logs);
        Assert.Empty(model.Logs);
    }

    [Fact]
    public async Task Index_ShouldCallAllServices()
    {
        // Arrange
        var employee = new Employee { Id = 1, Code = "EMP001" };
        _employeeService.GetEmployeeByIdAsync(1).Returns(employee);
        _employeeService
            .GetEmployeePayrollAllowanceByEmpIdAsync(1)
            .Returns(new List<EmployeePayrollAllowance>());
        _employeeLogService.GetLogDetailByIdAsync(1).Returns(new List<LogObjectDetail>());
        _commonDataService.IsEnableGroupDateAsync().Returns(false);
        _minIOService
            .GetFilePath(Bucket.Avatar, Arg.Any<string>(), "default-avatar.jpg")
            .Returns("default-avatar.jpg");

        // Act
        await _controller.Index();

        // Assert
        await _employeeService.Received(1).GetEmployeeByIdAsync(1);
        await _employeeService.Received(1).GetEmployeePayrollAllowanceByEmpIdAsync(1);
        await _employeeLogService.Received(1).GetLogDetailByIdAsync(1);
        await _commonDataService.Received(1).IsEnableGroupDateAsync();
        await _minIOService
            .Received(1)
            .GetFilePath(
                Bucket.Avatar,
                Arg.Is<string>(s => s.Contains("EMP001.jpg")),
                "default-avatar.jpg"
            );
    }

    #endregion

    #region ResetPassword Tests

    [Fact]
    public async Task ResetPassword_WithValidModel_ShouldResetPasswordAndReturnSuccess()
    {
        // Arrange
        var model = new EditPassword { UserId = 100, EmployeeId = 1 };

        var employeeResult = new CommandResult { Code = 0 };
        var userResult = new GO.UserControl.Client.Core.Models.CommandResult { Code = 0 };

        _employeeService.EditPasswordAsync(1, Arg.Any<EditPassword>()).Returns(employeeResult);
        _cryptoService.GenerateRijndaelKeyIV().Returns("testkey:testiv");
        _cryptoService.Encrypt("123456", "testkey", "testiv").Returns("encrypted123456");
        _userService.ResetPasswordAsync(Arg.Any<UpdatePasswordParam>()).Returns(userResult);

        SetupPermission(FunctionKey.EmployeeManagement, Policy.Host, true);

        // Act
        var result = await _controller.ResetPassword(model);

        // Assert
        var jsonResult = Assert.IsType<JsonResult>(result);
        Assert.Equal(userResult, jsonResult.Value);
        Assert.True(model.IsReset);
        await _employeeService
            .Received(1)
            .EditPasswordAsync(1, Arg.Is<EditPassword>(p => p.IsReset));
        await _userService.Received(1).ResetPasswordAsync(Arg.Any<UpdatePasswordParam>());
    }

    [Fact]
    public async Task ResetPassword_WithFailedEmployeeService_ShouldReturnFailure()
    {
        // Arrange
        var model = new EditPassword { UserId = 100, EmployeeId = 1 };

        var employeeResult = new CommandResult { Code = 1, Message = "Failed to update password" };

        _employeeService.EditPasswordAsync(1, Arg.Any<EditPassword>()).Returns(employeeResult);

        SetupPermission(FunctionKey.EmployeeManagement, Policy.Host, true);

        // Act
        var result = await _controller.ResetPassword(model);

        // Assert
        var jsonResult = Assert.IsType<JsonResult>(result);
        var commandResult = Assert.IsType<CommandResult>(jsonResult.Value);
        Assert.False(commandResult.IsSuccessful);
        await _userService.DidNotReceive().ResetPasswordAsync(Arg.Any<UpdatePasswordParam>());
    }

    [Fact]
    public async Task ResetPassword_ShouldSetPasswordToDefaultHash()
    {
        // Arrange
        var model = new EditPassword { UserId = 100, EmployeeId = 1 };

        var employeeResult = new CommandResult { Code = 0 };
        var userResult = new GO.UserControl.Client.Core.Models.CommandResult { Code = 0 };

        _employeeService.EditPasswordAsync(1, Arg.Any<EditPassword>()).Returns(employeeResult);
        _cryptoService.GenerateRijndaelKeyIV().Returns("testkey:testiv");
        _cryptoService.Encrypt("123456", "testkey", "testiv").Returns("encrypted123456");
        _userService.ResetPasswordAsync(Arg.Any<UpdatePasswordParam>()).Returns(userResult);

        SetupPermission(FunctionKey.EmployeeManagement, Policy.Host, true);

        // Act
        await _controller.ResetPassword(model);

        // Assert
        await _employeeService
            .Received(1)
            .EditPasswordAsync(
                1,
                Arg.Is<EditPassword>(p => !string.IsNullOrEmpty(p.Password) && p.IsReset)
            );
    }

    #endregion

    #region ChangePwd GET Tests

    [Fact]
    public void ChangePwd_Get_ShouldReturnView()
    {
        // Act
        var result = _controller.ChangePwd();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
    }

    #endregion

    #region ChangePwd POST Tests

    [Fact]
    public async Task ChangePwd_Post_WithInvalidModel_ShouldReturnViewWithModel()
    {
        // Arrange
        var model = new ChangePasswordViewModel();
        _controller.ModelState.AddModelError("Password", "Password is required");

        // Act
        var result = await _controller.ChangePwd(model);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(model, viewResult.Model);
        Assert.False(_controller.ModelState.IsValid);
    }

    [Fact]
    public async Task ChangePwd_Post_WithWrongPassword_ShouldReturnViewWithError()
    {
        // Arrange
        var model = new ChangePasswordViewModel
        {
            Password = "wrongpassword",
            NewPassword = "newpassword123",
            ConfirmPassword = "newpassword123",
        };

        _authService
            .Validate(Arg.Any<EmployeeSession>(), model.Password)
            .Returns((short)ValidateUserStatus.WrongPassword);

        // Act
        var result = await _controller.ChangePwd(model);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.False(_controller.ModelState.IsValid);
        Assert.Contains(
            _controller.ModelState[string.Empty].Errors,
            e => e.ErrorMessage == Messages.WrongPassword
        );
    }

    [Fact]
    public async Task ChangePwd_Post_WithValidPasswordAndSuccess_ShouldUpdatePassword()
    {
        // Arrange
        var model = new ChangePasswordViewModel
        {
            Password = "oldpassword",
            NewPassword = "newpassword123",
            ConfirmPassword = "newpassword123",
        };

        var employeeResult = new CommandResult { Code = 0 };
        var userResult = new GO.UserControl.Client.Core.Models.CommandResult { Code = 0 };

        _authService
            .Validate(Arg.Any<EmployeeSession>(), model.Password)
            .Returns((short)ValidateUserStatus.Success);
        _employeeService.EditPasswordAsync(1, Arg.Any<EditPassword>()).Returns(employeeResult);
        _cryptoService.GenerateRijndaelKeyIV().Returns("testkey:testiv");
        _cryptoService.Encrypt(Arg.Any<string>(), "testkey", "testiv").Returns("encrypted");
        _userService.UpdatePasswordAsync(Arg.Any<UpdatePasswordParam>()).Returns(userResult);

        // Act
        var result = await _controller.ChangePwd(model);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(Messages.ChangePwdOK, _controller.ViewBag.ChangePwdOK);
        await _employeeService.Received(1).EditPasswordAsync(1, Arg.Any<EditPassword>());
        await _userService.Received(1).UpdatePasswordAsync(Arg.Any<UpdatePasswordParam>());
    }

    [Fact]
    public async Task ChangePwd_Post_WithDeletedUser_ShouldReturnViewWithError()
    {
        // Arrange
        var model = new ChangePasswordViewModel
        {
            Password = "oldpassword",
            NewPassword = "newpassword123",
            ConfirmPassword = "newpassword123",
        };

        var employeeResult = new CommandResult { Code = (short)ValidateUserStatus.Deleted };

        _authService
            .Validate(Arg.Any<EmployeeSession>(), model.Password)
            .Returns((short)ValidateUserStatus.Success);
        _employeeService.EditPasswordAsync(1, Arg.Any<EditPassword>()).Returns(employeeResult);

        // Act
        var result = await _controller.ChangePwd(model);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.False(_controller.ModelState.IsValid);
        Assert.Contains(
            _controller.ModelState[string.Empty].Errors,
            e => e.ErrorMessage == Messages.DeletedUser
        );
        await _userService.DidNotReceive().UpdatePasswordAsync(Arg.Any<UpdatePasswordParam>());
    }

    [Fact]
    public async Task ChangePwd_Post_WithDisabledUser_ShouldReturnViewWithError()
    {
        // Arrange
        var model = new ChangePasswordViewModel
        {
            Password = "oldpassword",
            NewPassword = "newpassword123",
            ConfirmPassword = "newpassword123",
        };

        var employeeResult = new CommandResult { Code = (short)ValidateUserStatus.Disabled };

        _authService
            .Validate(Arg.Any<EmployeeSession>(), model.Password)
            .Returns((short)ValidateUserStatus.Success);
        _employeeService.EditPasswordAsync(1, Arg.Any<EditPassword>()).Returns(employeeResult);

        // Act
        var result = await _controller.ChangePwd(model);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.False(_controller.ModelState.IsValid);
        Assert.Contains(
            _controller.ModelState[string.Empty].Errors,
            e => e.ErrorMessage == Messages.DisabledUser
        );
        await _userService.DidNotReceive().UpdatePasswordAsync(Arg.Any<UpdatePasswordParam>());
    }

    [Fact]
    public async Task ChangePwd_Post_WithWrongPasswordFromEmployeeService_ShouldReturnViewWithError()
    {
        // Arrange
        var model = new ChangePasswordViewModel
        {
            Password = "oldpassword",
            NewPassword = "newpassword123",
            ConfirmPassword = "newpassword123",
        };

        var employeeResult = new CommandResult { Code = (short)ValidateUserStatus.WrongPassword };

        _authService
            .Validate(Arg.Any<EmployeeSession>(), model.Password)
            .Returns((short)ValidateUserStatus.Success);
        _employeeService.EditPasswordAsync(1, Arg.Any<EditPassword>()).Returns(employeeResult);

        // Act
        var result = await _controller.ChangePwd(model);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.False(_controller.ModelState.IsValid);
        Assert.Contains(
            _controller.ModelState[string.Empty].Errors,
            e => e.ErrorMessage == Messages.WrongPassword
        );
        await _userService.DidNotReceive().UpdatePasswordAsync(Arg.Any<UpdatePasswordParam>());
    }

    [Fact]
    public async Task ChangePwd_Post_WithFailedUserServiceUpdate_ShouldReturnViewWithError()
    {
        // Arrange
        var model = new ChangePasswordViewModel
        {
            Password = "oldpassword",
            NewPassword = "newpassword123",
            ConfirmPassword = "newpassword123",
        };

        var employeeResult = new CommandResult { Code = 0 };
        var userResult = new GO.UserControl.Client.Core.Models.CommandResult
        {
            Code = 1,
            Message = "Failed to update password",
        };

        _authService
            .Validate(Arg.Any<EmployeeSession>(), model.Password)
            .Returns((short)ValidateUserStatus.Success);
        _employeeService.EditPasswordAsync(1, Arg.Any<EditPassword>()).Returns(employeeResult);
        _cryptoService.GenerateRijndaelKeyIV().Returns("testkey:testiv");
        _cryptoService.Encrypt(Arg.Any<string>(), "testkey", "testiv").Returns("encrypted");
        _userService.UpdatePasswordAsync(Arg.Any<UpdatePasswordParam>()).Returns(userResult);

        // Act
        var result = await _controller.ChangePwd(model);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.False(_controller.ModelState.IsValid);
        Assert.Contains(
            _controller.ModelState[string.Empty].Errors,
            e => e.ErrorMessage == userResult.Message
        );
        Assert.Null(_controller.ViewBag.ChangePwdOK);
    }

    [Fact]
    public async Task ChangePwd_Post_ShouldHashNewPassword()
    {
        // Arrange
        var model = new ChangePasswordViewModel
        {
            Password = "oldpassword",
            NewPassword = "newpassword123",
            ConfirmPassword = "newpassword123",
        };

        var employeeResult = new CommandResult { Code = 0 };
        var userResult = new GO.UserControl.Client.Core.Models.CommandResult { Code = 0 };

        _authService
            .Validate(Arg.Any<EmployeeSession>(), model.Password)
            .Returns((short)ValidateUserStatus.Success);
        _employeeService.EditPasswordAsync(1, Arg.Any<EditPassword>()).Returns(employeeResult);
        _cryptoService.GenerateRijndaelKeyIV().Returns("testkey:testiv");
        _cryptoService.Encrypt(Arg.Any<string>(), "testkey", "testiv").Returns("encrypted");
        _userService.UpdatePasswordAsync(Arg.Any<UpdatePasswordParam>()).Returns(userResult);

        // Act
        await _controller.ChangePwd(model);

        // Assert
        await _employeeService
            .Received(1)
            .EditPasswordAsync(
                1,
                Arg.Is<EditPassword>(p =>
                    !string.IsNullOrEmpty(p.Password) && p.EmployeeId == 1 && p.UserId == 1
                )
            );
    }

    #endregion
}
