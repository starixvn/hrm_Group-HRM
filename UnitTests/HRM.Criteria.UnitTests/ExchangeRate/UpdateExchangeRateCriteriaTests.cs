using System;
using HRM.Criteria.ExchangeRate;
using Xunit;

namespace HRM.Criteria.UnitTests.ExchangeRate;

public class UpdateExchangeRateCriteriaTests
{
    [Fact]
    public void GetSettingKey_ShouldReturnCorrectSettingKey()
    {
        // Arrange
        var criteria = new UpdateExchangeRateCriteria();

        // Act
        var result = criteria.GetSettingKey();

        // Assert
        Assert.Equal("CurrencyExchangeRate_Update", result);
    }

    [Fact]
    public void IsValid_WithValidData_ShouldReturnTrue()
    {
        // Arrange
        var criteria = new UpdateExchangeRateCriteria
        {
            CurrencyId = 1,
            EffectiveDate = new DateTime(2024, 1, 15, 0, 0, 0, DateTimeKind.Unspecified),
            OldEffectiveDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Unspecified),
            CurrencyCode = "USD",
            ExchangeRate = 1.05m,
            ModifiedBy = 1,
        };

        // Act
        var result = criteria.IsValid();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsValid_WithZeroCurrencyId_ShouldReturnFalse()
    {
        // Arrange
        var criteria = new UpdateExchangeRateCriteria
        {
            CurrencyId = 0,
            EffectiveDate = new DateTime(2024, 1, 15, 0, 0, 0, DateTimeKind.Unspecified),
            OldEffectiveDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Unspecified),
            CurrencyCode = "USD",
            ExchangeRate = 1.05m,
            ModifiedBy = 1,
        };

        // Act
        var result = criteria.IsValid();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsValid_WithNegativeCurrencyId_ShouldReturnFalse()
    {
        // Arrange
        var criteria = new UpdateExchangeRateCriteria
        {
            CurrencyId = -1,
            EffectiveDate = new DateTime(2024, 1, 15, 0, 0, 0, DateTimeKind.Unspecified),
            OldEffectiveDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Unspecified),
            CurrencyCode = "USD",
            ExchangeRate = 1.05m,
            ModifiedBy = 1,
        };

        // Act
        var result = criteria.IsValid();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsValid_WithEmptyCurrencyCode_ShouldReturnFalse()
    {
        // Arrange
        var criteria = new UpdateExchangeRateCriteria
        {
            CurrencyId = 1,
            EffectiveDate = new DateTime(2024, 1, 15, 0, 0, 0, DateTimeKind.Unspecified),
            OldEffectiveDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Unspecified),
            CurrencyCode = string.Empty,
            ExchangeRate = 1.05m,
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
        var criteria = new UpdateExchangeRateCriteria
        {
            CurrencyId = 1,
            EffectiveDate = new DateTime(2024, 1, 15, 0, 0, 0, DateTimeKind.Unspecified),
            OldEffectiveDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Unspecified),
            CurrencyCode = "   ",
            ExchangeRate = 1.05m,
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
        var criteria = new UpdateExchangeRateCriteria
        {
            CurrencyId = 1,
            EffectiveDate = new DateTime(2024, 1, 15, 0, 0, 0, DateTimeKind.Unspecified),
            OldEffectiveDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Unspecified),
            CurrencyCode = null,
            ExchangeRate = 1.05m,
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
        var criteria = new UpdateExchangeRateCriteria
        {
            CurrencyId = 1,
            EffectiveDate = new DateTime(2024, 1, 15, 0, 0, 0, DateTimeKind.Unspecified),
            OldEffectiveDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Unspecified),
            CurrencyCode = "USD",
            ExchangeRate = 0m,
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
        var criteria = new UpdateExchangeRateCriteria
        {
            CurrencyId = 1,
            EffectiveDate = new DateTime(2024, 1, 15, 0, 0, 0, DateTimeKind.Unspecified),
            OldEffectiveDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Unspecified),
            CurrencyCode = "USD",
            ExchangeRate = -1.0m,
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
        var criteria = new UpdateExchangeRateCriteria
        {
            CurrencyId = 1,
            EffectiveDate = default,
            OldEffectiveDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Unspecified),
            CurrencyCode = "USD",
            ExchangeRate = 1.05m,
            ModifiedBy = 1,
        };

        // Act
        var result = criteria.IsValid();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsValid_WithValidDataAndAllFields_ShouldReturnTrue()
    {
        // Arrange
        var criteria = new UpdateExchangeRateCriteria
        {
            CurrencyId = 123,
            EffectiveDate = new DateTime(2024, 6, 1, 0, 0, 0, DateTimeKind.Unspecified),
            OldEffectiveDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Unspecified),
            CurrencyCode = "EUR",
            ExchangeRate = 1.2m,
            ModifiedBy = 456,
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
        var criteria = new UpdateExchangeRateCriteria();
        const int expectedCurrencyId = 123;
        var expectedEffectiveDate = new DateTime(2024, 6, 1, 0, 0, 0, DateTimeKind.Unspecified);
        var expectedOldEffectiveDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Unspecified);
        const string expectedCurrencyCode = "USD";
        const decimal expectedExchangeRate = 1.5m;
        const int expectedModifiedBy = 456;

        // Act
        criteria.CurrencyId = expectedCurrencyId;
        criteria.EffectiveDate = expectedEffectiveDate;
        criteria.OldEffectiveDate = expectedOldEffectiveDate;
        criteria.CurrencyCode = expectedCurrencyCode;
        criteria.ExchangeRate = expectedExchangeRate;
        criteria.ModifiedBy = expectedModifiedBy;

        // Assert
        Assert.Equal(expectedCurrencyId, criteria.CurrencyId);
        Assert.Equal(expectedEffectiveDate, criteria.EffectiveDate);
        Assert.Equal(expectedOldEffectiveDate, criteria.OldEffectiveDate);
        Assert.Equal(expectedCurrencyCode, criteria.CurrencyCode);
        Assert.Equal(expectedExchangeRate, criteria.ExchangeRate);
        Assert.Equal(expectedModifiedBy, criteria.ModifiedBy);
    }

    [Theory]
    [InlineData(1, "USD", 1.0, true)]
    [InlineData(123, "EUR", 1.1, true)]
    [InlineData(0, "USD", 1.0, false)]
    [InlineData(-1, "USD", 1.0, false)]
    [InlineData(1, "", 1.0, false)]
    [InlineData(1, "   ", 1.0, false)]
    [InlineData(1, null, 1.0, false)]
    [InlineData(1, "USD", 0, false)]
    [InlineData(1, "USD", -1.0, false)]
    [InlineData(1, "USD", 0.0001, true)]
    public void IsValid_WithVariousInputs_ShouldReturnCorrectResult(
        int currencyId,
        string currencyCode,
        decimal exchangeRate,
        bool expectedResult)
    {
        // Arrange
        var criteria = new UpdateExchangeRateCriteria
        {
            CurrencyId = currencyId,
            EffectiveDate = new DateTime(2024, 1, 15, 0, 0, 0, DateTimeKind.Unspecified),
            OldEffectiveDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Unspecified),
            CurrencyCode = currencyCode,
            ExchangeRate = exchangeRate,
            ModifiedBy = 1,
        };

        // Act
        var result = criteria.IsValid();

        // Assert
        Assert.Equal(expectedResult, result);
    }
}


