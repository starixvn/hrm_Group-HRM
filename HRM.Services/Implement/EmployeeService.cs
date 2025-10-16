namespace HRM.Services.Implement;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core;
using Core.Constants;
using Core.Security;
using Core.XMLExtensions;
using Criteria.AnnualLeave;
using Criteria.Employee;
using Criteria.Shift;
using Criteria.Supervisor;
using GO.UserControl.Client.Core.Enumerables;
using GO.UserControl.Client.Core.Services;
using GO.UserControl.Client.Core.Session;
using HRM.Core.Models;
using HRM.Core.Services;
using HRM.Criteria.Report;
using HRM.Models.Employees;
using HRM.Repositories.Interfaces;
using Interfaces;
using Models;
using Models.EmployeeInformation;
using Models.Group;
using Models.Supervisor;
using Resources;
using ViewModels.AnnualLeave;

public class EmployeeService(
    IEmployeeRepository employeeRepository,
    IBaseService baseService,
    IRoleService roleService,
    IUserControlSession session) : IEmployeeService
{
    public async Task<string> GenerateEmployeeCode()
    {
        var companyId = session.Get<short>(SessionKey.CurrentCompanyId);
        var criteria = new GenerateEmployeeCodeCriteria
        {
            CompanyId = companyId
        };
        return await baseService.GetDataEntityAsync<string>(criteria);
    }

    public async Task<Employee> GetEmployeeByIdAsync(int id)
    {
        var companyId = session.Get<short>(SessionKey.CurrentCompanyId);
        var criteria = new GetEmployeeByIdCriteria
        {
            CompanyId = companyId,
            EmpId = id
        };
        var employee = await employeeRepository.GetEmployeeByIdAsync(criteria);
        if (employee != null)
        {
            employee.Roles = await roleService.GetByUserIdAsync(employee.UserId);
        }

        return employee;
    }

    public async Task<Employee> GetEmployeeByIdInGroupScopeAsync(int id)
    {
        var criteria = new GetEmployeeByIdCriteria
        {
            EmpId = id
        };
        var employee = await employeeRepository.GetEmployeeByIdAsync(criteria);
        if (employee != null)
        {
            employee.Roles = await roleService.GetByUserIdAsync(employee.UserId);
        }

        return employee;
    }

    public async Task<IEnumerable<Employee>> GetEmployeeAsync(GetAllEmployeeCriteria criteria)
    {
        IEnumerable<Employee> employees = null;
        if (criteria != null && criteria.IsValid())
        {
            employees = await employeeRepository.GetEmployeeAsync(criteria);

            var roles = await roleService.GetBySiteIdAsync(session.SiteId);
            foreach (var employee in employees)
            {
                if (employee.Roles != null && employee.Roles.Any())
                {
                    employee.Roles = roles.Where(x => employee.Roles.Any(y => y.RoleId == x.RoleId)).ToList();
                }
            }
        }

        return employees ?? new List<Employee>();
    }

    public async Task<IEnumerable<Employee>> GetMemberByManagerAsync(GetMemberByManagerCriteria criteria)
    {
        return await baseService.GetDataAsync<Employee>(criteria);
    }

    public async Task<IEnumerable<EmployeePIC>> GetEmployeeForPICAsync(GetEmployeeForPICCriteria criteria)
    {
        return await baseService.GetDataAsync<EmployeePIC>(criteria);
    }

    public async Task<IEnumerable<Employee>> GetEmployeesByRoleAsync(GetEmployeeByRoleCriteria criteria)
    {
        if (criteria == null || !criteria.IsValid())
        {
            return new List<Employee>();
        }

        var employees = await employeeRepository.GetEmployeesByRoleAsync(criteria);
        var roles = await roleService.GetBySiteIdAsync(session.SiteId);
        foreach (var employee in employees)
        {
            if (employee.Roles != null && employee.Roles.Any())
            {
                employee.Roles = roles.Where(x => employee.Roles.Any(y => y.RoleId == x.RoleId)).ToList();
            }
        }

        return employees;
    }

    public async Task<EmployeeShift> GetShiftByEmployeeId(int id)
    {
        var criteria = new GetShiftByEmployeeIdCriteria
        {
            EmpId = id
        };
        return await baseService.GetDataEntityAsync<EmployeeShift>(criteria);
    }

    public async Task<IEnumerable<AnnualLeaveTransactionViewModel>> GetAnnualLeaveTransactionAsync(GetAnnualLeaveTransactionCriteria criteria)
    {
        return await baseService.GetDataAsync<AnnualLeaveTransactionViewModel>(criteria);
    }

    public async Task<AnnualLeaveTransactionViewModel> GetAnnualLeaveTransactionAsync(int leaveTransId)
    {
        var criteria = new GetAnnualLeaveTransactionByIdCriteria
        {
            LeaveTransId = leaveTransId
        };
        return await baseService.GetDataEntityAsync<AnnualLeaveTransactionViewModel>(criteria);
    }

    public async Task<IEnumerable<SupervisorModel>> GetEmployeeSupervisorByRoleIdAsync(int employeeId, short roleId)
    {
        var companyId = session.Get<short>(SessionKey.CurrentCompanyId);
        var criteria = new GetEmployeeSupervisorCriteria
        {
            EmpId = employeeId,
            Role = roleId,
            CompanyId = companyId
        };
        return await baseService.GetDataAsync<SupervisorModel>(criteria);
    }

    public async Task<IEnumerable<SupervisorNameModel>> GetListSupervisorNameAsync(GetListSupervisorNameCriteria criteria)
    {
        return await baseService.GetDataAsync<SupervisorNameModel>(criteria);
    }

    public async Task<IEnumerable<SupervisorNameModel>> GetSupervisorByRequestTypeAsync(GetSupervisorByRequestTypeCriteria criteria)
    {
        return await baseService.GetDataAsync<SupervisorNameModel>(criteria);
    }

    public async Task<IEnumerable<SupervisorNameModel>> GetSupervisorByQueryAsync(GetEmployeeSupervisorByQueryCriteria criteria)
    {
        return await baseService.GetDataAsync<SupervisorNameModel>(criteria);
    }

    public async Task<IEnumerable<Supervisor>> GetDisciplineSettingByEmployeeIdAsync(int employeeId)
    {
        var companyId = session.Get<int>(SessionKey.CurrentCompanyId);
        var criteria = new GetDisciplineSettingByEmployeeId
        {
            EmpId = employeeId,
            CompanyId = companyId
        };
        return await baseService.GetDataAsync<Supervisor>(criteria);
    }

    public async Task<IEnumerable<SupervisorNameModel>> GetHRAndSupervisorByEmpIdAsync(int employeeId)
    {
        var criteria = new GetHRAndSupervisorByEmpIdCriteria
        {
            EmpId = employeeId
        };
        return await baseService.GetDataAsync<SupervisorNameModel>(criteria);
    }

    public async Task<CommandResult> EditEmployeeSupervisorAsync(int actorId, int employeeId, short roleId, IEnumerable<EditTeamSupervisor> supervisors)
    {
        var criteria = new EditEmployeeSupervisorCriteria
        {
            EmployeeId = employeeId,
            ActorId = actorId,
            RoleId = roleId,
            SupervisorXml = supervisors.ToXML().ToString()
        };
        return await baseService.ExecuteNonQueryAsync(criteria);
    }

    public async Task<IEnumerable<Employee>> GetEmployeeByGender(string query, int gender, short companyId)
    {
        var criteria = new GetMemberByGenderCriteria
        {
            Query = query,
            Gender = gender,
            CompanyId = companyId
        };
        return await baseService.GetDataAsync<Employee>(criteria);
    }

    public async Task<IEnumerable<EmployeeFullProfile>> GetEmployeeFullProfile(GetEmployeeFullProfileCriteria criteria)
    {
        var employees = await employeeRepository.GetEmployeeFullInformation(criteria);
        var roles = await roleService.GetBySiteIdAsync(session.SiteId);
        foreach (var employee in employees)
        {
            if (employee.Roles != null && employee.Roles.Any())
            {
                employee.Roles = roles.Where(x => employee.Roles.Any(y => y.RoleId == x.RoleId)).ToList();
            }
        }

        return employees;
    }

    public async Task<CommandResult> EditPasswordAsync(int actorId, EditPassword model)
    {
        var criteria = new EditPasswordCriteria
        {
            EmpId = model.UserId,
            Password = model.Password,
            PasswordReset = model.IsReset,
            ModifiedBy = actorId
        };
        return await baseService.ExecuteNonQueryAsync(criteria);
    }

    public async Task<CommandResult> CreateAsync(CreateEmployeeParam param)
    {
        var criteria = new InsertEmployeeCriteria
        {
            CompanyId = param.CompanyId,
            UserId = param.UserId,
            Code = param.Code,
            CreateBy = param.ActorId,
            FullName = param.FullName,
            Nickname = param.Nickname,
            TeamId = param.TeamId,
            Password = Encryptor.ComputeHash(param.Code),
            CreatedDate = DateTimeOffset.Now.DateTime,
            JoinDate = DateTimeOffset.Now.DateTime.ToClientTimeZone()
        };
        var commandResult = await baseService.ExecuteNonQueryAsync(criteria);
        commandResult.OutputId = criteria.EmpId;

        return commandResult;
    }

    public async Task<CommandResult> CheckExistAsync(CreateEmployeeParam param)
    {
        var companyId = session.Get<short>(SessionKey.CurrentCompanyId);

        var criteria = new CheckExistEmployeeCriteria
        {
            Code = param.Code,
            Nickname = param.Nickname,
            CompanyId = companyId
        };
        return await baseService.GetDataEntityAsync<CommandResult>(criteria);
    }

    public async Task<CommandResult> EditAsync(int actorId, EditEmployeeParam param, short companyId)
    {
        var criteria = new UpdateEmployeeCriteria
        {
            EmpId = param.EmployeeId,
            Code = param.Code,
            DomainUsername = param.DomainUsername,
            FullName = param.FullName,
            Nickname = param.Nickname,
            EmployeeCard = param.EmployeeCard,
            DateOfBirth = param.DateOfBirth,
            Email = param.Email,
            JobTitleId = param.JobTitleId,
            JobLevelId = param.JobLevelId,
            ManagementLevelId = param.ManagementLevelId,
            Enabled = param.Enabled,
            Phone = param.Phone,
            Gender = param.Gender,
            Roles = param.Roles,
            JoinedDate = param.JoinedDate,
            CompanyDate = param.CompanyDate,
            ConfirmedDate = param.ConfirmedDate,
            ResignedDate = param.ResignedDate,
            ResignedReason = param.ResignedReason,
            LetsTalk = param.LetsTalk,
            Marriage = param.Marriage,
            PermanentAddress = param.PermanentAddress,
            CurrentAddress = param.CurrentAddress,
            IdCardNumber = param.IdCardNumber,
            PassportNo = param.PassportNo,
            PassportExp = param.PassportExp,
            ModifiedBy = actorId,
            TeamId = param.TeamId,
            IsLocal = param.IsLocal,
            CountryCode = param.CountryCode,
            CanOT = param.CanOT,
            SpecialOT = param.SpecialOT,
            WorkingDays = param.WorkingDays,
            WorkingHours = param.WorkingHours,
            WorkingHoursbyWeek = param.WorkingHoursbyWeek,
            IsTopManager = param.IsTopManager,
            OffDaysbyWeek = param.OffDaysbyWeek,
            PlaceOfbirth = param.PlaceOfBirth,
            Ethnic = param.Ethnic,
            Native = param.Native,
            Religion = param.Religion,
            DateOfIdCard = param.DateOfIdCard,
            PlaceOfIdCard = param.PlaceOfIdCard,
            HomePhone = param.HomePhone,
            Qualification = param.Qualification,
            Major = param.Major,
            IsExpatUseHoliday = !param.IsLocal && param.IsExpatUseHoliday,
            PickupRemark = param.PickupRemark,
            PromotionDate = param.PromotionDate,
            TeamDate = param.TeamDate,
            MilitaryFromDate = param.MilitaryFromDate,
            MilitaryToDate = param.MilitaryToDate,
            CompanyId = companyId,
            SubCompanyId = param.SubCompanyId,
            SalaryStructureId = param.SalaryStructureId,
            Salary = param.Salary,
            AutoApproveRequest = param.AutoApproveRequest
        };
        return await baseService.ExecuteNonQueryAsync(criteria);
    }

    public async Task<CommandResult> DeleteAsync(int actorId, DeleteEmployeeParam param)
    {
        var criteria = new DeleteEmployeeCriteria
        {
            ActorId = actorId,
            EmpId = param.EmployeeId,
            ResignedDate = param.ResignedDate
        };
        return await baseService.ExecuteNonQueryAsync(criteria);
    }

    public string CalculateLengthOfService(DateTime? joinedDate, DateTime? militaryFromDate, DateTime? militaryToDate)
    {
        const byte halfMonth = 15;
        const short dayOfYear = 365;
        const byte dayOfMonth = 30;
        const byte monthOfYear = 12;
        var lengthOfService = string.Empty;
        if (joinedDate.HasValue)
        {
            var totalDays = ((TimeSpan)(DateTimeOffset.Now.Date - joinedDate)).Days;
            if (militaryToDate.HasValue && militaryFromDate.HasValue)
            {
                totalDays -= ((TimeSpan)(militaryToDate - militaryFromDate)).Days;
            }
            var years = totalDays / dayOfYear;
            var months = (totalDays - years * dayOfYear) / dayOfMonth;
            var days = totalDays - years * dayOfYear - months * dayOfMonth;
            if (days >= halfMonth)
            {
                months++;
            }

            if (months >= monthOfYear)
            {
                years++;
                months = 0;
            }

            lengthOfService = string.Format(Messages.LengthOfService, years, months);
        }

        return lengthOfService;
    }

    public async Task<IList<SupervisorNameModel>> CheckAndAddMissingSupervisor(IList<SupervisorNameModel> supervisors, int? supervisorId)
    {
        if (supervisorId is null or 0)
        {
            return supervisors;
        }
        var supervisorIds = supervisors.Select(x => x.OwnerId).ToList();
        var isMissing = !supervisorIds.Contains(supervisorId.Value);

        if (isMissing)
        {
            var supervisor = await GetEmployeeByIdInGroupScopeAsync(supervisorId.Value);
            SupervisorNameModel missingSupervisor = new()
            {
                NickName = supervisor.NickName,
                FullName = supervisor.FullName,
                OwnerId = supervisorId.Value,
                Email = supervisor.Email
            };

            supervisors.Add(missingSupervisor);
            return supervisors;
        }

        return supervisors;
    }

    public async Task<EmployeeBioInfoModel> GetEmployeeBioInfoAsync(int employeeId)
    {
        var companyId = session.Get<short>(SessionKey.CurrentCompanyId);

        var criteria = new GetEmployeeBioInfoCriteria
        {
            EmpId = employeeId,
            CompanyId = companyId,
            CurrentDate = DateTimeOffset.Now.DateTime.ToClientTimeZone().Date
        };
        return await baseService.GetDataEntityAsync<EmployeeBioInfoModel>(criteria);
    }

    public async Task<IEnumerable<int>> GetCompaniesBelongToSupervisor()
    {
        var employeeSession = session.Get<EmployeeSession>(SessionKey.Employee);
        var supervisorCompanyCriteria = new GetSupervisorCompaniesCriteria
        {
            EmpId = employeeSession.EmpId
        };
        var supervisorCompanyIds = await baseService.GetDataAsync<int>(supervisorCompanyCriteria);

        var nexCompanies = supervisorCompanyIds
            .Where(s => Constants.NexCompanyIds.Contains(s));

        return nexCompanies;
    }

    public async Task<IEnumerable<T>> FilterByEmployeeDataPermission<T>(
        IEnumerable<T> data,
        Func<T, int> employeeIdSelector)
    {
        var canViewAllEmployees = session.HasPermissionOnFunction(FunctionKey.DataPermission_AllEmployees, Policy.Read);
        var canViewEmployeesAssigned = session.HasPermissionOnFunction(FunctionKey.DataPermission_EmployeesAssigned, Policy.Read);
        var currentCompanyId = session.Get<short>(SessionKey.CurrentCompanyId);
        var employeeSessionEmpId = session.Get<EmployeeSession>(SessionKey.Employee)?.EmpId ?? 0;

        var criteria = new GetEmployeeByDataPermissionCriteria
        {
            CompanyId = currentCompanyId,
            EmpId = employeeSessionEmpId,
            IsViewAll = canViewAllEmployees,
            IsViewEmployeeAssigned = canViewEmployeesAssigned
        };

        var assignedEmployeeIds = await baseService.GetDataAsync<int>(criteria);

        return data.Where(item => assignedEmployeeIds.Contains(employeeIdSelector(item)));
    }
}