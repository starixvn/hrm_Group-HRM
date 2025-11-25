using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using GO.UserControl.Client.Core.Enumerables;
using HRM.Controllers;
using HRM.Core.Constants;
using HRM.Core.Models;
using HRM.Core.Services;
using HRM.Criteria.EmployeeWorkFromHome;
using HRM.Models;
using HRM.Services.Interfaces;
using HRM.ViewModels.EmployeeWorkFromHome;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Xunit;

namespace HRM.Controllers.UnitTests;

public class EmployeeWorkFromHomeControllerTests : BaseControllerTest
{
    private readonly IBaseService _baseService;
    private readonly IAttachFileService _attachFileService;
    private readonly EmployeeWorkFromHomeController _controller;

    public EmployeeWorkFromHomeControllerTests()
    {
        _baseService = Substitute.For<IBaseService>();
        _attachFileService = Substitute.For<IAttachFileService>();
        _controller = new EmployeeWorkFromHomeController(
            _session,
            _baseService,
            _attachFileService
        );
    }

    private static string SerializeModel(EmployeeWorkFromHomeViewModel model)
    {
        // Format dates as "dd MMM, yyyy" which is what DateTimeConverter expects
        var fromDateStr = model.FromDate.ToString(
            "dd MMM, yyyy",
            System.Globalization.CultureInfo.InvariantCulture
        );
        var toDateStr = model.ToDate?.ToString(
            "dd MMM, yyyy",
            System.Globalization.CultureInfo.InvariantCulture
        );

        var json = $"{{\"EmployeeId\":{model.EmployeeId},\"FromDate\":\"{fromDateStr}\"";
        if (model.ToDate.HasValue)
        {
            json += $",\"ToDate\":\"{toDateStr}\"";
        }
        if (!string.IsNullOrEmpty(model.Remark))
        {
            json += $",\"Remark\":\"{model.Remark}\"";
        }
        json += $",\"IsCountInHeadcount\":{model.IsCountInHeadcount.ToString().ToLower()}";
        if (model.EmployeeWorkFromHomeId > 0)
        {
            json += $",\"EmployeeWorkFromHomeId\":{model.EmployeeWorkFromHomeId}";
        }
        json += "}";
        return json;
    }

    #region Index Tests

    [Fact]
    public void Index_ShouldReturnViewWithModel()
    {
        // Act
        var result = _controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<EmployeeWorkFromHomeFilterViewModel>(viewResult.Model);
        Assert.NotNull(model);
    }

    [Fact]
    public void Index_ShouldSetViewBagIsHost_WhenUserHasHostPermission()
    {
        // Arrange
        SetupPermission(FunctionKey.EmployeeManagement, Policy.Host, true);

        // Act
        _controller.Index();

        // Assert
        Assert.True((bool)_controller.ViewBag.IsHost);
        _session.Received(1).HasPermissionOnFunction(FunctionKey.EmployeeManagement, Policy.Host);
    }

    [Fact]
    public void Index_ShouldSetViewBagIsHost_WhenUserDoesNotHaveHostPermission()
    {
        // Arrange
        SetupPermission(FunctionKey.EmployeeManagement, Policy.Host, false);

        // Act
        _controller.Index();

        // Assert
        Assert.False((bool)_controller.ViewBag.IsHost);
    }

    #endregion

    #region GetEmployeeWorkFromHomeGrid Tests

    [Fact]
    public async Task GetEmployeeWorkFromHomeGrid_ShouldReturnPartialViewWithData()
    {
        // Arrange
        var criteria = new GetEmployeeWorkFromHomeCriteria
        {
            FromDate = DateTime.Now,
            ToDate = DateTime.Now.AddDays(7),
        };

        var employees = new List<EmployeeWorkFromHome>
        {
            new()
            {
                EmployeeWorkFromHomeId = 1,
                EmpId = 1,
                FullName = "Test User",
                FromDate = DateTime.Now,
            },
        };

        var attachFiles = new List<S3FileObject> { new() { FileName = "test.pdf" } };

        _baseService
            .GetDataAsync<EmployeeWorkFromHome>(Arg.Any<GetEmployeeWorkFromHomeCriteria>())
            .Returns(employees);
        _attachFileService
            .GetInfoAttachFileAsync(Arg.Any<long>(), Arg.Any<string>())
            .Returns(attachFiles);
        SetupPermission(FunctionKey.EmployeeManagement, Policy.Host, true);

        // Act
        var result = await _controller.GetEmployeeWorkFromHomeGrid(criteria);

        // Assert
        var partialViewResult = Assert.IsType<PartialViewResult>(result);
        Assert.Equal("_EmployeeWorkFromHomeGrid", partialViewResult.ViewName);
        var resultEmployees = Assert.IsType<List<EmployeeWorkFromHome>>(partialViewResult.Model);
        Assert.Single(resultEmployees);
        Assert.True((bool)_controller.ViewBag.IsHost);
    }

    [Fact]
    public async Task GetEmployeeWorkFromHomeGrid_ShouldCallAttachFileServiceForEachEmployee()
    {
        // Arrange
        var criteria = new GetEmployeeWorkFromHomeCriteria();
        var employees = new List<EmployeeWorkFromHome>
        {
            new() { EmployeeWorkFromHomeId = 1 },
            new() { EmployeeWorkFromHomeId = 2 },
        };

        _baseService
            .GetDataAsync<EmployeeWorkFromHome>(Arg.Any<GetEmployeeWorkFromHomeCriteria>())
            .Returns(employees);
        _attachFileService
            .GetInfoAttachFileAsync(Arg.Any<long>(), Arg.Any<string>())
            .Returns(new List<S3FileObject>());

        // Act
        await _controller.GetEmployeeWorkFromHomeGrid(criteria);

        // Assert
        await _attachFileService
            .Received(2)
            .GetInfoAttachFileAsync(Arg.Any<long>(), Arg.Is<string>(s => s.Contains("WFH")));
    }

    [Fact]
    public async Task GetEmployeeWorkFromHomeGrid_ShouldSetViewBagIsHost()
    {
        // Arrange
        var criteria = new GetEmployeeWorkFromHomeCriteria();
        SetupPermission(FunctionKey.EmployeeManagement, Policy.Host, false);

        _baseService
            .GetDataAsync<EmployeeWorkFromHome>(Arg.Any<GetEmployeeWorkFromHomeCriteria>())
            .Returns(new List<EmployeeWorkFromHome>());
        _attachFileService
            .GetInfoAttachFileAsync(Arg.Any<long>(), Arg.Any<string>())
            .Returns(new List<S3FileObject>());

        // Act
        await _controller.GetEmployeeWorkFromHomeGrid(criteria);

        // Assert
        Assert.False((bool)_controller.ViewBag.IsHost);
    }

    #endregion

    #region Create (GET) Tests

    [Fact]
    public void Create_Get_ShouldReturnPartialViewWithModel()
    {
        // Act
        var result = _controller.Create();

        // Assert
        var partialViewResult = Assert.IsType<PartialViewResult>(result);
        Assert.Equal("_Create", partialViewResult.ViewName);
        var model = Assert.IsType<EmployeeWorkFromHomeViewModel>(partialViewResult.Model);
        Assert.NotNull(model);
    }

    #endregion

    #region Create (POST) Tests

    [Fact]
    public async Task Create_Post_WithValidModel_ShouldReturnSuccess()
    {
        // Arrange
        var model = new EmployeeWorkFromHomeViewModel
        {
            EmployeeId = 1,
            FromDate = DateTime.Now,
            ToDate = DateTime.Now.AddDays(1),
            Remark = "Test remark",
            IsCountInHeadcount = true,
        };

        var requestModel = SerializeModel(model);
        var files = new List<IFormFile>();
        var commandResult = new CommandResult
        {
            Code = 0,
            Message = "Success",
            OutputId = 123,
        };

        _baseService
            .ExecuteNonQueryAsync(Arg.Any<InsertEmployeeWorkFromHomeCriteria>())
            .Returns(commandResult);

        // Act
        var result = await _controller.Create(requestModel, files);

        // Assert
        var jsonResult = Assert.IsType<JsonResult>(result);
        var response = Assert.IsType<CommandResult>(jsonResult.Value);
        Assert.Equal(0, response.Code);
        Assert.Equal("Success", response.Message);

        await _baseService
            .Received(1)
            .ExecuteNonQueryAsync(
                Arg.Is<InsertEmployeeWorkFromHomeCriteria>(c =>
                    c.EmpId == model.EmployeeId
                    && c.FromDate.Date == model.FromDate.Date
                    && (c.ToDate == model.ToDate
                        || (c.ToDate.HasValue
                            && model.ToDate.HasValue
                            && c.ToDate.Value.Date == model.ToDate.Value.Date))
                    && c.Remark == model.Remark
                    && c.IsCountInHeadcount == model.IsCountInHeadcount
                    && c.CreatedBy == 1 // From SetupEmployeeSession
                )
            );
    }

    [Fact]
    public async Task Create_Post_WithValidModel_ShouldUploadFiles_WhenSuccessful()
    {
        // Arrange
        var model = new EmployeeWorkFromHomeViewModel
        {
            EmployeeId = 1,
            FromDate = DateTime.Now,
            ToDate = DateTime.Now.AddDays(1),
        };

        var requestModel = SerializeModel(model);
        var file = Substitute.For<IFormFile>();
        var files = new List<IFormFile> { file };
        var commandResult = new CommandResult { Code = 0, OutputId = 123 };

        _baseService
            .ExecuteNonQueryAsync(Arg.Any<InsertEmployeeWorkFromHomeCriteria>())
            .Returns(callInfo =>
            {
                var criteria = callInfo.Arg<InsertEmployeeWorkFromHomeCriteria>();
                criteria.RequestId = 123;
                return commandResult;
            });

        // Act
        await _controller.Create(requestModel, files);

        // Assert
        await _attachFileService
            .Received(1)
            .UploadFileAsync(files, 123, Arg.Is<string>(s => s.Contains("WFH")));
    }

    [Fact]
    public async Task Create_Post_WithValidModel_ShouldNotUploadFiles_WhenUnsuccessful()
    {
        // Arrange
        var model = new EmployeeWorkFromHomeViewModel
        {
            EmployeeId = 1,
            FromDate = DateTime.Now,
            ToDate = DateTime.Now.AddDays(1),
        };

        var requestModel = SerializeModel(model);
        var file = Substitute.For<IFormFile>();
        var files = new List<IFormFile> { file };
        var commandResult = new CommandResult { Code = 1, Message = "Error" };

        _baseService
            .ExecuteNonQueryAsync(Arg.Any<InsertEmployeeWorkFromHomeCriteria>())
            .Returns(commandResult);

        // Act
        await _controller.Create(requestModel, files);

        // Assert
        await _attachFileService
            .DidNotReceive()
            .UploadFileAsync(Arg.Any<IList<IFormFile>>(), Arg.Any<long>(), Arg.Any<string>());
    }

    [Fact]
    public async Task Create_Post_WithInvalidModel_ShouldReturnError()
    {
        // Arrange
        var model = new EmployeeWorkFromHomeViewModel
        {
            EmployeeId = 0, // Invalid
            FromDate = DateTime.Now,
        };

        var requestModel = SerializeModel(model);
        _controller.ModelState.AddModelError("EmployeeId", "Employee is required");

        // Act
        var result = await _controller.Create(requestModel, new List<IFormFile>());

        // Assert
        var jsonResult = Assert.IsType<JsonResult>(result);
        var response = Assert.IsType<CommandResult>(jsonResult.Value);
        Assert.Equal("Employee is required", response.Message);
        await _baseService
            .DidNotReceive()
            .ExecuteNonQueryAsync(Arg.Any<InsertEmployeeWorkFromHomeCriteria>());
    }

    [Fact]
    public async Task Create_Post_WithInvalidJson_ShouldThrowException()
    {
        // Arrange
        string requestModel = string.Empty;

        // Act & Assert
        await Assert.ThrowsAsync<System.Text.Json.JsonException>(
            () => _controller.Create(requestModel, new List<IFormFile>())
        );
        await _baseService
            .DidNotReceive()
            .ExecuteNonQueryAsync(Arg.Any<InsertEmployeeWorkFromHomeCriteria>());
    }

    [Fact]
    public async Task Create_Post_ShouldUseEmployeeSessionEmpId()
    {
        // Arrange
        SetupEmployeeSession(456);
        var model = new EmployeeWorkFromHomeViewModel
        {
            EmployeeId = 1,
            FromDate = DateTime.Now,
            ToDate = DateTime.Now.AddDays(1),
        };

        var requestModel = SerializeModel(model);
        var commandResult = new CommandResult { Code = 0 };

        _baseService
            .ExecuteNonQueryAsync(Arg.Any<InsertEmployeeWorkFromHomeCriteria>())
            .Returns(commandResult);

        // Act
        await _controller.Create(requestModel, new List<IFormFile>());

        // Assert
        await _baseService
            .Received(1)
            .ExecuteNonQueryAsync(
                Arg.Is<InsertEmployeeWorkFromHomeCriteria>(c => c.CreatedBy == 456)
            );
    }

    #endregion

    #region Edit (GET) Tests

    [Fact]
    public async Task Edit_Get_WithValidId_ShouldReturnPartialViewWithModel()
    {
        // Arrange
        var id = 1;
        var employee = new EmployeeWorkFromHome
        {
            EmployeeWorkFromHomeId = id,
            EmpId = 1,
            Nickname = "Test",
            FullName = "Test User",
            FromDate = DateTime.Now,
            ToDate = DateTime.Now.AddDays(1),
            Remark = "Test remark",
            IsCountInHeadcount = true,
        };

        var attachFiles = new List<S3FileObject>
        {
            new() { FileName = "test.pdf" },
            new() { FileName = "document.docx" },
        };

        _baseService
            .GetDataEntityAsync<EmployeeWorkFromHome>(
                Arg.Any<GetEmployeeWorkFromHomeByIdCriteria>()
            )
            .Returns(employee);
        _attachFileService.GetInfoAttachFileAsync(id, Arg.Any<string>()).Returns(attachFiles);

        // Act
        var result = await _controller.Edit(id);

        // Assert
        var partialViewResult = Assert.IsType<PartialViewResult>(result);
        Assert.Equal("_Edit", partialViewResult.ViewName);
        var model = Assert.IsType<EmployeeWorkFromHomeViewModel>(partialViewResult.Model);
        Assert.Equal(id, model.EmployeeWorkFromHomeId);
        Assert.Equal(employee.EmpId, model.EmployeeId);
        Assert.Equal("Test (Test User)", model.Employee);
        Assert.Equal(2, model.Files.Count);
        Assert.Contains("test.pdf", model.Files);
        Assert.Contains("document.docx", model.Files);
    }

    [Fact]
    public async Task Edit_Get_ShouldCallAttachFileService()
    {
        // Arrange
        var id = 1;
        var employee = new EmployeeWorkFromHome { EmployeeWorkFromHomeId = id };

        _baseService
            .GetDataEntityAsync<EmployeeWorkFromHome>(
                Arg.Any<GetEmployeeWorkFromHomeByIdCriteria>()
            )
            .Returns(employee);
        _attachFileService
            .GetInfoAttachFileAsync(id, Arg.Any<string>())
            .Returns(new List<S3FileObject>());

        // Act
        await _controller.Edit(id);

        // Assert
        await _attachFileService
            .Received(1)
            .GetInfoAttachFileAsync(id, Arg.Is<string>(s => s.Contains("WFH")));
    }

    #endregion

    #region Edit (POST) Tests

    [Fact]
    public async Task Edit_Post_WithValidModel_ShouldReturnSuccess()
    {
        // Arrange
        var model = new EmployeeWorkFromHomeViewModel
        {
            EmployeeWorkFromHomeId = 1,
            EmployeeId = 1,
            FromDate = DateTime.Now,
            ToDate = DateTime.Now.AddDays(1),
            Remark = "Updated remark",
            IsCountInHeadcount = false,
        };

        var requestModel = SerializeModel(model);
        var files = new List<IFormFile>();
        var filesRemove = JsonSerializer.Serialize(new List<string>());
        var commandResult = new CommandResult { Code = 0, Message = "Success" };

        _baseService
            .ExecuteNonQueryAsync(Arg.Any<UpdateEmployeeWorkFromHomeCriteria>())
            .Returns(commandResult);

        // Act
        var result = await _controller.Edit(requestModel, files, filesRemove);

        // Assert
        var jsonResult = Assert.IsType<JsonResult>(result);
        var response = Assert.IsType<CommandResult>(jsonResult.Value);
        Assert.Equal(0, response.Code);

        await _baseService
            .Received(1)
            .ExecuteNonQueryAsync(
                Arg.Is<UpdateEmployeeWorkFromHomeCriteria>(c =>
                    c.EmployeeWorkFromHomeId == model.EmployeeWorkFromHomeId
                    && c.EmpId == model.EmployeeId
                    && c.FromDate.Date == model.FromDate.Date
                    && (c.ToDate == model.ToDate
                        || (c.ToDate.HasValue
                            && model.ToDate.HasValue
                            && c.ToDate.Value.Date == model.ToDate.Value.Date))
                    && c.Remark == model.Remark
                    && c.IsCountInHeadcount == model.IsCountInHeadcount
                    && c.ModifiedBy == 1 // From SetupEmployeeSession
                )
            );
    }

    [Fact]
    public async Task Edit_Post_WithValidModel_ShouldDeleteFiles_WhenFilesRemoveProvided()
    {
        // Arrange
        var model = new EmployeeWorkFromHomeViewModel
        {
            EmployeeWorkFromHomeId = 1,
            EmployeeId = 1,
            FromDate = DateTime.Now,
            ToDate = DateTime.Now.AddDays(1),
        };

        var requestModel = SerializeModel(model);
        var files = new List<IFormFile>();
        var filesToRemove = new List<string> { "old-file.pdf", "old-document.docx" };
        var filesRemove = JsonSerializer.Serialize(filesToRemove);
        var commandResult = new CommandResult { Code = 0 };

        _baseService
            .ExecuteNonQueryAsync(Arg.Any<UpdateEmployeeWorkFromHomeCriteria>())
            .Returns(commandResult);

        // Act
        await _controller.Edit(requestModel, files, filesRemove);

        // Assert
        await _attachFileService
            .Received(1)
            .DeleteFileAsync(
                Arg.Is<List<string>>(list =>
                    list.Count == 2
                    && list.Contains("old-file.pdf")
                    && list.Contains("old-document.docx")
                ),
                model.EmployeeWorkFromHomeId,
                Arg.Is<string>(s => s.Contains("WFH"))
            );
    }

    [Fact]
    public async Task Edit_Post_WithValidModel_ShouldNotDeleteFiles_WhenFilesRemoveIsEmpty()
    {
        // Arrange
        var model = new EmployeeWorkFromHomeViewModel
        {
            EmployeeWorkFromHomeId = 1,
            EmployeeId = 1,
            FromDate = DateTime.Now,
            ToDate = DateTime.Now.AddDays(1),
        };

        var requestModel = SerializeModel(model);
        var files = new List<IFormFile>();
        var filesRemove = string.Empty;
        var commandResult = new CommandResult { Code = 0 };

        _baseService
            .ExecuteNonQueryAsync(Arg.Any<UpdateEmployeeWorkFromHomeCriteria>())
            .Returns(commandResult);

        // Act
        await _controller.Edit(requestModel, files, filesRemove);

        // Assert
        await _attachFileService
            .DidNotReceive()
            .DeleteFileAsync(Arg.Any<List<string>>(), Arg.Any<long>(), Arg.Any<string>());
    }

    [Fact]
    public async Task Edit_Post_WithValidModel_ShouldUploadFiles_WhenSuccessful()
    {
        // Arrange
        var model = new EmployeeWorkFromHomeViewModel
        {
            EmployeeWorkFromHomeId = 1,
            EmployeeId = 1,
            FromDate = DateTime.Now,
            ToDate = DateTime.Now.AddDays(1),
        };

        var requestModel = SerializeModel(model);
        var file = Substitute.For<IFormFile>();
        var files = new List<IFormFile> { file };
        var filesRemove = JsonSerializer.Serialize(new List<string>());
        var commandResult = new CommandResult { Code = 0 };

        _baseService
            .ExecuteNonQueryAsync(Arg.Any<UpdateEmployeeWorkFromHomeCriteria>())
            .Returns(commandResult);

        // Act
        await _controller.Edit(requestModel, files, filesRemove);

        // Assert
        await _attachFileService
            .Received(1)
            .UploadFileAsync(
                files,
                model.EmployeeWorkFromHomeId,
                Arg.Is<string>(s => s.Contains("WFH"))
            );
    }

    [Fact]
    public async Task Edit_Post_WithValidModel_ShouldNotUploadFiles_WhenUnsuccessful()
    {
        // Arrange
        var model = new EmployeeWorkFromHomeViewModel
        {
            EmployeeWorkFromHomeId = 1,
            EmployeeId = 1,
            FromDate = DateTime.Now,
            ToDate = DateTime.Now.AddDays(1),
        };

        var requestModel = SerializeModel(model);
        var file = Substitute.For<IFormFile>();
        var files = new List<IFormFile> { file };
        var filesRemove = JsonSerializer.Serialize(new List<string>());
        var commandResult = new CommandResult { Code = 1, Message = "Error" };

        _baseService
            .ExecuteNonQueryAsync(Arg.Any<UpdateEmployeeWorkFromHomeCriteria>())
            .Returns(commandResult);

        // Act
        await _controller.Edit(requestModel, files, filesRemove);

        // Assert
        await _attachFileService
            .DidNotReceive()
            .UploadFileAsync(Arg.Any<IList<IFormFile>>(), Arg.Any<long>(), Arg.Any<string>());
    }

    [Fact]
    public async Task Edit_Post_WithInvalidModel_ShouldReturnError()
    {
        // Arrange
        var model = new EmployeeWorkFromHomeViewModel
        {
            EmployeeWorkFromHomeId = 1,
            EmployeeId = 0, // Invalid
            FromDate = DateTime.Now,
        };

        var requestModel = SerializeModel(model);
        _controller.ModelState.AddModelError("EmployeeId", "Employee is required");

        // Act
        var result = await _controller.Edit(requestModel, new List<IFormFile>(), string.Empty);

        // Assert
        var jsonResult = Assert.IsType<JsonResult>(result);
        var response = Assert.IsType<CommandResult>(jsonResult.Value);
        Assert.Equal("Employee is required", response.Message);
        await _baseService
            .DidNotReceive()
            .ExecuteNonQueryAsync(Arg.Any<UpdateEmployeeWorkFromHomeCriteria>());
    }

    [Fact]
    public async Task Edit_Post_ShouldUseEmployeeSessionEmpId()
    {
        // Arrange
        SetupEmployeeSession(789);
        var model = new EmployeeWorkFromHomeViewModel
        {
            EmployeeWorkFromHomeId = 1,
            EmployeeId = 1,
            FromDate = DateTime.Now,
            ToDate = DateTime.Now.AddDays(1),
        };

        var requestModel = SerializeModel(model);
        var commandResult = new CommandResult { Code = 0 };

        _baseService
            .ExecuteNonQueryAsync(Arg.Any<UpdateEmployeeWorkFromHomeCriteria>())
            .Returns(commandResult);

        // Act
        await _controller.Edit(requestModel, new List<IFormFile>(), string.Empty);

        // Assert
        await _baseService
            .Received(1)
            .ExecuteNonQueryAsync(
                Arg.Is<UpdateEmployeeWorkFromHomeCriteria>(c => c.ModifiedBy == 789)
            );
    }

    #endregion

    #region Delete Tests

    [Fact]
    public async Task Delete_WithValidId_ShouldReturnSuccess()
    {
        // Arrange
        var id = 1;
        var commandResult = new CommandResult { Code = 0, Message = "Success" };

        _baseService
            .ExecuteNonQueryAsync(Arg.Any<DeleteEmployeeWorkFromHomeCriteria>())
            .Returns(commandResult);

        // Act
        var result = await _controller.Delete(id);

        // Assert
        var jsonResult = Assert.IsType<JsonResult>(result);
        var response = Assert.IsType<CommandResult>(jsonResult.Value);
        Assert.Equal(0, response.Code);
        Assert.Equal("Success", response.Message);

        await _baseService
            .Received(1)
            .ExecuteNonQueryAsync(
                Arg.Is<DeleteEmployeeWorkFromHomeCriteria>(c => c.EmployeeWorkFromHomeId == id)
            );
    }

    [Fact]
    public async Task Delete_WithServiceFailure_ShouldReturnError()
    {
        // Arrange
        var id = 1;
        var commandResult = new CommandResult { Code = 1, Message = "Failed to delete" };

        _baseService
            .ExecuteNonQueryAsync(Arg.Any<DeleteEmployeeWorkFromHomeCriteria>())
            .Returns(commandResult);

        // Act
        var result = await _controller.Delete(id);

        // Assert
        var jsonResult = Assert.IsType<JsonResult>(result);
        var response = Assert.IsType<CommandResult>(jsonResult.Value);
        Assert.Equal(1, response.Code);
        Assert.Equal("Failed to delete", response.Message);
    }

    #endregion
}
