using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HRM.Core.Models;
using HRM.Core.Services;
using HRM.Criteria.Company;
using HRM.Criteria.ExchangeRate;
using HRM.Models;
using HRM.Services.Implement;
using NSubstitute;
using Xunit;

namespace HRM.Services.UnitTests;

public class ExchangeRateServiceTests
{
    private readonly IBaseService _baseService;
    private readonly ExchangeRateService _service;

    public ExchangeRateServiceTests()
    {
        _baseService = Substitute.For<IBaseService>();
        _service = new ExchangeRateService(_baseService);
    }

    [Fact]
    public async Task GetExchangeRatesAsync_ShouldReturnExchangeRates()
    {
        // Arrange
        DateTime dateTime = new(2024, 1, 1, 0, 0, 0, DateTimeKind.Unspecified);
        var expectedRates = new List<ExchangeRate>
        {
            new()
            {
                CurrencyId = 1,
                BaseCurrency = "USD",
                ToUsd = 1.0m,
                EffectiveDate = dateTime,
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

        _baseService
            .GetDataAsync<ExchangeRate>(Arg.Any<GetExchangeRatesCriteria>())
            .Returns(expectedRates);

        // Act
        var result = await _service.GetExchangeRatesAsync();

        // Assert
        Assert.NotNull(result);
        var resultList = result.ToList();
        Assert.Equal(2, resultList.Count);
        Assert.Equal("USD", resultList[0].BaseCurrency);
        Assert.Equal("EUR", resultList[1].BaseCurrency);

        await _baseService
            .Received(1)
            .GetDataAsync<ExchangeRate>(Arg.Any<GetExchangeRatesCriteria>());
    }

    [Fact]
    public async Task GetExchangeRatesAsync_WithEmptyResult_ShouldReturnEmptyCollection()
    {
        // Arrange
        var emptyRates = new List<ExchangeRate>();

        _baseService
            .GetDataAsync<ExchangeRate>(Arg.Any<GetExchangeRatesCriteria>())
            .Returns(emptyRates);

        // Act
        var result = await _service.GetExchangeRatesAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);

        await _baseService
            .Received(1)
            .GetDataAsync<ExchangeRate>(Arg.Any<GetExchangeRatesCriteria>());
    }

    [Fact]
    public async Task GetRoundingRulesAsync_ShouldReturnRoundingRules()
    {
        // Arrange
        var currencies = new List<Currency>
        {
            new() { Id = 1, CurrencyCode = "USD", Rounding = 0 },
            new() { Id = 2, CurrencyCode = "EUR", Rounding = 2 },
            new() { Id = 3, CurrencyCode = "JPY", Rounding = -1 },
        };

        _baseService.GetDataAsync<Currency>(Arg.Any<GetCurrencyCriteria>()).Returns(currencies);

        // Act
        var result = await _service.GetRoundingRulesAsync();

        // Assert
        Assert.NotNull(result);
        var resultList = result.ToList();
        Assert.Equal(3, resultList.Count);
        Assert.Equal((short)1, resultList[0].CurrencyId);
        Assert.Equal("USD", resultList[0].Currency);
        Assert.Equal(0, resultList[0].Rounding);

        Assert.Equal((short)2, resultList[1].CurrencyId);
        Assert.Equal("EUR", resultList[1].Currency);
        Assert.Equal(2, resultList[1].Rounding);

        Assert.Equal((short)3, resultList[2].CurrencyId);
        Assert.Equal("JPY", resultList[2].Currency);
        Assert.Equal(-1, resultList[2].Rounding);

        await _baseService.Received(1).GetDataAsync<Currency>(Arg.Any<GetCurrencyCriteria>());
    }

    [Fact]
    public async Task GetRoundingRulesAsync_ShouldMapRoundingValuesCorrectly()
    {
        // Arrange
        var currencies = new List<Currency>
        {
            new() { Id = 1, CurrencyCode = "USD", Rounding = -4 },
            new() { Id = 2, CurrencyCode = "EUR", Rounding = -2 },
            new() { Id = 3, CurrencyCode = "GBP", Rounding = 0 },
            new() { Id = 4, CurrencyCode = "JPY", Rounding = 3 },
        };

        _baseService.GetDataAsync<Currency>(Arg.Any<GetCurrencyCriteria>()).Returns(currencies);

        // Act
        var result = await _service.GetRoundingRulesAsync();

        // Assert
        Assert.NotNull(result);
        var resultList = result.ToList();
        Assert.Equal(4, resultList.Count);
        Assert.Equal((short)1, resultList[0].CurrencyId);
        Assert.Equal("USD", resultList[0].Currency);
        Assert.Equal(-4, resultList[0].Rounding);
        Assert.Equal((short)2, resultList[1].CurrencyId);
        Assert.Equal("EUR", resultList[1].Currency);
        Assert.Equal(-2, resultList[1].Rounding);
        Assert.Equal((short)3, resultList[2].CurrencyId);
        Assert.Equal("GBP", resultList[2].Currency);
        Assert.Equal(0, resultList[2].Rounding);
        Assert.Equal((short)4, resultList[3].CurrencyId);
        Assert.Equal("JPY", resultList[3].Currency);
        Assert.Equal(3, resultList[3].Rounding);

        await _baseService.Received(1).GetDataAsync<Currency>(Arg.Any<GetCurrencyCriteria>());
    }

    [Fact]
    public async Task GetRoundingRulesAsync_WithEmptyCurrencies_ShouldReturnEmptyCollection()
    {
        // Arrange
        var emptyCurrencies = new List<Currency>();

        _baseService
            .GetDataAsync<Currency>(Arg.Any<GetCurrencyCriteria>())
            .Returns(emptyCurrencies);

        // Act
        var result = await _service.GetRoundingRulesAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);

        await _baseService.Received(1).GetDataAsync<Currency>(Arg.Any<GetCurrencyCriteria>());
    }

    [Fact]
    public async Task GetCurrenciesAsync_ShouldReturnCurrencies()
    {
        // Arrange
        var expectedCurrencies = new List<Currency>
        {
            new() { CurrencyCode = "USD" },
            new() { CurrencyCode = "EUR" },
        };

        _baseService
            .GetDataAsync<Currency>(Arg.Any<GetCurrencyCriteria>())
            .Returns(expectedCurrencies);

        // Act
        var result = await _service.GetCurrenciesAsync();

        // Assert
        Assert.NotNull(result);
        var resultList = result.ToList();
        Assert.Equal(2, resultList.Count);
        Assert.Equal("USD", resultList[0].CurrencyCode);
        Assert.Equal("EUR", resultList[1].CurrencyCode);

        await _baseService.Received(1).GetDataAsync<Currency>(Arg.Any<GetCurrencyCriteria>());
    }

    [Fact]
    public async Task CreateExchangeRateAsync_WithValidParameters_ShouldReturnSuccess()
    {
        // Arrange
        const string currencyCode = "USD";
        const decimal exchangeRate = 1.0m;
        var effectiveDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Unspecified);
        const int modifiedBy = 1;

        var expectedResult = new CommandResult { Code = 0, Message = "Success" };

        _baseService
            .ExecuteNonQueryAsync(Arg.Any<InsertExchangeRateCriteria>())
            .Returns(expectedResult);

        // Act
        var result = await _service.CreateExchangeRateAsync(
            currencyCode,
            exchangeRate,
            effectiveDate,
            modifiedBy
        );

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccessful);
        Assert.Equal(0, result.Code);

        await _baseService
            .Received(1)
            .ExecuteNonQueryAsync(
                Arg.Is<InsertExchangeRateCriteria>(c =>
                    c.CurrencyCode == currencyCode
                    && c.ExchangeRate == exchangeRate
                    && c.EffectiveDate == effectiveDate
                    && c.ModifiedBy == modifiedBy
                )
            );
    }

    [Fact]
    public async Task CreateExchangeRateAsync_WithFailure_ShouldReturnError()
    {
        // Arrange
        const string currencyCode = "USD";
        const decimal exchangeRate = 1.0m;
        var effectiveDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Unspecified);
        const int modifiedBy = 1;

        var expectedResult = new CommandResult
        {
            Code = 1,
            Message = "Failed to create exchange rate",
        };

        _baseService
            .ExecuteNonQueryAsync(Arg.Any<InsertExchangeRateCriteria>())
            .Returns(expectedResult);

        // Act
        var result = await _service.CreateExchangeRateAsync(
            currencyCode,
            exchangeRate,
            effectiveDate,
            modifiedBy
        );

        // Assert
        Assert.NotNull(result);
        Assert.False(result.IsSuccessful);
        Assert.Equal(1, result.Code);
        Assert.Equal("Failed to create exchange rate", result.Message);

        await _baseService
            .Received(1)
            .ExecuteNonQueryAsync(
                Arg.Is<InsertExchangeRateCriteria>(c =>
                    c.CurrencyCode == currencyCode
                    && c.ExchangeRate == exchangeRate
                    && c.EffectiveDate == effectiveDate
                    && c.ModifiedBy == modifiedBy
                )
            );
    }

    [Fact]
    public async Task UpdateExchangeRateAsync_WithValidParameters_ShouldReturnSuccess()
    {
        // Arrange
        const int currencyId = 1;
        var effectiveDate = new DateTime(2024, 1, 15, 0, 0, 0, DateTimeKind.Unspecified);
        var oldEffectiveDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Unspecified);
        const string currencyCode = "USD";
        const decimal exchangeRate = 1.05m;
        const int modifiedBy = 1;

        var expectedResult = new CommandResult { Code = 0, Message = "Success" };

        _baseService
            .ExecuteNonQueryAsync(Arg.Any<UpdateExchangeRateCriteria>())
            .Returns(expectedResult);

        // Act
        var result = await _service.UpdateExchangeRateAsync(
            currencyId,
            effectiveDate,
            oldEffectiveDate,
            currencyCode,
            exchangeRate,
            modifiedBy
        );

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccessful);
        Assert.Equal(0, result.Code);

        await _baseService
            .Received(1)
            .ExecuteNonQueryAsync(
                Arg.Is<UpdateExchangeRateCriteria>(c =>
                    c.CurrencyId == currencyId
                    && c.EffectiveDate == effectiveDate
                    && c.OldEffectiveDate == oldEffectiveDate
                    && c.CurrencyCode == currencyCode
                    && c.ExchangeRate == exchangeRate
                    && c.ModifiedBy == modifiedBy
                )
            );
    }

    [Fact]
    public async Task UpdateExchangeRateAsync_WithFailure_ShouldReturnError()
    {
        // Arrange
        const int currencyId = 1;
        var effectiveDate = new DateTime(2024, 1, 15, 0, 0, 0, DateTimeKind.Unspecified);
        var oldEffectiveDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Unspecified);
        const string currencyCode = "USD";
        const decimal exchangeRate = 1.05m;
        const int modifiedBy = 1;

        var expectedResult = new CommandResult
        {
            Code = 1,
            Message = "Failed to update exchange rate",
        };

        _baseService
            .ExecuteNonQueryAsync(Arg.Any<UpdateExchangeRateCriteria>())
            .Returns(expectedResult);

        // Act
        var result = await _service.UpdateExchangeRateAsync(
            currencyId,
            effectiveDate,
            oldEffectiveDate,
            currencyCode,
            exchangeRate,
            modifiedBy
        );

        // Assert
        Assert.NotNull(result);
        Assert.False(result.IsSuccessful);
        Assert.Equal(1, result.Code);
        Assert.Equal("Failed to update exchange rate", result.Message);

        await _baseService
            .Received(1)
            .ExecuteNonQueryAsync(
                Arg.Is<UpdateExchangeRateCriteria>(c =>
                    c.CurrencyId == currencyId
                    && c.EffectiveDate == effectiveDate
                    && c.OldEffectiveDate == oldEffectiveDate
                    && c.CurrencyCode == currencyCode
                    && c.ExchangeRate == exchangeRate
                    && c.ModifiedBy == modifiedBy
                )
            );
    }

    [Fact]
    public async Task DeleteExchangeRateAsync_WithValidParameters_ShouldReturnSuccess()
    {
        // Arrange
        const int currencyId = 1;
        var effectiveDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Unspecified);
        const int modifiedBy = 1;

        var expectedResult = new CommandResult { Code = 0, Message = "Success" };

        _baseService
            .ExecuteNonQueryAsync(Arg.Any<DeleteExchangeRateCriteria>())
            .Returns(expectedResult);

        // Act
        var result = await _service.DeleteExchangeRateAsync(currencyId, effectiveDate, modifiedBy);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccessful);
        Assert.Equal(0, result.Code);

        await _baseService
            .Received(1)
            .ExecuteNonQueryAsync(
                Arg.Is<DeleteExchangeRateCriteria>(c =>
                    c.CurrencyId == currencyId
                    && c.EffectiveDate == effectiveDate
                    && c.ModifiedBy == modifiedBy
                )
            );
    }

    [Fact]
    public async Task DeleteExchangeRateAsync_WithFailure_ShouldReturnError()
    {
        // Arrange
        const int currencyId = 1;
        var effectiveDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Unspecified);
        const int modifiedBy = 1;

        var expectedResult = new CommandResult
        {
            Code = 1,
            Message = "Failed to delete exchange rate",
        };

        _baseService
            .ExecuteNonQueryAsync(Arg.Any<DeleteExchangeRateCriteria>())
            .Returns(expectedResult);

        // Act
        var result = await _service.DeleteExchangeRateAsync(currencyId, effectiveDate, modifiedBy);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.IsSuccessful);
        Assert.Equal(1, result.Code);
        Assert.Equal("Failed to delete exchange rate", result.Message);

        await _baseService
            .Received(1)
            .ExecuteNonQueryAsync(
                Arg.Is<DeleteExchangeRateCriteria>(c =>
                    c.CurrencyId == currencyId
                    && c.EffectiveDate == effectiveDate
                    && c.ModifiedBy == modifiedBy
                )
            );
    }
}
