using HRM.Criteria.AccessCounter;
using Xunit;

namespace HRM.Criteria.UnitTests.AccessCounter;

public class UpdateAccessCounterCommandTests
{
    [Fact]
    public void GetSettingKey_ReturnCorrectSettingKey()
    {
        // Arrange
        var command = new UpdateAccessCounterCommand();

        // Act
        var result = command.GetSettingKey();

        // Assert
        Assert.Equal("AccessCounter_Update", result);
    }

    [Theory]
    [InlineData(1, true)]
    [InlineData(0, false)]
    [InlineData(-1, false)]
    public void IsValid_BasedOnEmpId_ReturnCorrectResult(int empId, bool expectedResult)
    {
        // Arrange
        var command = new UpdateAccessCounterCommand
        {
            EmpId = empId
        };

        // Act
        var result = command.IsValid();

        // Assert
        Assert.Equal(expectedResult, result);
    }
}