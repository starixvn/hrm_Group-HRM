using GO.UserControl.Client.Core.Enumerables;
using GO.UserControl.Client.Core.Models.User;
using GO.UserControl.Client.Core.Services;
using HRM.Controllers.GroupSpace;
using HRM.Core.Constants;
using HRM.Core.Enums;
using HRM.Core.Models;
using HRM.Core.Services;
using HRM.Criteria.Employee;
using HRM.Criteria.EmployeeWorkFromHome;
using HRM.Criteria.Group;
using HRM.Criteria.Supervisor;
using HRM.Models;
using HRM.Models.Group;
using HRM.Models.Supervisor;
using HRM.Models.Unit;
using HRM.Services.ExportExcel.Employee;
using HRM.Services.Implement;
using HRM.Services.Interfaces;
using HRM.ViewModels.EmployeeGroup;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Xunit;

namespace HRM.Controllers.UnitTests;

public class GroupEmployeeListControllerTest : BaseControllerTest
{
    private readonly IBaseService _baseService;
    private readonly IMonthlyPayrollService _monthlyPayrollService;
    private readonly GroupEmployeeListController _controller;

    public GroupEmployeeListControllerTest()
    {
        var teamService = Substitute.For<ITeamService>();
        var companyService = Substitute.For<ICompanyService>();
        var unitService = Substitute.For<IUnitService>();
        var departmentService = Substitute.For<IDepartmentService>();
        var excelService = Substitute.For<IExportExcelEmployeeInfoService>();
        _baseService = Substitute.For<IBaseService>();
        var employeeService = Substitute.For<IEmployeeService>();
        var secondCompanyEmployeeService = Substitute.For<ISecondCompanyEmployeeService>();
        var userService = Substitute.For<IUserService>();
        var commonDataService = Substitute.For<ICommonDataService>();
        var jobLevelService = Substitute.For<IJobLevelService>();
        _monthlyPayrollService = Substitute.For<IMonthlyPayrollService>();
        var managementLevelService = Substitute.For<IManagementLevelService>();

        _controller = new GroupEmployeeListController(
            _session,
            teamService,
            companyService,
            unitService,
            departmentService,
            excelService,
            _baseService,
            employeeService,
            secondCompanyEmployeeService,
            userService,
            commonDataService,
            jobLevelService,
            _monthlyPayrollService,
            managementLevelService);
    }

    [Fact]
    public async Task ListEmployee_WithValidModel_ReturnsPartialView()
    {
        // Arrange
        var model = new GroupEmployeeViewModel
        {
            Keyword = "test",
            TeamIds = "1,2",
            CompanyIds = "1,2",
            EmployeeStatus = "1"
        };

        var employees = new List<GroupEmployeeModel>
        {
            new() { EmpId = 1, FullName = "John Doe", EmployeeCode = "EMP001" },
            new() { EmpId = 2, FullName = "Jane Smith", EmployeeCode = "EMP002" }
        };

        _baseService.GetDataAsync<GroupEmployeeModel>(Arg.Any<GetEmployeesInGroupScopeCriteria>())
            .Returns(employees);

        SetupPermission(FunctionKey.GroupEmployeeList_ViewAll, Policy.Read, true);
        SetupPermission(FunctionKey.ShowSalaryCurrencyInfo, Policy.Read, false);

        // Act
        var result = await _controller.ListEmployee(model);

        // Assert
        var partialViewResult = Assert.IsType<PartialViewResult>(result);
        Assert.Equal("_ListEmployee", partialViewResult.ViewName);
        Assert.Equal(employees, partialViewResult.Model);
    }

    [Fact]
    public async Task ListEmployee_WithSalaryPermission_CallsLocalPayrollService()
    {
        // Arrange
        var model = new GroupEmployeeViewModel
        {
            Keyword = "test",
            TeamIds = "1,2",
            CompanyIds = "1,2",
            EmployeeStatus = "1"
        };

        var employees = new List<GroupEmployeeModel>
        {
            new() { EmpId = 1, FullName = "John Doe", EmployeeCode = "EMP001" },
            new() { EmpId = 2, FullName = "Jane Smith", EmployeeCode = "EMP002" }
        };

        _baseService.GetDataAsync<GroupEmployeeModel>(Arg.Any<GetEmployeesInGroupScopeCriteria>())
            .Returns(employees);
        
        // Set up the mock to set salaries on the employees
        _monthlyPayrollService
            .When(x => x.CalculateMonthlySalary(Arg.Any<IEnumerable<GroupEmployeeModel>>()))
            .Do(call =>
            {
                var emps = call.Arg<IEnumerable<GroupEmployeeModel>>().ToList();
                var emp1 = emps.Find(e => e.EmpId == 1);
                var emp2 = emps.Find(e => e.EmpId == 2);
                if (emp1 != null)
                {
                    emp1.Salary = 5000m;
                }
                if (emp2 != null)
                {
                    emp2.Salary = 6000m;
                }
            });

        SetupPermission(FunctionKey.GroupEmployeeList_ViewAll, Policy.Read, true);
        SetupPermission(FunctionKey.ShowSalaryCurrencyInfo, Policy.Read, true);

        // Act
        var result = await _controller.ListEmployee(model);

        // Assert
        await _monthlyPayrollService.Received(1).CalculateMonthlySalary(Arg.Is<IEnumerable<GroupEmployeeModel>>(emps => 
            emps.Any(e => e.EmpId == 1) && emps.Any(e => e.EmpId == 2)));

        var partialViewResult = Assert.IsType<PartialViewResult>(result);
        var returnedEmployees = Assert.IsAssignableFrom<IEnumerable<GroupEmployeeModel>>(partialViewResult.Model);
        var employeeList = returnedEmployees.ToList();
        
        Assert.Equal(5000m, employeeList.First(e => e.EmpId == 1).Salary);
        Assert.Equal(6000m, employeeList.First(e => e.EmpId == 2).Salary);
    }

    [Fact]
    public async Task ListEmployee_WithViewAllPermission_ReturnsAllEmployees()
    {
        // Arrange
        var model = new GroupEmployeeViewModel
        {
            Keyword = "test",
            TeamIds = "1,2",
            CompanyIds = "1,2",
            EmployeeStatus = "1"
        };

        var employees = new List<GroupEmployeeModel>
        {
            new() { EmpId = 1, FullName = "John Doe", EmployeeCode = "EMP001" },
            new() { EmpId = 2, FullName = "Jane Smith", EmployeeCode = "EMP002" },
            new() { EmpId = 3, FullName = "Bob Wilson", EmployeeCode = "EMP003" }
        };

        _baseService.GetDataAsync<GroupEmployeeModel>(Arg.Any<GetEmployeesInGroupScopeCriteria>())
            .Returns(employees);

        SetupPermission(FunctionKey.GroupEmployeeList_ViewAll, Policy.Read, true);
        SetupPermission(FunctionKey.ShowSalaryCurrencyInfo, Policy.Read, false);

        // Act
        var result = await _controller.ListEmployee(model);

        // Assert
        var partialViewResult = Assert.IsType<PartialViewResult>(result);
        var returnedEmployees = Assert.IsAssignableFrom<IEnumerable<GroupEmployeeModel>>(partialViewResult.Model);
        var employeeList = returnedEmployees.ToList();
        
        Assert.Equal(3, employeeList.Count);
        Assert.Contains(employeeList, e => e.EmpId == 1);
        Assert.Contains(employeeList, e => e.EmpId == 2);
        Assert.Contains(employeeList, e => e.EmpId == 3);
    }

    [Fact]
    public async Task ListEmployee_WithBUViewPermission_FiltersByUnitPIC()
    {
        // Arrange
        var model = new GroupEmployeeViewModel
        {
            Keyword = "test",
            TeamIds = "1,2",
            CompanyIds = "1,2",
            EmployeeStatus = "1"
        };

        var employees = new List<GroupEmployeeModel>
        {
            new() { EmpId = 1, FullName = "John Doe", UnitPICEmpId = 1 }, // Current user is Unit PIC
            new() { EmpId = 2, FullName = "Jane Smith", UnitPICEmpId = 2 }, // Different Unit PIC
            new() { EmpId = 3, FullName = "Bob Wilson", UnitPICEmpId = 1 } // Current user is Unit PIC
        };

        _baseService.GetDataAsync<GroupEmployeeModel>(Arg.Any<GetEmployeesInGroupScopeCriteria>())
            .Returns(employees);

        SetupPermission(FunctionKey.GroupEmployeeList_ViewAll, Policy.Read, false);
        SetupPermission(FunctionKey.GroupEmployeeList_BUView, Policy.Read, true);
        SetupPermission(FunctionKey.ShowSalaryCurrencyInfo, Policy.Read, false);

        // Act
        var result = await _controller.ListEmployee(model);

        // Assert
        var partialViewResult = Assert.IsType<PartialViewResult>(result);
        var returnedEmployees = Assert.IsAssignableFrom<IEnumerable<GroupEmployeeModel>>(partialViewResult.Model);
        var employeeList = returnedEmployees.ToList();
        
        Assert.Equal(2, employeeList.Count);
        Assert.Contains(employeeList, e => e.EmpId == 1);
        Assert.Contains(employeeList, e => e.EmpId == 3);
        Assert.DoesNotContain(employeeList, e => e.EmpId == 2);
    }

    [Fact]
    public async Task ListEmployee_WithBUViewPermission_FiltersByUnitSupporter()
    {
        // Arrange
        var model = new GroupEmployeeViewModel
        {
            Keyword = "test",
            TeamIds = "1,2",
            CompanyIds = "1,2",
            EmployeeStatus = "1"
        };

        var employees = new List<GroupEmployeeModel>
        {
            new() { EmpId = 1, FullName = "John Doe", UnitSupporterEmpId = "1,5" }, // Current user is supporter
            new() { EmpId = 2, FullName = "Jane Smith", UnitSupporterEmpId = "2,3" }, // Different supporters
            new() { EmpId = 3, FullName = "Bob Wilson", UnitSupporterEmpId = "1" } // Current user is supporter
        };

        _baseService.GetDataAsync<GroupEmployeeModel>(Arg.Any<GetEmployeesInGroupScopeCriteria>())
            .Returns(employees);

        SetupPermission(FunctionKey.GroupEmployeeList_ViewAll, Policy.Read, false);
        SetupPermission(FunctionKey.GroupEmployeeList_BUView, Policy.Read, true);
        SetupPermission(FunctionKey.ShowSalaryCurrencyInfo, Policy.Read, false);

        // Act
        var result = await _controller.ListEmployee(model);

        // Assert
        var partialViewResult = Assert.IsType<PartialViewResult>(result);
        var returnedEmployees = Assert.IsAssignableFrom<IEnumerable<GroupEmployeeModel>>(partialViewResult.Model);
        var employeeList = returnedEmployees.ToList();
        
        Assert.Equal(2, employeeList.Count);
        Assert.Contains(employeeList, e => e.EmpId == 1);
        Assert.Contains(employeeList, e => e.EmpId == 3);
        Assert.DoesNotContain(employeeList, e => e.EmpId == 2);
    }

    [Fact]
    public async Task ListEmployee_WithDeptViewPermission_FiltersByDepartmentPIC()
    {
        // Arrange
        var model = new GroupEmployeeViewModel
        {
            Keyword = "test",
            TeamIds = "1,2",
            CompanyIds = "1,2",
            EmployeeStatus = "1"
        };

        var employees = new List<GroupEmployeeModel>
        {
            new() { EmpId = 1, FullName = "John Doe", DepartmentPICId = 1 }, // Current user is Dept PIC
            new() { EmpId = 2, FullName = "Jane Smith", DepartmentPICId = 2 }, // Different Dept PIC
            new() { EmpId = 3, FullName = "Bob Wilson", DepartmentPICId = 1 } // Current user is Dept PIC
        };

        _baseService.GetDataAsync<GroupEmployeeModel>(Arg.Any<GetEmployeesInGroupScopeCriteria>())
            .Returns(employees);

        SetupPermission(FunctionKey.GroupEmployeeList_ViewAll, Policy.Read, false);
        SetupPermission(FunctionKey.GroupEmployeeList_DeptView, Policy.Read, true);
        SetupPermission(FunctionKey.ShowSalaryCurrencyInfo, Policy.Read, false);

        // Act
        var result = await _controller.ListEmployee(model);

        // Assert
        var partialViewResult = Assert.IsType<PartialViewResult>(result);
        var returnedEmployees = Assert.IsAssignableFrom<IEnumerable<GroupEmployeeModel>>(partialViewResult.Model);
        var employeeList = returnedEmployees.ToList();
        
        Assert.Equal(2, employeeList.Count);
        Assert.Contains(employeeList, e => e.EmpId == 1);
        Assert.Contains(employeeList, e => e.EmpId == 3);
        Assert.DoesNotContain(employeeList, e => e.EmpId == 2);
    }

    [Fact]
    public async Task ListEmployee_WithBothBUAndDeptView_ReturnsUnionOfBothPermissions()
    {
        // Arrange
        var model = new GroupEmployeeViewModel
        {
            Keyword = "test",
            TeamIds = "1,2",
            CompanyIds = "1,2",
            EmployeeStatus = "1"
        };

        var employees = new List<GroupEmployeeModel>
        {
            new() { EmpId = 1, FullName = "John Doe", UnitPICEmpId = 1 }, // Unit PIC only
            new() { EmpId = 2, FullName = "Jane Smith", DepartmentPICId = 1 }, // Dept PIC only
            new() { EmpId = 3, FullName = "Bob Wilson", UnitPICEmpId = 2, DepartmentPICId = 2 }, // Neither
            new() { EmpId = 4, FullName = "Alice Brown", UnitPICEmpId = 1, DepartmentPICId = 1 } // Both
        };

        _baseService.GetDataAsync<GroupEmployeeModel>(Arg.Any<GetEmployeesInGroupScopeCriteria>())
            .Returns(employees);

        SetupPermission(FunctionKey.GroupEmployeeList_ViewAll, Policy.Read, false);
        SetupPermission(FunctionKey.GroupEmployeeList_BUView, Policy.Read, true);
        SetupPermission(FunctionKey.GroupEmployeeList_DeptView, Policy.Read, true);
        SetupPermission(FunctionKey.ShowSalaryCurrencyInfo, Policy.Read, false);

        // Act
        var result = await _controller.ListEmployee(model);

        // Assert
        var partialViewResult = Assert.IsType<PartialViewResult>(result);
        var returnedEmployees = Assert.IsAssignableFrom<IEnumerable<GroupEmployeeModel>>(partialViewResult.Model);
        var employeeList = returnedEmployees.ToList();
        
        Assert.Equal(3, employeeList.Count);
        Assert.Contains(employeeList, e => e.EmpId == 1); // Unit PIC
        Assert.Contains(employeeList, e => e.EmpId == 2); // Dept PIC
        Assert.Contains(employeeList, e => e.EmpId == 4); // Both
        Assert.DoesNotContain(employeeList, e => e.EmpId == 3); // Neither
    }

    [Fact]
    public async Task Index_WithTeamIds_ReturnsViewWithModel()
    {
        // Arrange
        var teams = new List<Models.Team> { new() { Id = 1, Name = "Team1" } };
        var companies = new List<Models.Company> { new() { CompanyId = 1, Name = "Company1" } };

        var teamService = Substitute.For<ITeamService>();
        var companyService = Substitute.For<ICompanyService>();
        teamService.GetAllAsync().Returns(teams);
        companyService.GetAllCompany().Returns(companies);

        var controller = new GroupEmployeeListController(
            _session, teamService, companyService, Substitute.For<IUnitService>(),
            Substitute.For<IDepartmentService>(), Substitute.For<IExportExcelEmployeeInfoService>(),
            _baseService, Substitute.For<IEmployeeService>(), Substitute.For<ISecondCompanyEmployeeService>(),
            Substitute.For<IUserService>(), Substitute.For<ICommonDataService>(),
            Substitute.For<IJobLevelService>(), _monthlyPayrollService, Substitute.For<IManagementLevelService>());

        // Act
        var result = await controller.Index("1,2");

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<GroupEmployeeViewModel>(viewResult.Model);
        Assert.Equal("1,2", model.TeamIds);
    }

    [Fact]
    public async Task Index_WithoutTeamIds_ReturnsViewWithAllTeams()
    {
        // Arrange
        var teams = new List<Models.Team> 
        { 
            new() { Id = 1, Name = "Team1" },
            new() { Id = 2, Name = "Team2" }
        };
        var companies = new List<Models.Company> { new() { CompanyId = 1, Name = "Company1" } };

        var teamService = Substitute.For<ITeamService>();
        var companyService = Substitute.For<ICompanyService>();
        teamService.GetAllAsync().Returns(teams);
        companyService.GetAllCompany().Returns(companies);

        var controller = new GroupEmployeeListController(
            _session, teamService, companyService, Substitute.For<IUnitService>(),
            Substitute.For<IDepartmentService>(), Substitute.For<IExportExcelEmployeeInfoService>(),
            _baseService, Substitute.For<IEmployeeService>(), Substitute.For<ISecondCompanyEmployeeService>(),
            Substitute.For<IUserService>(), Substitute.For<ICommonDataService>(),
            Substitute.For<IJobLevelService>(), _monthlyPayrollService, Substitute.For<IManagementLevelService>());

        // Act
        var result = await controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<GroupEmployeeViewModel>(viewResult.Model);
        Assert.Equal("1,2", model.TeamIds);
    }

    [Fact]
    public async Task GetCompanyDropdownList_ReturnsJsonWithCompanies()
    {
        // Arrange
        var companies = new List<Models.Company> 
        { 
            new() { CompanyId = 1, Name = "Company1" },
            new() { CompanyId = 2, Name = "Company2" }
        };

        var companyService = Substitute.For<ICompanyService>();
        companyService.GetAllCompany().Returns(companies);

        var controller = new GroupEmployeeListController(
            _session, Substitute.For<ITeamService>(), companyService, Substitute.For<IUnitService>(),
            Substitute.For<IDepartmentService>(), Substitute.For<IExportExcelEmployeeInfoService>(),
            _baseService, Substitute.For<IEmployeeService>(), Substitute.For<ISecondCompanyEmployeeService>(),
            Substitute.For<IUserService>(), Substitute.For<ICommonDataService>(),
            Substitute.For<IJobLevelService>(), _monthlyPayrollService, Substitute.For<IManagementLevelService>());

        // Act
        var result = await controller.GetCompanyDropdownList();

        // Assert
        var jsonResult = Assert.IsType<JsonResult>(result);
        Assert.NotNull(jsonResult.Value);
    }

    [Fact]
    public async Task GetOrgChart_WithDefaultParameters_ReturnsJsonWithOrgChart()
    {
        // Arrange
        var units = new List<Unit> { new() { Id = 1, Code = "Unit1" } };
        var departments = new List<Department> { new() { Id = 1, Name = "Dept1", Unit = units[0] } };
        var teams = new List<GroupTeamModel> { new() { Id = 1, TeamName = "Team1", DepartmentId = 1 } };

        var unitService = Substitute.For<IUnitService>();
        var departmentService = Substitute.For<IDepartmentService>();
        var teamService = Substitute.For<ITeamService>();

        unitService.GetUnitAsync().Returns(units);
        departmentService.GetDepartmentAsync().Returns(departments);
        teamService.GetAllTeamByUnitScopeAsync().Returns(teams);

        var controller = new GroupEmployeeListController(
            _session, teamService, Substitute.For<ICompanyService>(), unitService,
            departmentService, Substitute.For<IExportExcelEmployeeInfoService>(),
            _baseService, Substitute.For<IEmployeeService>(), Substitute.For<ISecondCompanyEmployeeService>(),
            Substitute.For<IUserService>(), Substitute.For<ICommonDataService>(),
            Substitute.For<IJobLevelService>(), _monthlyPayrollService, Substitute.For<IManagementLevelService>());

        // Act
        var result = await controller.GetOrgChart();

        // Assert
        var jsonResult = Assert.IsType<JsonResult>(result);
        var response = Assert.IsType<GroupTeamMultiSelectViewModel>(jsonResult.Value);
        Assert.NotNull(response.Data);
        Assert.Equal("1", response.TeamIds);
    }

    [Fact]
    public async Task GetOrgChart_WithIsGroupTrue_ReturnsJsonWithOrgChart()
    {
        // Arrange
        var units = new List<Unit> { new() { Id = 1, Code = "Unit1" } };
        var departments = new List<Department> { new() { Id = 1, Name = "Dept1", Unit = units[0] } };
        var teams = new List<GroupTeamModel> { new() { Id = 1, TeamName = "Team1", DepartmentId = 1 } };

        var unitService = Substitute.For<IUnitService>();
        var departmentService = Substitute.For<IDepartmentService>();
        var teamService = Substitute.For<ITeamService>();

        unitService.GetUnitAsync().Returns(units);
        departmentService.GetDepartmentAsync().Returns(departments);
        teamService.GetAllTeamByUnitScopeAsync().Returns(teams);

        var controller = new GroupEmployeeListController(
            _session, teamService, Substitute.For<ICompanyService>(), unitService,
            departmentService, Substitute.For<IExportExcelEmployeeInfoService>(),
            _baseService, Substitute.For<IEmployeeService>(), Substitute.For<ISecondCompanyEmployeeService>(),
            Substitute.For<IUserService>(), Substitute.For<ICommonDataService>(),
            Substitute.For<IJobLevelService>(), _monthlyPayrollService, Substitute.For<IManagementLevelService>());

        // Act
        var result = await controller.GetOrgChart(true);

        // Assert
        var jsonResult = Assert.IsType<JsonResult>(result);
        var response = Assert.IsType<GroupTeamMultiSelectViewModel>(jsonResult.Value);
        Assert.NotNull(response.Data);
        Assert.Equal("1", response.TeamIds);
    }

    [Fact]
    public async Task EditInfo_WithValidId_ReturnsView()
    {
        // Arrange
        var employee = new Models.Employee { Id = 1, FullName = "John Doe" };
        var employeeService = Substitute.For<IEmployeeService>();
        employeeService.GetEmployeeByIdInGroupScopeAsync(1).Returns(employee);

        var controller = new GroupEmployeeListController(
            _session, Substitute.For<ITeamService>(), Substitute.For<ICompanyService>(), Substitute.For<IUnitService>(),
            Substitute.For<IDepartmentService>(), Substitute.For<IExportExcelEmployeeInfoService>(),
            _baseService, employeeService, Substitute.For<ISecondCompanyEmployeeService>(),
            Substitute.For<IUserService>(), Substitute.For<ICommonDataService>(),
            Substitute.For<IJobLevelService>(), _monthlyPayrollService, Substitute.For<IManagementLevelService>());

        SetupPermission(FunctionKey.GroupEmployeeList, Policy.Host, true);

        // Act
        var result = await controller.EditInfo(1);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<UpdateEmployeeViewModel>(viewResult.Model);
        Assert.Equal(1, model.EmpId);
    }

    [Fact]
    public async Task EditInfo_WithInvalidId_RedirectsToError()
    {
        // Arrange
        var employeeService = Substitute.For<IEmployeeService>();
        employeeService.GetEmployeeByIdInGroupScopeAsync(999).Returns((Models.Employee)null);

        var controller = new GroupEmployeeListController(
            _session, Substitute.For<ITeamService>(), Substitute.For<ICompanyService>(), Substitute.For<IUnitService>(),
            Substitute.For<IDepartmentService>(), Substitute.For<IExportExcelEmployeeInfoService>(),
            _baseService, employeeService, Substitute.For<ISecondCompanyEmployeeService>(),
            Substitute.For<IUserService>(), Substitute.For<ICommonDataService>(),
            Substitute.For<IJobLevelService>(), _monthlyPayrollService, Substitute.For<IManagementLevelService>());

        SetupPermission(FunctionKey.GroupEmployeeList, Policy.Host, true);

        // Act
        var result = await controller.EditInfo(999);

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("PageNotFound", redirectResult.ActionName);
        Assert.Equal("Error", redirectResult.ControllerName);
    }

    [Fact]
    public async Task Transfer_WithValidModel_ReturnsSuccessfulResult()
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

        var controller = new GroupEmployeeListController(
            _session, Substitute.For<ITeamService>(), companyService, Substitute.For<IUnitService>(),
            Substitute.For<IDepartmentService>(), Substitute.For<IExportExcelEmployeeInfoService>(),
            _baseService, employeeService, Substitute.For<ISecondCompanyEmployeeService>(),
            userService, Substitute.For<ICommonDataService>(),
            Substitute.For<IJobLevelService>(), _monthlyPayrollService, Substitute.For<IManagementLevelService>());

        SetupPermission(FunctionKey.GroupEmployeeList, Policy.Host, true);

        // Act
        var result = await controller.Transfer(model);

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
    public async Task Transfer_WithCheckExistFailure_ReturnsFailureResult()
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

        var controller = new GroupEmployeeListController(
            _session, Substitute.For<ITeamService>(), Substitute.For<ICompanyService>(), Substitute.For<IUnitService>(),
            Substitute.For<IDepartmentService>(), Substitute.For<IExportExcelEmployeeInfoService>(),
            _baseService, employeeService, Substitute.For<ISecondCompanyEmployeeService>(),
            Substitute.For<IUserService>(), Substitute.For<ICommonDataService>(),
            Substitute.For<IJobLevelService>(), _monthlyPayrollService, Substitute.For<IManagementLevelService>());

        SetupPermission(FunctionKey.GroupEmployeeList, Policy.Host, true);

        // Act
        var result = await controller.Transfer(model);

        // Assert
        var jsonResult = Assert.IsType<JsonResult>(result);
        var commandResult = Assert.IsType<CommandResult>(jsonResult.Value);
        Assert.False(commandResult.IsSuccessful);
        Assert.Equal("Employee already exists", commandResult.Message);

        await employeeService.Received(1).CheckExistAsync(model.Code, model.NickName);
        await employeeService.DidNotReceive().TransferEmployeeAsync(Arg.Any<EmployeeTransfer>());
    }

    [Fact]
    public async Task Transfer_WithEditFailure_DoesNotUpdateUserInfo()
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

        var controller = new GroupEmployeeListController(
            _session, Substitute.For<ITeamService>(), Substitute.For<ICompanyService>(), Substitute.For<IUnitService>(),
            Substitute.For<IDepartmentService>(), Substitute.For<IExportExcelEmployeeInfoService>(),
            _baseService, employeeService, Substitute.For<ISecondCompanyEmployeeService>(),
            Substitute.For<IUserService>(), Substitute.For<ICommonDataService>(),
            Substitute.For<IJobLevelService>(), _monthlyPayrollService, Substitute.For<IManagementLevelService>());

        SetupPermission(FunctionKey.GroupEmployeeList, Policy.Host, true);

        // Act
        var result = await controller.Transfer(model);

        // Assert
        var jsonResult = Assert.IsType<JsonResult>(result);
        var commandResult = Assert.IsType<CommandResult>(jsonResult.Value);
        Assert.False(commandResult.IsSuccessful);

        await employeeService.Received(1).EditAsync(Arg.Any<int>(), Arg.Any<EditEmployeeParam>(), model.TargetCompanyId);
        await employeeService.DidNotReceive().UpdateUserInfoAsync(Arg.Any<int>(), Arg.Any<EditEmployeeParam>());
    }

    [Fact]
    public async Task ExportExcel_WithEmployeeListExport_ReturnsGroupEmployeeListFile()
    {
        // Arrange
        var model = new GroupEmployeeViewModel
        {
            Keyword = "test",
            TeamIds = "1,2",
            CompanyIds = "1,2",
            EmployeeStatus = "1"
        };

        var employees = new List<GroupEmployeeModel>
        {
            new() { EmpId = 1, FullName = "John Doe", EmployeeCode = "EMP001" },
            new() { EmpId = 2, FullName = "Jane Smith", EmployeeCode = "EMP002" }
        };

        var excelBytes = new byte[] { 1, 2, 3, 4, 5 };

        _baseService.GetDataAsync<GroupEmployeeModel>(Arg.Any<GetEmployeesInGroupScopeCriteria>())
            .Returns(employees);

        var excelService = Substitute.For<IExportExcelEmployeeInfoService>();
        excelService.ExportGroupEmployeesAsync(Arg.Any<IEnumerable<GroupEmployeeModel>>())
            .Returns(excelBytes);

        var controller = new GroupEmployeeListController(
            _session, Substitute.For<ITeamService>(), Substitute.For<ICompanyService>(), Substitute.For<IUnitService>(),
            Substitute.For<IDepartmentService>(), excelService,
            _baseService, Substitute.For<IEmployeeService>(), Substitute.For<ISecondCompanyEmployeeService>(),
            Substitute.For<IUserService>(), Substitute.For<ICommonDataService>(),
            Substitute.For<IJobLevelService>(), _monthlyPayrollService, Substitute.For<IManagementLevelService>());

        SetupPermission(FunctionKey.GroupEmployeeList, Policy.Read, true);
        SetupPermission(FunctionKey.GroupEmployeeList_ViewAll, Policy.Read, true);
        SetupPermission(FunctionKey.ShowSalaryCurrencyInfo, Policy.Read, false);

        // Act
        var result = await controller.ExportExcel(model, true);

        // Assert
        var fileResult = Assert.IsType<FileContentResult>(result);
        Assert.Equal(excelBytes, fileResult.FileContents);
        Assert.Contains("GroupEmployeeList_", fileResult.FileDownloadName);
        Assert.Contains(".xlsx", fileResult.FileDownloadName);

        await excelService.Received(1).ExportGroupEmployeesAsync(Arg.Any<IEnumerable<GroupEmployeeModel>>());
        await excelService.DidNotReceive().ExportProfitCenterReportAsync(Arg.Any<IEnumerable<GroupEmployeeModel>>());
    }

    [Fact]
    public async Task ExportExcel_WithProfitCenterReport_ReturnsProfitCenterFile()
    {
        // Arrange
        var model = new GroupEmployeeViewModel
        {
            Keyword = "test",
            TeamIds = "1,2",
            CompanyIds = "1,2",
            EmployeeStatus = "1"
        };

        var employees = new List<GroupEmployeeModel>
        {
            new() { EmpId = 1, FullName = "John Doe", EmployeeCode = "EMP001", Location = "Office" },
            new() { EmpId = 2, FullName = "Jane Smith", EmployeeCode = "EMP002", Location = "Office" }
        };

        var employeesWFH = new List<EmployeeWorkFromHome>
        {
            new() { EmpId = 1 }
        };

        var secondments = new List<CompanySecondmentEmployee>
        {
            new() { EmpId = 2, Location = "Remote Office" }
        };

        var excelBytes = new byte[] { 6, 7, 8, 9, 10 };

        _baseService.GetDataAsync<GroupEmployeeModel>(Arg.Any<GetEmployeesInGroupScopeCriteria>())
            .Returns(employees);
        _baseService.GetDataAsync<EmployeeWorkFromHome>(Arg.Any<GetEmployeeWorkFromHomeCriteria>())
            .Returns(employeesWFH);

        var excelService = Substitute.For<IExportExcelEmployeeInfoService>();
        var secondCompanyEmployeeService = Substitute.For<ISecondCompanyEmployeeService>();

        excelService.ExportProfitCenterReportAsync(Arg.Any<IEnumerable<GroupEmployeeModel>>())
            .Returns(excelBytes);
        secondCompanyEmployeeService.GetAllAsync().Returns(secondments);

        var controller = new GroupEmployeeListController(
            _session, Substitute.For<ITeamService>(), Substitute.For<ICompanyService>(), Substitute.For<IUnitService>(),
            Substitute.For<IDepartmentService>(), excelService,
            _baseService, Substitute.For<IEmployeeService>(), secondCompanyEmployeeService,
            Substitute.For<IUserService>(), Substitute.For<ICommonDataService>(),
            Substitute.For<IJobLevelService>(), _monthlyPayrollService, Substitute.For<IManagementLevelService>());

        SetupPermission(FunctionKey.GroupEmployeeList, Policy.Read, true);
        SetupPermission(FunctionKey.GroupEmployeeList_ViewAll, Policy.Read, true);
        SetupPermission(FunctionKey.ShowSalaryCurrencyInfo, Policy.Read, false);

        // Act
        var result = await controller.ExportExcel(model, false);

        // Assert
        var fileResult = Assert.IsType<FileContentResult>(result);
        Assert.Equal(excelBytes, fileResult.FileContents);
        Assert.Contains("Profit Center Report -", fileResult.FileDownloadName);
        Assert.Contains(".xlsx", fileResult.FileDownloadName);

        await excelService.Received(1).ExportProfitCenterReportAsync(Arg.Any<IEnumerable<GroupEmployeeModel>>());
        await excelService.DidNotReceive().ExportGroupEmployeesAsync(Arg.Any<IEnumerable<GroupEmployeeModel>>());
    }

    [Fact]
    public async Task ExportExcel_WithProfitCenterReport_UpdatesEmployeeLocations()
    {
        // Arrange
        var model = new GroupEmployeeViewModel
        {
            Keyword = "test",
            TeamIds = "1,2",
            CompanyIds = "1,2",
            EmployeeStatus = "1"
        };

        var employees = new List<GroupEmployeeModel>
        {
            new() { EmpId = 1, FullName = "John Doe", Location = "Office" },
            new() { EmpId = 2, FullName = "Jane Smith", Location = "Office" },
            new() { EmpId = 3, FullName = "Bob Wilson", Location = "Office" }
        };

        var employeesWFH = new List<EmployeeWorkFromHome>
        {
            new() { EmpId = 1 } // Employee 1 is WFH
        };

        var secondments = new List<CompanySecondmentEmployee>
        {
            new() { EmpId = 2, Location = "Remote Office" } // Employee 2 is on secondment
        };

        var excelBytes = new byte[] { 1, 2, 3 };

        _baseService.GetDataAsync<GroupEmployeeModel>(Arg.Any<GetEmployeesInGroupScopeCriteria>())
            .Returns(employees);
        _baseService.GetDataAsync<EmployeeWorkFromHome>(Arg.Any<GetEmployeeWorkFromHomeCriteria>())
            .Returns(employeesWFH);

        var excelService = Substitute.For<IExportExcelEmployeeInfoService>();
        var secondCompanyEmployeeService = Substitute.For<ISecondCompanyEmployeeService>();

        excelService.ExportProfitCenterReportAsync(Arg.Any<IEnumerable<GroupEmployeeModel>>())
            .Returns(excelBytes);
        secondCompanyEmployeeService.GetAllAsync().Returns(secondments);

        var controller = new GroupEmployeeListController(
            _session, Substitute.For<ITeamService>(), Substitute.For<ICompanyService>(), Substitute.For<IUnitService>(),
            Substitute.For<IDepartmentService>(), excelService,
            _baseService, Substitute.For<IEmployeeService>(), secondCompanyEmployeeService,
            Substitute.For<IUserService>(), Substitute.For<ICommonDataService>(),
            Substitute.For<IJobLevelService>(), _monthlyPayrollService, Substitute.For<IManagementLevelService>());

        SetupPermission(FunctionKey.GroupEmployeeList, Policy.Read, true);
        SetupPermission(FunctionKey.GroupEmployeeList_ViewAll, Policy.Read, true);
        SetupPermission(FunctionKey.ShowSalaryCurrencyInfo, Policy.Read, false);

        // Act
        var result = await controller.ExportExcel(model, false);

        // Assert
        var fileResult = Assert.IsType<FileContentResult>(result);
        Assert.Equal(excelBytes, fileResult.FileContents);

        // Verify that the employees passed to ExcelService have updated locations
        await excelService.Received(1).ExportProfitCenterReportAsync(
            Arg.Is<IEnumerable<GroupEmployeeModel>>(emps => 
                emps.Any(e => e.EmpId == 1 && e.Location == "WFH") &&
                emps.Any(e => e.EmpId == 2 && e.Location == "Remote Office") &&
                emps.Any(e => e.EmpId == 3 && e.Location == "Office")));
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
        jobLevelService.GetJobLevel().Returns(jobLevels);
        managementLevelService.GetManagementLevel().Returns(managementLevels);
        companyService.GetSubCompanies(Arg.Any<short>()).Returns(subCompanies);

        var controller = new GroupEmployeeListController(
            _session, Substitute.For<ITeamService>(), companyService, Substitute.For<IUnitService>(),
            Substitute.For<IDepartmentService>(), Substitute.For<IExportExcelEmployeeInfoService>(),
            _baseService, employeeService, Substitute.For<ISecondCompanyEmployeeService>(),
            Substitute.For<IUserService>(), commonDataService,
            jobLevelService, _monthlyPayrollService, managementLevelService);

        SetupPermission(FunctionKey.GroupEmployeeList, Policy.Host, true);

        // Act
        var result = await controller.GetInfoEmployeeTransfer(1, 2, 1);

        // Assert
        var jsonResult = Assert.IsType<JsonResult>(result);
        Assert.Equal(employee, jsonResult.Value);
    }

    [Fact]
    public async Task GroupEmployeeList_WithValidModel_ReturnsEmployees()
    {
        // Arrange
        var model = new GroupEmployeeViewModel
        {
            Keyword = "test",
            TeamIds = "1,2",
            CompanyIds = "1,2"
        };

        var employees = new List<GroupEmployeeModel>
        {
            new() { EmpId = 1, FullName = "John Doe" }
        };

        _baseService.GetDataAsync<GroupEmployeeModel>(Arg.Any<GetEmployeesInGroupScopeCriteria>())
            .Returns(employees);

        SetupPermission(FunctionKey.GroupEmployeeList_ViewAll, Policy.Read, true);

        // Act
        var result = await _controller.GroupEmployeeList(model);

        // Assert
        var jsonResult = Assert.IsType<JsonResult>(result);
        var returnedEmployees = Assert.IsAssignableFrom<IEnumerable<GroupEmployeeModel>>(jsonResult.Value);
        Assert.Single(returnedEmployees);
    }

    [Fact]
    public async Task Edit_WithValidId_ReturnsViewWithEmployee()
    {
        // Arrange
        var employee = new Models.Employee { Id = 1, FullName = "John Doe" };
        var supervisors = new List<SupervisorNameModel>
        {
            new() { ActionType = (short)RoleSystem.Verifier, NickName = "Verifier1" },
            new() { ActionType = (short)RoleSystem.Approver, NickName = "Approver1" }
        };

        var employeeService = Substitute.For<IEmployeeService>();
        employeeService.GetEmployeeByIdInGroupScopeAsync(1).Returns(employee);
        employeeService.GetListSupervisorNameAsync(Arg.Any<GetListSupervisorNameCriteria>()).Returns(supervisors);

        var controller = new GroupEmployeeListController(
            _session, Substitute.For<ITeamService>(), Substitute.For<ICompanyService>(), Substitute.For<IUnitService>(),
            Substitute.For<IDepartmentService>(), Substitute.For<IExportExcelEmployeeInfoService>(),
            _baseService, employeeService, Substitute.For<ISecondCompanyEmployeeService>(),
            Substitute.For<IUserService>(), Substitute.For<ICommonDataService>(),
            Substitute.For<IJobLevelService>(), _monthlyPayrollService, Substitute.For<IManagementLevelService>());

        SetupPermission(FunctionKey.GroupEmployeeList, Policy.Host, true);

        // Act
        var result = await controller.Edit(1);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(employee, viewResult.Model);
    }

    [Fact]
    public async Task Edit_WithInvalidId_RedirectsToError()
    {
        // Arrange
        var employeeService = Substitute.For<IEmployeeService>();
        employeeService.GetEmployeeByIdInGroupScopeAsync(999).Returns((Employee)null);

        var controller = new GroupEmployeeListController(
            _session, Substitute.For<ITeamService>(), Substitute.For<ICompanyService>(), Substitute.For<IUnitService>(),
            Substitute.For<IDepartmentService>(), Substitute.For<IExportExcelEmployeeInfoService>(),
            _baseService, employeeService, Substitute.For<ISecondCompanyEmployeeService>(),
            Substitute.For<IUserService>(), Substitute.For<ICommonDataService>(),
            Substitute.For<IJobLevelService>(), _monthlyPayrollService, Substitute.For<IManagementLevelService>());

        SetupPermission(FunctionKey.GroupEmployeeList, Policy.Host, true);

        // Act
        var result = await controller.Edit(999);

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("PageNotFound", redirectResult.ActionName);
        Assert.Equal("Error", redirectResult.ControllerName);
    }

    [Fact]
    public void ShiftManager_ReturnsViewWithViewBag()
    {
        // Act
        var result = _controller.ShiftManager(123);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(123, viewResult.ViewData["EmployeeId"]);
        Assert.Equal((short)RoleSystem.ShiftManager, viewResult.ViewData["RoleId"]);
    }

    [Fact]
    public void ManageSupervisor_ReturnsViewWithViewBag()
    {
        // Act
        var result = _controller.ManageSupervisor(456, (short)RoleSystem.DisciplineManager);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(456, viewResult.ViewData["EmployeeId"]);
        Assert.Equal((short)RoleSystem.DisciplineManager, viewResult.ViewData["RoleId"]);
    }

    [Fact]
    public void DisciplineManager_ReturnsViewWithViewBag()
    {
        // Act
        var result = _controller.DisciplineManager(789);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(789, viewResult.ViewData["EmployeeId"]);
        Assert.Equal((short)RoleSystem.DisciplineManager, viewResult.ViewData["RoleId"]);
    }

    [Fact]
    public async Task GetSupervisors_WithValidParameters_ReturnsEmployees()
    {
        // Arrange
        const string roleIds = "1,2";
        const string query = "John";
        const string excludeIds = "3,4";

        var employees = new List<Employee>
        {
            new() { FullName = "John Doe", NickName = "John" },
            new() { FullName = "John Smith", NickName = "JohnS" }
        };

        var employeeService = Substitute.For<IEmployeeService>();
        employeeService.GetEmployeesByRoleAsync(Arg.Any<GetEmployeeByRoleCriteria>())
            .Returns(employees);

        var controller = new GroupEmployeeListController(
            _session, Substitute.For<ITeamService>(), Substitute.For<ICompanyService>(), Substitute.For<IUnitService>(),
            Substitute.For<IDepartmentService>(), Substitute.For<IExportExcelEmployeeInfoService>(),
            _baseService, employeeService, Substitute.For<ISecondCompanyEmployeeService>(),
            Substitute.For<IUserService>(), Substitute.For<ICommonDataService>(),
            Substitute.For<IJobLevelService>(), _monthlyPayrollService, Substitute.For<IManagementLevelService>());

        // Act
        var result = await controller.GetSupervisors(roleIds, query, excludeIds);

        // Assert
        var jsonResult = Assert.IsType<JsonResult>(result);
        var returnedEmployees = Assert.IsAssignableFrom<IEnumerable<Employee>>(jsonResult.Value);
        Assert.Equal(2, returnedEmployees.Count());

        await employeeService.Received(1).GetEmployeesByRoleAsync(
            Arg.Is<GetEmployeeByRoleCriteria>(c => 
                c.RoleIds == roleIds &&
                c.Query == query.Trim() &&
                c.ExcludeIds == excludeIds));
    }
}