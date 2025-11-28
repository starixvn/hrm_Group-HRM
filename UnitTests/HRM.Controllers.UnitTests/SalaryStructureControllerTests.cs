using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GO.UserControl.Client.Core.Enumerables;
using HRM.Core.Constants;
using HRM.Core.Enums;
using HRM.Core.Models;
using HRM.Core.Services;
using HRM.Criteria.Company;
using HRM.Criteria.SalaryStructure;
using HRM.Models;
using HRM.Models.SalaryStructure;
using HRM.Services.Interfaces;
using HRM.ViewModels.SalaryStructure;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Xunit;

namespace HRM.Controllers.UnitTests;

public class SalaryStructureControllerTests : BaseControllerTest
{
    private readonly IJobLevelService _jobLevelService;
    private readonly ISalaryStructureService _salaryStructureService;
    private readonly IBaseService _baseService;
    private readonly SalaryStructureController _controller;

    public SalaryStructureControllerTests()
    {
        _jobLevelService = Substitute.For<IJobLevelService>();
        _salaryStructureService = Substitute.For<ISalaryStructureService>();
        _baseService = Substitute.For<IBaseService>();

        _controller = new SalaryStructureController(
            _session,
            _jobLevelService,
            _salaryStructureService,
            _baseService
        );
    }

    [Fact]
    public void Index_ShouldReturnViewWithPermission()
    {
        // Arrange
        SetupPermission(FunctionKey.SalaryStructureConfiguration, Policy.Host, true);

        // Act
        var result = _controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.True((bool)viewResult.Model!);
    }

    [Fact]
    public void Index_ShouldReturnViewWithoutPermission()
    {
        // Arrange
        SetupPermission(FunctionKey.SalaryStructureConfiguration, Policy.Host, false);

        // Act
        var result = _controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.False((bool)viewResult.Model!);
    }

    [Fact]
    public async Task GetSalaryStructures_ShouldReturnJsonResult()
    {
        // Arrange
        var expectedSalaryStructures = new List<SalaryStructure>
        {
            new() { SalaryStructureId = 1, Name = "Test Structure 1" },
            new() { SalaryStructureId = 2, Name = "Test Structure 2" },
        };
        _salaryStructureService
            .GetSalaryStructureAsync(Arg.Any<int>())
            .Returns(expectedSalaryStructures);

        // Act
        var result = await _controller.GetSalaryStructures();

        // Assert
        var jsonResult = Assert.IsType<JsonResult>(result);
        Assert.Equal(expectedSalaryStructures, jsonResult.Value);
        await _salaryStructureService.Received(1).GetSalaryStructureAsync(3); // From SetupCompanyId
    }

    [Fact]
    public async Task GetJobLevels_ShouldReturnJsonResult()
    {
        // Arrange
        var expectedJobLevels = new List<JobLevel>
        {
            new() { Id = 1, Name = "Level 1" },
            new() { Id = 2, Name = "Level 2" },
        };
        _jobLevelService.GetJobLevel().Returns(Task.FromResult<IList<JobLevel>>(expectedJobLevels));

        // Act
        var result = await _controller.GetJobLevels();

        // Assert
        var jsonResult = Assert.IsType<JsonResult>(result);
        Assert.Equal(expectedJobLevels, jsonResult.Value);
        await _jobLevelService.Received(1).GetJobLevel();
    }

    [Fact]
    public void Create_ShouldReturnView()
    {
        // Act
        var result = _controller.Create();

        // Assert
        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public async Task Create_WithNullModel_ShouldReturnJsonResult()
    {
        // Arrange
        CreateSalaryStructureViewModel? model = null;

        // Act
        var result = await _controller.Create(model);

        // Assert
        var jsonResult = Assert.IsType<JsonResult>(result);
        Assert.IsType<CommandResult>(jsonResult.Value);
    }

    [Fact]
    public async Task Create_WithInvalidModel_ShouldReturnJsonResult()
    {
        // Arrange
        var model = new CreateSalaryStructureViewModel
        {
            Name = "", // Invalid - required field is empty
            Details = new List<SalaryStructureDetail>(),
        };
        _controller.ModelState.AddModelError("Name", "Name is required");

        // Act
        var result = await _controller.Create(model);

        // Assert
        var jsonResult = Assert.IsType<JsonResult>(result);
        Assert.IsType<CommandResult>(jsonResult.Value);
    }

    [Fact]
    public async Task Create_WithValidModel_ShouldReturnJsonResult()
    {
        // Arrange
        var model = new CreateSalaryStructureViewModel
        {
            Name = "Test Structure",
            Details = new List<SalaryStructureDetail>
            {
                new()
                {
                    JobLevelId = 1,
                    Name = "Test Level",
                    BasicSalary = "1000",
                    ExpatSalary = "1500",
                    JobLevelAddOn = "200",
                },
            },
        };
        var expectedResult = new CommandResult { Code = 0 };
        _baseService.ExecuteNonQueryAsync(Arg.Any<InsertSalaryStructureCriteria>())
            .Returns(expectedResult);

        // Act
        var result = await _controller.Create(model);

        // Assert
        var jsonResult = Assert.IsType<JsonResult>(result);
        Assert.Equal(expectedResult, jsonResult.Value);
        await _baseService
            .Received(1)
            .ExecuteNonQueryAsync(
                Arg.Is<InsertSalaryStructureCriteria>(c =>
                    c.CompanyId == 3 && // From SetupCompanyId
                    c.Name == model.Name &&
                    c.CreatedBy == 1 && // From SetupEmployeeSession
                    !string.IsNullOrWhiteSpace(c.Details)
                )
            );
    }

    [Fact]
    public async Task Create_WithValidModel_ShouldEncryptSalaryFields()
    {
        // Arrange
        var model = new CreateSalaryStructureViewModel
        {
            Name = "Test Structure",
            Details = new List<SalaryStructureDetail>
            {
                new()
                {
                    JobLevelId = 1,
                    Name = "Test Level",
                    BasicSalary = "1000",
                    ExpatSalary = "1500",
                    JobLevelAddOn = "200",
                },
            },
        };
        var expectedResult = new CommandResult { Code = 0 };
        _baseService.ExecuteNonQueryAsync(Arg.Any<InsertSalaryStructureCriteria>())
            .Returns(expectedResult);

        // Act
        await _controller.Create(model);

        // Assert
        await _baseService
            .Received(1)
            .ExecuteNonQueryAsync(
                Arg.Is<InsertSalaryStructureCriteria>(c =>
                    c.Details.Contains("1000") == false && // Should be encrypted
                    c.Details.Contains("1500") == false && // Should be encrypted
                    c.Details.Contains("200") == false // Should be encrypted
                )
            );
    }

    [Fact]
    public async Task GetById_WithValidId_ShouldReturnJsonResult()
    {
        // Arrange
        var id = 1;
        var expectedSalaryStructure = new SalaryStructure
        {
            SalaryStructureId = id,
            Name = "Test Structure",
            Details = new List<SalaryStructureDetail>
            {
                new() { JobLevelId = 1, Name = "Test Level" },
            },
        };
        _salaryStructureService.GetByIdAsync(id).Returns(expectedSalaryStructure);

        // Act
        var result = await _controller.GetById(id);

        // Assert
        var jsonResult = Assert.IsType<JsonResult>(result);
        var model = Assert.IsType<EditSalaryStructureViewModel>(jsonResult.Value);
        Assert.Equal(expectedSalaryStructure.SalaryStructureId, model.SalaryStructureId);
        Assert.Equal(expectedSalaryStructure.Name, model.Name);
        Assert.Equal(expectedSalaryStructure.Details, model.Details);
        await _salaryStructureService.Received(1).GetByIdAsync(id);
    }

    [Fact]
    public void Edit_WithValidId_ShouldReturnView()
    {
        // Arrange
        var id = 1;

        // Act
        var result = _controller.Edit(id);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(id, viewResult.Model);
    }

    [Fact]
    public async Task Edit_WithNullModel_ShouldReturnJsonResult()
    {
        // Arrange
        EditSalaryStructureViewModel? model = null;

        // Act
        var result = await _controller.Edit(model);

        // Assert
        var jsonResult = Assert.IsType<JsonResult>(result);
        Assert.IsType<CommandResult>(jsonResult.Value);
    }

    [Fact]
    public async Task Edit_WithInvalidModel_ShouldReturnJsonResult()
    {
        // Arrange
        var model = new EditSalaryStructureViewModel
        {
            SalaryStructureId = 1,
            Name = "", // Invalid - required field is empty
            Details = new List<SalaryStructureDetail>(),
        };
        _controller.ModelState.AddModelError("Name", "Name is required");

        // Act
        var result = await _controller.Edit(model);

        // Assert
        var jsonResult = Assert.IsType<JsonResult>(result);
        Assert.IsType<CommandResult>(jsonResult.Value);
    }

    [Fact]
    public async Task Edit_WithValidModel_ShouldReturnJsonResult()
    {
        // Arrange
        var model = new EditSalaryStructureViewModel
        {
            SalaryStructureId = 1,
            Name = "Updated Structure",
            Details = new List<SalaryStructureDetail>
            {
                new()
                {
                    JobLevelId = 1,
                    Name = "Test Level",
                    BasicSalary = "1200",
                    ExpatSalary = "1800",
                    JobLevelAddOn = "300",
                },
            },
        };
        var expectedResult = new CommandResult { Code = 0 };
        _baseService.ExecuteNonQueryAsync(Arg.Any<UpdateSalaryStructureCriteria>())
            .Returns(expectedResult);

        // Act
        var result = await _controller.Edit(model);

        // Assert
        var jsonResult = Assert.IsType<JsonResult>(result);
        Assert.Equal(expectedResult, jsonResult.Value);
        await _baseService
            .Received(1)
            .ExecuteNonQueryAsync(
                Arg.Is<UpdateSalaryStructureCriteria>(c =>
                    c.CompanyId == 3 && // From SetupCompanyId
                    c.SalaryStructureId == model.SalaryStructureId &&
                    c.Name == model.Name &&
                    c.ModifiedBy == 1 && // From SetupEmployeeSession
                    !string.IsNullOrWhiteSpace(c.Details)
                )
            );
    }

    [Fact]
    public async Task Edit_WithValidModel_ShouldEncryptSalaryFields()
    {
        // Arrange
        var model = new EditSalaryStructureViewModel
        {
            SalaryStructureId = 1,
            Name = "Updated Structure",
            Details = new List<SalaryStructureDetail>
            {
                new()
                {
                    JobLevelId = 1,
                    Name = "Test Level",
                    BasicSalary = "1200",
                    ExpatSalary = "1800",
                    JobLevelAddOn = "300",
                },
            },
        };
        var expectedResult = new CommandResult { Code = 0 };
        _baseService.ExecuteNonQueryAsync(Arg.Any<UpdateSalaryStructureCriteria>())
            .Returns(expectedResult);

        // Act
        await _controller.Edit(model);

        // Assert
        await _baseService
            .Received(1)
            .ExecuteNonQueryAsync(
                Arg.Is<UpdateSalaryStructureCriteria>(c =>
                    c.Details.Contains("1200") == false && // Should be encrypted
                    c.Details.Contains("1800") == false && // Should be encrypted
                    c.Details.Contains("300") == false // Should be encrypted
                )
            );
    }

    [Fact]
    public async Task Delete_WithValidModel_ShouldReturnJsonResult()
    {
        // Arrange
        var model = new DeleteSalaryStructureViewModel { Id = 1 };
        var expectedResult = new CommandResult { Code = 0 };
        _salaryStructureService.DeleteAsync(Arg.Any<int>(), Arg.Any<int>())
            .Returns(expectedResult);

        // Act
        var result = await _controller.Delete(model);

        // Assert
        var jsonResult = Assert.IsType<JsonResult>(result);
        Assert.Equal(expectedResult, jsonResult.Value);
        await _salaryStructureService.Received(1).DeleteAsync(1, model.Id); // From SetupEmployeeSession
    }

    [Fact]
    public async Task GetBasicSalaryConfig_ShouldReturnJsonResult()
    {
        // Arrange
        var expectedViewModel = new BasicSalaryConfigViewModel
        {
            Config = "{\"config\": \"test\"}",
            ConfigOptions = new List<BasicSalaryConfigOption>
            {
                new BasicSalaryConfigOption { Value = "1", Label = "Basic Salary" },
                new BasicSalaryConfigOption { Value = "2", Label = "Job Level Add-On" },
                new BasicSalaryConfigOption { Value = "3", Label = "Special Subsidy" },
                new BasicSalaryConfigOption { Value = "4", Label = "Management Allowance" }
            }
        };
        _salaryStructureService.GetBasicSalaryConfigAsync(Arg.Any<short>())
            .Returns(expectedViewModel);

        // Act
        var result = await _controller.GetBasicSalaryConfig();

        // Assert
        var jsonResult = Assert.IsType<JsonResult>(result);
        var resultViewModel = Assert.IsType<BasicSalaryConfigViewModel>(jsonResult.Value);
        Assert.Equal(expectedViewModel.Config, resultViewModel.Config);
        Assert.NotNull(resultViewModel.ConfigOptions);
        Assert.Equal(4, resultViewModel.ConfigOptions.Count());
        await _salaryStructureService.Received(1).GetBasicSalaryConfigAsync(3); // From SetupCompanyId
    }

    [Fact]
    public async Task EditBasicSalaryConfig_WithNullConfigData_ShouldReturnJsonResult()
    {
        // Arrange
        object? configData = null;

        // Act
        var result = await _controller.EditBasicSalaryConfig(configData);

        // Assert
        var jsonResult = Assert.IsType<JsonResult>(result);
        Assert.IsType<CommandResult>(jsonResult.Value);
    }

    [Fact]
    public async Task EditBasicSalaryConfig_WithValidConfigData_ShouldReturnJsonResult()
    {
        // Arrange
        var configData = new { setting1 = "value1", setting2 = "value2" };
        var expectedResult = new CommandResult { Code = 0 };
        _baseService.ExecuteNonQueryAsync(Arg.Any<UpdateSystemConfigsCriteria>())
            .Returns(expectedResult);

        // Act
        var result = await _controller.EditBasicSalaryConfig(configData);

        // Assert
        var jsonResult = Assert.IsType<JsonResult>(result);
        Assert.Equal(expectedResult, jsonResult.Value);
        await _baseService
            .Received(1)
            .ExecuteNonQueryAsync(
                Arg.Is<UpdateSystemConfigsCriteria>(c =>
                    c.CompanyId == 3 && // From SetupCompanyId
                    c.ModifiedBy == 1 && // From SetupEmployeeSession
                    c.ObjectTypeId == (int)LogObjectType.SalaryStructure &&
                    !string.IsNullOrWhiteSpace(c.SettingsJson)
                )
            );
    }

    [Fact]
    public async Task Create_WithEmptySalaryFields_ShouldNotEncryptEmptyFields()
    {
        // Arrange
        var model = new CreateSalaryStructureViewModel
        {
            Name = "Test Structure",
            Details = new List<SalaryStructureDetail>
            {
                new()
                {
                    JobLevelId = 1,
                    Name = "Test Level",
                    BasicSalary = "", // Empty
                    ExpatSalary = null, // Null
                    JobLevelAddOn = "   ", // Whitespace
                },
            },
        };
        var expectedResult = new CommandResult { Code = 0 };
        _baseService.ExecuteNonQueryAsync(Arg.Any<InsertSalaryStructureCriteria>())
            .Returns(expectedResult);

        // Act
        await _controller.Create(model);

        // Assert
        await _baseService
            .Received(1)
            .ExecuteNonQueryAsync(
                Arg.Is<InsertSalaryStructureCriteria>(c =>
                    c.Details.Contains("\"BasicSalary\":null") && // Should be null
                    c.Details.Contains("\"ExpatSalary\":null") && // Should be null
                    c.Details.Contains("\"JobLevelAddOn\":null") // Should be null
                )
            );
    }

    [Fact]
    public async Task Edit_WithEmptySalaryFields_ShouldNotEncryptEmptyFields()
    {
        // Arrange
        var model = new EditSalaryStructureViewModel
        {
            SalaryStructureId = 1,
            Name = "Updated Structure",
            Details = new List<SalaryStructureDetail>
            {
                new()
                {
                    JobLevelId = 1,
                    Name = "Test Level",
                    BasicSalary = "", // Empty
                    ExpatSalary = null, // Null
                    JobLevelAddOn = "   ", // Whitespace
                },
            },
        };
        var expectedResult = new CommandResult { Code = 0 };
        _baseService.ExecuteNonQueryAsync(Arg.Any<UpdateSalaryStructureCriteria>())
            .Returns(expectedResult);

        // Act
        await _controller.Edit(model);

        // Assert
        await _baseService
            .Received(1)
            .ExecuteNonQueryAsync(
                Arg.Is<UpdateSalaryStructureCriteria>(c =>
                    c.Details.Contains("\"BasicSalary\":null") && // Should be null
                    c.Details.Contains("\"ExpatSalary\":null") && // Should be null
                    c.Details.Contains("\"JobLevelAddOn\":null") // Should be null
                )
            );
    }
}