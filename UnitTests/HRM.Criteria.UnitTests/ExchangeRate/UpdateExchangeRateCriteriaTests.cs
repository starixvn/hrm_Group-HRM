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
            EffectiveDate = new DateTime(2024, 1, 15),
            OldEffectiveDate = new DateTime(2024, 1, 1),
            CurrencyCode = "USD",
            ExchangeRate = 1.05m,
            Rounding = 0,
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
            EffectiveDate = new DateTime(2024, 1, 15),
            OldEffectiveDate = new DateTime(2024, 1, 1),
            CurrencyCode = "USD",
            ExchangeRate = 1.05m,
            Rounding = 0,
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
            EffectiveDate = new DateTime(2024, 1, 15),
            OldEffectiveDate = new DateTime(2024, 1, 1),
            CurrencyCode = "USD",
            ExchangeRate = 1.05m,
            Rounding = 0,
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
            EffectiveDate = new DateTime(2024, 1, 15),
            OldEffectiveDate = new DateTime(2024, 1, 1),
            CurrencyCode = string.Empty,
            ExchangeRate = 1.05m,
            Rounding = 0,
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
            EffectiveDate = new DateTime(2024, 1, 15),
            OldEffectiveDate = new DateTime(2024, 1, 1),
            CurrencyCode = "   ",
            ExchangeRate = 1.05m,
            Rounding = 0,
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
            EffectiveDate = new DateTime(2024, 1, 15),
            OldEffectiveDate = new DateTime(2024, 1, 1),
            CurrencyCode = null,
            ExchangeRate = 1.05m,
            Rounding = 0,
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
            EffectiveDate = new DateTime(2024, 1, 15),
            OldEffectiveDate = new DateTime(2024, 1, 1),
            CurrencyCode = "USD",
            ExchangeRate = 0m,
            Rounding = 0,
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
            EffectiveDate = new DateTime(2024, 1, 15),
            OldEffectiveDate = new DateTime(2024, 1, 1),
            CurrencyCode = "USD",
            ExchangeRate = -1.0m,
            Rounding = 0,
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
            OldEffectiveDate = new DateTime(2024, 1, 1),
            CurrencyCode = "USD",
            ExchangeRate = 1.05m,
            Rounding = 0,
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
            EffectiveDate = new DateTime(2024, 6, 1),
            OldEffectiveDate = new DateTime(2024, 1, 1),
            CurrencyCode = "EUR",
            ExchangeRate = 1.2m,
            Rounding = 2,
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
        var expectedEffectiveDate = new DateTime(2024, 6, 1);
        var expectedOldEffectiveDate = new DateTime(2024, 1, 1);
        const string expectedCurrencyCode = "USD";
        const decimal expectedExchangeRate = 1.5m;
        const decimal expectedRounding = 2;
        const int expectedModifiedBy = 456;

        // Act
        criteria.CurrencyId = expectedCurrencyId;
        criteria.EffectiveDate = expectedEffectiveDate;
        criteria.OldEffectiveDate = expectedOldEffectiveDate;
        criteria.CurrencyCode = expectedCurrencyCode;
        criteria.ExchangeRate = expectedExchangeRate;
        criteria.Rounding = expectedRounding;
        criteria.ModifiedBy = expectedModifiedBy;

        // Assert
        Assert.Equal(expectedCurrencyId, criteria.CurrencyId);
        Assert.Equal(expectedEffectiveDate, criteria.EffectiveDate);
        Assert.Equal(expectedOldEffectiveDate, criteria.OldEffectiveDate);
        Assert.Equal(expectedCurrencyCode, criteria.CurrencyCode);
        Assert.Equal(expectedExchangeRate, criteria.ExchangeRate);
        Assert.Equal(expectedRounding, criteria.Rounding);
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
            EffectiveDate = new DateTime(2024, 1, 15),
            OldEffectiveDate = new DateTime(2024, 1, 1),
            CurrencyCode = currencyCode,
            ExchangeRate = exchangeRate,
            Rounding = 0,
            ModifiedBy = 1,
        };

        // Act
        var result = criteria.IsValid();

        // Assert
        Assert.Equal(expectedResult, result);
    }
}


