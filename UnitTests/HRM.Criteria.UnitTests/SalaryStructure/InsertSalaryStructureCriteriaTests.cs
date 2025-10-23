using HRM.Criteria.SalaryStructure;
using Xunit;

namespace HRM.Criteria.UnitTests.SalaryStructure;

public class InsertSalaryStructureCriteriaTests
{
    [Fact]
    public void GetSettingKey_ShouldReturnCorrectSettingKey()
    {
        // Arrange
        var criteria = new InsertSalaryStructureCriteria();

        // Act
        var result = criteria.GetSettingKey();

        // Assert
        Assert.Equal("SalaryStructure_Insert", result);
    }

    [Theory]
    [InlineData(1, 1, "Valid Name", true)]
    [InlineData(0, 1, "Valid Name", false)]
    [InlineData(1, 0, "Valid Name", false)]
    [InlineData(1, 1, "", false)]
    [InlineData(1, 1, "   ", false)]
    [InlineData(1, 1, null, false)]
    [InlineData(0, 0, "", false)]
    [InlineData(-1, 1, "Valid Name", false)]
    [InlineData(1, -1, "Valid Name", false)]
    [InlineData(999, 888, "Test Structure", true)]
    public void IsValid_WithVariousInputs_ShouldReturnCorrectResult(int companyId, int createdBy, string name, bool expectedResult)
    {
        // Arrange
        var criteria = new InsertSalaryStructureCriteria
        {
            CompanyId = companyId,
            CreatedBy = createdBy,
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
        var criteria = new InsertSalaryStructureCriteria();
        const int expectedCompanyId = 123;
        const int expectedCreatedBy = 456;
        const string expectedName = "Test Structure";
        const string expectedDetails = "Test Details";

        // Act
        criteria.CompanyId = expectedCompanyId;
        criteria.CreatedBy = expectedCreatedBy;
        criteria.Name = expectedName;
        criteria.Details = expectedDetails;

        // Assert
        Assert.Equal(expectedCompanyId, criteria.CompanyId);
        Assert.Equal(expectedCreatedBy, criteria.CreatedBy);
        Assert.Equal(expectedName, criteria.Name);
        Assert.Equal(expectedDetails, criteria.Details);
    }

    [Fact]
    public void IsValid_WithValidData_ShouldReturnTrue()
    {
        // Arrange
        var criteria = new InsertSalaryStructureCriteria
        {
            CompanyId = 1,
            CreatedBy = 1,
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
        var criteria = new InsertSalaryStructureCriteria
        {
            CompanyId = 1,
            CreatedBy = 1,
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
        var criteria = new InsertSalaryStructureCriteria
        {
            CompanyId = 1,
            CreatedBy = 1,
            Name = "   ",
            Details = "Some details"
        };

        // Act
        var result = criteria.IsValid();

        // Assert
        Assert.False(result);
    }
}
