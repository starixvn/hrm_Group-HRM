using System;
using HRM.Criteria.ExchangeRate;
using Xunit;

namespace HRM.Criteria.UnitTests.ExchangeRate;

public class InsertExchangeRateCriteriaTests
{
    [Fact]
    public void GetSettingKey_ShouldReturnCorrectSettingKey()
    {
        // Arrange
        var criteria = new InsertExchangeRateCriteria();

        // Act
        var result = criteria.GetSettingKey();

        // Assert
        Assert.Equal("CurrencyExchangeRate_Insert", result);
    }

    [Fact]
    public void IsValid_WithValidData_ShouldReturnTrue()
    {
        // Arrange
        var criteria = new InsertExchangeRateCriteria
        {
            CurrencyCode = "USD",
            ExchangeRate = 1.0m,
            EffectiveDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Unspecified),
            ModifiedBy = 1,
        };

        // Act
        var result = criteria.IsValid();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsValid_WithEmptyCurrencyCode_ShouldReturnFalse()
    {
        // Arrange
        var criteria = new InsertExchangeRateCriteria
        {
            CurrencyCode = string.Empty,
            ExchangeRate = 1.0m,
            EffectiveDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Unspecified),
            ModifiedBy = 1,
        };

        // Act
        var result = criteria.IsValid();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsValid_WithWhitespaceCurrencyCode_ShouldReturnFalse()
    {
        // Arrange
        var criteria = new InsertExchangeRateCriteria
        {
            CurrencyCode = "   ",
            ExchangeRate = 1.0m,
            EffectiveDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Unspecified),
            ModifiedBy = 1,
        };

        // Act
        var result = criteria.IsValid();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsValid_WithNullCurrencyCode_ShouldReturnFalse()
    {
        // Arrange
        var criteria = new InsertExchangeRateCriteria
        {
            CurrencyCode = null,
            ExchangeRate = 1.0m,
            EffectiveDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Unspecified),
            ModifiedBy = 1,
        };

        // Act
        var result = criteria.IsValid();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsValid_WithZeroExchangeRate_ShouldReturnFalse()
    {
        // Arrange
        var criteria = new InsertExchangeRateCriteria
        {
            CurrencyCode = "USD",
            ExchangeRate = 0m,
            EffectiveDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Unspecified),
            ModifiedBy = 1,
        };

        // Act
        var result = criteria.IsValid();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsValid_WithNegativeExchangeRate_ShouldReturnFalse()
    {
        // Arrange
        var criteria = new InsertExchangeRateCriteria
        {
            CurrencyCode = "USD",
            ExchangeRate = -1.0m,
            EffectiveDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Unspecified),
            ModifiedBy = 1,
        };

        // Act
        var result = criteria.IsValid();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsValid_WithDefaultEffectiveDate_ShouldReturnFalse()
    {
        // Arrange
        var criteria = new InsertExchangeRateCriteria
        {
            CurrencyCode = "USD",
            ExchangeRate = 1.0m,
            EffectiveDate = default,
            ModifiedBy = 1,
        };

        // Act
        var result = criteria.IsValid();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsValid_WithValidDataAndPositiveExchangeRate_ShouldReturnTrue()
    {
        // Arrange
        var criteria = new InsertExchangeRateCriteria
        {
            CurrencyCode = "EUR",
            ExchangeRate = 1.1m,
            EffectiveDate = new DateTime(2024, 1, 15, 0, 0, 0, DateTimeKind.Unspecified),
            ModifiedBy = 123,
        };

        // Act
        var result = criteria.IsValid();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Properties_ShouldBeSettableAndGettable()
    {
        // Arrange
        var criteria = new InsertExchangeRateCriteria();
        const string expectedCurrencyCode = "USD";
        const decimal expectedExchangeRate = 1.5m;
        var expectedEffectiveDate = new DateTime(2024, 6, 1, 0, 0, 0, DateTimeKind.Unspecified);
        const int expectedModifiedBy = 456;

        // Act
        criteria.CurrencyCode = expectedCurrencyCode;
        criteria.ExchangeRate = expectedExchangeRate;
        criteria.EffectiveDate = expectedEffectiveDate;
        criteria.ModifiedBy = expectedModifiedBy;

        // Assert
        Assert.Equal(expectedCurrencyCode, criteria.CurrencyCode);
        Assert.Equal(expectedExchangeRate, criteria.ExchangeRate);
        Assert.Equal(expectedEffectiveDate, criteria.EffectiveDate);
        Assert.Equal(expectedModifiedBy, criteria.ModifiedBy);
    }

    [Theory]
    [InlineData("USD", 1.0, true)]
    [InlineData("EUR", 1.1, true)]
    [InlineData("", 1.0, false)]
    [InlineData("   ", 1.0, false)]
    [InlineData(null, 1.0, false)]
    [InlineData("USD", 0, false)]
    [InlineData("USD", -1.0, false)]
    [InlineData("USD", 0.0001, true)]
    public void IsValid_WithVariousInputs_ShouldReturnCorrectResult(
        string currencyCode,
        decimal exchangeRate,
        bool expectedResult)
    {
        // Arrange
        var criteria = new InsertExchangeRateCriteria
        {
            CurrencyCode = currencyCode,
            ExchangeRate = exchangeRate,
            EffectiveDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Unspecified),
            ModifiedBy = 1,
        };

        // Act
        var result = criteria.IsValid();

        // Assert
        Assert.Equal(expectedResult, result);
    }
}


