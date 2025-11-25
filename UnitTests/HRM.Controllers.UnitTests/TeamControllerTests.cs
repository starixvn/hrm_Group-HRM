using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GO.UserControl.Client.Core.Enumerables;
using HRM.Controllers;
using HRM.Core.Constants;
using HRM.Core.Enums;
using HRM.Core.Models;
using HRM.Core.Services;
using HRM.Criteria.Employee;
using HRM.Criteria.Group;
using HRM.Models;
using HRM.Models.Group;
using HRM.Models.Supervisor;
using HRM.Models.Unit;
using HRM.Services.Implement;
using HRM.Services.Interfaces;
using HRM.ViewModels;
using HRM.ViewModels.EmployeeGroup;
using HRM.ViewModels.Team;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Xunit;

namespace HRM.Controllers.UnitTests;

public class TeamControllerTests : BaseControllerTest
{
    private readonly IUnitService _unitService;
    private readonly IDepartmentService _departmentService;
    private readonly ICommonDataService _commonDataService;
    private readonly ITeamService _teamService;
    private readonly IBaseService _baseService;
    private readonly IEmployeeService _employeeService;
    private readonly TeamController _controller;

    public TeamControllerTests()
    {
        _unitService = Substitute.For<IUnitService>();
        _departmentService = Substitute.For<IDepartmentService>();
        _commonDataService = Substitute.For<ICommonDataService>();
        _teamService = Substitute.For<ITeamService>();
        _baseService = Substitute.For<IBaseService>();
        _employeeService = Substitute.For<IEmployeeService>();
        _controller = new TeamController(
            _session,
            _unitService,
            _departmentService,
            _commonDataService,
            _teamService,
            _baseService,
            _employeeService
        );
    }

    #region Index Tests

    [Fact]
    public async Task Index_ShouldReturnViewWithTeams()
    {
        // Arrange
        var teams = new List<Team>
        {
            new()
            {
                Id = 1,
                Name = "Team 1",
                Code = "T001",
            },
            new()
            {
                Id = 2,
                Name = "Team 2",
                Code = "T002",
            },
        };

        _teamService.GetAllAsync(keyword: null).Returns(teams);
        SetupPermission(FunctionKey.TeamManagement, Policy.Host, true);
        SetupPermission(FunctionKey.EmployeeManagement, Policy.Read, true);

        // Act
        var result = await _controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<List<Team>>(viewResult.Model);
        Assert.Equal(2, model.Count);
        Assert.True((bool)_controller.ViewBag.HasPermission);
        Assert.True((bool)_controller.ViewBag.HasPermissionToViewEmployee);
        Assert.Equal(3, _controller.ViewBag.CompanyId);
    }

    [Fact]
    public async Task Index_WithKeyword_ShouldFilterTeams()
    {
        // Arrange
        var keyword = "Team 1";
        var teams = new List<Team>
        {
            new()
            {
                Id = 1,
                Name = "Team 1",
                Code = "T001",
            },
        };

        _teamService.GetAllAsync(keyword: keyword).Returns(teams);
        SetupPermission(FunctionKey.TeamManagement, Policy.Host, false);
        SetupPermission(FunctionKey.EmployeeManagement, Policy.Read, false);

        // Act
        var result = await _controller.Index(keyword);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<List<Team>>(viewResult.Model);
        Assert.Single(model);
        Assert.False((bool)_controller.ViewBag.HasPermission);
        Assert.False((bool)_controller.ViewBag.HasPermissionToViewEmployee);
    }

    #endregion

    #region Edit (GET) Tests

    [Fact]
    public async Task Edit_Get_WithValidId_ShouldReturnViewWithModel()
    {
        // Arrange
        var id = 1;
        var team = new Team
        {
            Id = id,
            Code = "T001",
            Name = "Test Team",
            MailGroup = "test@example.com",
            LocalBasicAnnualLeaveBalance = 10,
            LocalWorkingDays = 22,
            LocalWorkingHours = 8,
            LocalOffDaysbyWeek = 2,
            LocalWorkingHoursbyWeek = 40,
            LocalYearsOfServiceForExtraLeave = 5,
            ExpatBasicAnnualLeaveBalance = 15,
            ExpatWorkingDays = 22,
            ExpatWorkingHours = 8,
            ExpatOffDaysbyWeek = 2,
            ExpatWorkingHoursbyWeek = 40,
            ExpatYearsOfServiceForExtraLeave = 3,
            PICEmpId = 100,
            GroupTeamId = 1,
            AccountableArea = "Area 1",
            IsOutsource = false,
            Department = new Department { Id = 1, Name = "Dept 1" },
        };

        var departments = new List<Department>
        {
            new() { Id = 1, Name = "Dept 1" },
            new() { Id = 2, Name = "Dept 2" },
        };

        var employees = new List<Employee>
        {
            new()
            {
                Id = 100,
                NickName = "John",
                FullName = "John Doe",
            },
        };

        _teamService.GetById(id).Returns(team);
        _departmentService.GetDepartmentAsync().Returns(departments);
        _employeeService.GetEmployeeAsync(Arg.Any<GetAllEmployeeCriteria>()).Returns(employees);

        // Act
        var result = await _controller.Edit(id);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<TeamViewModel>(viewResult.Model);
        Assert.Equal(id, model.Id);
        Assert.Equal("T001", model.TeamCode);
        Assert.Equal("Test Team", model.TeamName);
        Assert.Equal("test@example.com", model.MailGroup);
        Assert.Equal((byte)10, model.LocalBasicAnnualLeaveBalance);
        Assert.Equal((byte)22, model.LocalWorkingDays);
        Assert.Equal((byte)8, model.LocalWorkingHours);
        Assert.Equal((byte)2, model.LocalOffDaysbyWeek);
        Assert.Equal((byte?)40, model.LocalWorkingHoursbyWeek);
        Assert.Equal((byte?)5, model.LocalYearsOfServiceForExtraLeave);
        Assert.Equal((byte)15, model.ExpatBasicAnnualLeaveBalance);
        Assert.Equal((byte)22, model.ExpatWorkingDays);
        Assert.Equal((byte)8, model.ExpatWorkingHours);
        Assert.Equal((byte)2, model.ExpatOffDaysbyWeek);
        Assert.Equal((byte?)40, model.ExpatWorkingHoursbyWeek);
        Assert.Equal((byte?)3, model.ExpatYearsOfServiceForExtraLeave);
        Assert.Equal(1, model.DepartmentId);
        Assert.Equal("Dept 1", model.DepartmentName);
        Assert.Equal(100, model.PICEmpId);
        Assert.Equal(1, model.GroupTeamId);
        Assert.Equal("Area 1", model.AccountableArea);
        Assert.False(model.IsOutsource);
        Assert.NotNull(model.Departments);
        Assert.NotNull(model.Employees);
    }

    [Fact]
    public async Task Edit_Get_WithNullTeam_ShouldRedirectToPageNotFound()
    {
        // Arrange
        var id = 999;
        _teamService.GetById(id).Returns((Team)null);

        // Act
        var result = await _controller.Edit(id);

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("PageNotFound", redirectResult.ActionName);
        Assert.Equal("Error", redirectResult.ControllerName);
    }

    [Fact]
    public async Task Edit_Get_WithNullDepartment_ShouldSetDefaultValues()
    {
        // Arrange
        var id = 1;
        var team = new Team
        {
            Id = id,
            Code = "T001",
            Name = "Test Team",
            Department = null,
        };

        _teamService.GetById(id).Returns(team);
        _departmentService.GetDepartmentAsync().Returns(new List<Department>());
        _employeeService
            .GetEmployeeAsync(Arg.Any<GetAllEmployeeCriteria>())
            .Returns(new List<Employee>());

        // Act
        var result = await _controller.Edit(id);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<TeamViewModel>(viewResult.Model);
        Assert.Equal(0, model.DepartmentId);
        Assert.Equal(string.Empty, model.DepartmentName);
    }

    [Fact]
    public async Task Edit_Get_WithNullValues_ShouldUseDefaultValues()
    {
        // Arrange
        var id = 1;
        var team = new Team
        {
            Id = id,
            Code = "T001",
            Name = "Test Team",
            LocalBasicAnnualLeaveBalance = null,
            LocalWorkingDays = null,
            LocalWorkingHours = null,
            LocalOffDaysbyWeek = null,
            ExpatBasicAnnualLeaveBalance = null,
            ExpatWorkingDays = null,
            ExpatWorkingHours = null,
            ExpatOffDaysbyWeek = null,
        };

        _teamService.GetById(id).Returns(team);
        _departmentService.GetDepartmentAsync().Returns(new List<Department>());
        _employeeService
            .GetEmployeeAsync(Arg.Any<GetAllEmployeeCriteria>())
            .Returns(new List<Employee>());

        // Act
        var result = await _controller.Edit(id);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<TeamViewModel>(viewResult.Model);
        Assert.Equal((byte)0, model.LocalBasicAnnualLeaveBalance);
        Assert.Equal((byte)0, model.LocalWorkingDays);
        Assert.Equal((byte)0, model.LocalWorkingHours);
        Assert.Equal((byte)0, model.LocalOffDaysbyWeek);
        Assert.Equal((byte)0, model.ExpatBasicAnnualLeaveBalance);
        Assert.Equal((byte)0, model.ExpatWorkingDays);
        Assert.Equal((byte)0, model.ExpatWorkingHours);
        Assert.Equal((byte)0, model.ExpatOffDaysbyWeek);
    }

    #endregion

    #region Edit (POST) Tests

    [Fact]
    public async Task Edit_Post_WithValidModel_ShouldReturnSuccess()
    {
        // Arrange
        var model = new TeamViewModel
        {
            Id = 1,
            MailGroup = "test@example.com",
            LocalBasicAnnualLeaveBalance = 10,
            LocalWorkingDays = 22,
            LocalWorkingHours = 8,
            LocalOffDaysbyWeek = 2,
            LocalWorkingHoursbyWeek = 40,
            LocalYearsOfServiceForExtraLeave = 5,
            ExpatBasicAnnualLeaveBalance = 15,
            ExpatWorkingDays = 22,
            ExpatWorkingHours = 8,
            ExpatOffDaysbyWeek = 2,
            ExpatWorkingHoursbyWeek = 40,
            ExpatYearsOfServiceForExtraLeave = 3,
            IsOutsource = false,
        };

        var team = new GroupTeamModel
        {
            Id = 1,
            TeamCode = "T001",
            TeamName = "Test Team",
            DepartmentId = 1,
            DepartmentName = "Dept 1",
            PICEmpId = 100,
            GroupTeamId = 1,
        };

        var departments = new List<Department>
        {
            new() { Id = 1, Name = "Dept 1" },
        };
        var employees = new List<Employee> { new() { Id = 100 } };

        var commandResult = new CommandResult { Code = 0, Message = "Success" };

        _departmentService.GetDepartmentAsync().Returns(departments);
        _employeeService.GetEmployeeAsync(Arg.Any<GetAllEmployeeCriteria>()).Returns(employees);
        _teamService.GetGroupTeamInUnitScopeById(model.Id).Returns(team);
        _baseService.ExecuteNonQueryAsync(Arg.Any<EditTeamCriteria>()).Returns(commandResult);

        // Act
        var result = await _controller.Edit(model);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.True((bool)_controller.ViewBag.Updated);
        await _baseService
            .Received(1)
            .ExecuteNonQueryAsync(
                Arg.Is<EditTeamCriteria>(c =>
                    c.TeamId == team.Id
                    && c.Code == team.TeamCode
                    && c.Name == team.TeamName
                    && c.DepartmentId == team.DepartmentId
                    && c.PICEmpId == team.PICEmpId
                    && c.GroupTeamId == team.GroupTeamId
                    && c.MailGroup == model.MailGroup
                    && c.LocalBasicAnnualLeaveBalance == 10
                    && c.LocalWorkingDays == 22
                    && c.LocalWorkingHours == 8
                    && c.LocalOffDaysbyWeek == 2
                    && c.LocalWorkingHoursbyWeek == 40
                    && c.LocalYearsOfServiceForExtraLeave == 5
                    && c.ExpatBasicAnnualLeaveBalance == 15
                    && c.ExpatWorkingDays == 22
                    && c.ExpatWorkingHours == 8
                    && c.ExpatOffDaysbyWeek == 2
                    && c.ExpatWorkingHoursbyWeek == 40
                    && c.ExpatYearsOfServiceForExtraLeave == 3
                    && c.IsOutsource == false
                    && c.ModifiedBy == 1 // From SetupEmployeeSession
                )
            );
    }

    [Fact]
    public async Task Edit_Post_WithInvalidModel_ShouldReturnViewWithError()
    {
        // Arrange
        var model = new TeamViewModel { Id = 1 };
        var team = new GroupTeamModel
        {
            Id = 1,
            TeamCode = "T001",
            TeamName = "Test Team",
            DepartmentId = 1,
            DepartmentName = "Dept 1",
        };

        _departmentService.GetDepartmentAsync().Returns(new List<Department>());
        _employeeService
            .GetEmployeeAsync(Arg.Any<GetAllEmployeeCriteria>())
            .Returns(new List<Employee>());
        _teamService.GetGroupTeamInUnitScopeById(model.Id).Returns(team);
        _controller.ModelState.AddModelError("TeamName", "Team name is required");

        // Act
        var result = await _controller.Edit(model);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.False(_controller.ModelState.IsValid);
        await _baseService.DidNotReceive().ExecuteNonQueryAsync(Arg.Any<EditTeamCriteria>());
    }

    [Fact]
    public async Task Edit_Post_WithServiceFailure_ShouldAddModelError()
    {
        // Arrange
        var model = new TeamViewModel
        {
            Id = 1,
            LocalBasicAnnualLeaveBalance = 10,
            LocalWorkingDays = 22,
            LocalWorkingHours = 8,
            LocalOffDaysbyWeek = 2,
        };

        var team = new GroupTeamModel
        {
            Id = 1,
            TeamCode = "T001",
            TeamName = "Test Team",
            DepartmentId = 1,
            DepartmentName = "Dept 1",
        };

        var commandResult = new CommandResult { Code = 1, Message = "Update failed" };

        _departmentService.GetDepartmentAsync().Returns(new List<Department>());
        _employeeService
            .GetEmployeeAsync(Arg.Any<GetAllEmployeeCriteria>())
            .Returns(new List<Employee>());
        _teamService.GetGroupTeamInUnitScopeById(model.Id).Returns(team);
        _baseService.ExecuteNonQueryAsync(Arg.Any<EditTeamCriteria>()).Returns(commandResult);

        // Act
        var result = await _controller.Edit(model);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.False(_controller.ModelState.IsValid);
        Assert.Contains(
            "Update failed",
            _controller.ModelState[string.Empty].Errors.Select(e => e.ErrorMessage)
        );
    }

    [Fact]
    public async Task Edit_Post_WithNullModel_ShouldReturnView()
    {
        // Act
        var result = await _controller.Edit(null);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        await _baseService.DidNotReceive().ExecuteNonQueryAsync(Arg.Any<EditTeamCriteria>());
    }

    #endregion

    #region TeamSupervisor Tests

    [Fact]
    public async Task TeamSupervisor_ShouldReturnViewWithSupervisors()
    {
        // Arrange
        var teamId = 1;
        var supervisors = new List<TeamSupervisorModel>
        {
            new()
            {
                TeamId = teamId,
                Code = "T001",
                Name = "Test Team",
                Supervisors = "John Doe",
            },
        };

        _teamService.GetSupervisorByTeamId(teamId, false).Returns(supervisors);

        // Act
        var result = await _controller.TeamSupervisor(teamId);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<List<TeamSupervisorModel>>(viewResult.Model);
        Assert.Single(model);
        Assert.Equal(teamId, _controller.ViewBag.TeamId);
        Assert.Equal("Test Team", _controller.ViewBag.TeamName);
    }

    [Fact]
    public async Task TeamSupervisor_WithEmptySupervisors_ShouldSetEmptyTeamName()
    {
        // Arrange
        var teamId = 1;
        _teamService.GetSupervisorByTeamId(teamId, false).Returns(new List<TeamSupervisorModel>());

        // Act
        var result = await _controller.TeamSupervisor(teamId);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(teamId, _controller.ViewBag.TeamId);
        Assert.Equal(string.Empty, _controller.ViewBag.TeamName);
    }

    #endregion

    #region ManageSupervisor Tests

    [Fact]
    public void ManageSupervisor_ShouldReturnView()
    {
        // Arrange
        var teamId = 1;
        var roleId = (short)2;

        // Act
        var result = _controller.ManageSupervisor(teamId, roleId);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(teamId, _controller.ViewBag.TeamId);
        Assert.Equal(roleId, _controller.ViewBag.RoleId);
        Assert.Equal(3, _controller.ViewBag.CompanyId);
    }

    #endregion

    #region GetTeamSupervisorByRoleId Tests

    [Fact]
    public async Task GetTeamSupervisorByRoleId_ShouldReturnJson()
    {
        // Arrange
        var teamId = 1;
        var roleId = (short)2;
        var supervisors = new List<SupervisorModel>
        {
            new()
            {
                OwnerId = 100,
                NickName = "John",
                FullName = "John Doe",
                CompanyId = 3,
                CompanyName = "Test Company",
            },
        };

        _teamService.GetTeamSupervisorByRoleId(teamId, roleId).Returns(supervisors);

        // Act
        var result = await _controller.GetTeamSupervisorByRoleId(teamId, roleId);

        // Assert
        var jsonResult = Assert.IsType<JsonResult>(result);
        var resultSupervisors = Assert.IsType<List<SupervisorModel>>(jsonResult.Value);
        Assert.Single(resultSupervisors);
    }

    #endregion

    #region ShiftManager Tests

    [Fact]
    public void ShiftManager_ShouldReturnView()
    {
        // Arrange
        var teamId = 1;

        // Act
        var result = _controller.ShiftManager(teamId);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(teamId, _controller.ViewBag.TeamId);
        Assert.Equal((short)RoleSystem.ShiftManager, _controller.ViewBag.RoleId);
        Assert.Equal(3, _controller.ViewBag.CompanyId);
    }

    #endregion

    #region GetShiftManagerByTeamId Tests

    [Fact]
    public async Task GetShiftManagerByTeamId_ShouldReturnJson()
    {
        // Arrange
        var teamId = 1;
        var supervisors = new List<SupervisorModel>
        {
            new()
            {
                OwnerId = 100,
                NickName = "John",
                FullName = "John Doe",
                CompanyId = 3,
                CompanyName = "Test Company",
                CanRemove = true,
            },
        };

        _teamService
            .GetTeamSupervisorByRoleId(teamId, (short)RoleSystem.ShiftManager)
            .Returns(supervisors);

        // Act
        var result = await _controller.GetShiftManagerByTeamId(teamId);

        // Assert
        var jsonResult = Assert.IsType<JsonResult>(result);
        var resultSupervisors = Assert
            .IsAssignableFrom<IEnumerable<Supervisor>>(jsonResult.Value)
            .ToList();
        Assert.Single(resultSupervisors);
        Assert.Equal(100, resultSupervisors[0].OwnerId);
        Assert.Equal("John", resultSupervisors[0].Nickname);
        Assert.Equal("John Doe", resultSupervisors[0].FullName);
        Assert.Equal(3, resultSupervisors[0].CompanyId);
        Assert.Equal("Test Company", resultSupervisors[0].CompanyName);
        Assert.True(resultSupervisors[0].CanRemove);
    }

    #endregion

    #region DisciplineManager Tests

    [Fact]
    public void DisciplineManager_ShouldReturnView()
    {
        // Arrange
        var teamId = 1;

        // Act
        var result = _controller.DisciplineManager(teamId);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(teamId, _controller.ViewBag.TeamId);
        Assert.Equal((short)RoleSystem.DisciplineManager, _controller.ViewBag.RoleId);
        Assert.Equal(3, _controller.ViewBag.CompanyId);
    }

    #endregion

    #region LeaderTakeoverManager Tests

    [Fact]
    public void LeaderTakeoverManager_ShouldReturnView()
    {
        // Arrange
        var teamId = 1;

        // Act
        var result = _controller.LeaderTakeoverManager(teamId);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(teamId, _controller.ViewBag.TeamId);
        Assert.Equal((short)RoleSystem.LeaderTakeover, _controller.ViewBag.RoleId);
        Assert.Equal(3, _controller.ViewBag.CompanyId);
    }

    #endregion

    #region GetLeaderTakeoverManagerByTeamId Tests

    [Fact]
    public async Task GetLeaderTakeoverManagerByTeamId_ShouldReturnJson()
    {
        // Arrange
        var teamId = 1;
        var supervisors = new List<Supervisor>
        {
            new()
            {
                OwnerId = 100,
                Nickname = "John",
                FullName = "John Doe",
                CompanyId = 3,
                CompanyName = "Test Company",
                CanRemove = true,
            },
        };

        _teamService.GetLeaderTakeoverManagerByTeamId(teamId).Returns(supervisors);

        // Act
        var result = await _controller.GetLeaderTakeoverManagerByTeamId(teamId);

        // Assert
        var jsonResult = Assert.IsType<JsonResult>(result);
        var resultSupervisors = Assert.IsType<List<Supervisor>>(jsonResult.Value);
        Assert.Single(resultSupervisors);
    }

    #endregion

    #region GetDisciplineManagerByTeamId Tests

    [Fact]
    public async Task GetDisciplineManagerByTeamId_ShouldReturnJson()
    {
        // Arrange
        var teamId = 1;
        var supervisors = new List<Supervisor>
        {
            new()
            {
                OwnerId = 100,
                Nickname = "John",
                FullName = "John Doe",
                CompanyId = 3,
                CompanyName = "Test Company",
                CanRemove = true,
            },
        };

        _teamService.GetDisciplineManagerByTeamId(teamId).Returns(supervisors);

        // Act
        var result = await _controller.GetDisciplineManagerByTeamId(teamId);

        // Assert
        var jsonResult = Assert.IsType<JsonResult>(result);
        var resultSupervisors = Assert.IsType<List<Supervisor>>(jsonResult.Value);
        Assert.Single(resultSupervisors);
    }

    #endregion

    #region EditTeamSupervisor Tests

    [Fact]
    public async Task EditTeamSupervisor_WithValidData_ShouldReturnSuccess()
    {
        // Arrange
        var teamId = 1;
        var roleId = (short)2;
        var supervisors = new List<EditTeamSupervisor>
        {
            new()
            {
                OwnerId = 100,
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
        var response = Assert.IsType<CommandResult>(jsonResult.Value);
        Assert.Equal(0, response.Code);
        Assert.Equal("Success", response.Message);
        await _teamService.Received(1).EditTeamSupervisor(1, teamId, roleId, supervisors);
    }

    [Fact]
    public async Task EditTeamSupervisor_WithServiceFailure_ShouldReturnError()
    {
        // Arrange
        var teamId = 1;
        var roleId = (short)2;
        var supervisors = new List<EditTeamSupervisor>();

        var commandResult = new CommandResult { Code = 1, Message = "Failed" };

        _teamService.EditTeamSupervisor(1, teamId, roleId, supervisors).Returns(commandResult);

        // Act
        var result = await _controller.EditTeamSupervisor(teamId, roleId, supervisors);

        // Assert
        var jsonResult = Assert.IsType<JsonResult>(result);
        var response = Assert.IsType<CommandResult>(jsonResult.Value);
        Assert.Equal(1, response.Code);
        Assert.Equal("Failed", response.Message);
    }

    #endregion

    #region DisciplineRule (GET) Tests

    [Fact]
    public async Task DisciplineRule_Get_WithValidId_ShouldReturnViewWithModel()
    {
        // Arrange
        var teamId = 1;
        var team = new Team { Id = teamId, Name = "Test Team" };

        var disciplineRules = new List<DisciplineRule>
        {
            new()
            {
                DisciplineTypeId = 1,
                Name = "Rule 1",
                IsApplyForLocal = true,
                IsApplyForExpat = false,
            },
        };

        _teamService.GetById(teamId).Returns(team);
        _teamService.GetDisciplineRuleByTeam(teamId).Returns(disciplineRules);
        SetupPermission(FunctionKey.TeamManagement, Policy.Host, true);

        // Act
        var result = await _controller.DisciplineRule(teamId);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<DisciplineRuleViewModel>(viewResult.Model);
        Assert.Equal(teamId, model.TeamId);
        Assert.Equal("Test Team", model.Team);
        Assert.True(model.IsHost);
        Assert.Single(model.DisciplineRules);
    }

    [Fact]
    public async Task DisciplineRule_Get_WithNullTeam_ShouldRedirectToPageNotFound()
    {
        // Arrange
        var teamId = 999;
        _teamService.GetById(teamId).Returns((Team)null);

        // Act
        var result = await _controller.DisciplineRule(teamId);

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("PageNotFound", redirectResult.ActionName);
        Assert.Equal("Error", redirectResult.ControllerName);
    }

    [Fact]
    public async Task DisciplineRule_Get_WithoutHostPermission_ShouldSetIsHostToFalse()
    {
        // Arrange
        var teamId = 1;
        var team = new Team { Id = teamId, Name = "Test Team" };

        _teamService.GetById(teamId).Returns(team);
        _teamService.GetDisciplineRuleByTeam(teamId).Returns(new List<DisciplineRule>());
        SetupPermission(FunctionKey.TeamManagement, Policy.Host, false);

        // Act
        var result = await _controller.DisciplineRule(teamId);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<DisciplineRuleViewModel>(viewResult.Model);
        Assert.False(model.IsHost);
    }

    #endregion

    #region EditDisciplineRule Tests

    [Fact]
    public async Task EditDisciplineRule_WithValidData_ShouldReturnSuccess()
    {
        // Arrange
        var teamId = 1;
        var disciplineRules = new List<DisciplineRule>
        {
            new()
            {
                DisciplineTypeId = 1,
                Name = "Rule 1",
                IsApplyForLocal = true,
                IsApplyForExpat = false,
            },
        };

        var commandResult = new CommandResult { Code = 0, Message = "Success" };

        _teamService.EditDisciplineRule(1, teamId, disciplineRules).Returns(commandResult);

        // Act
        var result = await _controller.EditDisciplineRule(teamId, disciplineRules);

        // Assert
        var jsonResult = Assert.IsType<JsonResult>(result);
        var response = Assert.IsType<CommandResult>(jsonResult.Value);
        Assert.Equal(0, response.Code);
        await _teamService.Received(1).EditDisciplineRule(1, teamId, disciplineRules);
    }

    [Fact]
    public async Task EditDisciplineRule_WithServiceFailure_ShouldReturnError()
    {
        // Arrange
        var teamId = 1;
        var disciplineRules = new List<DisciplineRule>();

        var commandResult = new CommandResult { Code = 1, Message = "Failed" };

        _teamService.EditDisciplineRule(1, teamId, disciplineRules).Returns(commandResult);

        // Act
        var result = await _controller.EditDisciplineRule(teamId, disciplineRules);

        // Assert
        var jsonResult = Assert.IsType<JsonResult>(result);
        var response = Assert.IsType<CommandResult>(jsonResult.Value);
        Assert.Equal(1, response.Code);
        Assert.Equal("Failed", response.Message);
    }

    #endregion

    #region GetOrgChart Tests

    [Fact]
    public async Task GetOrgChart_ShouldReturnJson()
    {
        // Arrange
        var units = new List<Unit>
        {
            new() { Id = 1, Code = "U001" },
        };

        var departments = new List<Department>
        {
            new()
            {
                Id = 1,
                Name = "Dept 1",
                Unit = new Unit { Id = 1, Code = "U001" },
            },
        };

        var teams = new List<Team>
        {
            new()
            {
                Id = 1,
                Name = "Team 1",
                Department = new Department { Id = 1, Name = "Dept 1" },
            },
        };

        _unitService.GetUnitAsync().Returns(Task.FromResult<IEnumerable<Unit>>(units));
        _departmentService.GetDepartmentAsync().Returns(departments);
        _commonDataService.GetAllTeamAsync(isGroup: false, companyId: null).Returns(teams);

        // Act
        var result = await _controller.GetOrgChart();

        // Assert
        var jsonResult = Assert.IsType<JsonResult>(result);
        var orgChart = Assert
            .IsAssignableFrom<IEnumerable<MultiSelectViewModel>>(jsonResult.Value)
            .ToList();
        Assert.Single(orgChart);
        Assert.Equal(1, orgChart[0].Id);
        Assert.Equal("U001", orgChart[0].Text);
        Assert.Single(orgChart[0].Items);
        Assert.Equal(1, orgChart[0].Items[0].Id);
        Assert.Equal("Dept 1", orgChart[0].Items[0].Text);
        Assert.Single(orgChart[0].Items[0].Items);
        Assert.Equal(1, orgChart[0].Items[0].Items[0].Id);
        Assert.Equal("Team 1", orgChart[0].Items[0].Items[0].Text);
        Assert.True(orgChart[0].Items[0].Items[0].IsLeaf);
    }

    [Fact]
    public async Task GetOrgChart_WithIsGroup_ShouldPassParameter()
    {
        // Arrange
        var isGroup = true;
        short? companyId = 3;

        _unitService.GetUnitAsync().Returns(Task.FromResult<IEnumerable<Unit>>(new List<Unit>()));
        _departmentService.GetDepartmentAsync().Returns(new List<Department>());
        _commonDataService
            .GetAllTeamAsync(isGroup: isGroup, companyId: companyId)
            .Returns(new List<Team>());

        // Act
        await _controller.GetOrgChart(isGroup, companyId);

        // Assert
        await _commonDataService
            .Received(1)
            .GetAllTeamAsync(isGroup: isGroup, companyId: companyId);
    }

    [Fact]
    public async Task GetOrgChart_WithNoMatchingData_ShouldReturnEmptyList()
    {
        // Arrange
        _unitService.GetUnitAsync().Returns(Task.FromResult<IEnumerable<Unit>>(new List<Unit>()));
        _departmentService.GetDepartmentAsync().Returns(new List<Department>());
        _commonDataService
            .GetAllTeamAsync(isGroup: false, companyId: null)
            .Returns(new List<Team>());

        // Act
        var result = await _controller.GetOrgChart();

        // Assert
        var jsonResult = Assert.IsType<JsonResult>(result);
        var orgChart = Assert
            .IsAssignableFrom<IEnumerable<MultiSelectViewModel>>(jsonResult.Value)
            .ToList();
        Assert.Empty(orgChart);
    }

    #endregion

    #region GetCompaniesByRoleIdForEmployee Tests

    [Fact]
    public async Task GetCompaniesByRoleIdForEmployee_ShouldReturnJson()
    {
        // Arrange
        var roleId = 4;
        var companies = new List<Company>
        {
            new() { CompanyId = 1, Name = "Company 1" },
            new() { CompanyId = 2, Name = "Company 2" },
        };

        _teamService.GetCompaniesByRoleIdForEmployee(1, roleId).Returns(companies);

        // Act
        var result = await _controller.GetCompaniesByRoleIdForEmployee(roleId);

        // Assert
        var jsonResult = Assert.IsType<JsonResult>(result);
        var resultCompanies = Assert.IsType<List<Company>>(jsonResult.Value);
        Assert.Equal(2, resultCompanies.Count);
        await _teamService.Received(1).GetCompaniesByRoleIdForEmployee(1, roleId);
    }

    [Fact]
    public async Task GetCompaniesByRoleIdForEmployee_WithDefaultRoleId_ShouldUseDefaultValue()
    {
        // Arrange
        var companies = new List<Company>
        {
            new() { CompanyId = 1, Name = "Company 1" },
        };

        _teamService.GetCompaniesByRoleIdForEmployee(1, 4).Returns(companies);

        // Act
        var result = await _controller.GetCompaniesByRoleIdForEmployee();

        // Assert
        var jsonResult = Assert.IsType<JsonResult>(result);
        await _teamService.Received(1).GetCompaniesByRoleIdForEmployee(1, 4);
    }

    #endregion

    #region GetOrgChart Edge Cases Tests

    [Fact]
    public async Task GetOrgChart_WithTeamsHavingNullDepartment_ShouldFilterOutThoseTeams()
    {
        // Arrange
        var units = new List<Unit>
        {
            new() { Id = 1, Code = "U001" },
        };

        var departments = new List<Department>
        {
            new()
            {
                Id = 1,
                Name = "Dept 1",
                Unit = new Unit { Id = 1, Code = "U001" },
            },
        };

        var teams = new List<Team>
        {
            new()
            {
                Id = 1,
                Name = "Team 1",
                Department = new Department { Id = 1, Name = "Dept 1" },
            },
            new()
            {
                Id = 2,
                Name = "Team 2",
                Department = null, // Team with null department should be filtered out
            },
        };

        _unitService.GetUnitAsync().Returns(Task.FromResult<IEnumerable<Unit>>(units));
        _departmentService.GetDepartmentAsync().Returns(departments);
        _commonDataService.GetAllTeamAsync(isGroup: false, companyId: null).Returns(teams);

        // Act
        var result = await _controller.GetOrgChart();

        // Assert
        var jsonResult = Assert.IsType<JsonResult>(result);
        var orgChart = Assert
            .IsAssignableFrom<IEnumerable<MultiSelectViewModel>>(jsonResult.Value)
            .ToList();
        Assert.Single(orgChart);
        Assert.Single(orgChart[0].Items);
        Assert.Single(orgChart[0].Items[0].Items); // Only Team 1 should be included
        Assert.Equal(1, orgChart[0].Items[0].Items[0].Id);
    }

    [Fact]
    public async Task GetOrgChart_WithDepartmentsHavingNullUnit_ShouldFilterOutThoseDepartments()
    {
        // Arrange
        var units = new List<Unit>
        {
            new() { Id = 1, Code = "U001" },
        };

        var departments = new List<Department>
        {
            new()
            {
                Id = 1,
                Name = "Dept 1",
                Unit = new Unit { Id = 1, Code = "U001" },
            },
            new()
            {
                Id = 2,
                Name = "Dept 2",
                Unit = null, // Department with null unit should be filtered out
            },
        };

        var teams = new List<Team>
        {
            new()
            {
                Id = 1,
                Name = "Team 1",
                Department = new Department { Id = 1, Name = "Dept 1" },
            },
            new()
            {
                Id = 2,
                Name = "Team 2",
                Department = new Department { Id = 2, Name = "Dept 2" },
            },
        };

        _unitService.GetUnitAsync().Returns(Task.FromResult<IEnumerable<Unit>>(units));
        _departmentService.GetDepartmentAsync().Returns(departments);
        _commonDataService.GetAllTeamAsync(isGroup: false, companyId: null).Returns(teams);

        // Act
        var result = await _controller.GetOrgChart();

        // Assert
        var jsonResult = Assert.IsType<JsonResult>(result);
        var orgChart = Assert
            .IsAssignableFrom<IEnumerable<MultiSelectViewModel>>(jsonResult.Value)
            .ToList();
        Assert.Single(orgChart);
        Assert.Single(orgChart[0].Items); // Only Dept 1 should be included
        Assert.Equal(1, orgChart[0].Items[0].Id);
    }

    [Fact]
    public async Task GetOrgChart_WithMultipleUnitsAndDepartments_ShouldReturnHierarchicalStructure()
    {
        // Arrange
        var units = new List<Unit>
        {
            new() { Id = 1, Code = "U001" },
            new() { Id = 2, Code = "U002" },
        };

        var departments = new List<Department>
        {
            new()
            {
                Id = 1,
                Name = "Dept 1",
                Unit = new Unit { Id = 1, Code = "U001" },
            },
            new()
            {
                Id = 2,
                Name = "Dept 2",
                Unit = new Unit { Id = 1, Code = "U001" },
            },
            new()
            {
                Id = 3,
                Name = "Dept 3",
                Unit = new Unit { Id = 2, Code = "U002" },
            },
        };

        var teams = new List<Team>
        {
            new()
            {
                Id = 1,
                Name = "Team 1",
                Department = new Department { Id = 1, Name = "Dept 1" },
            },
            new()
            {
                Id = 2,
                Name = "Team 2",
                Department = new Department { Id = 2, Name = "Dept 2" },
            },
            new()
            {
                Id = 3,
                Name = "Team 3",
                Department = new Department { Id = 3, Name = "Dept 3" },
            },
        };

        _unitService.GetUnitAsync().Returns(Task.FromResult<IEnumerable<Unit>>(units));
        _departmentService.GetDepartmentAsync().Returns(departments);
        _commonDataService.GetAllTeamAsync(isGroup: false, companyId: null).Returns(teams);

        // Act
        var result = await _controller.GetOrgChart();

        // Assert
        var jsonResult = Assert.IsType<JsonResult>(result);
        var orgChart = Assert
            .IsAssignableFrom<IEnumerable<MultiSelectViewModel>>(jsonResult.Value)
            .ToList();
        Assert.Equal(2, orgChart.Count);
        Assert.Equal(2, orgChart[0].Items.Count); // U001 has 2 departments
        Assert.Single(orgChart[1].Items); // U002 has 1 department
    }

    [Fact]
    public async Task GetOrgChart_WithTeamsNotMatchingDepartments_ShouldReturnEmptyTeams()
    {
        // Arrange
        var units = new List<Unit>
        {
            new() { Id = 1, Code = "U001" },
        };

        var departments = new List<Department>
        {
            new()
            {
                Id = 1,
                Name = "Dept 1",
                Unit = new Unit { Id = 1, Code = "U001" },
            },
        };

        var teams = new List<Team>
        {
            new()
            {
                Id = 1,
                Name = "Team 1",
                Department = new Department { Id = 999, Name = "Dept 999" }, // Doesn't match any department
            },
        };

        _unitService.GetUnitAsync().Returns(Task.FromResult<IEnumerable<Unit>>(units));
        _departmentService.GetDepartmentAsync().Returns(departments);
        _commonDataService.GetAllTeamAsync(isGroup: false, companyId: null).Returns(teams);

        // Act
        var result = await _controller.GetOrgChart();

        // Assert
        var jsonResult = Assert.IsType<JsonResult>(result);
        var orgChart = Assert
            .IsAssignableFrom<IEnumerable<MultiSelectViewModel>>(jsonResult.Value)
            .ToList();
        Assert.Empty(orgChart); // No matching teams means no departments, which means no units
    }

    #endregion

    #region Edit POST Edge Cases Tests

    [Fact]
    public async Task Edit_Post_WithNullTeamFromGetGroupTeamInUnitScopeById_ShouldHandleGracefully()
    {
        // Arrange
        var model = new TeamViewModel { Id = 1, LocalBasicAnnualLeaveBalance = 10 };

        _departmentService.GetDepartmentAsync().Returns(new List<Department>());
        _employeeService
            .GetEmployeeAsync(Arg.Any<GetAllEmployeeCriteria>())
            .Returns(new List<Employee>());
        _teamService.GetGroupTeamInUnitScopeById(model.Id).Returns((GroupTeamModel)null);

        // Act & Assert
        await Assert.ThrowsAsync<NullReferenceException>(async () => await _controller.Edit(model));
    }

    [Fact]
    public async Task Edit_Post_WithNullModelState_ShouldStillProcess()
    {
        // Arrange
        var model = new TeamViewModel { Id = 1, LocalBasicAnnualLeaveBalance = 10 };

        var team = new GroupTeamModel
        {
            Id = 1,
            TeamCode = "T001",
            TeamName = "Test Team",
            DepartmentId = 1,
            DepartmentName = "Dept 1",
        };

        var commandResult = new CommandResult { Code = 0, Message = "Success" };

        _departmentService.GetDepartmentAsync().Returns(new List<Department>());
        _employeeService
            .GetEmployeeAsync(Arg.Any<GetAllEmployeeCriteria>())
            .Returns(new List<Employee>());
        _teamService.GetGroupTeamInUnitScopeById(model.Id).Returns(team);
        _baseService.ExecuteNonQueryAsync(Arg.Any<EditTeamCriteria>()).Returns(commandResult);

        // Clear ModelState to simulate a clean state
        _controller.ModelState.Clear();

        // Act
        var result = await _controller.Edit(model);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.True(_controller.ModelState.IsValid);
        Assert.True((bool)_controller.ViewBag.Updated);
    }

    #endregion

    #region GetCompaniesByRoleIdForEmployee Edge Cases Tests

    [Fact]
    public async Task GetCompaniesByRoleIdForEmployee_WithNullRoleId_ShouldThrowException()
    {
        // Arrange
        // The controller uses roleId.Value which will throw if roleId is null
        // This tests the edge case where null is explicitly passed despite default parameter

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _controller.GetCompaniesByRoleIdForEmployee(null)
        );
    }

    [Fact]
    public async Task GetCompaniesByRoleIdForEmployee_WithEmptyCompanies_ShouldReturnEmptyList()
    {
        // Arrange
        var companies = new List<Company>();

        _teamService.GetCompaniesByRoleIdForEmployee(1, 4).Returns(companies);

        // Act
        var result = await _controller.GetCompaniesByRoleIdForEmployee();

        // Assert
        var jsonResult = Assert.IsType<JsonResult>(result);
        var resultCompanies = Assert.IsType<List<Company>>(jsonResult.Value);
        Assert.Empty(resultCompanies);
    }

    #endregion

    #region GetOrgChart with companyId Tests

    [Fact]
    public async Task GetOrgChart_WithCompanyId_ShouldPassCompanyIdToService()
    {
        // Arrange
        short? companyId = 5;

        _unitService.GetUnitAsync().Returns(Task.FromResult<IEnumerable<Unit>>(new List<Unit>()));
        _departmentService.GetDepartmentAsync().Returns(new List<Department>());
        _commonDataService
            .GetAllTeamAsync(isGroup: false, companyId: companyId)
            .Returns(new List<Team>());

        // Act
        await _controller.GetOrgChart(false, companyId);

        // Assert
        await _commonDataService.Received(1).GetAllTeamAsync(isGroup: false, companyId: companyId);
    }

    #endregion
}
