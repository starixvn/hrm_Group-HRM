namespace HRM.Services
{
    using HRM.Core.Models;

    public interface IHRMAuthService
    {
        EmployeeSession GetByUserId(int userId);

        int Validate(EmployeeSession employee, string password);
    }
}