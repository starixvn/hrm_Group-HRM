using HRM.Criteria.SalaryStructure;
using Xunit;

namespace HRM.Criteria.UnitTests.SalaryStructure;

public class DeleteSalaryStructureCriteriaTests
{
    [Fact]
    public void GetSettingKey_ShouldReturnCorrectSettingKey()
    {
        // Arrange
        var criteria = new DeleteSalaryStructureCriteria();

        // Act
        var result = criteria.GetSettingKey();

        // Assert
        Assert.Equal("SalaryStructure_Delete", result);
    }

    [Theory]
    [InlineData(1, 1, true)]
    [InlineData(0, 1, false)]
    [InlineData(1, 0, false)]
    [InlineData(0, 0, false)]
    [InlineData(-1, 1, false)]
    [InlineData(1, -1, false)]
    [InlineData(-1, -1, false)]
    public void IsValid_WithVariousInputs_ShouldReturnCorrectResult(int salaryStructureId, int deletedBy, bool expectedResult)
    {
        // Arrange
        var criteria = new DeleteSalaryStructureCriteria
        {
            SalaryStructureId = salaryStructureId,
            DeletedBy = deletedBy
        };

        // Act
        var result = criteria.IsValid();

        // Assert
        Assert.Equal(expectedResult, result);
    }

    [Fact]
    public void Properties_ShouldBeSettableAndGettable()
    {
        // Arrange
        var criteria = new DeleteSalaryStructureCriteria();
        const int expectedSalaryStructureId = 123;
        const int expectedDeletedBy = 456;

        // Act
        criteria.SalaryStructureId = expectedSalaryStructureId;
        criteria.DeletedBy = expectedDeletedBy;

        // Assert
        Assert.Equal(expectedSalaryStructureId, criteria.SalaryStructureId);
        Assert.Equal(expectedDeletedBy, criteria.DeletedBy);
    }
}
