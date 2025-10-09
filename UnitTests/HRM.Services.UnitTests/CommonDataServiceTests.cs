using System.Collections.Generic;
using System.Threading.Tasks;
using GO.UserControl.Client.Core.Session;
using HRM.Core.Services;
using HRM.Criteria.Lookup;
using HRM.Models;
using HRM.Services.Implement;
using HRM.Services.Interfaces;
using NSubstitute;
using Xunit;

namespace HRM.Services.UnitTests;

public class CommonDataServiceTests
{
    private IUserControlSession session;
    private IBaseService baseService;
    private ITeamService teamService;

    [Fact]
    public async Task GetLookupDataByCodeAsync_ShouldReturnLookupData()
    {
        // Arrange
        const string code = "TestCode";
        const int companyId = 1;
        var expectedData = new List<LookupData>
        {
            new() { Id = 1, Code = "TestCode1", Value = "Value1" },
            new() { Id = 2, Code = "TestCode2", Value = "Value2" }
        };
        var service = GetService();
        baseService.GetDataAsync<LookupData>(Arg.Any<GetAllLookupDataCriteria>())
            .Returns(expectedData);

        // Act
        var result = await service.GetLookupDataByCodeAsync(code, companyId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedData, result);
    }

    [Fact]
    public async Task GetJobTitleAsync_ShouldReturnJobTitles()
    {
        // Arrange
        const string jobTitle = "Developer";
        const int companyId = 1;
        var expectedData = new List<LookupData>
        {
            new() { Id = 1, Code = "Dev1", Value = "Developer 1" },
            new() { Id = 2, Code = "Dev2", Value = "Developer 2" }
        };
        var service = GetService();
        baseService.GetDataAsync<LookupData>(Arg.Any<GetJobTitleCriteria>())
            .Returns(expectedData);

        // Act
        var result = await service.GetJobTitleAsync(jobTitle, companyId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedData, result);
    }

    #region Helpers

    private CommonDataService GetService()
    {
        session = Substitute.For<IUserControlSession>();
        baseService = Substitute.For<IBaseService>();
        teamService = Substitute.For<ITeamService>();

        return new CommonDataService(session, baseService, teamService);
    }

    #endregion Helpers
}