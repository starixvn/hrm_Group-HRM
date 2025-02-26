namespace HRM.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HRM.Core.Models;
    using HRM.Criteria.AnnualLeave;
    using HRM.Criteria.Employee;
    using HRM.Criteria.Supervisor;
    using HRM.Models;
    using HRM.Models.EmployeeInformation;
    using HRM.Models.Group;
    using HRM.Models.Supervisor;
    using HRM.ViewModels.AnnualLeave;

    public interface IEmployeeService
    {
        string GenerateEmployeeCode();

        IList<SupervisorNameModel> GetListSupervisorName(GetListSupervisorNameCriteria criteria);

        IList<SupervisorModel> GetEmployeeSupervisorByRoleId(int employeeId, short roleId);

        Task<Employee> GetEmployeeByIdAsync(int id);

        IList<Employee> GetEmployees(GetAllEmployeeCriteria criteria);

        IList<Employee> GetMemberByManager(GetMemberByManagerCriteria criteria);

        IList<Employee> GetEmployeesByRole(GetEmployeeByRoleCriteria criteria);

        EmployeeShift GetShiftByEmployeeId(int id);

        Task<IEnumerable<AnnualLeaveTransactionViewModel>> GetAnnualLeaveTransactionAsync(GetAnnualLeaveTransactionCriteria criteria);

        Task<AnnualLeaveTransactionViewModel> GetAnnualLeaveTransactionAsync(int leaveTransId);

        IList<SupervisorNameModel> GetSupervisorByRequestType(GetSupervisorByRequestTypeCriteria criteria);

        IList<SupervisorNameModel> GetSupervisorByQuery(GetEmployeeSupervisorByQueryCriteria criteria);

        IEnumerable<Supervisor> GetDisciplineSettingByEmployeeId(int employeeId);

        IList<SupervisorNameModel> GetHRAndSupervisorByEmpId(int employeeId);

        IList<Employee> GetEmployeeByGender(string query, int gender);

        IList<EmployeeFullProfile> GetEmployeeFullProfile(GetEmployeeFullProfileCriteria criteria);

        CommandResult EditEmployeeSupervisor(int actorId, int employeeId, short roleId, IEnumerable<EditTeamSupervisor> supervisors);

        CommandResult EditPassword(int actorId, EditPassword model);

        CommandResult Create(int actorId, CreateEmployeeParam param);

        CommandResult Edit(int actorId, EditEmployeeParam param);

        CommandResult Delete(int actorId, DeleteEmployeeParam param);

        string CaculateLengthOfService(DateTime? joinedDate, DateTime? militaryFromDate, DateTime? militaryToDate);

        CommandResult UpdateUserId(UpdateUserIdCommand command);
    }
}