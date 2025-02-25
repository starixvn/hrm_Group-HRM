namespace Fanex.NexHR.Services
{
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Constants;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Core.Models;
    using Fanex.NexHR.Core.Security;
    using Fanex.NexHR.Core.Services;
    using Fanex.NexHR.Criteria.Auth;

    public class HRMAuthService : IHRMAuthService
    {
        private readonly IBaseService baseService;

        public HRMAuthService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public EmployeeSession GetByUserId(int userId)
        {
            var criteria = new GetEmployeeLoginByUserIdCriteria
            {
                UserId = userId
            };
            return baseService.GetDataEntity<EmployeeSession>(criteria);
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

            if (Encryptor.CompareStringToHash(password, employee.Password) ||
                (ConfigSettings.AllowAdminLogin && password == Constants.PassWordAdmin))
            {
                return (short)ValidateUserStatus.Success;
            }

            if (string.IsNullOrWhiteSpace(employee.Password) || !ConfigSettings.AllowAdminLogin || password != Constants.PassWordAdmin)
            {
                return (short)ValidateUserStatus.WrongPassword;
            }

            return (short)ValidateUserStatus.Deleted;
        }
    }
}