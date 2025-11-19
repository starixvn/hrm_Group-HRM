using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using GO.UserControl.Client.Core.Enumerables;
using HRM.Controllers.GroupSpace;
using HRM.Core.Constants;
using HRM.Core.Models;
using HRM.Models;
using HRM.Models.Group.ExchangeRate;
using HRM.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Xunit;

namespace HRM.Controllers.UnitTests.GroupSpace;

public class ExchangeRateControllerTests : BaseControllerTest
{
    private readonly IExchangeRateService _exchangeRateService;
    private readonly ExchangeRateController _controller;

    public ExchangeRateControllerTests()
    {
        _exchangeRateService = Substitute.For<IExchangeRateService>();
        _controller = new ExchangeRateController(_session, _exchangeRateService);
    }

    [Fact]
    public void Index_WithPermission_ShouldReturnViewWithTrue()
    {
        // Arrange
        SetupPermission(FunctionKey.ExchangeRate, Policy.Host, true);

        // Act
        var result = _controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.True((bool)viewResult.ViewData.Model!);
        _session.Received(1).HasPermissionOnFunction(FunctionKey.ExchangeRate, Policy.Host);
    }

    [Fact]
    public void Index_WithoutPermission_ShouldReturnViewWithFalse()
    {
        // Arrange
        SetupPermission(FunctionKey.ExchangeRate, Policy.Host, false);

        // Act
        var result = _controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.False((bool)viewResult.ViewData.Model!);
    }

    [Fact]
    public async Task GetExchangeRates_ShouldReturnJsonWithExchangeRates()
    {
        // Arrange
        var expectedRates = new List<ExchangeRate>
        {
            new()
            {
                CurrencyId = 1,
                BaseCurrency = "USD",
                ToUsd = 1.0m,
                EffectiveDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Unspecified),
                Rounding = 0,
                ModifiedDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Local),
            },
            new()
            {
                CurrencyId = 2,
                BaseCurrency = "EUR",
                ToUsd = 1.1m,
                EffectiveDate = new DateTime(2024, 1, 15, 0, 0, 0, DateTimeKind.Unspecified),
                Rounding = 2,
                ModifiedDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Local),
            },
        };

        _exchangeRateService.GetExchangeRatesAsync().Returns(expectedRates);

        // Act
        var result = await _controller.GetExchangeRates();

        // Assert
        var jsonResult = Assert.IsType<JsonResult>(result);
        Assert.Equal(expectedRates, jsonResult.Value);
        await _exchangeRateService.Received(1).GetExchangeRatesAsync();
    }

    [Fact]
    public async Task GetExchangeRates_WithEmptyList_ShouldReturnEmptyJson()
    {
        // Arrange
        var emptyRates = new List<ExchangeRate>();
        _exchangeRateService.GetExchangeRatesAsync().Returns(emptyRates);

        // Act
        var result = await _controller.GetExchangeRates();

        // Assert
        var jsonResult = Assert.IsType<JsonResult>(result);
        var resultList = Assert.IsType<IEnumerable<ExchangeRate>>(
            jsonResult.Value,
            exactMatch: false
        );
        Assert.Empty(resultList);
    }

    [Fact]
    public async Task GetRoundingRules_ShouldReturnJsonWithRoundingRules()
    {
        // Arrange
        var expectedRules = new List<CurrencyRoundingRule>
        {
            new()
            {
                Currency = "USD",
                Rounding = 0,
                Description = "Rounded to nearest 1",
            },
            new()
            {
                Currency = "EUR",
                Rounding = 2,
                Description = "Rounded to 2 decimal places",
            },
        };

        _exchangeRateService.GetRoundingRulesAsync().Returns(expectedRules);

        // Act
        var result = await _controller.GetRoundingRules();

        // Assert
        var jsonResult = Assert.IsType<JsonResult>(result);
        Assert.Equal(expectedRules, jsonResult.Value);
        await _exchangeRateService.Received(1).GetRoundingRulesAsync();
    }

    [Fact]
    public async Task Create_WithValidModel_ShouldReturnSuccess()
    {
        // Arrange
        var model = new CreateExchangeRate
        {
            CurrencyCode = "USD",
            ExchangeRate = 1.0m,
            EffectiveDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Unspecified),
            Rounding = 0,
        };

        var commandResult = new CommandResult { Code = 0, Message = "Success" };

        _exchangeRateService
            .CreateExchangeRateAsync(
                model.CurrencyCode,
                model.ExchangeRate,
                model.EffectiveDate,
                model.Rounding,
                Arg.Any<int>()
            )
            .Returns(commandResult);

        // Act
        var result = await _controller.Create(model);

        // Assert
        var jsonResult = Assert.IsType<JsonResult>(result);
        var json = JsonSerializer.Serialize(jsonResult.Value);
        var response = JsonSerializer.Deserialize<JsonElement>(json);
        Assert.Equal(0, response.GetProperty("code").GetInt32());
        Assert.Equal("Success", response.GetProperty("message").GetString());

        await _exchangeRateService
            .Received(1)
            .CreateExchangeRateAsync(
                model.CurrencyCode,
                model.ExchangeRate,
                model.EffectiveDate,
                model.Rounding,
                Arg.Any<int>()
            );
    }

    [Fact]
    public async Task Create_WithInvalidModel_ShouldReturnError()
    {
        // Arrange
        CreateExchangeRate? model = null;

        // Act
        var result = await _controller.Create(model);

        // Assert
        var jsonResult = Assert.IsType<JsonResult>(result);
        var json = JsonSerializer.Serialize(jsonResult.Value);
        var response = JsonSerializer.Deserialize<JsonElement>(json);
        Assert.Equal(1, response.GetProperty("code").GetInt32());
        Assert.Equal("Invalid request", response.GetProperty("message").GetString());

        await _exchangeRateService
            .DidNotReceive()
            .CreateExchangeRateAsync(
                Arg.Any<string>(),
                Arg.Any<decimal>(),
                Arg.Any<DateTime>(),
                Arg.Any<decimal>(),
                Arg.Any<int>()
            );
    }

    [Fact]
    public async Task Create_WithServiceFailure_ShouldReturnError()
    {
        // Arrange
        var model = new CreateExchangeRate
        {
            CurrencyCode = "USD",
            ExchangeRate = 1.0m,
            EffectiveDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Unspecified),
            Rounding = 0,
        };

        var commandResult = new CommandResult { Code = 1, Message = "Failed to create" };

        _exchangeRateService
            .CreateExchangeRateAsync(
                Arg.Any<string>(),
                Arg.Any<decimal>(),
                Arg.Any<DateTime>(),
                Arg.Any<decimal>(),
                Arg.Any<int>()
            )
            .Returns(commandResult);

        // Act
        var result = await _controller.Create(model);

        // Assert
        var jsonResult = Assert.IsType<JsonResult>(result);
        var json = JsonSerializer.Serialize(jsonResult.Value);
        var response = JsonSerializer.Deserialize<JsonElement>(json);
        Assert.Equal(1, response.GetProperty("code").GetInt32());
        Assert.Equal("Failed to create", response.GetProperty("message").GetString());
    }

    [Fact]
    public async Task Update_WithValidModel_ShouldReturnSuccess()
    {
        // Arrange
        var model = new UpdateExchangeRate
        {
            CurrencyId = 1,
            EffectiveDate = new DateTime(2024, 1, 15, 0, 0, 0, DateTimeKind.Unspecified),
            OldEffectiveDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Unspecified),
            CurrencyCode = "USD",
            ExchangeRate = 1.05m,
            Rounding = 0,
        };

        var commandResult = new CommandResult { Code = 0, Message = "Success" };

        _exchangeRateService
            .UpdateExchangeRateAsync(
                model.CurrencyId,
                model.EffectiveDate,
                model.OldEffectiveDate,
                model.CurrencyCode,
                model.ExchangeRate,
                model.Rounding,
                Arg.Any<int>()
            )
            .Returns(commandResult);

        // Act
        var result = await _controller.Update(model);

        // Assert
        var jsonResult = Assert.IsType<JsonResult>(result);
        var json = JsonSerializer.Serialize(jsonResult.Value);
        var response = JsonSerializer.Deserialize<JsonElement>(json);
        Assert.Equal(0, response.GetProperty("code").GetInt32());
        Assert.Equal("Success", response.GetProperty("message").GetString());

        await _exchangeRateService
            .Received(1)
            .UpdateExchangeRateAsync(
                model.CurrencyId,
                model.EffectiveDate,
                model.OldEffectiveDate,
                model.CurrencyCode,
                model.ExchangeRate,
                model.Rounding,
                Arg.Any<int>()
            );
    }

    [Fact]
    public async Task Update_WithInvalidModel_ShouldReturnError()
    {
        // Arrange
        UpdateExchangeRate? model = null;

        // Act
        var result = await _controller.Update(model);

        // Assert
        var jsonResult = Assert.IsType<JsonResult>(result);
        var json = JsonSerializer.Serialize(jsonResult.Value);
        var response = JsonSerializer.Deserialize<JsonElement>(json);
        Assert.Equal(1, response.GetProperty("code").GetInt32());
        Assert.Equal("Invalid request", response.GetProperty("message").GetString());

        await _exchangeRateService
            .DidNotReceive()
            .UpdateExchangeRateAsync(
                Arg.Any<int>(),
                Arg.Any<DateTime>(),
                Arg.Any<DateTime>(),
                Arg.Any<string>(),
                Arg.Any<decimal>(),
                Arg.Any<decimal>(),
                Arg.Any<int>()
            );
    }

    [Fact]
    public async Task Update_WithServiceFailure_ShouldReturnError()
    {
        // Arrange
        var model = new UpdateExchangeRate
        {
            CurrencyId = 1,
            EffectiveDate = new DateTime(2024, 1, 15, 0, 0, 0, DateTimeKind.Unspecified),
            OldEffectiveDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Unspecified),
            CurrencyCode = "USD",
            ExchangeRate = 1.05m,
            Rounding = 0,
        };

        var commandResult = new CommandResult { Code = 1, Message = "Failed to update" };

        _exchangeRateService
            .UpdateExchangeRateAsync(
                Arg.Any<int>(),
                Arg.Any<DateTime>(),
                Arg.Any<DateTime>(),
                Arg.Any<string>(),
                Arg.Any<decimal>(),
                Arg.Any<decimal>(),
                Arg.Any<int>()
            )
            .Returns(commandResult);

        // Act
        var result = await _controller.Update(model);

        // Assert
        var jsonResult = Assert.IsType<JsonResult>(result);
        var json = JsonSerializer.Serialize(jsonResult.Value);
        var response = JsonSerializer.Deserialize<JsonElement>(json);
        Assert.Equal(1, response.GetProperty("code").GetInt32());
        Assert.Equal("Failed to update", response.GetProperty("message").GetString());
    }

    [Fact]
    public async Task Delete_WithValidModel_ShouldReturnSuccess()
    {
        // Arrange
        var model = new DeleteExchangeRate
        {
            CurrencyId = 1,
            EffectiveDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Unspecified),
        };

        var commandResult = new CommandResult { Code = 0, Message = "Success" };

        _exchangeRateService
            .DeleteExchangeRateAsync(model.CurrencyId, model.EffectiveDate, Arg.Any<int>())
            .Returns(commandResult);

        // Act
        var result = await _controller.Delete(model);

        // Assert
        var jsonResult = Assert.IsType<JsonResult>(result);
        var json = JsonSerializer.Serialize(jsonResult.Value);
        var response = JsonSerializer.Deserialize<JsonElement>(json);
        Assert.Equal(0, response.GetProperty("code").GetInt32());
        Assert.Equal("Success", response.GetProperty("message").GetString());

        await _exchangeRateService
            .Received(1)
            .DeleteExchangeRateAsync(model.CurrencyId, model.EffectiveDate, Arg.Any<int>());
    }

    [Fact]
    public async Task Delete_WithInvalidModel_ShouldReturnError()
    {
        // Arrange
        DeleteExchangeRate? model = null;

        // Act
        var result = await _controller.Delete(model);

        // Assert
        var jsonResult = Assert.IsType<JsonResult>(result);
        var json = JsonSerializer.Serialize(jsonResult.Value);
        var response = JsonSerializer.Deserialize<JsonElement>(json);
        Assert.Equal(1, response.GetProperty("code").GetInt32());
        Assert.Equal("Invalid request", response.GetProperty("message").GetString());

        await _exchangeRateService
            .DidNotReceive()
            .DeleteExchangeRateAsync(Arg.Any<int>(), Arg.Any<DateTime>(), Arg.Any<int>());
    }

    [Fact]
    public async Task Delete_WithServiceFailure_ShouldReturnError()
    {
        // Arrange
        var model = new DeleteExchangeRate
        {
            CurrencyId = 1,
            EffectiveDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Unspecified),
        };

        var commandResult = new CommandResult { Code = 1, Message = "Failed to delete" };

        _exchangeRateService
            .DeleteExchangeRateAsync(Arg.Any<int>(), Arg.Any<DateTime>(), Arg.Any<int>())
            .Returns(commandResult);

        // Act
        var result = await _controller.Delete(model);

        // Assert
        var jsonResult = Assert.IsType<JsonResult>(result);
        var json = JsonSerializer.Serialize(jsonResult.Value);
        var response = JsonSerializer.Deserialize<JsonElement>(json);
        Assert.Equal(1, response.GetProperty("code").GetInt32());
        Assert.Equal("Failed to delete", response.GetProperty("message").GetString());
    }

    [Fact]
    public async Task Create_ShouldUseEmployeeSessionEmpId()
    {
        // Arrange
        var model = new CreateExchangeRate
        {
            CurrencyCode = "USD",
            ExchangeRate = 1.0m,
            EffectiveDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Unspecified),
            Rounding = 0,
        };

        var commandResult = new CommandResult { Code = 0, Message = "Success" };

        SetupEmployeeSession(123); // Set EmpId to 123

        _exchangeRateService
            .CreateExchangeRateAsync(
                Arg.Any<string>(),
                Arg.Any<decimal>(),
                Arg.Any<DateTime>(),
                Arg.Any<decimal>(),
                Arg.Any<int>()
            )
            .Returns(commandResult);

        // Act
        await _controller.Create(model);

        // Assert
        await _exchangeRateService
            .Received(1)
            .CreateExchangeRateAsync(
                model.CurrencyCode,
                model.ExchangeRate,
                model.EffectiveDate,
                model.Rounding,
                123 // Verify EmpId is used
            );
    }

    [Fact]
    public async Task Update_ShouldUseEmployeeSessionEmpId()
    {
        // Arrange
        var model = new UpdateExchangeRate
        {
            CurrencyId = 1,
            EffectiveDate = new DateTime(2024, 1, 15, 0, 0, 0, DateTimeKind.Unspecified),
            OldEffectiveDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Unspecified),
            CurrencyCode = "USD",
            ExchangeRate = 1.05m,
            Rounding = 0,
        };

        var commandResult = new CommandResult { Code = 0, Message = "Success" };

        SetupEmployeeSession(456); // Set EmpId to 456

        _exchangeRateService
            .UpdateExchangeRateAsync(
                Arg.Any<int>(),
                Arg.Any<DateTime>(),
                Arg.Any<DateTime>(),
                Arg.Any<string>(),
                Arg.Any<decimal>(),
                Arg.Any<decimal>(),
                Arg.Any<int>()
            )
            .Returns(commandResult);

        // Act
        await _controller.Update(model);

        // Assert
        await _exchangeRateService
            .Received(1)
            .UpdateExchangeRateAsync(
                model.CurrencyId,
                model.EffectiveDate,
                model.OldEffectiveDate,
                model.CurrencyCode,
                model.ExchangeRate,
                model.Rounding,
                456 // Verify EmpId is used
            );
    }

    [Fact]
    public async Task Delete_ShouldUseEmployeeSessionEmpId()
    {
        // Arrange
        var model = new DeleteExchangeRate
        {
            CurrencyId = 1,
            EffectiveDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Unspecified),
        };

        var commandResult = new CommandResult { Code = 0, Message = "Success" };

        SetupEmployeeSession(789); // Set EmpId to 789

        _exchangeRateService
            .DeleteExchangeRateAsync(Arg.Any<int>(), Arg.Any<DateTime>(), Arg.Any<int>())
            .Returns(commandResult);

        // Act
        await _controller.Delete(model);

        // Assert
        await _exchangeRateService
            .Received(1)
            .DeleteExchangeRateAsync(
                model.CurrencyId,
                model.EffectiveDate,
                789 // Verify EmpId is used
            );
    }
}
