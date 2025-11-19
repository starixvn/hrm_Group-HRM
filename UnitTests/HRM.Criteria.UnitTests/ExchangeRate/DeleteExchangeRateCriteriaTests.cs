using System;
using HRM.Criteria.ExchangeRate;
using Xunit;

namespace HRM.Criteria.UnitTests.ExchangeRate;

public class DeleteExchangeRateCriteriaTests
{
    [Fact]
    public void GetSettingKey_ShouldReturnCorrectSettingKey()
    {
        // Arrange
        var criteria = new DeleteExchangeRateCriteria();

        // Act
        var result = criteria.GetSettingKey();

        // Assert
        Assert.Equal("CurrencyExchangeRate_Delete", result);
    }

    [Fact]
    public void IsValid_WithValidCurrencyId_ShouldReturnTrue()
    {
        // Arrange
        var criteria = new DeleteExchangeRateCriteria
        {
            CurrencyId = 1,
            EffectiveDate = new DateTime(2024, 1, 1),
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
        var criteria = new DeleteExchangeRateCriteria
        {
            CurrencyId = 0,
            EffectiveDate = new DateTime(2024, 1, 1),
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
        var criteria = new DeleteExchangeRateCriteria
        {
            CurrencyId = -1,
            EffectiveDate = new DateTime(2024, 1, 1),
            ModifiedBy = 1,
        };

        // Act
        var result = criteria.IsValid();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsValid_WithPositiveCurrencyId_ShouldReturnTrue()
    {
        // Arrange
        var criteria = new DeleteExchangeRateCriteria
        {
            CurrencyId = 123,
            EffectiveDate = new DateTime(2024, 1, 1),
            ModifiedBy = 1,
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
        var criteria = new DeleteExchangeRateCriteria();
        const int expectedCurrencyId = 123;
        var expectedEffectiveDate = new DateTime(2024, 6, 1);
        const int expectedModifiedBy = 456;

        // Act
        criteria.CurrencyId = expectedCurrencyId;
        criteria.EffectiveDate = expectedEffectiveDate;
        criteria.ModifiedBy = expectedModifiedBy;

        // Assert
        Assert.Equal(expectedCurrencyId, criteria.CurrencyId);
        Assert.Equal(expectedEffectiveDate, criteria.EffectiveDate);
        Assert.Equal(expectedModifiedBy, criteria.ModifiedBy);
    }

    [Theory]
    [InlineData(1, true)]
    [InlineData(123, true)]
    [InlineData(999, true)]
    [InlineData(0, false)]
    [InlineData(-1, false)]
    [InlineData(-100, false)]
    public void IsValid_WithVariousCurrencyIds_ShouldReturnCorrectResult(int currencyId, bool expectedResult)
    {
        // Arrange
        var criteria = new DeleteExchangeRateCriteria
        {
            CurrencyId = currencyId,
            EffectiveDate = new DateTime(2024, 1, 1),
            ModifiedBy = 1,
        };

        // Act
        var result = criteria.IsValid();

        // Assert
        Assert.Equal(expectedResult, result);
    }

    [Fact]
    public void IsValid_WithDefaultEffectiveDate_ShouldStillReturnTrueIfCurrencyIdIsValid()
    {
        // Arrange
        var criteria = new DeleteExchangeRateCriteria
        {
            CurrencyId = 1,
            EffectiveDate = default,
            ModifiedBy = 1,
        };

        // Act
        var result = criteria.IsValid();

        // Assert
        // Note: IsValid only checks CurrencyId > 0, not EffectiveDate
        Assert.True(result);
    }

    [Fact]
    public void IsValid_WithZeroModifiedBy_ShouldStillReturnTrueIfCurrencyIdIsValid()
    {
        // Arrange
        var criteria = new DeleteExchangeRateCriteria
        {
            CurrencyId = 1,
            EffectiveDate = new DateTime(2024, 1, 1),
            ModifiedBy = 0,
        };

        // Act
        var result = criteria.IsValid();

        // Assert
        // Note: IsValid only checks CurrencyId > 0, not ModifiedBy
        Assert.True(result);
    }
}


