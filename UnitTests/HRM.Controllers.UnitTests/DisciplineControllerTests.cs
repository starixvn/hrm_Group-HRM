using HRM.Core.Enums;
using HRM.Core.Models;
using HRM.Core.Services;
using HRM.Criteria.Discipline;
using HRM.Models.Discipline;
using HRM.Services.Interfaces;
using HRM.ViewModels.Discipline;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Xunit;

namespace HRM.Controllers.UnitTests;

public class DisciplineControllerTests : BaseControllerTest
{
    private readonly IDisciplineService _disciplineService;
    private readonly IBaseService _baseService;
    private readonly DisciplineController _controller;

    public DisciplineControllerTests()
    {
        _disciplineService = Substitute.For<IDisciplineService>();
        _baseService = Substitute.For<IBaseService>();
            
        _controller = new DisciplineController(_session, _disciplineService, _baseService);
    }

    [Fact]
    public async Task DisciplineSummary_WithNullModel_ShouldSetViewBagProperties()
    {
        // Arrange
        DisciplineSummaryViewModel model = null;

        // Act
        var result = await _controller.DisciplineSummary(model);

        // Assert
        Assert.IsType<ViewResult>(result);
        Assert.Equal((short)RoleSystem.DisciplineManager, _controller.ViewBag.RoleId);
    }

    [Fact]
    public async Task DisciplineSummary_WithValidModel_ShouldInitializeModelProperties()
    {
        // Arrange
        var model = new DisciplineSummaryViewModel();

        // Act
        var result = await _controller.DisciplineSummary(model);

        // Assert
        Assert.IsType<ViewResult>(result);
        Assert.NotNull(model.DisciplineTypes);
        Assert.NotNull(model.DateRange);
        Assert.Equal((short)RoleSystem.DisciplineManager, _controller.ViewBag.RoleId);
    }

    [Fact]
    public async Task DisciplineSummary_ShouldCallGetDisciplineTypeWithIncludeOtherTypeTrue()
    {
        // Arrange
        var model = new DisciplineSummaryViewModel();
        var expectedDisciplineTypes = new List<DisciplineTypeModel>
        {
            new() { DisciplineTypeId = 1, Name = "Test Type" }
        };
        _disciplineService.GetDisciplineTypeAsync(Arg.Any<GetDisciplineTypeCriteria>())
            .Returns(expectedDisciplineTypes);

        // Act
        await _controller.DisciplineSummary(model);

        // Assert
        await _disciplineService.Received(1).GetDisciplineTypeAsync(
            Arg.Is<GetDisciplineTypeCriteria>(c => c.IncludeOtherType));
        Assert.Equal(expectedDisciplineTypes, model.DisciplineTypes);
    }

    [Fact]
    public async Task GetDisciplineSummaryGrid_WithValidModel_ShouldReturnPartialView()
    {
        // Arrange
        var model = new DisciplineSummaryViewModel
        {
            FromDate = DateTime.Now.AddDays(-30),
            ToDate = DateTime.Now,
            TeamId = "1,2,3",
            IsLocal = true,
            EmployeeId = 123,
            ReasonId = 1,
            IsShowAll = true
        };

        var expectedDisciplines = new List<DisciplineSummaryModel>
        {
            new() { Employee = new EmployeeInfoModel{ EmployeeId = 123 } }
        };

        _disciplineService.GetDisciplineSummaryAsync(Arg.Any<GetDisciplineSummaryCriteria>())
            .Returns(expectedDisciplines);

        // Act
        var result = await _controller.GetDisciplineSummaryGrid(model);

        // Assert
        var partialViewResult = Assert.IsType<PartialViewResult>(result);
        Assert.Equal("_DisciplineSummaryGrid", partialViewResult.ViewName);
    }

    [Fact]
    public async Task GetDisciplineSummaryGrid_ShouldCallServiceWithCorrectCriteria()
    {
        // Arrange
        var model = new DisciplineSummaryViewModel
        {
            FromDate = DateTime.Now.AddDays(-30),
            ToDate = DateTime.Now,
            TeamId = "1,2,3",
            IsLocal = false,
            EmployeeId = 456,
            ReasonId = 2,
            IsShowAll = false
        };

        var expectedDisciplines = new List<DisciplineSummaryModel>();
        _disciplineService.GetDisciplineSummaryAsync(Arg.Any<GetDisciplineSummaryCriteria>())
            .Returns(expectedDisciplines);

        // Act
        await _controller.GetDisciplineSummaryGrid(model);

        // Assert
        await _disciplineService.Received(1).GetDisciplineSummaryAsync(
            Arg.Is<GetDisciplineSummaryCriteria>(c =>
                c.FromDate == model.FromDate &&
                c.ToDate == model.ToDate &&
                c.TeamIds == model.TeamId &&
                c.IsLocal == model.IsLocal &&
                c.EmployeeId == model.EmployeeId &&
                c.ReasonId == model.ReasonId &&
                c.ApproverUserId == _session.UserId &&
                c.IsShowAll == model.IsShowAll &&
                c.CompanyId == 3)); // From SetupSession
    }

    [Fact]
    public async Task GetDisciplineSummaryGrid_WithEmptyResults_ShouldStillReturnPartialView()
    {
        // Arrange
        var model = new DisciplineSummaryViewModel();
        _disciplineService.GetDisciplineSummaryAsync(Arg.Any<GetDisciplineSummaryCriteria>())
            .Returns(new List<DisciplineSummaryModel>());

        // Act
        var result = await _controller.GetDisciplineSummaryGrid(model);

        // Assert
        var partialViewResult = Assert.IsType<PartialViewResult>(result);
        Assert.Equal("_DisciplineSummaryGrid", partialViewResult.ViewName);
        var modelResult = Assert.IsType<List<DisciplineSummaryModel>>(partialViewResult.Model);
        Assert.Empty(modelResult);
    }

    [Fact]
    public async Task DisciplineDetail_WithValidModel_ShouldReturnView()
    {
        // Arrange
        var model = new DisciplineSummaryViewModel();
        var expectedDisciplineTypes = new List<DisciplineTypeModel>
        {
            new() { DisciplineTypeId = 1, Name = "Test Type" }
        };
        _disciplineService.GetDisciplineTypeAsync(Arg.Any<GetDisciplineTypeCriteria>())
            .Returns(expectedDisciplineTypes);

        // Act
        var result = await _controller.DisciplineDetail(model);

        // Assert
        Assert.IsType<ViewResult>(result);
        Assert.Equal((short)RoleSystem.DisciplineManager, _controller.ViewBag.RoleId);
        Assert.Equal(3, _controller.ViewBag.CompanyId);
    }

    [Fact]
    public async Task GetDisciplineDetailGrid_WithValidModel_ShouldReturnPartialView()
    {
        // Arrange
        var model = new DisciplineSummaryViewModel();
        var expectedDisciplines = new List<DisciplineDetailModel>();
        _disciplineService.GetDisciplineDetailAsync(Arg.Any<GetDisciplineDetailCriteria>())
            .Returns(expectedDisciplines);

        // Act
        var result = await _controller.GetDisciplineDetailGrid(model);

        // Assert
        var partialViewResult = Assert.IsType<PartialViewResult>(result);
        Assert.Equal("_DisciplineDetailGrid", partialViewResult.ViewName);
    }

    [Fact]
    public async Task ViewOtherDiscipline_WithValidParameters_ShouldReturnPartialView()
    {
        // Arrange
        var employeeId = 123;
        var fromDate = DateTime.Now.AddDays(-30);
        var toDate = DateTime.Now;
        var expectedDisciplines = new List<OtherDisciplineModel>();
        var expectedMembers = new List<EmployeeInfoModel>();

        _disciplineService.GetOtherDisciplineAsync(Arg.Any<GetOtherDisciplineOfEmployeeCriteria>())
            .Returns(expectedDisciplines);
        _disciplineService.GetMemberToAddOtherDisciplineAsync(Arg.Any<int>())
            .Returns(expectedMembers);

        // Act
        var result = await _controller.ViewOtherDiscipline(employeeId, fromDate, toDate);

        // Assert
        var partialViewResult = Assert.IsType<PartialViewResult>(result);
        Assert.Equal("_ViewOtherDiscipline", partialViewResult.ViewName);
    }

    [Fact]
    public async Task PartialAddOtherDiscipline_WithValidParameters_ShouldReturnPartialView()
    {
        // Arrange
        const string employee = "John Doe";
        const int employeeId = 123;
        var expectedTypes = new List<DisciplineTypeModel>();
        _disciplineService.GetOtherDisciplineTypeAsync(Arg.Any<GetOtherDisciplineTypeCriteria>())
            .Returns(expectedTypes);

        // Act
        var result = await _controller.PartialAddOtherDiscipline(employee, employeeId);

        // Assert
        var partialViewResult = Assert.IsType<PartialViewResult>(result);
        Assert.Equal("_AddOtherDiscipline", partialViewResult.ViewName);
    }

    [Fact]
    public async Task AddOtherDiscipline_WithValidModel_ShouldReturnJson()
    {
        // Arrange
        var model = new AddOtherDisciplineViewModel
        {
            EmployeeId = 123,
            WorkDate = DateTime.Now,
            DisciplineTypeId = 1,
            MinusScore = 5.0m,
            Comment = "Test comment"
        };
        var expectedResult = new CommandResult();
        _baseService.ExecuteNonQueryAsync(Arg.Any<InsertOtherDisciplineCriteria>())
            .Returns(expectedResult);

        // Act
        var result = await _controller.AddOtherDiscipline(model);

        // Assert
        var jsonResult = Assert.IsType<JsonResult>(result);
        Assert.Equal(expectedResult, jsonResult.Value);
    }

    [Fact]
    public async Task DeleteOtherDiscipline_WithValidCriteria_ShouldReturnEmptyString()
    {
        // Arrange
        var criteria = new DeleteOtherDisciplineCriteria
        {
            EmployeeId = 123,
            WorkDate = DateTime.Now,
            DisciplineTypeId = 1
        };
        var expectedResult = new CommandResult{ Code = 0 };
        _baseService.ExecuteNonQueryAsync(Arg.Any<DeleteOtherDisciplineCriteria>())
            .Returns(expectedResult);

        // Act
        var result = await _controller.DeleteOtherDiscipline(criteria);

        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public async Task EditDisciplineIgnore_WithValidData_ShouldReturnJson()
    {
        // Arrange
        var ignoreDetails = new List<IgnoredDisciplineModel>();
        var disciplineIgnores = new List<IgnoredDisciplineDetailModel>();
        var expectedResult = new CommandResult ();
        _baseService.ExecuteNonQueryAsync(Arg.Any<UpdateIgnoredDisciplineCriteria>())
            .Returns(expectedResult);

        // Act
        var result = await _controller.EditDisciplineIgnore(ignoreDetails, disciplineIgnores);

        // Assert
        var jsonResult = Assert.IsType<JsonResult>(result);
        Assert.Equal(expectedResult, jsonResult.Value);
    }

    [Fact]
    public async Task GetMemberToAddOtherDiscipline_WithKeyword_ShouldReturnJson()
    {
        // Arrange
        const string keyword = "test";
        var expectedMembers = new List<EmployeeInfoModel>
        {
            new() { EmployeeId = 123, NickName = "test", FullName = "Test User" }
        };
        _disciplineService.GetMemberToAddOtherDisciplineAsync(Arg.Any<int>(), keyword)
            .Returns(expectedMembers);

        // Act
        var result = await _controller.GetMemberToAddOtherDiscipline(keyword);

        // Assert
        var jsonResult = Assert.IsType<JsonResult>(result);
        Assert.Equal(expectedMembers, jsonResult.Value);
    }

}