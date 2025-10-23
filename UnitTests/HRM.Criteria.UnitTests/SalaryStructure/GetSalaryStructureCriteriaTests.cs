using HRM.Criteria.SalaryStructure;
using Xunit;

namespace HRM.Criteria.UnitTests.SalaryStructure;

public class GetSalaryStructureCriteriaTests
{
    [Fact]
    public void GetSettingKey_ShouldReturnCorrectSettingKey()
    {
        // Arrange
        var criteria = new GetSalaryStructureCriteria();

        // Act
        var result = criteria.GetSettingKey();

        // Assert
        Assert.Equal("SalaryStructure_Get", result);
    }

    [Theory]
    [InlineData(1, true)]
    [InlineData(0, false)]
    [InlineData(-1, false)]
    [InlineData(999, true)]
    public void IsValid_WithVariousInputs_ShouldReturnCorrectResult(int companyId, bool expectedResult)
    {
        // Arrange
        var criteria = new GetSalaryStructureCriteria
        {
            CompanyId = companyId
        };

        // Act
        var result = criteria.IsValid();

        // Assert
        Assert.Equal(expectedResult, result);
    }

    [Fact]
    public void CompanyId_ShouldBeSettableAndGettable()
    {
        // Arrange
        var criteria = new GetSalaryStructureCriteria();
        const int expectedCompanyId = 456;

        // Act
        criteria.CompanyId = expectedCompanyId;

        // Assert
        Assert.Equal(expectedCompanyId, criteria.CompanyId);
    }
}
