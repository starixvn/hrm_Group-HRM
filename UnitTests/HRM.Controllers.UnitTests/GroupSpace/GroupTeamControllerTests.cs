using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GO.UserControl.Client.Core.Enumerables;
using HRM.Controllers.GroupSpace;
using HRM.Core.Constants;
using HRM.Core.Enums;
using HRM.Core.Models;
using HRM.Core.Services;
using HRM.Criteria.Employee;
using HRM.Criteria.Group;
using HRM.Models;
using HRM.Models.Group;
using HRM.Models.Supervisor;
using HRM.Resources;
using HRM.Services.Interfaces;
using HRM.ViewModels.EmployeeGroup;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Xunit;

namespace HRM.Controllers.UnitTests.GroupSpace;

public class GroupTeamControllerTests : BaseControllerTest
{
    private readonly ITeamService _teamService;
    private readonly IDepartmentService _departmentService;
    private readonly IBaseService _baseService;
    private readonly IEmployeeService _employeeService;
    private readonly ICompanyService _companyService;
    private readonly GroupTeamController _controller;

    public GroupTeamControllerTests()
    {
        _teamService = Substitute.For<ITeamService>();
        _departmentService = Substitute.For<IDepartmentService>();
        _baseService = Substitute.For<IBaseService>();
        _employeeService = Substitute.For<IEmployeeService>();
        _companyService = Substitute.For<ICompanyService>();

        _controller = new GroupTeamController(
            _teamService,
            _session,
            _departmentService,
            _baseService,
            _employeeService,
            _companyService
        );
    }

    #region Index Tests

    [Fact]
    public void Index_WithPermission_ShouldReturnViewWithTrue()
    {
        // Arrange
        SetupPermission(FunctionKey.GroupTeamManagement, Policy.Host, true);

        // Act
        var result = _controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.True((bool)_controller.ViewBag.HasPermission);
        _session.Received(1).HasPermissionOnFunction(FunctionKey.GroupTeamManagement, Policy.Host);
    }

    [Fact]
    public void Index_WithoutPermission_ShouldReturnViewWithFalse()
    {
        // Arrange
        SetupPermission(FunctionKey.GroupTeamManagement, Policy.Host, false);

        // Act
        var result = _controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.False((bool)_controller.ViewBag.HasPermission);
    }

    #endregion

    #region GetTeamList Tests

    [Fact]
    public async Task GetTeamList_WithValidParameters_ShouldReturnPartialView()
    {
        // Arrange
        var teams = new List<GroupTeamModel>
        {
            new()
            {
                Id = 1,
                TeamCode = "TEAM001",
                TeamName = "Test Team 1",
                CompanyId = 1,
            },
            new()
            {
                Id = 2,
                TeamCode = "TEAM002",
                TeamName = "Test Team 2",
                CompanyId = 2,
            },
        };

        var filteredTeams = teams.ToList();
        _teamService.GetAllTeamByUnitScopeAsync("1,2", "1,2", "true").Returns(teams);
        _teamService.GetTeamEmployeeBelongsTo(teams, 1).Returns(filteredTeams);
        SetupPermission(FunctionKey.GroupTeamManagement, Policy.Host, true);
        SetupPermission(FunctionKey.GroupEmployeeList, Policy.Read, true);

        // Act
        var result = await _controller.GetTeamList("1,2", "1,2", "true");

        // Assert
        var partialViewResult = Assert.IsType<PartialViewResult>(result);
        Assert.Equal("_TeamList", partialViewResult.ViewName);
        Assert.Equal(filteredTeams, partialViewResult.Model);
        Assert.True((bool)_controller.ViewBag.HasPermission);
        Assert.True((bool)_controller.ViewBag.HasPermissionToViewEmployee);
        await _teamService.Received(1).GetAllTeamByUnitScopeAsync("1,2", "1,2", "true");
        _teamService.Received(1).GetTeamEmployeeBelongsTo(teams, 1);
    }

    [Fact]
    public async Task GetTeamList_WithNullParameters_ShouldReturnPartialView()
    {
        // Arrange
        var teams = new List<GroupTeamModel>();
        var filteredTeams = new List<GroupTeamModel>();
        _teamService.GetAllTeamByUnitScopeAsync(null, null, null).Returns(teams);
        _teamService.GetTeamEmployeeBelongsTo(teams, 1).Returns(filteredTeams);
        SetupPermission(FunctionKey.GroupTeamManagement, Policy.Host, false);
        SetupPermission(FunctionKey.GroupEmployeeList, Policy.Read, false);

        // Act
        var result = await _controller.GetTeamList();

        // Assert
        var partialViewResult = Assert.IsType<PartialViewResult>(result);
        Assert.Equal("_TeamList", partialViewResult.ViewName);
        Assert.False((bool)_controller.ViewBag.HasPermission);
        Assert.False((bool)_controller.ViewBag.HasPermissionToViewEmployee);
    }

    #endregion

    #region Create GET Tests

    [Fact]
    public async Task Create_Get_ShouldReturnViewWithViewModel()
    {
        // Arrange
        var groupTeamId = 100;
        var companies = new List<Company>
        {
            new() { CompanyId = 1, Name = "Company 1" },
            new() { CompanyId = 2, Name = "Company 2" },
        };
        var employeePics = new List<EmployeePIC>
        {
            new() { Id = 1, FullName = "Employee 1" },
            new() { Id = 2, FullName = "Employee 2" },
        };
        var departments = new List<Department>
        {
            new() { Id = 1, Name = "Department 1" },
        };

        _teamService.GetNextGroupTeamIdAsync().Returns(groupTeamId);
        _companyService.GetAllCompany().Returns(companies);
        _employeeService
            .GetEmployeeForPICAsync(Arg.Any<GetEmployeeForPICCriteria>())
            .Returns(employeePics);
        _departmentService.GetDepartmentAsync().Returns(departments);

        // Act
        var result = await _controller.Create();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<GroupTeamViewModel>(viewResult.Model);
        Assert.Equal(groupTeamId, model.GroupTeamId);
        Assert.Equal(companies, model.Companies);
        Assert.Equal(employeePics, model.EmployeePics);
        Assert.Equal(departments, model.Departments);
        await _teamService.Received(1).GetNextGroupTeamIdAsync();
        await _companyService.Received(1).GetAllCompany();
        await _employeeService
            .Received(1)
            .GetEmployeeForPICAsync(Arg.Any<GetEmployeeForPICCriteria>());
        await _departmentService.Received(1).GetDepartmentAsync();
    }

    #endregion

    #region Create POST Tests

    [Fact]
    public async Task Create_Post_WithValidModel_ShouldRedirectToIndex()
    {
        // Arrange
        var model = new GroupTeamViewModel
        {
            TeamCode = "TEAM001",
            TeamName = "Test Team",
            DepartmentId = 1,
            GroupTeamId = 100,
            PICEmpId = 5,
            AccountableArea = "Test Area",
            CompanyId = 1,
            IsOutsource = false,
        };

        var companies = new List<Company>
        {
            new() { CompanyId = 1, Name = "Company 1" },
        };
        var employeePics = new List<EmployeePIC>
        {
            new() { Id = 5, FullName = "PIC Employee" },
        };
        var departments = new List<Department>
        {
            new() { Id = 1, Name = "Department 1" },
        };

        var commandResult = new CommandResult { Code = 0, Message = "Success" };
        var insertedTeamId = 10;

        _companyService.GetAllCompany().Returns(companies);
        _employeeService
            .GetEmployeeForPICAsync(Arg.Any<GetEmployeeForPICCriteria>())
            .Returns(employeePics);
        _departmentService.GetDepartmentAsync().Returns(departments);
        _baseService.ExecuteNonQueryAsync(Arg.Any<CreateTeamCriteria>()).Returns(commandResult);
        _teamService.GetGroupIdByGroupTeamId(100).Returns(insertedTeamId);
        _teamService
            .InsertTeamSupervisorByPIC(
                Arg.Any<int>(),
                insertedTeamId,
                Arg.Any<IEnumerable<EditTeamSupervisor>>()
            )
            .Returns(new CommandResult { Code = 0 });

        // Act
        var result = await _controller.Create(model);

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
        Assert.Equal("GroupTeam", redirectResult.ControllerName);
        await _baseService
            .Received(1)
            .ExecuteNonQueryAsync(
                Arg.Is<CreateTeamCriteria>(c =>
                    c.Code == model.TeamCode
                    && c.Name == model.TeamName
                    && c.DepartmentId == model.DepartmentId
                    && c.PICEmpId == model.PICEmpId
                    && c.GroupTeamId == model.GroupTeamId
                    && c.AccountableArea == model.AccountableArea
                    && c.CompanyId == (short)model.CompanyId
                    && c.IsOutsource == model.IsOutsource
                    && c.CreatedBy == 1
                )
            );
        await _teamService.Received(1).GetGroupIdByGroupTeamId(100);
        await _teamService
            .Received(1)
            .InsertTeamSupervisorByPIC(
                1,
                insertedTeamId,
                Arg.Is<IEnumerable<EditTeamSupervisor>>(s =>
                    s.Count() == 8 &&
                    s.All(x => x.OwnerId == 5 && x.DisplayOrder == 1) &&
                    s.Select(x => x.RequestType).OrderBy(x => x).SequenceEqual(new ushort[] { 1, 2, 3, 4, 5, 6, 7, 8 })
                )
            );
    }

    [Fact]
    public async Task Create_Post_WithNullDepartmentId_ShouldUseZeroAsDefault()
    {
        // Arrange
        var model = new GroupTeamViewModel
        {
            TeamCode = "TEAM001",
            TeamName = "Test Team",
            DepartmentId = null, // Null department ID
            GroupTeamId = 100,
            CompanyId = 1,
        };

        var companies = new List<Company> { new() { CompanyId = 1, Name = "Company 1" } };
        var employeePics = new List<EmployeePIC>();
        var departments = new List<Department>();

        var commandResult = new CommandResult { Code = 0, Message = "Success" };

        _companyService.GetAllCompany().Returns(companies);
        _employeeService
            .GetEmployeeForPICAsync(Arg.Any<GetEmployeeForPICCriteria>())
            .Returns(employeePics);
        _departmentService.GetDepartmentAsync().Returns(departments);
        _baseService.ExecuteNonQueryAsync(Arg.Any<CreateTeamCriteria>()).Returns(commandResult);

        // Act
        var result = await _controller.Create(model);

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        await _baseService
            .Received(1)
            .ExecuteNonQueryAsync(Arg.Is<CreateTeamCriteria>(c => c.DepartmentId == 0));
    }

    [Fact]
    public async Task Create_Post_WithNullEmployeePics_ShouldHandleNullGracefully()
    {
        // Arrange
        var model = new GroupTeamViewModel
        {
            TeamCode = "TEAM001",
            TeamName = "Test Team",
            DepartmentId = 1,
            GroupTeamId = 100,
            CompanyId = 1,
        };

        var companies = new List<Company> { new() { CompanyId = 1, Name = "Company 1" } };
        IEnumerable<EmployeePIC> employeePics = null; // Null employee pics
        var departments = new List<Department> { new() { Id = 1, Name = "Department 1" } };

        var commandResult = new CommandResult { Code = 0, Message = "Success" };

        _companyService.GetAllCompany().Returns(companies);
        _employeeService
            .GetEmployeeForPICAsync(Arg.Any<GetEmployeeForPICCriteria>())
            .Returns(employeePics);
        _departmentService.GetDepartmentAsync().Returns(departments);
        _baseService.ExecuteNonQueryAsync(Arg.Any<CreateTeamCriteria>()).Returns(commandResult);

        // Act
        var result = await _controller.Create(model);

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
    }

    [Fact]
    public async Task Create_Post_WithNullCompanies_ShouldHandleNullGracefully()
    {
        // Arrange
        var model = new GroupTeamViewModel
        {
            TeamCode = "TEAM001",
            TeamName = "Test Team",
            DepartmentId = 1,
            GroupTeamId = 100,
            CompanyId = 1,
        };

        IEnumerable<Company> companies = null; // Null companies
        var employeePics = new List<EmployeePIC>();
        var departments = new List<Department> { new() { Id = 1, Name = "Department 1" } };

        var commandResult = new CommandResult { Code = 0, Message = "Success" };

        _companyService.GetAllCompany().Returns(companies);
        _employeeService
            .GetEmployeeForPICAsync(Arg.Any<GetEmployeeForPICCriteria>())
            .Returns(employeePics);
        _departmentService.GetDepartmentAsync().Returns(departments);
        _baseService.ExecuteNonQueryAsync(Arg.Any<CreateTeamCriteria>()).Returns(commandResult);

        // Act
        var result = await _controller.Create(model);

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
    }

    [Fact]
    public async Task Create_Post_WithValidModelWithoutPIC_ShouldRedirectToIndex()
    {
        // Arrange
        var model = new GroupTeamViewModel
        {
            TeamCode = "TEAM001",
            TeamName = "Test Team",
            DepartmentId = 1,
            GroupTeamId = 100,
            PICEmpId = null,
            AccountableArea = "Test Area",
            CompanyId = 1,
            IsOutsource = false,
        };

        var companies = new List<Company>
        {
            new() { CompanyId = 1, Name = "Company 1" },
        };
        var employeePics = new List<EmployeePIC>();
        var departments = new List<Department>
        {
            new() { Id = 1, Name = "Department 1" },
        };

        var commandResult = new CommandResult { Code = 0, Message = "Success" };

        _companyService.GetAllCompany().Returns(companies);
        _employeeService
            .GetEmployeeForPICAsync(Arg.Any<GetEmployeeForPICCriteria>())
            .Returns(employeePics);
        _departmentService.GetDepartmentAsync().Returns(departments);
        _baseService.ExecuteNonQueryAsync(Arg.Any<CreateTeamCriteria>()).Returns(commandResult);

        // Act
        var result = await _controller.Create(model);

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
        await _teamService.DidNotReceive().GetGroupIdByGroupTeamId(Arg.Any<int>());
        await _teamService
            .DidNotReceive()
            .InsertTeamSupervisorByPIC(
                Arg.Any<int>(),
                Arg.Any<int>(),
                Arg.Any<IEnumerable<EditTeamSupervisor>>()
            );
    }

    [Fact]
    public async Task Create_Post_WithInvalidModel_ShouldReturnViewWithError()
    {
        // Arrange
        var model = new GroupTeamViewModel
        {
            TeamCode = null, // Invalid - required field
            TeamName = "Test Team",
        };

        var companies = new List<Company>();
        var employeePics = new List<EmployeePIC>();
        var departments = new List<Department>();

        _companyService.GetAllCompany().Returns(companies);
        _employeeService
            .GetEmployeeForPICAsync(Arg.Any<GetEmployeeForPICCriteria>())
            .Returns(employeePics);
        _departmentService.GetDepartmentAsync().Returns(departments);

        _controller.ModelState.AddModelError("TeamCode", "TeamCode is required");

        // Act
        var result = await _controller.Create(model);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(model, viewResult.Model);
        Assert.False(_controller.ModelState.IsValid);
        Assert.Contains(
            _controller.ModelState[string.Empty].Errors,
            e => e.ErrorMessage == Messages.InvalidInputData
        );
        await _baseService.DidNotReceive().ExecuteNonQueryAsync(Arg.Any<CreateTeamCriteria>());
    }

    [Fact]
    public async Task Create_Post_WithFailedService_ShouldReturnViewWithError()
    {
        // Arrange
        var model = new GroupTeamViewModel
        {
            TeamCode = "TEAM001",
            TeamName = "Test Team",
            DepartmentId = 1,
            GroupTeamId = 100,
            CompanyId = 1,
        };

        var companies = new List<Company>();
        var employeePics = new List<EmployeePIC>();
        var departments = new List<Department>();

        var commandResult = new CommandResult { Code = 1, Message = "Failed to create team" };

        _companyService.GetAllCompany().Returns(companies);
        _employeeService
            .GetEmployeeForPICAsync(Arg.Any<GetEmployeeForPICCriteria>())
            .Returns(employeePics);
        _departmentService.GetDepartmentAsync().Returns(departments);
        _baseService.ExecuteNonQueryAsync(Arg.Any<CreateTeamCriteria>()).Returns(commandResult);

        // Act
        var result = await _controller.Create(model);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(model, viewResult.Model);
        Assert.False(_controller.ModelState.IsValid);
        Assert.Contains(
            _controller.ModelState[string.Empty].Errors,
            e => e.ErrorMessage == commandResult.Message
        );
    }

    #endregion

    #region Edit GET Tests

    [Fact]
    public async Task Edit_Get_WithValidId_ShouldReturnViewWithViewModel()
    {
        // Arrange
        var teamId = 1;
        var team = new GroupTeamModel
        {
            Id = teamId,
            TeamCode = "TEAM001",
            TeamName = "Test Team",
            DepartmentId = 1,
            PICEmpId = 5,
            GroupTeamId = 100,
            AccountableArea = "Test Area",
            CompanyId = 1,
            IsOutsource = false,
        };

        var companies = new List<Company>
        {
            new() { CompanyId = 1, Name = "Company 1" },
        };
        var employeePics = new List<EmployeePIC>
        {
            new() { Id = 5, FullName = "PIC Employee" },
        };
        var departments = new List<Department>
        {
            new() { Id = 1, Name = "Department 1" },
        };

        _teamService.GetGroupTeamInUnitScopeById(teamId).Returns(team);
        _companyService.GetAllCompany().Returns(companies);
        _employeeService
            .GetEmployeeForPICAsync(Arg.Any<GetEmployeeForPICCriteria>())
            .Returns(employeePics);
        _departmentService.GetDepartmentAsync().Returns(departments);

        // Act
        var result = await _controller.Edit(teamId);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<GroupTeamViewModel>(viewResult.Model);
        Assert.Equal(team.Id, model.Id);
        Assert.Equal(team.TeamCode, model.TeamCode);
        Assert.Equal(team.TeamName, model.TeamName);
        Assert.Equal(team.DepartmentId, model.DepartmentId);
        Assert.Equal(team.PICEmpId, model.PICEmpId);
        Assert.Equal(team.GroupTeamId, model.GroupTeamId);
        Assert.Equal(team.AccountableArea, model.AccountableArea);
        Assert.Equal(team.CompanyId, model.CompanyId);
        Assert.Equal(team.IsOutsource, model.IsOutsource);
        await _teamService.Received(1).GetGroupTeamInUnitScopeById(teamId);
    }

    [Fact]
    public async Task Edit_Get_WithNullTeam_ShouldRedirectToPageNotFound()
    {
        // Arrange
        var teamId = 999;
        _teamService.GetGroupTeamInUnitScopeById(teamId).Returns((GroupTeamModel)null);

        // Act
        var result = await _controller.Edit(teamId);

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("PageNotFound", redirectResult.ActionName);
        Assert.Equal("Error", redirectResult.ControllerName);
    }

    #endregion

    #region Edit POST Tests

    [Fact]
    public async Task Edit_Post_WithValidModel_ShouldReturnViewWithUpdatedFlag()
    {
        // Arrange
        var model = new GroupTeamViewModel
        {
            Id = 1,
            TeamCode = "TEAM001",
            TeamName = "Updated Team",
            DepartmentId = 2,
            GroupTeamId = 100,
            AccountableArea = "Updated Area",
            IsOutsource = true,
        };

        var team = new GroupTeamModel
        {
            Id = 1,
            CompanyId = 1,
            MailGroup = "test@example.com",
            LocalBasicAnnualLeaveBalance = 10,
            LocalWorkingDays = 5,
            LocalWorkingHours = 8,
            LocalOffDaysbyWeek = 2,
            LocalWorkingHoursbyWeek = 40,
            LocalYearsOfServiceForExtraLeave = 3,
            ExpatBasicAnnualLeaveBalance = 15,
            ExpatWorkingDays = 5,
            ExpatWorkingHours = 8,
            ExpatOffDaysbyWeek = 2,
            ExpatWorkingHoursbyWeek = 40,
            ExpatYearsOfServiceForExtraLeave = 5,
        };

        var companies = new List<Company>();
        var employeePics = new List<EmployeePIC>();
        var departments = new List<Department>();

        var commandResult = new CommandResult { Code = 0, Message = "Success" };

        _teamService.GetGroupTeamInUnitScopeById(1).Returns(team);
        _companyService.GetAllCompany().Returns(companies);
        _employeeService
            .GetEmployeeForPICAsync(Arg.Any<GetEmployeeForPICCriteria>())
            .Returns(employeePics);
        _departmentService.GetDepartmentAsync().Returns(departments);
        _baseService.ExecuteNonQueryAsync(Arg.Any<EditTeamCriteria>()).Returns(commandResult);

        // Act
        var result = await _controller.Edit(model);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(model, viewResult.Model);
        Assert.True((bool)_controller.ViewBag.Updated);
        await _baseService
            .Received(1)
            .ExecuteNonQueryAsync(
                Arg.Is<EditTeamCriteria>(c =>
                    c.TeamId == model.Id
                    && c.Code == model.TeamCode
                    && c.Name == model.TeamName
                    && c.DepartmentId == model.DepartmentId
                    && c.AccountableArea == model.AccountableArea
                    && c.ModifiedBy == 1
                    && c.GroupTeamId == model.GroupTeamId
                    && c.MailGroup == team.MailGroup
                    && c.IsOutsource == model.IsOutsource
                )
            );
    }

    [Fact]
    public async Task Edit_Post_WithInvalidModel_ShouldReturnViewWithError()
    {
        // Arrange
        var model = new GroupTeamViewModel
        {
            Id = 1,
            TeamCode = null, // Invalid
        };

        var team = new GroupTeamModel { Id = 1, CompanyId = 1 };
        var companies = new List<Company>();
        var employeePics = new List<EmployeePIC>();
        var departments = new List<Department>();

        _teamService.GetGroupTeamInUnitScopeById(1).Returns(team);
        _companyService.GetAllCompany().Returns(companies);
        _employeeService
            .GetEmployeeForPICAsync(Arg.Any<GetEmployeeForPICCriteria>())
            .Returns(employeePics);
        _departmentService.GetDepartmentAsync().Returns(departments);

        _controller.ModelState.AddModelError("TeamCode", "TeamCode is required");

        // Act
        var result = await _controller.Edit(model);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(model, viewResult.Model);
        Assert.False(_controller.ModelState.IsValid);
        Assert.Contains(
            _controller.ModelState[string.Empty].Errors,
            e => e.ErrorMessage == Messages.InvalidInputData
        );
    }

    [Fact]
    public async Task Edit_Post_WithFailedService_ShouldReturnViewWithError()
    {
        // Arrange
        var model = new GroupTeamViewModel
        {
            Id = 1,
            TeamCode = "TEAM001",
            TeamName = "Test Team",
        };

        var team = new GroupTeamModel { Id = 1, CompanyId = 1 };
        var companies = new List<Company>();
        var employeePics = new List<EmployeePIC>();
        var departments = new List<Department>();

        var commandResult = new CommandResult { Code = 1, Message = "Failed to update" };

        _teamService.GetGroupTeamInUnitScopeById(1).Returns(team);
        _companyService.GetAllCompany().Returns(companies);
        _employeeService
            .GetEmployeeForPICAsync(Arg.Any<GetEmployeeForPICCriteria>())
            .Returns(employeePics);
        _departmentService.GetDepartmentAsync().Returns(departments);
        _baseService.ExecuteNonQueryAsync(Arg.Any<EditTeamCriteria>()).Returns(commandResult);

        // Act
        var result = await _controller.Edit(model);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(model, viewResult.Model);
        Assert.False(_controller.ModelState.IsValid);
        Assert.Contains(
            _controller.ModelState[string.Empty].Errors,
            e => e.ErrorMessage == commandResult.Message
        );
    }

    [Fact]
    public async Task Edit_Post_WithNullModel_ShouldReturnView()
    {
        // Arrange
        GroupTeamViewModel model = null;

        // Act
        var result = await _controller.Edit(model);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Null(viewResult.Model);
        await _baseService.DidNotReceive().ExecuteNonQueryAsync(Arg.Any<EditTeamCriteria>());
    }

    [Fact]
    public async Task Edit_Post_WithNullDepartmentId_ShouldUseZeroAsDefault()
    {
        // Arrange
        var model = new GroupTeamViewModel
        {
            Id = 1,
            TeamCode = "TEAM001",
            TeamName = "Test Team",
            DepartmentId = null, // Null department ID
            GroupTeamId = 100,
        };

        var team = new GroupTeamModel
        {
            Id = 1,
            CompanyId = 1,
            MailGroup = "test@example.com",
        };

        var companies = new List<Company>();
        var employeePics = new List<EmployeePIC>();
        var departments = new List<Department>();

        var commandResult = new CommandResult { Code = 0, Message = "Success" };

        _teamService.GetGroupTeamInUnitScopeById(1).Returns(team);
        _companyService.GetAllCompany().Returns(companies);
        _employeeService
            .GetEmployeeForPICAsync(Arg.Any<GetEmployeeForPICCriteria>())
            .Returns(employeePics);
        _departmentService.GetDepartmentAsync().Returns(departments);
        _baseService.ExecuteNonQueryAsync(Arg.Any<EditTeamCriteria>()).Returns(commandResult);

        // Act
        var result = await _controller.Edit(model);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        await _baseService
            .Received(1)
            .ExecuteNonQueryAsync(Arg.Is<EditTeamCriteria>(c => c.DepartmentId == 0));
    }

    [Fact]
    public async Task Edit_Post_WithNullEmployeePics_ShouldHandleNullGracefully()
    {
        // Arrange
        var model = new GroupTeamViewModel
        {
            Id = 1,
            TeamCode = "TEAM001",
            TeamName = "Test Team",
            DepartmentId = 1,
            GroupTeamId = 100,
        };

        var team = new GroupTeamModel { Id = 1, CompanyId = 1 };
        var companies = new List<Company>();
        IEnumerable<EmployeePIC> employeePics = null; // Null employee pics
        var departments = new List<Department>();

        var commandResult = new CommandResult { Code = 0, Message = "Success" };

        _teamService.GetGroupTeamInUnitScopeById(1).Returns(team);
        _companyService.GetAllCompany().Returns(companies);
        _employeeService
            .GetEmployeeForPICAsync(Arg.Any<GetEmployeeForPICCriteria>())
            .Returns(employeePics);
        _departmentService.GetDepartmentAsync().Returns(departments);
        _baseService.ExecuteNonQueryAsync(Arg.Any<EditTeamCriteria>()).Returns(commandResult);

        // Act
        var result = await _controller.Edit(model);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(model, viewResult.Model);
    }

    [Fact]
    public async Task Edit_Post_WithNullCompanies_ShouldHandleNullGracefully()
    {
        // Arrange
        var model = new GroupTeamViewModel
        {
            Id = 1,
            TeamCode = "TEAM001",
            TeamName = "Test Team",
            DepartmentId = 1,
            GroupTeamId = 100,
        };

        var team = new GroupTeamModel { Id = 1, CompanyId = 1 };
        IEnumerable<Company> companies = null; // Null companies
        var employeePics = new List<EmployeePIC>();
        var departments = new List<Department>();

        var commandResult = new CommandResult { Code = 0, Message = "Success" };

        _teamService.GetGroupTeamInUnitScopeById(1).Returns(team);
        _companyService.GetAllCompany().Returns(companies);
        _employeeService
            .GetEmployeeForPICAsync(Arg.Any<GetEmployeeForPICCriteria>())
            .Returns(employeePics);
        _departmentService.GetDepartmentAsync().Returns(departments);
        _baseService.ExecuteNonQueryAsync(Arg.Any<EditTeamCriteria>()).Returns(commandResult);

        // Act
        var result = await _controller.Edit(model);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(model, viewResult.Model);
    }

    [Fact]
    public async Task Edit_Post_WithNullTeamProperties_ShouldUseGetValueMethod()
    {
        // Arrange
        var model = new GroupTeamViewModel
        {
            Id = 1,
            TeamCode = "TEAM001",
            TeamName = "Test Team",
            DepartmentId = 1,
            GroupTeamId = 100,
        };

        var team = new GroupTeamModel
        {
            Id = 1,
            CompanyId = 1,
            MailGroup = "test@example.com",
            // All nullable properties are null to test GetValue method
            LocalBasicAnnualLeaveBalance = null,
            LocalWorkingDays = null,
            LocalWorkingHours = null,
            LocalOffDaysbyWeek = null,
            LocalWorkingHoursbyWeek = null,
            LocalYearsOfServiceForExtraLeave = null,
            ExpatBasicAnnualLeaveBalance = null,
            ExpatWorkingDays = null,
            ExpatWorkingHours = null,
            ExpatOffDaysbyWeek = null,
            ExpatWorkingHoursbyWeek = null,
            ExpatYearsOfServiceForExtraLeave = null,
        };

        var companies = new List<Company>();
        var employeePics = new List<EmployeePIC>();
        var departments = new List<Department>();

        var commandResult = new CommandResult { Code = 0, Message = "Success" };

        _teamService.GetGroupTeamInUnitScopeById(1).Returns(team);
        _companyService.GetAllCompany().Returns(companies);
        _employeeService
            .GetEmployeeForPICAsync(Arg.Any<GetEmployeeForPICCriteria>())
            .Returns(employeePics);
        _departmentService.GetDepartmentAsync().Returns(departments);
        _baseService.ExecuteNonQueryAsync(Arg.Any<EditTeamCriteria>()).Returns(commandResult);

        // Act
        var result = await _controller.Edit(model);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        await _baseService
            .Received(1)
            .ExecuteNonQueryAsync(Arg.Is<EditTeamCriteria>(c =>
                c.LocalBasicAnnualLeaveBalance == 0
                && c.LocalWorkingDays == 0
                && c.LocalWorkingHours == 0
                && c.LocalOffDaysbyWeek == 0
                && c.ExpatBasicAnnualLeaveBalance == 0
                && c.ExpatWorkingDays == 0
                && c.ExpatWorkingHours == 0
                && c.ExpatOffDaysbyWeek == 0
            ));
    }

    #endregion

    #region Delete Tests

    [Fact]
    public async Task Delete_WithValidId_ShouldReturnJsonWithSuccess()
    {
        // Arrange
        var teamId = 1;
        var commandResult = new CommandResult { Code = 0, Message = "Success" };

        _teamService.Delete(1, teamId).Returns(commandResult);

        // Act
        var result = await _controller.Delete(teamId);

        // Assert
        var jsonResult = Assert.IsType<JsonResult>(result);
        Assert.Equal(commandResult, jsonResult.Value);
        await _teamService.Received(1).Delete(1, teamId);
    }

    [Fact]
    public async Task Delete_WithFailedService_ShouldReturnJsonWithError()
    {
        // Arrange
        var teamId = 1;
        var commandResult = new CommandResult { Code = 1, Message = "Failed to delete" };

        _teamService.Delete(1, teamId).Returns(commandResult);

        // Act
        var result = await _controller.Delete(teamId);

        // Assert
        var jsonResult = Assert.IsType<JsonResult>(result);
        var resultValue = Assert.IsType<CommandResult>(jsonResult.Value);
        Assert.False(resultValue.IsSuccessful);
        Assert.Equal("Failed to delete", resultValue.Message);
    }

    #endregion

    #region ExportGroupTeamList Tests

    [Fact]
    public async Task ExportGroupTeamList_WithValidParameters_ShouldReturnFile()
    {
        // Arrange
        var teams = new List<GroupTeamModel>
        {
            new()
            {
                Id = 1,
                TeamCode = "TEAM001",
                TeamName = "Test Team 1",
            },
        };

        var filteredTeams = teams.ToList();
        var fileBytes = new byte[] { 1, 2, 3, 4, 5 };

        _teamService.GetAllTeamByUnitScopeAsync("1", "1", "true").Returns(teams);
        _teamService.GetTeamEmployeeBelongsTo(teams, 1).Returns(filteredTeams);
        _teamService.GenerateGroupTeamExcel(filteredTeams).Returns(fileBytes);

        // Act
        var result = await _controller.ExportGroupTeamList("1", "1", "true");

        // Assert
        var fileResult = Assert.IsType<FileContentResult>(result);
        Assert.Equal(fileBytes, fileResult.FileContents);
        Assert.Equal(MimeType.XLS, fileResult.ContentType);
        Assert.Contains("GroupTeam_", fileResult.FileDownloadName);
        await _teamService.Received(1).GetAllTeamByUnitScopeAsync("1", "1", "true");
        await _teamService.Received(1).GenerateGroupTeamExcel(filteredTeams);
    }

    [Fact]
    public async Task ExportGroupTeamList_WithEmptyTeams_ShouldReturnFile()
    {
        // Arrange
        var teams = new List<GroupTeamModel>();
        var filteredTeams = new List<GroupTeamModel>();
        var fileBytes = new byte[] { };

        _teamService.GetAllTeamByUnitScopeAsync(null, null, null).Returns(teams);
        _teamService.GetTeamEmployeeBelongsTo(teams, 1).Returns(filteredTeams);
        _teamService.GenerateGroupTeamExcel(filteredTeams).Returns(fileBytes);

        // Act
        var result = await _controller.ExportGroupTeamList();

        // Assert
        var fileResult = Assert.IsType<FileContentResult>(result);
        Assert.Equal(fileBytes, fileResult.FileContents);
        Assert.Equal(MimeType.XLS, fileResult.ContentType);
        Assert.Contains("GroupTeam_", fileResult.FileDownloadName);
    }

    #endregion

    #region TeamSupervisor Tests

    [Fact]
    public async Task TeamSupervisor_WithValidId_ShouldReturnViewWithSupervisors()
    {
        // Arrange
        var teamId = 1;
        var supervisors = new List<TeamSupervisorModel>
        {
            new()
            {
                TeamId = teamId,
                Code = "TEAM001",
                Name = "Test Team",
                ActionType = 1,
            },
        };

        _teamService.GetSupervisorByTeamId(teamId, true).Returns(supervisors);

        // Act
        var result = await _controller.TeamSupervisor(teamId);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(supervisors, viewResult.Model);
        Assert.Equal(teamId, _controller.ViewBag.TeamId);
        Assert.Equal("Test Team", _controller.ViewBag.TeamName);
        await _teamService.Received(1).GetSupervisorByTeamId(teamId, true);
    }

    [Fact]
    public async Task TeamSupervisor_WithEmptySupervisors_ShouldReturnViewWithEmptyName()
    {
        // Arrange
        var teamId = 1;
        var supervisors = new List<TeamSupervisorModel>();

        _teamService.GetSupervisorByTeamId(teamId, true).Returns(supervisors);

        // Act
        var result = await _controller.TeamSupervisor(teamId);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(supervisors, viewResult.Model);
        Assert.Equal(string.Empty, _controller.ViewBag.TeamName);
    }

    [Fact]
    public async Task TeamSupervisor_WithSupervisorsHavingName_ShouldReturnViewWithName()
    {
        // Arrange
        var teamId = 1;
        var supervisors = new List<TeamSupervisorModel>
        {
            new()
            {
                TeamId = teamId,
                Code = "TEAM001",
                Name = "Test Team Name",
                ActionType = 1,
            },
        };

        _teamService.GetSupervisorByTeamId(teamId, true).Returns(supervisors);

        // Act
        var result = await _controller.TeamSupervisor(teamId);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(supervisors, viewResult.Model);
        Assert.Equal(teamId, _controller.ViewBag.TeamId);
        Assert.Equal("Test Team Name", _controller.ViewBag.TeamName);
    }

    #endregion

    #region ShiftManager Tests

    [Fact]
    public void ShiftManager_WithValidId_ShouldReturnView()
    {
        // Arrange
        var teamId = 1;

        // Act
        var result = _controller.ShiftManager(teamId);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(teamId, _controller.ViewBag.TeamId);
        Assert.Equal((short)RoleSystem.ShiftManager, _controller.ViewBag.RoleId);
    }

    #endregion

    #region DisciplineManager Tests

    [Fact]
    public void DisciplineManager_WithValidId_ShouldReturnView()
    {
        // Arrange
        var teamId = 1;

        // Act
        var result = _controller.DisciplineManager(teamId);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(teamId, _controller.ViewBag.TeamId);
        Assert.Equal((short)RoleSystem.DisciplineManager, _controller.ViewBag.RoleId);
    }

    #endregion

    #region LeaderTakeoverManager Tests

    [Fact]
    public void LeaderTakeoverManager_WithValidId_ShouldReturnView()
    {
        // Arrange
        var teamId = 1;

        // Act
        var result = _controller.LeaderTakeoverManager(teamId);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(teamId, _controller.ViewBag.TeamId);
        Assert.Equal((short)RoleSystem.LeaderTakeover, _controller.ViewBag.RoleId);
    }

    #endregion

    #region GetLeaderTakeoverManagerByTeamId Tests

    [Fact]
    public async Task GetLeaderTakeoverManagerByTeamId_WithValidId_ShouldReturnJson()
    {
        // Arrange
        var teamId = 1;
        var supervisors = new List<Supervisor>
        {
            new()
            {
                OwnerId = 1,
                Nickname = "Test",
                FullName = "Test User",
                CanRemove = true,
            },
        };

        _teamService.GetLeaderTakeoverManagerByTeamId(teamId).Returns(supervisors);

        // Act
        var result = await _controller.GetLeaderTakeoverManagerByTeamId(teamId);

        // Assert
        var jsonResult = Assert.IsType<JsonResult>(result);
        Assert.Equal(supervisors, jsonResult.Value);
        await _teamService.Received(1).GetLeaderTakeoverManagerByTeamId(teamId);
    }

    #endregion

    #region ManageSupervisor Tests

    [Fact]
    public void ManageSupervisor_WithValidParameters_ShouldReturnView()
    {
        // Arrange
        var teamId = 1;
        var roleId = (short)RoleSystem.ShiftManager;

        // Act
        var result = _controller.ManageSupervisor(teamId, roleId);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(teamId, _controller.ViewBag.TeamId);
        Assert.Equal(roleId, _controller.ViewBag.RoleId);
    }

    #endregion

    #region GetTeamSupervisorByRoleId Tests

    [Fact]
    public async Task GetTeamSupervisorByRoleId_WithValidParameters_ShouldReturnJson()
    {
        // Arrange
        var teamId = 1;
        var roleId = (short)RoleSystem.ShiftManager;
        var supervisors = new List<SupervisorModel>
        {
            new()
            {
                OwnerId = 1,
                NickName = "Test",
                FullName = "Test User",
                CanRemove = true,
            },
        };

        _teamService.GetTeamSupervisorByRoleId(teamId, roleId).Returns(supervisors);

        // Act
        var result = await _controller.GetTeamSupervisorByRoleId(teamId, roleId);

        // Assert
        var jsonResult = Assert.IsType<JsonResult>(result);
        Assert.Equal(supervisors, jsonResult.Value);
        await _teamService.Received(1).GetTeamSupervisorByRoleId(teamId, roleId);
    }

    #endregion

    #region GetDisciplineManagerByTeamId Tests

    [Fact]
    public async Task GetDisciplineManagerByTeamId_WithValidId_ShouldReturnJson()
    {
        // Arrange
        var teamId = 1;
        var supervisors = new List<Supervisor>
        {
            new()
            {
                OwnerId = 1,
                Nickname = "Test",
                FullName = "Test User",
                CanRemove = true,
            },
        };

        _teamService.GetDisciplineManagerByTeamId(teamId, true).Returns(supervisors);

        // Act
        var result = await _controller.GetDisciplineManagerByTeamId(teamId);

        // Assert
        var jsonResult = Assert.IsType<JsonResult>(result);
        Assert.Equal(supervisors, jsonResult.Value);
        await _teamService.Received(1).GetDisciplineManagerByTeamId(teamId, true);
    }

    #endregion

    #region GetShiftManagerByTeamId Tests

    [Fact]
    public async Task GetShiftManagerByTeamId_WithValidId_ShouldReturnJson()
    {
        // Arrange
        var teamId = 1;
        var supervisors = new List<SupervisorModel>
        {
            new()
            {
                OwnerId = 1,
                NickName = "Test",
                FullName = "Test User",
                CanRemove = true,
            },
        };

        _teamService
            .GetTeamSupervisorByRoleId(teamId, (short)RoleSystem.ShiftManager, true)
            .Returns(supervisors);

        // Act
        var result = await _controller.GetShiftManagerByTeamId(teamId);

        // Assert
        var jsonResult = Assert.IsType<JsonResult>(result);
        Assert.NotNull(jsonResult.Value);
        // The result is a LINQ Select iterator, so we materialize it directly
        var resultList =
            (jsonResult.Value as IEnumerable<Supervisor>)?.ToList()
            ?? throw new InvalidOperationException("Result is not enumerable");
        Assert.Single(resultList);
        Assert.Equal(1, resultList[0].OwnerId);
        Assert.Equal("Test", resultList[0].Nickname);
        Assert.Equal("Test User", resultList[0].FullName);
        Assert.True(resultList[0].CanRemove);
        await _teamService
            .Received(1)
            .GetTeamSupervisorByRoleId(teamId, (short)RoleSystem.ShiftManager, true);
    }

    #endregion

    #region EditTeamSupervisor Tests

    [Fact]
    public async Task EditTeamSupervisor_WithValidParameters_ShouldReturnJson()
    {
        // Arrange
        var teamId = 1;
        var roleId = (short)RoleSystem.ShiftManager;
        var supervisors = new List<EditTeamSupervisor>
        {
            new()
            {
                OwnerId = 1,
                RequestType = 1,
                DisplayOrder = 1,
            },
        };

        var commandResult = new CommandResult { Code = 0, Message = "Success" };

        _teamService.EditTeamSupervisor(1, teamId, roleId, supervisors).Returns(commandResult);

        // Act
        var result = await _controller.EditTeamSupervisor(teamId, roleId, supervisors);

        // Assert
        var jsonResult = Assert.IsType<JsonResult>(result);
        Assert.Equal(commandResult, jsonResult.Value);
        await _teamService.Received(1).EditTeamSupervisor(1, teamId, roleId, supervisors);
    }

    [Fact]
    public async Task EditTeamSupervisor_WithFailedService_ShouldReturnJsonWithError()
    {
        // Arrange
        var teamId = 1;
        var roleId = (short)RoleSystem.ShiftManager;
        var supervisors = new List<EditTeamSupervisor>();

        var commandResult = new CommandResult { Code = 1, Message = "Failed" };

        _teamService.EditTeamSupervisor(1, teamId, roleId, supervisors).Returns(commandResult);

        // Act
        var result = await _controller.EditTeamSupervisor(teamId, roleId, supervisors);

        // Assert
        var jsonResult = Assert.IsType<JsonResult>(result);
        var resultValue = Assert.IsType<CommandResult>(jsonResult.Value);
        Assert.False(resultValue.IsSuccessful);
    }

    #endregion

    #region GetEmployees Tests

    [Fact]
    public async Task GetEmployees_WithValidModel_ShouldReturnOk()
    {
        // Arrange
        var model = new GroupEmployeeViewModel
        {
            Keyword = "test",
            IsDelete = false,
            IsEnable = true,
        };

        var employees = new List<GroupEmployeeModel>
        {
            new() { EmpId = 1, FullName = "Test Employee" },
        };

        _baseService
            .GetDataAsync<GroupEmployeeModel>(Arg.Any<GetEmployeesInGroupScopeCriteria>())
            .Returns(employees);

        // Act
        var result = await _controller.GetEmployees(model);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(employees, okResult.Value);
        await _baseService
            .Received(1)
            .GetDataAsync<GroupEmployeeModel>(
                Arg.Is<GetEmployeesInGroupScopeCriteria>(c =>
                    c.Query == model.Keyword
                    && c.IsDeleteUser == model.IsDelete
                    && c.EnableUser == model.IsEnable
                )
            );
    }

    [Fact]
    public async Task GetEmployees_WithNullProperties_ShouldReturnOk()
    {
        // Arrange
        var model = new GroupEmployeeViewModel
        {
            Keyword = null,
            IsDelete = null,
            IsEnable = null,
        };

        var employees = new List<GroupEmployeeModel>();

        _baseService
            .GetDataAsync<GroupEmployeeModel>(Arg.Any<GetEmployeesInGroupScopeCriteria>())
            .Returns(employees);

        // Act
        var result = await _controller.GetEmployees(model);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(employees, okResult.Value);
        await _baseService
            .Received(1)
            .GetDataAsync<GroupEmployeeModel>(
                Arg.Is<GetEmployeesInGroupScopeCriteria>(c =>
                    c.Query == null
                    && c.IsDeleteUser == null
                    && c.EnableUser == null
                )
            );
    }

    [Fact]
    public async Task GetEmployees_WithNullModel_ShouldThrowNullReferenceException()
    {
        // Arrange
        GroupEmployeeViewModel model = null;

        // Act & Assert
        await Assert.ThrowsAsync<NullReferenceException>(async () =>
            await _controller.GetEmployees(model)
        );
    }

    #endregion
}
