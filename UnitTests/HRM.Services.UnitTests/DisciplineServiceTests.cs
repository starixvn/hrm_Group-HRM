using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HRM.Controllers.UnitTests;
using HRM.Core.Constants;
using HRM.Core.Services;
using HRM.Criteria.Discipline;
using HRM.Models.Discipline;
using HRM.Repositories.Interfaces;
using HRM.Services.Implement;
using HRM.Services.Interfaces;
using NSubstitute;
using Xunit;

namespace HRM.Services.UnitTests;

public class DisciplineServiceTests: BaseControllerTest
{
    private readonly IDisciplineRepository _disciplineRepository;
    private readonly IBaseService _baseService;
    private readonly IEmployeeService _employeeService;
    private readonly DisciplineService _service;

    public DisciplineServiceTests()
    {
        _disciplineRepository = Substitute.For<IDisciplineRepository>();
        _baseService = Substitute.For<IBaseService>();
        _employeeService = Substitute.For<IEmployeeService>();
        _service = new DisciplineService(
            _disciplineRepository,
            _baseService,
            _employeeService,
            _session);
    }

    [Fact]
    public async Task GetDisciplineSummaryAsync_WithValidCriteria_ShouldReturnFilteredDisciplines()
    {
        // Arrange
        var criteria = new GetDisciplineSummaryCriteria
        {
            FromDate = DateTime.Now.AddDays(-30),
            ToDate = DateTime.Now,
            TeamIds = "1,2",
            IsLocal = true,
            EmployeeId = 123,
            ReasonId = 1,
            ApproverUserId = 456,
            IsShowAll = false,
            CompanyId = 1
        };
        var expectedDisciplines = new List<DisciplineSummaryModel>();
        _disciplineRepository.GetDisciplineSummaryAsync(criteria).Returns(expectedDisciplines);
        _employeeService.FilterByEmployeeDataPermission(Arg.Any<IEnumerable<DisciplineSummaryModel>>(), Arg.Any<Func<DisciplineSummaryModel, int>>())
            .Returns(expectedDisciplines);

        // Act
        var result = await _service.GetDisciplineSummaryAsync(criteria);

        // Assert
        Assert.Equal(expectedDisciplines.Count, result.Count);
    }

    [Fact]
    public async Task GetDisciplineSummaryAsync_WithNullCriteria_ShouldReturnEmptyList()
    {
        // Act
        var result = await _service.GetDisciplineSummaryAsync(null);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetDisciplineDetailAsync_WithValidCriteria_ShouldReturnFilteredDisciplines()
    {
        // Arrange
        var criteria = new GetDisciplineDetailCriteria
        {
            FromDate = DateTime.Now.AddDays(-30),
            ToDate = DateTime.Now,
            TeamIds = "1,2",
            IsLocal = true,
            EmployeeId = 123,
            ReasonId = 1,
            HasIgnored = false,
            ApproverUserId = 456,
            CompanyId = 1
        };
        var expectedDisciplines = new List<DisciplineDetailModel>
        {
            new()
            {
                CheckIn = new DateTime(2025,10,10),
                CheckOut = new DateTime(2025,10,10)
            }
        };
        _disciplineRepository.GetDisciplineDetailAsync(criteria).Returns(expectedDisciplines);
        _employeeService.FilterByEmployeeDataPermission(Arg.Any<IEnumerable<DisciplineDetailModel>>(), Arg.Any<Func<DisciplineDetailModel, int>>())
            .Returns(expectedDisciplines);

        // Act
        var result = await _service.GetDisciplineDetailAsync(criteria);

        // Assert
        Assert.Equal(expectedDisciplines.Count, result.Count);
    }

    [Fact]
    public async Task GetDisciplineDetailAsync_WithNullCriteria_ShouldReturnEmptyList()
    {
        // Act
        var result = await _service.GetDisciplineDetailAsync(null);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetMemberToAddOtherDisciplineAsync_WithValidParameters_ShouldReturnMembers()
    {
        // Arrange
        const int supervisorId = 123;
        const string query = "test";
        const short companyId = 1;
        var expectedMembers = new List<EmployeeInfoModel>
        {
            new() { EmployeeId = 123, NickName = "test", FullName = "Test User" }
        };
        _session.Get<short>(SessionKey.CurrentCompanyId).Returns(companyId);
        _baseService.GetDataAsync<EmployeeInfoModel>(Arg.Any<GetMemberToAddOtherDisciplineBySupervisorIdCriteria>())
            .Returns(expectedMembers);

        // Act
        var result = await _service.GetMemberToAddOtherDisciplineAsync(supervisorId, query, companyId);

        // Assert
        Assert.Equal(expectedMembers.Count, result.Count());
    }

    [Fact]
    public async Task GetOtherDisciplineAsync_WithValidCriteria_ShouldReturnList()
    {
        // Arrange
        var criteria = new GetOtherDisciplineOfEmployeeCriteria
        {
            EmployeeId = 123,
            FromDate = DateTime.Now.AddDays(-30),
            ToDate = DateTime.Now
        };
        var expectedOtherDisciplines = new List<OtherDisciplineModel>
        {
            new() { EmployeeId = 123, WorkDate = DateTime.Now }
        };
        _baseService.GetDataAsync<OtherDisciplineModel>(criteria).Returns(expectedOtherDisciplines);

        // Act
        var result = await _service.GetOtherDisciplineAsync(criteria);

        // Assert
        Assert.Equal(expectedOtherDisciplines.Count, result.Count);
    }

    [Fact]
    public async Task GetDisciplineTypeAsync_WithValidCriteria_ShouldReturnTypes()
    {
        // Arrange
        var criteria = new GetDisciplineTypeCriteria { IncludeOtherType = true };
        var expectedTypes = new List<DisciplineTypeModel>
        {
            new() { DisciplineTypeId = 1, Name = "Late In" }
        };
        _baseService.GetDataAsync<DisciplineTypeModel>(criteria).Returns(expectedTypes);

        // Act
        var result = await _service.GetDisciplineTypeAsync(criteria);

        // Assert
        Assert.Equal(expectedTypes.Count, result.Count);
    }

    [Fact]
    public async Task GetOtherDisciplineTypeAsync_WithValidCriteria_ShouldReturnTypes()
    {
        // Arrange
        var criteria = new GetOtherDisciplineTypeCriteria();
        var expectedTypes = new List<DisciplineTypeModel>
        {
            new() { DisciplineTypeId = 1, Name = "Other Type" }
        };
        _baseService.GetDataAsync<DisciplineTypeModel>(criteria).Returns(expectedTypes);

        // Act
        var result = await _service.GetOtherDisciplineTypeAsync(criteria);

        // Assert
        Assert.Equal(expectedTypes.Count, result.Count);
    }
}