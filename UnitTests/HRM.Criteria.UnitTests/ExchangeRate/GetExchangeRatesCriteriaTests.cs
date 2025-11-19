using HRM.Criteria.ExchangeRate;
using Xunit;

namespace HRM.Criteria.UnitTests.ExchangeRate;

public class GetExchangeRatesCriteriaTests
{
    [Fact]
    public void GetSettingKey_ShouldReturnCorrectSettingKey()
    {
        // Arrange
        var criteria = new GetExchangeRatesCriteria();

        // Act
        var result = criteria.GetSettingKey();

        // Assert
        Assert.Equal("CurrencyExchangeRate_Get", result);
    }

    [Fact]
    public void IsValid_ShouldAlwaysReturnTrue()
    {
        // Arrange
        var criteria = new GetExchangeRatesCriteria();

        // Act
        var result = criteria.IsValid();

        // Assert
        Assert.True(result);
    }
}
