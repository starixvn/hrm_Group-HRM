namespace Fanex.NexHR.Services
{
    using Fanex.NexHR.Core.Models;

    public interface IHRMAuthService
    {
        EmployeeSession GetByUserId(int userId);

        int Validate(EmployeeSession employee, string password);
    }
}