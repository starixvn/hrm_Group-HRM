namespace HRM.Services.Implement;

using System;
using System.Collections.Generic;
using System.Linq;
using HRM.Core.Models;
using Core.Security;
using HRM.Core.Services;
using Core.XMLExtensions;
using Criteria;
using Criteria.AnnualLeave;
using Criteria.Employee;
using Criteria.Shift;
using Criteria.Supervisor;
using Models;
using Models.EmployeeInformation;
using HRM.Models.Group;
using Models.Supervisor;
using Repositories;
using Resources;
using ViewModels.AnnualLeave;
using GO.UserControl.Client.Core;
using GO.UserControl.Client.Core.Services;
using System.Threading.Tasks;
using Interfaces;

public class EmployeeService(
    IUserControlSettings userControlSettings,
    IEmployeeRepository employeeRepository,
    IBaseService baseService,
    IRoleService roleService) : IEmployeeService
{
    public string GenerateEmployeeCode()
    {
        var criteria = new GenerateEmployeeCodeCriteria();
        return baseService.GetDataEntity<string>(criteria);
    }

    public async Task<Employee> GetEmployeeByIdAsync(int id)
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

            var roles = await roleService.GetBySiteIdAsync(userControlSettings.SiteId);
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

    public async Task<IList<Employee>> GetMemberByManagerAsync(GetMemberByManagerCriteria criteria)
    {
        return (await baseService.GetDataAsync<Employee>(criteria)).ToList();
    }

    public IList<Employee> GetEmployeesByRole(GetEmployeeByRoleCriteria criteria)
    {
        IList<Employee> employees = null;
        if (criteria != null && criteria.IsValid())
        {
            employees = employeeRepository.GetEmployeesByRole(criteria);
            var roles = roleService.GetBySiteId(userControlSettings.SiteId);
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

    public EmployeeShift GetShiftByEmployeeId(int id)
    {
        var criteria = new GetShiftByEmployeeIdCriteria
        {
            EmpId = id
        };
        return baseService.GetDataEntity<EmployeeShift>(criteria);
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

    public IList<SupervisorModel> GetEmployeeSupervisorByRoleId(int employeeId, short roleId)
    {
        var criteria = new GetEmployeeSupervisorCriteria
        {
            EmpId = employeeId,
            Role = roleId
        };
        return baseService.GetData<SupervisorModel>(criteria).ToList();
    }

    public async Task<IList<SupervisorModel>> GetEmployeeSupervisorByRoleIdAsync(int employeeId, short roleId)
    {
        var criteria = new GetEmployeeSupervisorCriteria
        {
            EmpId = employeeId,
            Role = roleId
        };
        var data = await baseService.GetDataAsync<SupervisorModel>(criteria);
        return data.ToList();
    }

    public IList<SupervisorNameModel> GetListSupervisorName(GetListSupervisorNameCriteria criteria)
    {
        return baseService.GetData<SupervisorNameModel>(criteria).ToList();
    }

    public async Task<IList<SupervisorNameModel>> GetListSupervisorNameAsync(GetListSupervisorNameCriteria criteria)
    {
        var data = await baseService.GetDataAsync<SupervisorNameModel>(criteria);
        return data.ToList();
    }

    public IList<SupervisorNameModel> GetSupervisorByRequestType(GetSupervisorByRequestTypeCriteria criteria)
    {
        return baseService.GetData<SupervisorNameModel>(criteria).ToList();
    }

    public async Task<IList<SupervisorNameModel>> GetSupervisorByQueryAsync(GetEmployeeSupervisorByQueryCriteria criteria)
    {
        return (await baseService.GetDataAsync<SupervisorNameModel>(criteria)).ToList();
    }

    public IEnumerable<Supervisor> GetDisciplineSettingByEmployeeId(int employeeId)
    {
        var criteria = new GetDisciplineSettingByEmployeeId
        {
            EmpId = employeeId
        };
        return baseService.GetData<Supervisor>(criteria).ToList();
    }


    public async Task<IEnumerable<Supervisor>> GetDisciplineSettingByEmployeeIdAsync(int employeeId)
    {
        var criteria = new GetDisciplineSettingByEmployeeId
        {
            EmpId = employeeId
        };
        var data = await baseService.GetDataAsync<Supervisor>(criteria);
        return data.ToList();
    }

    public IList<SupervisorNameModel> GetHRAndSupervisorByEmpId(int employeeId)
    {
        var criteria = new GetHRAndSupervisorByEmpIdCriteria
        {
            EmpId = employeeId
        };
        return baseService.GetData<SupervisorNameModel>(criteria).ToList();
    }

    public CommandResult EditEmployeeSupervisor(int actorId, int employeeId, short roleId, IEnumerable<EditTeamSupervisor> supervisors)
    {
        var criteria = new EditEmployeeSupervisorCriteria
        {
            EmployeeId = employeeId,
            ActorId = actorId,
            RoleId = roleId,
            SupervisorXml = supervisors.ToXML().ToString()
        };
        return baseService.ExecuteNonQuery(criteria);
    }

    public IList<Employee> GetEmployeeByGender(string query, int gender)
    {
        var criteria = new GetMemberByGenderCriteria
        {
            Query = query,
            Gender = gender
        };

        return baseService.GetData<Employee>(criteria).ToList();
    }

    public IList<EmployeeFullProfile> GetEmployeeFullProfile(GetEmployeeFullProfileCriteria criteria)
    {
        var employees = employeeRepository.GetEmployeeFullInformation(criteria);
        var roles = roleService.GetBySiteId(userControlSettings.SiteId);
        foreach (var employee in employees)
        {
            if (employee.Roles != null && employee.Roles.Any())
            {
                employee.Roles = roles.Where(x => employee.Roles.Any(y => y.RoleId == x.RoleId)).ToList();
            }
        }

        return employees;
    }

    public CommandResult EditPassword(int actorId, EditPassword model)
    {
        var criteria = new EditPasswordCriteria
        {
            EmpId = model.EmployeeId,
            Password = model.Password,
            PasswordReset = model.IsReset,
            ModifiedBy = actorId
        };
        return baseService.ExecuteNonQuery(criteria);
    }

    public async Task<CommandResult> CreateAsync(int actorId, CreateEmployeeParam param)
    {
        var criteria = new InsertEmployeeCriteria
        {
            Code = param.Code,
            CreateBy = actorId,
            DomainUsername = param.DomainUsername,
            FullName = param.FullName,
            Nickname = param.Nickname,
            TeamId = param.TeamId,
            Password = Encryptor.ComputeHash(param.Code)
        };
        var commandResult = await baseService.ExecuteNonQueryAsync(criteria);
        commandResult.OutputId = criteria.EmpId;

        return commandResult;
    }

    public async Task<CommandResult> EditAsync(int actorId, EditEmployeeParam param)
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
            Skype = param.Skype,
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
            MilitaryToDate = param.MilitaryToDate
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

    public async Task<CommandResult> UpdateUserIdAsync(UpdateUserIdCommand command)
    {
        return await baseService.ExecuteNonQueryAsync(command);
    }

    public string CaculateLengthOfService(DateTime? joinedDate, DateTime? militaryFromDate, DateTime? militaryToDate)
    {
        const byte HalfMonth = 15;
        const short DayOfYear = 365;
        const byte DayOfMonth = 30;
        const byte MonthOfYear = 12;
        int totalDays;
        int years;
        int months;
        int days;
        string lengOfService = string.Empty;
        if (joinedDate.HasValue)
        {
            totalDays = ((TimeSpan)(DateTimeOffset.Now.Date - joinedDate)).Days;
            if (militaryToDate.HasValue && militaryFromDate.HasValue)
            {
                totalDays -= ((TimeSpan)(militaryToDate - militaryFromDate)).Days;
            }
            years = totalDays / DayOfYear;
            months = (totalDays - (years * DayOfYear)) / DayOfMonth;
            days = totalDays - (years * DayOfYear) - (months * DayOfMonth);
            if (days >= HalfMonth)
            {
                months++;
            }

            if (months >= MonthOfYear)
            {
                years++;
                months = 0;
            }

            lengOfService = string.Format(Messages.LengthOfService, years, months);
        }

        return lengOfService;
    }
}