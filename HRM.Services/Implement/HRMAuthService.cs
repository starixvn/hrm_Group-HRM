namespace HRM.Services
{
    using HRM.Core;
    using HRM.Core.Constants;
    using HRM.Core.Enums;
    using HRM.Core.Models;
    using HRM.Core.Security;
    using HRM.Core.Services;
    using HRM.Criteria.Auth;

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