using HRM.Criteria.SalaryStructure;
using Xunit;

namespace HRM.Criteria.UnitTests.SalaryStructure;

public class UpdateSalaryStructureCriteriaTests
{
    [Fact]
    public void GetSettingKey_ShouldReturnCorrectSettingKey()
    {
        // Arrange
        var criteria = new UpdateSalaryStructureCriteria();

        // Act
        var result = criteria.GetSettingKey();

        // Assert
        Assert.Equal("SalaryStructure_Update", result);
    }

    [Theory]
    [InlineData(1, 1, 1, "Valid Name", true)]
    [InlineData(0, 1, 1, "Valid Name", false)]
    [InlineData(1, 0, 1, "Valid Name", false)]
    [InlineData(1, 1, 0, "Valid Name", false)]
    [InlineData(1, 1, 1, "", false)]
    [InlineData(1, 1, 1, "   ", false)]
    [InlineData(1, 1, 1, null, false)]
    [InlineData(0, 0, 0, "", false)]
    [InlineData(-1, 1, 1, "Valid Name", false)]
    [InlineData(1, -1, 1, "Valid Name", false)]
    [InlineData(1, 1, -1, "Valid Name", false)]
    [InlineData(999, 888, 777, "Test Structure", true)]
    public void IsValid_WithVariousInputs_ShouldReturnCorrectResult(int salaryStructureId, int companyId, int modifiedBy, string name, bool expectedResult)
    {
        // Arrange
        var criteria = new UpdateSalaryStructureCriteria
        {
            SalaryStructureId = salaryStructureId,
            CompanyId = companyId,
            ModifiedBy = modifiedBy,
            Name = name
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
        var criteria = new UpdateSalaryStructureCriteria();
        const int expectedSalaryStructureId = 123;
        const int expectedCompanyId = 456;
        const int expectedModifiedBy = 789;
        const string expectedName = "Updated Structure";
        const string expectedDetails = "Updated Details";

        // Act
        criteria.SalaryStructureId = expectedSalaryStructureId;
        criteria.CompanyId = expectedCompanyId;
        criteria.ModifiedBy = expectedModifiedBy;
        criteria.Name = expectedName;
        criteria.Details = expectedDetails;

        // Assert
        Assert.Equal(expectedSalaryStructureId, criteria.SalaryStructureId);
        Assert.Equal(expectedCompanyId, criteria.CompanyId);
        Assert.Equal(expectedModifiedBy, criteria.ModifiedBy);
        Assert.Equal(expectedName, criteria.Name);
        Assert.Equal(expectedDetails, criteria.Details);
    }

    [Fact]
    public void IsValid_WithValidData_ShouldReturnTrue()
    {
        // Arrange
        var criteria = new UpdateSalaryStructureCriteria
        {
            SalaryStructureId = 1,
            CompanyId = 1,
            ModifiedBy = 1,
            Name = "Valid Structure Name",
            Details = "Some details"
        };

        // Act
        var result = criteria.IsValid();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsValid_WithEmptyName_ShouldReturnFalse()
    {
        // Arrange
        var criteria = new UpdateSalaryStructureCriteria
        {
            SalaryStructureId = 1,
            CompanyId = 1,
            ModifiedBy = 1,
            Name = "",
            Details = "Some details"
        };

        // Act
        var result = criteria.IsValid();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsValid_WithWhitespaceName_ShouldReturnFalse()
    {
        // Arrange
        var criteria = new UpdateSalaryStructureCriteria
        {
            SalaryStructureId = 1,
            CompanyId = 1,
            ModifiedBy = 1,
            Name = "   ",
            Details = "Some details"
        };

        // Act
        var result = criteria.IsValid();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsValid_WithZeroSalaryStructureId_ShouldReturnFalse()
    {
        // Arrange
        var criteria = new UpdateSalaryStructureCriteria
        {
            SalaryStructureId = 0,
            CompanyId = 1,
            ModifiedBy = 1,
            Name = "Valid Name",
            Details = "Some details"
        };

        // Act
        var result = criteria.IsValid();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsValid_WithZeroCompanyId_ShouldReturnFalse()
    {
        // Arrange
        var criteria = new UpdateSalaryStructureCriteria
        {
            SalaryStructureId = 1,
            CompanyId = 0,
            ModifiedBy = 1,
            Name = "Valid Name",
            Details = "Some details"
        };

        // Act
        var result = criteria.IsValid();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsValid_WithZeroModifiedBy_ShouldReturnFalse()
    {
        // Arrange
        var criteria = new UpdateSalaryStructureCriteria
        {
            SalaryStructureId = 1,
            CompanyId = 1,
            ModifiedBy = 0,
            Name = "Valid Name",
            Details = "Some details"
        };

        // Act
        var result = criteria.IsValid();

        // Assert
        Assert.False(result);
    }
}
