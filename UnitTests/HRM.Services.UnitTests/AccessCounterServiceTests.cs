using System.Threading.Tasks;
using GO.UserControl.Client.Core.Session;
using HRM.Core;
using HRM.Core.Constants;
using HRM.Core.Services;
using HRM.Criteria.AccessCounter;
using HRM.Services.Implement;
using NSubstitute;
using Xunit;

namespace HRM.Services.UnitTests;

public class AccessCounterServiceTests
{
    private IUserControlSession session;
    private IBaseService baseService;

    [Fact]
    public async Task UpdateCounterAsync_ShouldCallBaseServiceWithCorrectCommand()
    {
        // Arrange
        var service = GetAccessCounterService();
        const string functionName = "TestFunction";

        // Act
        await service.UpdateCounterAsync(functionName);

        // Assert
        await baseService!.Received(1).ExecuteNonQueryAsync(Arg.Is<UpdateAccessCounterCommand>(command =>
            command.EmpId == session.UserId &&
            command.FunctionName == functionName));
    }

    #region Helpers

    private static IUserControlSession FakeSession()
    {
        var session = Substitute.For<IUserControlSession>();
        session.UserId.Returns(1);
        session.Get<float>(SessionKey.CurrentTimeZone).Returns(7);

        return session;
    }

    private AccessCounterService GetAccessCounterService()
    {
        session = FakeSession();
        baseService = Substitute.For<IBaseService>();

        DateTimeExtensions.Configure(session);

        var service = new AccessCounterService(session, baseService);

        return service;
    }

    #endregion Helpers
}