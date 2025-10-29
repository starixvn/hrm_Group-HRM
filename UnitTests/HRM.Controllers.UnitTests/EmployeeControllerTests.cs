using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GO.UserControl.Client.Core.Enumerables;
using GO.UserControl.Client.Core.Models.Role;
using GO.UserControl.Client.Core.Models.User;
using GO.UserControl.Client.Core.Services;
using HRM.Core;
using HRM.Core.Constants;
using HRM.Core.Enums;
using HRM.Core.Models;
using HRM.Core.Services;
using HRM.Services.Implement.ActionLog;
using HRM.Criteria.Employee;
using HRM.Criteria.PayrollManagement;
using HRM.Criteria.Supervisor;
using HRM.Models;
using HRM.Models.Employees;
using HRM.Models.Group;
using HRM.Models.Log;
using HRM.Models.PayrollManagement;
using HRM.Models.SalaryStructure;
using HRM.Models.Supervisor;
using HRM.Services.Implement;
using HRM.Services.Interfaces;
using HRM.ViewModels;
using HRM.ViewModels.Employee;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NSubstitute;
using Xunit;

namespace HRM.Controllers.UnitTests;

public class EmployeeControllerTests : BaseControllerTest
{
    private readonly IEmployeeService _employeeService;
    private readonly ICommonDataService _commonDataService;
    private readonly IHRMAuthService _authService;
    private readonly IEmployeeLogService _employeeLogService;
    private readonly ITeamService _teamService;
    private readonly IJobLevelService _jobLevelService;
    private readonly IManagementLevelService _managementLevelService;
    private readonly ICryptoService _cryptoService;
    private readonly IUserService _userService;
    private readonly IRoleService _roleService;
    private readonly IMinIOService _minIOService;
    private readonly ICompanyService _companyService;
    private readonly IPayrollSettingService _payrollSettingService;
    private readonly ISalaryStructureService _salaryStructureService;
    private readonly EmployeeController _controller;

    public EmployeeControllerTests()
    {
        _employeeService = Substitute.For<IEmployeeService>();
        _commonDataService = Substitute.For<ICommonDataService>();
        _authService = Substitute.For<IHRMAuthService>();
        _employeeLogService = Substitute.For<IEmployeeLogService>();
        _teamService = Substitute.For<ITeamService>();
        _jobLevelService = Substitute.For<IJobLevelService>();
        _managementLevelService = Substitute.For<IManagementLevelService>();
        _cryptoService = Substitute.For<ICryptoService>();
        _userService = Substitute.For<IUserService>();
        _roleService = Substitute.For<IRoleService>();
        _minIOService = Substitute.For<IMinIOService>();
        _companyService = Substitute.For<ICompanyService>();
        _payrollSettingService = Substitute.For<IPayrollSettingService>();
        _salaryStructureService = Substitute.For<ISalaryStructureService>();

        _controller = new EmployeeController(
            _session,
            _employeeService,
            _commonDataService,
            _authService,
            _employeeLogService,
            _teamService,
            _jobLevelService,
            _managementLevelService,
            _cryptoService,
            _userService,
            _roleService,
            _minIOService,
            _companyService,
            _payrollSettingService,
            _salaryStructureService
        );
    }

    #region Index Tests

    [Fact]
    public async Task Index_ShouldReturnViewWithModel()
    {
        // Arrange
        var teams = new List<Team>
        {
            new() { Id = 1, Name = "Team 1" },
            new() { Id = 2, Name = "Team 2" }
        };
        _teamService.GetAllAsync().Returns(teams);

        // Act
        var result = await _controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<UserManagementViewModel>(viewResult.Model);
        Assert.True(model.IsEnable);
        Assert.Equal(Constants.CurrentPage, model.CurrentPage);
        Assert.Equal(Constants.ItemsPerPage, model.ItemsPerPage);
        Assert.Equal("1,2", model.TeamIds);
    }

    [Fact]
    public async Task Index_ShouldCallTeamServiceGetAllAsync()
    {
        // Arrange
        _teamService.GetAllAsync().Returns(new List<Team>());

        // Act
        await _controller.Index();

        // Assert
        await _teamService.Received(1).GetAllAsync();
    }

    #endregion

    #region ContactList Tests

    [Fact]
    public async Task ContactList_WithNullModel_ShouldReturnPartialViewWithEmptyList()
    {
        // Arrange
        UserManagementViewModel model = null;

        // Act
        var result = await _controller.ContactList(model);

        // Assert
        var partialViewResult = Assert.IsType<PartialViewResult>(result);
        Assert.Equal("_ContactList", partialViewResult.ViewName);
        var employees = Assert.IsType<List<Employee>>(partialViewResult.Model);
        Assert.Empty(employees);
    }

    [Fact]
    public async Task ContactList_WithValidModel_ShouldReturnPartialViewWithEmployees()
    {
        // Arrange
        var model = new UserManagementViewModel
        {
            Keyword = "test",
            TeamIds = "1,2",
            IsEnable = true,
            IsLocal = true,
            CurrentPage = 1,
            ItemsPerPage = 10
        };

        var employees = new List<Employee>
        {
            new() { Id = 1, FullName = "Test User 1" },
            new() { Id = 2, FullName = "Test User 2" }
        };

        _employeeService.GetEmployeeAsync(Arg.Any<GetAllEmployeeCriteria>()).Returns(employees);
        _roleService.GetByUserIdAsync(Arg.Any<int>()).Returns(new List<Role>());
        _commonDataService.IsEnableGroupDateAsync().Returns(false);

        // Act
        var result = await _controller.ContactList(model);

        // Assert
        var partialViewResult = Assert.IsType<PartialViewResult>(result);
        Assert.Equal("_ContactList", partialViewResult.ViewName);
        var resultEmployees = Assert.IsType<List<Employee>>(partialViewResult.Model);
        Assert.Equal(2, resultEmployees.Count);
    }

    [Fact]
    public async Task ContactList_ShouldCallEmployeeServiceWithCorrectCriteria()
    {
        // Arrange
        var model = new UserManagementViewModel
        {
            Keyword = "search",
            TeamIds = "1,2,3",
            IsEnable = false,
            IsLocal = true,
            CurrentPage = 2,
            ItemsPerPage = 20
        };

        _employeeService.GetEmployeeAsync(Arg.Any<GetAllEmployeeCriteria>()).Returns(new List<Employee>());
        _roleService.GetByUserIdAsync(Arg.Any<int>()).Returns(new List<Role>());
        _commonDataService.IsEnableGroupDateAsync().Returns(false);

        // Act
        await _controller.ContactList(model);

        // Assert
        await _employeeService.Received(1).GetEmployeeAsync(
            Arg.Is<GetAllEmployeeCriteria>(c =>
                c.Query == model.Keyword &&
                c.TeamIds == model.TeamIds &&
                c.EnableUser == model.IsEnable &&
                c.IsLocalUser == model.IsLocal &&
                c.PageNumber == model.CurrentPage &&
                c.PageSize == model.ItemsPerPage &&
                c.CompanyId == 3
            )
        );
    }

    [Fact]
    public async Task ContactList_ShouldSetViewBagIsShowJobtitle_WhenUserIsHROrSystemAdmin()
    {
        // Arrange
        var model = new UserManagementViewModel();
        var roles = new List<Role>
        {
            new() { RoleId = (short)RoleSystem.HR }
        };

        _employeeService.GetEmployeeAsync(Arg.Any<GetAllEmployeeCriteria>()).Returns(new List<Employee>());
        _roleService.GetByUserIdAsync(Arg.Any<int>()).Returns(roles);
        _commonDataService.IsEnableGroupDateAsync().Returns(false);

        // Act
        var result = await _controller.ContactList(model);

        // Assert
        Assert.True((bool)_controller.ViewBag.IsShowJobtitle);
    }

    #endregion

    #region UserManagement Tests

    [Fact]
    public async Task UserManagement_ShouldReturnViewWithModel()
    {
        // Arrange
        var teams = new List<Team> { new() { Id = 1 } };
        var lookupData = new List<LookupData> { new() { Id = 1, Name = "Title 1" } };

        _teamService.GetAllAsync().Returns(teams);
        _commonDataService.GetLookupDataByCodeAsync(Arg.Any<string>(), Arg.Any<int>()).Returns(lookupData);

        // Act
        var result = await _controller.UserManagement();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<UserManagementViewModel>(viewResult.Model);
        Assert.True(model.IsEnable);
        Assert.NotNull(model.JobTitles);
    }

    [Fact]
    public async Task UserManagement_WithJobTitleId_ShouldSelectCorrectJobTitle()
    {
        // Arrange
        var jobTitleId = 5;
        var teams = new List<Team> { new() { Id = 1 } };
        var lookupData = new List<LookupData>
        {
            new() { Id = 5, Name = "Manager" },
            new() { Id = 6, Name = "Developer" }
        };

        _teamService.GetAllAsync().Returns(teams);
        _commonDataService.GetLookupDataByCodeAsync(Arg.Any<string>(), Arg.Any<int>()).Returns(lookupData);

        // Act
        var result = await _controller.UserManagement(jobTitleId);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<UserManagementViewModel>(viewResult.Model);
        Assert.Equal(jobTitleId, model.JobTitleId);
        Assert.Contains(model.JobTitles, j => j.Selected && j.Value == jobTitleId.ToString());
    }

    #endregion

    #region ListEmployee Tests

    [Fact]
    public async Task ListEmployee_WithValidModel_ShouldReturnPartialView()
    {
        // Arrange
        var model = new UserManagementViewModel
        {
            Keyword = "test",
            TeamIds = "1",
            IsEnable = true,
            CurrentPage = 1,
            ItemsPerPage = 10
        };

        var employees = new List<Employee> { new() { Id = 1 } };
        _employeeService.GetEmployeeAsync(Arg.Any<GetAllEmployeeCriteria>()).Returns(employees);
        _commonDataService.IsEnableGroupDateAsync().Returns(false);

        // Act
        var result = await _controller.ListEmployee(model);

        // Assert
        var partialViewResult = Assert.IsType<PartialViewResult>(result);
        Assert.Equal("_ListEmployee", partialViewResult.ViewName);
    }

    [Fact]
    public async Task ListEmployee_WithNullModel_ShouldReturnEmptyList()
    {
        // Arrange
        _commonDataService.IsEnableGroupDateAsync().Returns(false);

        // Act
        var result = await _controller.ListEmployee(null);

        // Assert
        var partialViewResult = Assert.IsType<PartialViewResult>(result);
        var employees = Assert.IsType<List<Employee>>(partialViewResult.Model);
        Assert.Empty(employees);
    }

    [Fact]
    public async Task ListEmployee_ShouldSetViewBagPagerOption()
    {
        // Arrange
        var model = new UserManagementViewModel
        {
            CurrentPage = 2,
            ItemsPerPage = 15
        };

        var criteria = new GetAllEmployeeCriteria { TotalRow = 100 };
        _employeeService.GetEmployeeAsync(Arg.Any<GetAllEmployeeCriteria>())
            .Returns(callInfo =>
            {
                var param = callInfo.Arg<GetAllEmployeeCriteria>();
                param.TotalRow = 100;
                return new List<Employee>();
            });
        _commonDataService.IsEnableGroupDateAsync().Returns(false);

        // Act
        await _controller.ListEmployee(model);

        // Assert
        Assert.NotNull(_controller.ViewBag.PagerOption);
    }

    #endregion

    #region Detail Tests

    [Fact]
    public async Task Detail_WithValidId_ShouldReturnViewWithEmployee()
    {
        // Arrange
        var employeeId = 1;
        var employee = new Employee { Id = employeeId, FullName = "Test User" };
        var employeeShift = new EmployeeShift();
        var historyWork = new List<LogObjectDetail>();

        _employeeService.GetEmployeeByIdAsync(employeeId).Returns(employee);
        _employeeService.GetShiftByEmployeeId(employeeId).Returns(employeeShift);
        _employeeLogService.GetTransferHistoryWork(employeeId).Returns(historyWork);
        _commonDataService.IsEnableGroupDateAsync().Returns(false);
        _employeeService.GetEmployeePayrollAllowanceByEmpIdAsync(employeeId).Returns(new List<EmployeePayrollAllowance>());

        // Act
        var result = await _controller.Detail(employeeId);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<Employee>(viewResult.Model);
        Assert.Equal(employeeId, model.Id);
    }

    [Fact]
    public async Task Detail_ShouldSetShiftViewBags()
    {
        // Arrange
        var employeeId = 1;
        var employee = new Employee { Id = employeeId };
        var employeeShift = new EmployeeShift
        {
            SunShiftName = "Morning",
            MonShiftName = "Evening",
            TueShiftName = "Night"
        };

        _employeeService.GetEmployeeByIdAsync(employeeId).Returns(employee);
        _employeeService.GetShiftByEmployeeId(employeeId).Returns(employeeShift);
        _employeeLogService.GetTransferHistoryWork(employeeId).Returns(new List<LogObjectDetail>());
        _commonDataService.IsEnableGroupDateAsync().Returns(false);
        _employeeService.GetEmployeePayrollAllowanceByEmpIdAsync(employeeId).Returns(new List<EmployeePayrollAllowance>());

        // Act
        await _controller.Detail(employeeId);

        // Assert
        Assert.Equal("Morning", _controller.ViewBag.Sun);
        Assert.Equal("Evening", _controller.ViewBag.Mon);
        Assert.Equal("Night", _controller.ViewBag.Tue);
    }

    #endregion

    #region Create Tests

    [Fact]
    public void Create_Get_ShouldReturnViewWithEmptyModel()
    {
        // Act
        var result = _controller.Create();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<CreateEmployeeViewModel>(viewResult.Model);
        Assert.NotNull(model);
    }

    [Fact]
    public async Task Create_Post_WithInvalidModel_ShouldReturnViewWithModel()
    {
        // Arrange
        var model = new CreateEmployeeViewModel();
        _controller.ModelState.AddModelError("Code", "Code is required");

        // Act
        var result = await _controller.Create(model);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(model, viewResult.Model);
    }

    [Fact]
    public async Task Create_Post_WithExistingEmployee_ShouldReturnViewWithError()
    {
        // Arrange
        var model = new CreateEmployeeViewModel
        {
            Code = "EMP001",
            Nickname = "Test"
        };

        var commandResult = new CommandResult
        {
            Code = 1,
            Message = "Employee already exists"
        };

        _employeeService.CheckExistAsync(model.Code, model.Nickname).Returns(commandResult);

        // Act
        var result = await _controller.Create(model);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.False(_controller.ModelState.IsValid);
    }

    [Fact]
    public async Task Create_Post_WithValidModel_ShouldCallUserServiceCreate()
    {
        // Arrange
        var model = new CreateEmployeeViewModel
        {
            Username = "testuser",
            Code = "EMP001",
            Nickname = "Test",
            FullName = "Test User",
            TeamId = 1
        };

        var checkResult = new CommandResult { Code = 0 };
        var createUserResult = new GO.UserControl.Client.Core.Models.CommandResult { Code = 0, ObjectId = 100 };
        var createEmployeeResult = new CommandResult { Code = 0, OutputId = 1 };

        _employeeService.CheckExistAsync(model.Code, model.Nickname).Returns(checkResult);
        _cryptoService.GenerateRijndaelKeyIV().Returns("key:iv");
        _cryptoService.Encrypt(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>()).Returns("encrypted");
        _userService.CreateAsync(Arg.Any<CreateUserParam>()).Returns(createUserResult);
        _employeeService.CreateAsync(Arg.Any<CreateEmployeeParam>()).Returns(createEmployeeResult);

        // Act
        var result = await _controller.Create(model);

        // Assert
        await _userService.Received(1).CreateAsync(Arg.Any<CreateUserParam>());
        await _employeeService.Received(1).CreateAsync(Arg.Any<CreateEmployeeParam>());
    }

    [Fact]
    public async Task Create_Post_WithSuccessfulCreation_ShouldRedirectToEdit()
    {
        // Arrange
        var model = new CreateEmployeeViewModel
        {
            Username = "testuser",
            Code = "EMP001",
            Nickname = "Test",
            FullName = "Test User",
            TeamId = 1
        };

        var checkResult = new CommandResult { Code = 0 };
        var createUserResult = new GO.UserControl.Client.Core.Models.CommandResult { Code = 0, ObjectId = 100 };
        var createEmployeeResult = new CommandResult { Code = 0, OutputId = 5 };

        _employeeService.CheckExistAsync(model.Code, model.Nickname).Returns(checkResult);
        _cryptoService.GenerateRijndaelKeyIV().Returns("key:iv");
        _cryptoService.Encrypt(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>()).Returns("encrypted");
        _userService.CreateAsync(Arg.Any<CreateUserParam>()).Returns(createUserResult);
        _employeeService.CreateAsync(Arg.Any<CreateEmployeeParam>()).Returns(createEmployeeResult);

        // Act
        var result = await _controller.Create(model);

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Edit", redirectResult.ActionName);
        Assert.Equal(5L, redirectResult.RouteValues["id"]);
    }

    #endregion

    #region Edit Tests

    [Fact]
    public async Task Edit_Get_WithValidId_ShouldReturnViewWithEmployee()
    {
        // Arrange
        var employeeId = 1;
        var employee = new Employee
        {
            Id = employeeId,
            IsLocal = true
        };

        SetupPermission(FunctionKey.ManageLocalEmployee, Policy.Host, true);
        SetupPermission(FunctionKey.ManageExpatEmployee, Policy.Host, false);

        _employeeService.GetEmployeeByIdAsync(employeeId).Returns(employee);
        _commonDataService.GetAllCountry().Returns(new List<Country>());
        _employeeService.GetEmployeeSupervisorByRoleIdAsync(Arg.Any<int>(), Arg.Any<short>()).Returns(new List<SupervisorModel>());
        _commonDataService.IsEnableGroupDateAsync().Returns(false);
        _minIOService.GetFilePath(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>()).Returns("avatar.jpg");
        _commonDataService.GetLookupDataByCodeAsync(Arg.Any<string>(), Arg.Any<int>()).Returns(new List<LookupData>());
        _jobLevelService.GetJobLevel().Returns(new List<JobLevel>());
        _managementLevelService.GetManagementLevel().Returns(new List<ManagementLevel>());
        _roleService.GetBySiteIdAsync(Arg.Any<short>()).Returns(new List<Role>());
        _roleService.GetByUserIdAsync(Arg.Any<int>()).Returns(new List<Role>());
        _companyService.GetSubCompanies(Arg.Any<short>()).Returns(new List<SubCompany>());
        _salaryStructureService.GetSalaryStructureAsync(Arg.Any<int>()).Returns(new List<SalaryStructure>());
        _payrollSettingService.GetPayrollItemSettingsAsync(Arg.Any<GetPayrollItemSettingsCriteria>()).Returns(new List<PayrollSetting>());

        // Act
        var result = await _controller.Edit(employeeId);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<Employee>(viewResult.Model);
        Assert.Equal(employeeId, model.Id);
    }

    [Fact]
    public async Task Edit_Get_WithNonExistentEmployee_ShouldRedirectToPageNotFound()
    {
        // Arrange
        var employeeId = 999;
        _employeeService.GetEmployeeByIdAsync(employeeId).Returns((Employee)null);

        // Act
        var result = await _controller.Edit(employeeId);

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("PageNotFound", redirectResult.ActionName);
        Assert.Equal("Error", redirectResult.ControllerName);
    }

    [Fact]
    public async Task Edit_Post_WithValidModel_ShouldCallEmployeeServiceEdit()
    {
        // Arrange
        var model = new Employee
        {
            Id = 1,
            UserId = 100,
            Code = "EMP001",
            FullName = "Updated Name",
            NickName = "Test",
            Team = new Team { Id = 1, Name = "Team 1" },
            Roles = new List<Role>()
        };

        var commandResult = new CommandResult { Code = 0 };
        var updateResult = new GO.UserControl.Client.Core.Models.CommandResult { Code = 0 };

        _employeeService.EditAsync(Arg.Any<int>(), Arg.Any<EditEmployeeParam>(), Arg.Any<short>()).Returns(commandResult);
        _employeeService.UpdateUserInfoAsync(Arg.Any<int>(), Arg.Any<EditEmployeeParam>()).Returns(updateResult);
        _userService.AssignRoleAsync(Arg.Any<AssignRoleParam>()).Returns(new GO.UserControl.Client.Core.Models.CommandResult());
        _commonDataService.GetAllCountry().Returns(new List<Country>());
        _employeeService.GetEmployeeSupervisorByRoleIdAsync(Arg.Any<int>(), Arg.Any<short>()).Returns(new List<SupervisorModel>());
        _commonDataService.IsEnableGroupDateAsync().Returns(false);
        _minIOService.GetFilePath(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>()).Returns("avatar.jpg");
        _commonDataService.GetLookupDataByCodeAsync(Arg.Any<string>(), Arg.Any<int>()).Returns(new List<LookupData>());
        _jobLevelService.GetJobLevel().Returns(new List<JobLevel>());
        _managementLevelService.GetManagementLevel().Returns(new List<ManagementLevel>());
        _roleService.GetBySiteIdAsync(Arg.Any<short>()).Returns(new List<Role>());
        _roleService.GetBySiteId(Arg.Any<short>()).Returns(new List<Role> { new() { RoleId = 1, IsDefault = true } });
        _roleService.GetByUserIdAsync(Arg.Any<int>()).Returns(new List<Role>());
        _companyService.GetSubCompanies(Arg.Any<short>()).Returns(new List<SubCompany>());
        _salaryStructureService.GetSalaryStructureAsync(Arg.Any<int>()).Returns(new List<SalaryStructure>());
        _payrollSettingService.GetPayrollItemSettingsAsync(Arg.Any<GetPayrollItemSettingsCriteria>()).Returns(new List<PayrollSetting>());

        // Act
        var result = await _controller.Edit(model);

        // Assert
        await _employeeService.Received(1).EditAsync(
            Arg.Any<int>(),
            Arg.Any<EditEmployeeParam>(),
            Arg.Any<short>()
        );
    }

    [Fact]
    public async Task Edit_Post_WithInvalidModel_ShouldReturnView()
    {
        // Arrange
        var model = new Employee 
        { 
            Id = 1, 
            Code = "EMP001",
            NickName = "Test",
            Team = new Team { Id = 1, Name = "Team 1" },
            Roles = new List<Role>() 
        };
        _controller.ModelState.AddModelError("FullName", "Full name is required");

        _commonDataService.GetAllCountry().Returns(new List<Country>());
        _employeeService.GetEmployeeSupervisorByRoleIdAsync(Arg.Any<int>(), Arg.Any<short>()).Returns(new List<SupervisorModel>());
        _commonDataService.IsEnableGroupDateAsync().Returns(false);
        _minIOService.GetFilePath(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>()).Returns("avatar.jpg");
        _commonDataService.GetLookupDataByCodeAsync(Arg.Any<string>(), Arg.Any<int>()).Returns(new List<LookupData>());
        _jobLevelService.GetJobLevel().Returns(new List<JobLevel>());
        _managementLevelService.GetManagementLevel().Returns(new List<ManagementLevel>());
        _roleService.GetBySiteIdAsync(Arg.Any<short>()).Returns(new List<Role>());
        _roleService.GetBySiteId(Arg.Any<short>()).Returns(new List<Role>());
        _roleService.GetByUserIdAsync(Arg.Any<int>()).Returns(new List<Role>());
        _companyService.GetSubCompanies(Arg.Any<short>()).Returns(new List<SubCompany>());
        _salaryStructureService.GetSalaryStructureAsync(Arg.Any<int>()).Returns(new List<SalaryStructure>());
        _payrollSettingService.GetPayrollItemSettingsAsync(Arg.Any<GetPayrollItemSettingsCriteria>()).Returns(new List<PayrollSetting>());

        // Act
        var result = await _controller.Edit(model);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(model, viewResult.Model);
    }

    #endregion

    #region Delete Tests

    [Fact]
    public async Task Delete_WithValidParam_ShouldCallEmployeeAndUserService()
    {
        // Arrange
        var param = new DeleteEmployeeParam
        {
            EmployeeId = 1,
            UserId = 100
        };

        var employeeResult = new CommandResult { Code = 0 };
        var userResult = new GO.UserControl.Client.Core.Models.CommandResult { Code = 0 };

        _employeeService.DeleteAsync(Arg.Any<int>(), Arg.Any<DeleteEmployeeParam>()).Returns(employeeResult);
        _userService.DeleteAsync(Arg.Any<int>()).Returns(userResult);

        // Act
        var result = await _controller.Delete(param);

        // Assert
        await _employeeService.Received(1).DeleteAsync(1, param); // From SetupEmployeeSession
        await _userService.Received(1).DeleteAsync(param.UserId);
        var jsonResult = Assert.IsType<JsonResult>(result);
    }

    [Fact]
    public async Task Delete_WithFailedEmployeeDelete_ShouldStillCallUserService()
    {
        // Arrange
        var param = new DeleteEmployeeParam
        {
            EmployeeId = 1,
            UserId = 100
        };

        var employeeResult = new CommandResult { Code = 1 };
        var userResult = new GO.UserControl.Client.Core.Models.CommandResult { Code = 0 };

        _employeeService.DeleteAsync(Arg.Any<int>(), Arg.Any<DeleteEmployeeParam>()).Returns(employeeResult);
        _userService.DeleteAsync(Arg.Any<int>()).Returns(userResult);

        // Act
        var result = await _controller.Delete(param);

        // Assert
        await _userService.Received(1).DeleteAsync(param.UserId);
    }

    #endregion

    #region Avatar Tests

    [Fact]
    public async Task Avatar_WithEmployeeId_ShouldReturnAvatarUrl()
    {
        // Arrange
        var employeeId = "EMP001";
        var expectedUrl = "https://cdn.example.com/avatar.jpg";

        _minIOService.GetFilePath(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(expectedUrl);

        // Act
        var result = await _controller.Avatar(employeeId);

        // Assert
        var contentResult = Assert.IsType<ContentResult>(result);
        Assert.Equal(expectedUrl, contentResult.Content);
        Assert.Equal("text/plain", contentResult.ContentType);
    }

    [Fact]
    public async Task Avatar_WithNullId_ShouldUseSessionEmployeeCode()
    {
        // Arrange
        var expectedUrl = "https://cdn.example.com/avatar.jpg";
        _minIOService.GetFilePath(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(expectedUrl);

        // Act
        var result = await _controller.Avatar(null);

        // Assert
        await _minIOService.Received(1).GetFilePath(
            Bucket.Avatar,
            Arg.Is<string>(s => s.Contains(".jpg")),
            "default-avatar.jpg"
        );
    }

    #endregion

    #region UploadAvatar Tests

    [Fact]
    public async Task UploadAvatar_WithNullFile_ShouldReturnNull()
    {
        // Arrange
        var code = "EMP001";
        IFormFile file = null;

        // Act
        var result = await _controller.UploadAvatar(code, file);

        // Assert
        var jsonResult = Assert.IsType<JsonResult>(result);
        Assert.Null(jsonResult.Value);
    }

    [Fact]
    public async Task UploadAvatar_WithValidFile_ShouldUploadAndReturnUrl()
    {
        // Arrange
        var code = "EMP001";
        var file = Substitute.For<IFormFile>();
        file.Length.Returns(1024);

        var expectedUrl = "https://cdn.example.com/avatar.jpg";
        _minIOService.UploadFileAsync(Arg.Any<IFormFile>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(true);
        _minIOService.GetFilePath(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(expectedUrl);

        // Act
        var result = await _controller.UploadAvatar(code, file);

        // Assert
        await _minIOService.Received(1).UploadFileAsync(file, Bucket.Avatar, $"3/{code}.jpg");
        var jsonResult = Assert.IsType<JsonResult>(result);
        Assert.Equal(expectedUrl, jsonResult.Value);
    }

    [Fact]
    public async Task UploadAvatar_WithUploadFailure_ShouldReturnNull()
    {
        // Arrange
        var code = "EMP001";
        var file = Substitute.For<IFormFile>();
        file.Length.Returns(1024);

        _minIOService.UploadFileAsync(Arg.Any<IFormFile>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(false);

        // Act
        var result = await _controller.UploadAvatar(code, file);

        // Assert
        var jsonResult = Assert.IsType<JsonResult>(result);
        Assert.Null(jsonResult.Value);
    }

    #endregion

    #region GetEmployees Tests

    [Fact]
    public async Task GetEmployees_WithValidCriteria_ShouldReturnJsonWithEmployees()
    {
        // Arrange
        var query = "test";
        var teamIds = "1,2";
        var enable = true;
        var isLocal = true;

        var employees = new List<Employee>
        {
            new() { Id = 1, FullName = "Test Employee 1" },
            new() { Id = 2, FullName = "Test Employee 2" }
        };

        _employeeService.GetEmployeeAsync(Arg.Any<GetAllEmployeeCriteria>()).Returns(employees);

        // Act
        var result = await _controller.GetEmployees(query, teamIds, enable, null, null, isLocal);

        // Assert
        var jsonResult = Assert.IsType<JsonResult>(result);
        Assert.Equal(employees, jsonResult.Value);
    }

    [Fact]
    public async Task GetEmployees_ShouldCallServiceWithCorrectCriteria()
    {
        // Arrange
        var query = "search";
        var teamIds = "1,2,3";
        var enable = false;
        var titleId = 5;

        _employeeService.GetEmployeeAsync(Arg.Any<GetAllEmployeeCriteria>()).Returns(new List<Employee>());

        // Act
        await _controller.GetEmployees(query, teamIds, enable, null, titleId, null);

        // Assert
        await _employeeService.Received(1).GetEmployeeAsync(
            Arg.Is<GetAllEmployeeCriteria>(c =>
                c.Query == query &&
                c.TeamIds == teamIds &&
                c.EnableUser == enable &&
                c.TitleId == titleId &&
                c.CompanyId == 3
            )
        );
    }

    #endregion

    #region GetSupervisors Tests

    [Fact]
    public async Task GetSupervisors_WithValidParams_ShouldReturnJsonWithEmployees()
    {
        // Arrange
        var roleIds = "1,2";
        var query = "test";
        var excludeIds = "10,20";

        var employees = new List<Employee>
        {
            new() { Id = 1, FullName = "Supervisor 1" }
        };

        _employeeService.GetEmployeesByRoleAsync(Arg.Any<GetEmployeeByRoleCriteria>()).Returns(employees);

        // Act
        var result = await _controller.GetSupervisors(roleIds, query, excludeIds);

        // Assert
        var jsonResult = Assert.IsType<JsonResult>(result);
        Assert.Equal(employees, jsonResult.Value);
    }

    [Fact]
    public async Task GetSupervisors_ShouldCallServiceWithCorrectCriteria()
    {
        // Arrange
        var roleIds = "1,2";
        var query = "  test  ";
        var excludeIds = "10";

        _employeeService.GetEmployeesByRoleAsync(Arg.Any<GetEmployeeByRoleCriteria>()).Returns(new List<Employee>());

        // Act
        await _controller.GetSupervisors(roleIds, query, excludeIds);

        // Assert
        await _employeeService.Received(1).GetEmployeesByRoleAsync(
            Arg.Is<GetEmployeeByRoleCriteria>(c =>
                c.RoleIds == roleIds &&
                c.Query == "test" && // Should be trimmed
                c.ExcludeIds == excludeIds &&
                c.CompanyId == 3
            )
        );
    }

    [Fact]
    public async Task GetSupervisors_WithNullExcludeIds_ShouldUseEmptyString()
    {
        // Arrange
        var roleIds = "1";
        var query = "test";

        _employeeService.GetEmployeesByRoleAsync(Arg.Any<GetEmployeeByRoleCriteria>()).Returns(new List<Employee>());

        // Act
        await _controller.GetSupervisors(roleIds, query, null);

        // Assert
        await _employeeService.Received(1).GetEmployeesByRoleAsync(
            Arg.Is<GetEmployeeByRoleCriteria>(c => c.ExcludeIds == string.Empty)
        );
    }

    #endregion

    #region ManageSupervisor Tests

    [Fact]
    public void ManageSupervisor_ShouldReturnViewWithViewBagSet()
    {
        // Arrange
        var employeeId = 1;
        var roleId = (short)RoleSystem.Verifier;

        // Act
        var result = _controller.ManageSupervisor(employeeId, roleId);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(employeeId, _controller.ViewBag.EmployeeId);
        Assert.Equal(roleId, _controller.ViewBag.RoleId);
        Assert.Equal(3, _controller.ViewBag.CompanyId);
    }

    #endregion

    #region GetEmployeeSupervisorByRoleId Tests

    [Fact]
    public async Task GetEmployeeSupervisorByRoleId_ShouldReturnJsonWithSupervisors()
    {
        // Arrange
        var employeeId = 1;
        var roleId = (short)RoleSystem.Approver;
        var supervisors = new List<SupervisorModel>
        {
            new() { OwnerId = 10, NickName = "Supervisor 1" }
        };

        _employeeService.GetEmployeeSupervisorByRoleIdAsync(employeeId, roleId).Returns(supervisors);

        // Act
        var result = await _controller.GetEmployeeSupervisorByRoleId(employeeId, roleId);

        // Assert
        var jsonResult = Assert.IsType<JsonResult>(result);
        Assert.Equal(supervisors, jsonResult.Value);
    }

    #endregion

    #region EditEmployeeSupervisor Tests

    [Fact]
    public async Task EditEmployeeSupervisor_ShouldCallServiceAndReturnJson()
    {
        // Arrange
        var employeeId = 1;
        var roleId = (short)RoleSystem.Verifier;
        var supervisors = new List<EditTeamSupervisor>
        {
            new() { OwnerId = 10 }
        };

        var commandResult = new CommandResult { Code = 0 };
        _employeeService.EditEmployeeSupervisorAsync(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<short>(), Arg.Any<IEnumerable<EditTeamSupervisor>>())
            .Returns(commandResult);

        // Act
        var result = await _controller.EditEmployeeSupervisor(employeeId, roleId, supervisors);

        // Assert
        await _employeeService.Received(1).EditEmployeeSupervisorAsync(1, employeeId, roleId, supervisors);
        var jsonResult = Assert.IsType<JsonResult>(result);
        Assert.Equal(commandResult, jsonResult.Value);
    }

    #endregion

    #region GetMemberByManager Tests

    [Fact]
    public async Task GetMemberByManager_ShouldReturnTop10Employees()
    {
        // Arrange
        var keyword = "test";
        var employees = Enumerable.Range(1, 15)
            .Select(i => new Employee { Id = i, FullName = $"Employee {i}" })
            .ToList();

        _employeeService.GetMemberByManagerAsync(Arg.Any<GetMemberByManagerCriteria>()).Returns(employees);

        // Act
        var result = await _controller.GetMemberByManager(keyword);

        // Assert
        var jsonResult = Assert.IsType<JsonResult>(result);
        var resultList = Assert.IsAssignableFrom<IEnumerable<Employee>>(jsonResult.Value);
        Assert.Equal(10, resultList.Count());
    }

    [Fact]
    public async Task GetMemberByManager_ShouldCallServiceWithCorrectCriteria()
    {
        // Arrange
        var keyword = "search";
        var companyId = (short)5;

        _employeeService.GetMemberByManagerAsync(Arg.Any<GetMemberByManagerCriteria>()).Returns(new List<Employee>());

        // Act
        await _controller.GetMemberByManager(keyword, companyId);

        // Assert
        await _employeeService.Received(1).GetMemberByManagerAsync(
            Arg.Is<GetMemberByManagerCriteria>(c =>
                c.SupervisorId == 1 && // From SetupEmployeeSession
                c.Query == keyword &&
                c.ExcludedManager == false &&
                c.CompanyId == companyId
            )
        );
    }

    #endregion

    #region GetMemberByGender Tests

    [Fact]
    public async Task GetMemberByGender_ShouldReturnJsonWithEmployees()
    {
        // Arrange
        var keyword = "test";
        var gender = 1;
        var employees = new List<Employee>
        {
            new() { Id = 1, Gender = (byte?)gender }
        };

        _employeeService.GetEmployeeByGender(keyword, gender, Arg.Any<short>()).Returns(employees);

        // Act
        var result = await _controller.GetMemberByGender(keyword, gender);

        // Assert
        var jsonResult = Assert.IsType<JsonResult>(result);
        Assert.NotNull(jsonResult.Value);
    }

    #endregion

    #region GenerateEmployeeCode Tests

    [Fact]
    public async Task GenerateEmployeeCode_ShouldReturnJsonWithCode()
    {
        // Arrange
        var companyId = (short)5;
        var expectedCode = "EMP00123";

        _employeeService.GenerateEmployeeCode(companyId).Returns(expectedCode);

        // Act
        var result = await _controller.GenerateEmployeeCode(companyId);

        // Assert
        var jsonResult = Assert.IsType<JsonResult>(result);
        Assert.Equal(expectedCode, jsonResult.Value);
    }

    #endregion

    #region ActivityLog Tests

    [Fact]
    public async Task ActivityLog_ShouldReturnViewWithLogs()
    {
        // Arrange
        var employeeId = 1;
        var employee = new Employee
        {
            Id = employeeId,
            NickName = "Test",
            FullName = "Test User"
        };
        var logs = new List<LogObjectDetail>
        {
            new() { UserId = 1, FullName = "Test User" }
        };

        _employeeService.GetEmployeeByIdAsync(employeeId).Returns(employee);
        _employeeLogService.GetLogDetailByIdAsync(employeeId).Returns(logs);
        _commonDataService.IsEnableGroupDateAsync().Returns(false);

        // Act
        var result = await _controller.ActivityLog(employeeId);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("ActivityLog", viewResult.ViewName);
        Assert.Equal(logs, viewResult.Model);
        Assert.Equal("Test (Test User)", _controller.ViewBag.Employee);
    }

    #endregion

    #region ShiftManager Tests

    [Fact]
    public void ShiftManager_ShouldReturnViewWithViewBagSet()
    {
        // Arrange
        var employeeId = 1;

        // Act
        var result = _controller.ShiftManager(employeeId);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(employeeId, _controller.ViewBag.EmployeeId);
        Assert.Equal((short)RoleSystem.ShiftManager, _controller.ViewBag.RoleId);
        Assert.Equal(3, _controller.ViewBag.CompanyId);
    }

    #endregion

    #region GetShiftManagerByEmployeeId Tests

    [Fact]
    public async Task GetShiftManagerByEmployeeId_ShouldReturnJsonWithSupervisors()
    {
        // Arrange
        var employeeId = 1;
        var supervisors = new List<SupervisorModel>
        {
            new()
            {
                OwnerId = 10,
                NickName = "Manager 1",
                FullName = "Full Manager 1",
                CompanyName = "Company A",
                CompanyId = 1,
                CanRemove = true
            }
        };

        _employeeService.GetEmployeeSupervisorByRoleIdAsync(employeeId, (short)RoleSystem.ShiftManager)
            .Returns(supervisors);

        // Act
        var result = await _controller.GetShiftManagerByEmployeeId(employeeId);

        // Assert
        var jsonResult = Assert.IsType<JsonResult>(result);
        var resultList = Assert.IsAssignableFrom<IEnumerable<Supervisor>>(jsonResult.Value);
        Assert.Single(resultList);
    }

    #endregion

    #region ToggleLeftNavigation Tests

    [Fact]
    public void ToggleLeftNavigation_ShouldSetSessionAndReturnOk()
    {
        // Arrange
        var isShow = true;

        // Act
        var result = _controller.ToggleLeftNavigation(isShow);

        // Assert
        _session.Received(1).Set(SessionKey.IsShowLeftNavigation, isShow);
        Assert.IsType<OkResult>(result);
    }

    #endregion

    #region DisciplineManager Tests

    [Fact]
    public void DisciplineManager_ShouldReturnViewWithViewBagSet()
    {
        // Arrange
        var employeeId = 1;

        // Act
        var result = _controller.DisciplineManager(employeeId);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(employeeId, _controller.ViewBag.EmployeeId);
        Assert.Equal((short)RoleSystem.DisciplineManager, _controller.ViewBag.RoleId);
        Assert.Equal(3, _controller.ViewBag.CompanyId);
    }

    #endregion

    #region GetDisciplineManagerByEmployeeId Tests

    [Fact]
    public async Task GetDisciplineManagerByEmployeeId_ShouldReturnJsonWithSupervisors()
    {
        // Arrange
        var employeeId = 1;
        var supervisors = new List<Supervisor>
        {
            new() { OwnerId = 10, Nickname = "Discipline Manager" }
        };

        _employeeService.GetDisciplineSettingByEmployeeIdAsync(employeeId).Returns(supervisors);

        // Act
        var result = await _controller.GetDisciplineManagerByEmployeeId(employeeId);

        // Assert
        var jsonResult = Assert.IsType<JsonResult>(result);
        Assert.Equal(supervisors, jsonResult.Value);
    }

    #endregion
}

