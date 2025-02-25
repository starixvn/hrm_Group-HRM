namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.NexHR.Core.Models;
    using Fanex.NexHR.Core.Security;
    using Fanex.NexHR.Core.Services;
    using Fanex.NexHR.Core.XMLExtensions;
    using Fanex.NexHR.Criteria;
    using Fanex.NexHR.Criteria.AnnualLeave;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Criteria.Shift;
    using Fanex.NexHR.Criteria.Supervisor;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.EmployeeInformation;
    using Fanex.NexHR.Models.Group;
    using Fanex.NexHR.Models.Supervisor;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.ViewModels.AnnualLeave;
    using GO.UserControl.Client.Core;
    using GO.UserControl.Client.Core.Services;

    public class EmployeeService : IEmployeeService
    {
        private readonly IUserControlSettings userControlSettings;
        private readonly IEmployeeRepository employeeRepository;
        private readonly IBaseService baseService;
        private readonly IRoleService roleService;

        public EmployeeService(
            IUserControlSettings userControlSettings,
            IEmployeeRepository employeeRepository,
            IBaseService baseService,
            IRoleService roleService)
        {
            this.userControlSettings = userControlSettings;
            this.employeeRepository = employeeRepository;
            this.roleService = roleService;
            this.baseService = baseService;
        }

        public string GenerateEmployeeCode()
        {
            var criteria = new GenerateEmployeeCodeCriteria();
            return baseService.GetDataEntity<string>(criteria);
        }

        public Employee GetEmployeeById(int id)
        {
            var criteria = new GetEmployeeByIdCriteria
            {
                EmpId = id
            };
            Employee employee = employeeRepository.GetEmployeeById(criteria);
            if (employee != null)
            {
                employee.Roles = roleService.GetByUserId(employee.UserId).ToList();
            }

            return employee;
        }

        public IList<Employee> GetEmployees(GetAllEmployeeCriteria criteria)
        {
            IList<Employee> employees = null;
            if (criteria != null && criteria.IsValid())
            {
                employees = employeeRepository.GetEmployees(criteria);

                var roles = roleService.GetBySiteId(userControlSettings.SiteId);
                foreach (var employee in employees)
                {
                    if (employee.Roles != null && employee.Roles.Any())
                    {
                        employee.Roles = roles.Where(x => employee.Roles.Exists(y => y.RoleId == x.RoleId)).ToList();
                    }
                }
            }

            return employees ?? new List<Employee>();
        }

        public IList<Employee> GetMemberByManager(GetMemberByManagerCriteria criteria)
        {
            return baseService.GetData<Employee>(criteria).ToList();
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
                        employee.Roles = roles.Where(x => employee.Roles.Exists(y => y.RoleId == x.RoleId)).ToList();
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

        public IList<AnnualLeaveTransactionViewModel> GetAnnualLeaveTransactions(GetAnnualLeaveTransactionCriteria criteria)
        {
            return baseService.GetData<AnnualLeaveTransactionViewModel>(criteria).ToList();
        }

        public AnnualLeaveTransactionViewModel GetAnnualLeaveTransaction(int leaveTransId)
        {
            var criteria = new GetAnnualLeaveTransactionByIdCriteria
            {
                LeaveTransId = leaveTransId
            };
            return baseService.GetDataEntity<AnnualLeaveTransactionViewModel>(criteria);
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

        public IList<SupervisorNameModel> GetListSupervisorName(GetListSupervisorNameCriteria criteria)
        {
            return baseService.GetData<SupervisorNameModel>(criteria).ToList();
        }

        public IList<SupervisorNameModel> GetSupervisorByRequestType(GetSupervisorByRequestTypeCriteria criteria)
        {
            return baseService.GetData<SupervisorNameModel>(criteria).ToList();
        }

        public IList<SupervisorNameModel> GetSupervisorByQuery(GetEmployeeSupervisorByQueryCriteria criteria)
        {
            return baseService.GetData<SupervisorNameModel>(criteria).ToList();
        }

        public IEnumerable<Supervisor> GetDisciplineSettingByEmployeeId(int employeeId)
        {
            var criteria = new GetDisciplineSettingByEmployeeId
            {
                EmpId = employeeId
            };
            return baseService.GetData<Supervisor>(criteria).ToList();
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

        public CommandResult Create(int actorId, CreateEmployeeParam param)
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
            CommandResult commandResult = baseService.ExecuteNonQuery(criteria);
            commandResult.OutputId = criteria.EmpId;

            return commandResult;
        }

        public CommandResult Edit(int actorId, EditEmployeeParam param)
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
                ExpertGradeId = param.ExpertGradeId,
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
            return baseService.ExecuteNonQuery(criteria);
        }

        public CommandResult Delete(int actorId, DeleteEmployeeParam param)
        {
            var criteria = new DeleteEmployeeCriteria
            {
                ActorId = actorId,
                EmpId = param.EmployeeId,
                ResignedDate = param.ResignedDate
            };
            return baseService.ExecuteNonQuery(criteria);
        }

        public CommandResult UpdateUserId(UpdateUserIdCommand command)
        {
            return baseService.ExecuteNonQuery(command);
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
}