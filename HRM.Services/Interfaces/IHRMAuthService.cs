namespace HRM.Services
{
    using System.Threading.Tasks;
    using HRM.Core.Models;

    public interface IHRMAuthService
    {
        EmployeeSession GetByUserId(int userId);

        Task<EmployeeSession> GetByUserIdAsync(int userId);

        int Validate(EmployeeSession employee, string password);
    }
}