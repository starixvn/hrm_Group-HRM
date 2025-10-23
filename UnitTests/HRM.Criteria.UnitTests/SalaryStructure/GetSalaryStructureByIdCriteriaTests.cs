using HRM.Criteria.SalaryStructure;
using Xunit;

namespace HRM.Criteria.UnitTests.SalaryStructure;

public class GetSalaryStructureByIdCriteriaTests
{
    [Fact]
    public void GetSettingKey_ShouldReturnCorrectSettingKey()
    {
        // Arrange
        var criteria = new GetSalaryStructureByIdCriteria();

        // Act
        var result = criteria.GetSettingKey();

        // Assert
        Assert.Equal("SalaryStructure_GetById", result);
    }

    [Theory]
    [InlineData(1, true)]
    [InlineData(0, false)]
    [InlineData(-1, false)]
    [InlineData(999, true)]
    public void IsValid_WithVariousInputs_ShouldReturnCorrectResult(int salaryStructureId, bool expectedResult)
    {
        // Arrange
        var criteria = new GetSalaryStructureByIdCriteria
        {
            SalaryStructureId = salaryStructureId
        };

        // Act
        var result = criteria.IsValid();

        // Assert
        Assert.Equal(expectedResult, result);
    }

    [Fact]
    public void SalaryStructureId_ShouldBeSettableAndGettable()
    {
        // Arrange
        var criteria = new GetSalaryStructureByIdCriteria();
        const int expectedSalaryStructureId = 789;

        // Act
        criteria.SalaryStructureId = expectedSalaryStructureId;

        // Assert
        Assert.Equal(expectedSalaryStructureId, criteria.SalaryStructureId);
    }
}
