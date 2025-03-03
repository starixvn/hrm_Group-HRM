using System.Threading.Tasks;

namespace HRM.Services
{
    using HRM.Core.Enums;
    using HRM.Core.Models;
    using HRM.Core.Security;
    using HRM.Core.Services;
    using HRM.Criteria.Auth;

    public class HRMAuthService(IBaseService baseService) : IHRMAuthService
    {
        public EmployeeSession GetByUserId(int userId)
        {
            var criteria = new GetEmployeeLoginByUserIdCriteria
            {
                UserId = userId
            };
            return baseService.GetDataEntity<EmployeeSession>(criteria);
        }
        public async Task<EmployeeSession> GetByUserIdAsync(int userId)
        {
            var criteria = new GetEmployeeLoginByUserIdCriteria
            {
                UserId = userId
            };
            return await baseService.GetDataEntityAsync<EmployeeSession>(criteria);
        }

        public int Validate(EmployeeSession employee, string password)
        {
            if (employee == null)
            {
                return (short)ValidateUserStatus.WrongUsername;
            }

            if (!employee.Enabled)
            {
                return (short)ValidateUserStatus.Disabled;
            }

            if (Encryptor.CompareStringToHash(password, employee.Password))
            {
                return (short)ValidateUserStatus.Success;
            }

            if (string.IsNullOrWhiteSpace(employee.Password))
            {
                return (short)ValidateUserStatus.WrongPassword;
            }

            return (short)ValidateUserStatus.Deleted;
        }
    }
}