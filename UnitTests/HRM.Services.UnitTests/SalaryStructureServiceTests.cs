using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fanex.Data;
using HRM.Core.Constants;
using HRM.Core.Models;
using HRM.Core.Services;
using HRM.Criteria.SalaryStructure;
using HRM.Criteria.SystemSetting;
using HRM.Models.SalaryStructure;
using HRM.Services.Implement;
using HRM.ViewModels.SalaryStructure;
using NSubstitute;
using Xunit;

namespace HRM.Services.UnitTests;

public class SalaryStructureServiceTests
{
    private readonly IBaseService _baseService;
    private readonly SalaryStructureService _service;

    public SalaryStructureServiceTests()
    {
        _baseService = Substitute.For<IBaseService>();
        _service = new SalaryStructureService(_baseService);
    }

    [Fact]
    public async Task GetSalaryStructureAsync_WithValidCompanyId_ShouldReturnSalaryStructures()
    {
        // Arrange
        const int companyId = 1;
        var salaryStructures = new List<SalaryStructure>
        {
            new() { SalaryStructureId = 1, Name = "Structure 1" },
            new() { SalaryStructureId = 2, Name = "Structure 2" },
        };
        var details = new List<SalaryStructureDetail>
        {
            new()
            {
                SalaryStructureId = 1,
                JobLevelId = 1,
                BasicSalary = "",
                ExpatSalary = "",
                JobLevelAddOn = "",
            },
            new()
            {
                SalaryStructureId = 2,
                JobLevelId = 2,
                BasicSalary = "",
                ExpatSalary = "",
                JobLevelAddOn = "",
            },
        };

        _baseService
            .FetchMultipleAsync<SalaryStructure, SalaryStructureDetail>(
                Arg.Any<GetSalaryStructureCriteria>()
            )
            .Returns((salaryStructures, details));

        // Act
        var result = await _service.GetSalaryStructureAsync(companyId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());

        // Verify that the service was called with correct criteria
        await _baseService
            .Received(1)
            .FetchMultipleAsync<SalaryStructure, SalaryStructureDetail>(
                Arg.Is<GetSalaryStructureCriteria>(c => c.CompanyId == companyId)
            );
    }

    [Fact]
    public async Task GetSalaryStructureAsync_ShouldDecryptSalaryFields()
    {
        // Arrange
        const int companyId = 1;
        var salaryStructures = new List<SalaryStructure>
        {
            new() { SalaryStructureId = 1, Name = "Structure 1" },
        };
        var details = new List<SalaryStructureDetail>
        {
            new()
            {
                SalaryStructureId = 1,
                JobLevelId = 1,
                BasicSalary = "",
                ExpatSalary = "",
                JobLevelAddOn = "",
            },
        };

        _baseService
            .FetchMultipleAsync<SalaryStructure, SalaryStructureDetail>(
                Arg.Any<GetSalaryStructureCriteria>()
            )
            .Returns((salaryStructures, details));

        // Act
        var result = await _service.GetSalaryStructureAsync(companyId);

        // Assert
        var resultList = result.ToList();
        var detail = resultList.First().Details.First();

        // Verify that fields are processed (null for empty input)
        Assert.Null(detail.BasicSalary);
        Assert.Null(detail.ExpatSalary);
        Assert.Null(detail.JobLevelAddOn);
    }

    [Fact]
    public async Task GetSalaryStructureAsync_WithEmptySalaryFields_ShouldNotDecryptEmptyFields()
    {
        // Arrange
        const int companyId = 1;
        var salaryStructures = new List<SalaryStructure>
        {
            new() { SalaryStructureId = 1, Name = "Structure 1" },
        };
        var details = new List<SalaryStructureDetail>
        {
            new()
            {
                SalaryStructureId = 1,
                JobLevelId = 1,
                BasicSalary = "",
                ExpatSalary = null,
                JobLevelAddOn = "",
            },
        };

        _baseService
            .FetchMultipleAsync<SalaryStructure, SalaryStructureDetail>(
                Arg.Any<GetSalaryStructureCriteria>()
            )
            .Returns((salaryStructures, details));

        // Act
        var result = await _service.GetSalaryStructureAsync(companyId);

        // Assert
        var resultList = result.ToList();
        var detail = resultList.First().Details.First();

        Assert.Null(detail.BasicSalary);
        Assert.Null(detail.ExpatSalary);
        Assert.Null(detail.JobLevelAddOn);
    }

    [Fact]
    public async Task GetSalaryStructureAsync_ShouldAssociateDetailsWithStructures()
    {
        // Arrange
        const int companyId = 1;
        var salaryStructures = new List<SalaryStructure>
        {
            new() { SalaryStructureId = 1, Name = "Structure 1" },
            new() { SalaryStructureId = 2, Name = "Structure 2" },
        };
        var details = new List<SalaryStructureDetail>
        {
            new()
            {
                SalaryStructureId = 1,
                JobLevelId = 1,
                BasicSalary = "",
            },
            new()
            {
                SalaryStructureId = 1,
                JobLevelId = 2,
                BasicSalary = "",
            },
            new()
            {
                SalaryStructureId = 2,
                JobLevelId = 3,
                BasicSalary = "",
            },
        };

        _baseService
            .FetchMultipleAsync<SalaryStructure, SalaryStructureDetail>(
                Arg.Any<GetSalaryStructureCriteria>()
            )
            .Returns((salaryStructures, details));

        // Act
        var result = await _service.GetSalaryStructureAsync(companyId);

        // Assert
        var resultList = result.ToList();

        var structure1 = resultList.First(s => s.SalaryStructureId == 1);
        var structure2 = resultList.First(s => s.SalaryStructureId == 2);

        Assert.Equal(2, structure1.Details.Count());
        Assert.Single(structure2.Details);
        Assert.All(structure1.Details, d => Assert.Equal(1, d.SalaryStructureId));
        Assert.All(structure2.Details, d => Assert.Equal(2, d.SalaryStructureId));
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ShouldReturnSalaryStructure()
    {
        // Arrange
        const int salaryStructureId = 1;
        var salaryStructures = new List<SalaryStructure>
        {
            new() { SalaryStructureId = 1, Name = "Structure 1" },
        };
        var details = new List<SalaryStructureDetail>
        {
            new()
            {
                SalaryStructureId = 1,
                JobLevelId = 1,
                BasicSalary = "",
            },
        };

        _baseService
            .FetchMultipleAsync<SalaryStructure, SalaryStructureDetail>(
                Arg.Any<GetSalaryStructureByIdCriteria>()
            )
            .Returns((salaryStructures, details));

        // Act
        var result = await _service.GetByIdAsync(salaryStructureId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(salaryStructureId, result.SalaryStructureId);
        Assert.Equal("Structure 1", result.Name);
        Assert.Single(result.Details);

        // Verify that the service was called with correct criteria
        await _baseService
            .Received(1)
            .FetchMultipleAsync<SalaryStructure, SalaryStructureDetail>(
                Arg.Is<GetSalaryStructureByIdCriteria>(c =>
                    c.SalaryStructureId == salaryStructureId
                )
            );
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        const int salaryStructureId = 999;
        var salaryStructures = new List<SalaryStructure>();
        var details = new List<SalaryStructureDetail>();

        _baseService
            .FetchMultipleAsync<SalaryStructure, SalaryStructureDetail>(
                Arg.Any<GetSalaryStructureByIdCriteria>()
            )
            .Returns((salaryStructures, details));

        // Act
        var result = await _service.GetByIdAsync(salaryStructureId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldDecryptSalaryFields()
    {
        // Arrange
        const int salaryStructureId = 1;
        var salaryStructures = new List<SalaryStructure>
        {
            new() { SalaryStructureId = 1, Name = "Structure 1" },
        };
        var details = new List<SalaryStructureDetail>
        {
            new()
            {
                SalaryStructureId = 1,
                JobLevelId = 1,
                BasicSalary = "",
                ExpatSalary = "",
                JobLevelAddOn = "",
            },
        };

        _baseService
            .FetchMultipleAsync<SalaryStructure, SalaryStructureDetail>(
                Arg.Any<GetSalaryStructureByIdCriteria>()
            )
            .Returns((salaryStructures, details));

        // Act
        var result = await _service.GetByIdAsync(salaryStructureId);

        // Assert
        Assert.NotNull(result);
        var detail = result.Details.First();

        // Verify that fields are processed (null for empty input)
        Assert.Null(detail.BasicSalary);
        Assert.Null(detail.ExpatSalary);
        Assert.Null(detail.JobLevelAddOn);
    }

    [Fact]
    public async Task DeleteAsync_WithValidParameters_ShouldCallBaseService()
    {
        // Arrange
        const int actorId = 1;
        const int id = 2;
        var expectedResult = new CommandResult { Code = 0 };

        _baseService
            .ExecuteNonQueryAsync(Arg.Any<DeleteSalaryStructureCriteria>())
            .Returns(expectedResult);

        // Act
        var result = await _service.DeleteAsync(actorId, id);

        // Assert
        Assert.Equal(expectedResult, result);

        // Verify that the service was called with correct criteria
        await _baseService
            .Received(1)
            .ExecuteNonQueryAsync(
                Arg.Is<DeleteSalaryStructureCriteria>(c =>
                    c.SalaryStructureId == id && c.DeletedBy == actorId
                )
            );
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnCommandResultFromBaseService()
    {
        // Arrange
        const int actorId = 1;
        const int id = 2;
        var expectedResult = new CommandResult { Code = 1, Message = "Delete failed" };

        _baseService
            .ExecuteNonQueryAsync(Arg.Any<DeleteSalaryStructureCriteria>())
            .Returns(expectedResult);

        // Act
        var result = await _service.DeleteAsync(actorId, id);

        // Assert
        Assert.False(result.IsSuccessful);
        Assert.Equal("Delete failed", result.Message);
    }

    [Fact]
    public async Task GetBasicSalaryConfigAsync_WithValidCompanyId_ShouldReturnConfigViewModel()
    {
        // Arrange
        const short companyId = 1;
        const string expectedConfig = "{\"basicSalary\": 1000}";

        _baseService
            .GetDataEntityAsync<string>(Arg.Any<GetSystemConfigsByKeyCriteria>())
            .Returns(expectedConfig);

        // Act
        var result = await _service.GetBasicSalaryConfigAsync(companyId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedConfig, result.Config);
        Assert.NotNull(result.ConfigOptions);
        Assert.Equal(3, result.ConfigOptions.Count());
        Assert.Contains(result.ConfigOptions, o => o.Value == "1" && o.Label == "Basic Salary");
        Assert.Contains(result.ConfigOptions, o => o.Value == "2" && o.Label == "Job Level Add-On");
        Assert.Contains(result.ConfigOptions, o => o.Value == "3" && o.Label == "Special Subsidy");

        // Verify that the service was called with correct criteria
        await _baseService
            .Received(1)
            .GetDataEntityAsync<string>(
                Arg.Is<GetSystemConfigsByKeyCriteria>(c =>
                    c.CompanyId == companyId && c.Key == SystemConfigKey.BasicSalaryConfig
                )
            );
    }

    [Fact]
    public async Task GetBasicSalaryConfigAsync_ShouldReturnViewModelWithConfigOptions()
    {
        // Arrange
        const short companyId = 1;
        const string expectedConfig = "test config data";

        _baseService
            .GetDataEntityAsync<string>(Arg.Any<GetSystemConfigsByKeyCriteria>())
            .Returns(expectedConfig);

        // Act
        var result = await _service.GetBasicSalaryConfigAsync(companyId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedConfig, result.Config);
        Assert.NotNull(result.ConfigOptions);
        Assert.Equal(3, result.ConfigOptions.Count());
    }

    [Fact]
    public async Task GetBasicSalaryConfigAsync_WithNullResult_ShouldReturnViewModelWithNullConfig()
    {
        // Arrange
        const short companyId = 1;

        _baseService
            .GetDataEntityAsync<string>(Arg.Any<GetSystemConfigsByKeyCriteria>())
            .Returns((string)null);

        // Act
        var result = await _service.GetBasicSalaryConfigAsync(companyId);

        // Assert
        Assert.NotNull(result);
        Assert.Null(result.Config);
        Assert.NotNull(result.ConfigOptions);
        Assert.Equal(3, result.ConfigOptions.Count());
    }

    [Fact]
    public async Task GetSalaryStructureAsync_WithEmptyResults_ShouldReturnEmptyCollection()
    {
        // Arrange
        const int companyId = 1;
        var salaryStructures = new List<SalaryStructure>();
        var details = new List<SalaryStructureDetail>();

        _baseService
            .FetchMultipleAsync<SalaryStructure, SalaryStructureDetail>(
                Arg.Any<GetSalaryStructureCriteria>()
            )
            .Returns((salaryStructures, details));

        // Act
        var result = await _service.GetSalaryStructureAsync(companyId);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByIdAsync_WithEmptyDetails_ShouldReturnStructureWithoutDetails()
    {
        // Arrange
        const int salaryStructureId = 1;
        var salaryStructures = new List<SalaryStructure>
        {
            new() { SalaryStructureId = 1, Name = "Structure 1" },
        };
        var details = new List<SalaryStructureDetail>();

        _baseService
            .FetchMultipleAsync<SalaryStructure, SalaryStructureDetail>(
                Arg.Any<GetSalaryStructureByIdCriteria>()
            )
            .Returns((salaryStructures, details));

        // Act
        var result = await _service.GetByIdAsync(salaryStructureId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(salaryStructureId, result.SalaryStructureId);
        Assert.Empty(result.Details);
    }
}
