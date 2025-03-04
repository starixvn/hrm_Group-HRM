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

namespace HRM.Services.Interfaces;

public interface IEmployeeService
{
    Task<string> GenerateEmployeeCode();

    IList<SupervisorNameModel> GetListSupervisorName(GetListSupervisorNameCriteria criteria);

    Task<IList<SupervisorNameModel>> GetListSupervisorNameAsync(GetListSupervisorNameCriteria criteria);

    IList<SupervisorModel> GetEmployeeSupervisorByRoleId(int employeeId, short roleId);

    Task<IList<SupervisorModel>> GetEmployeeSupervisorByRoleIdAsync(int employeeId, short roleId);

    Task<Employee> GetEmployeeByIdAsync(int id);

    Task<IEnumerable<Employee>> GetEmployeeAsync(GetAllEmployeeCriteria criteria);

    Task<IList<Employee>> GetMemberByManagerAsync(GetMemberByManagerCriteria criteria);

    IList<Employee> GetEmployeesByRole(GetEmployeeByRoleCriteria criteria);

    Task<IList<Employee>> GetEmployeesByRoleAsync(GetEmployeeByRoleCriteria criteria);

    Task<EmployeeShift> GetShiftByEmployeeId(int id);

    Task<IEnumerable<AnnualLeaveTransactionViewModel>> GetAnnualLeaveTransactionAsync(GetAnnualLeaveTransactionCriteria criteria);

    Task<AnnualLeaveTransactionViewModel> GetAnnualLeaveTransactionAsync(int leaveTransId);

    IList<SupervisorNameModel> GetSupervisorByRequestType(GetSupervisorByRequestTypeCriteria criteria);

    Task<IList<SupervisorNameModel>> GetSupervisorByRequestTypeAsync(GetSupervisorByRequestTypeCriteria criteria);

    Task<IList<SupervisorNameModel>> GetSupervisorByQueryAsync(GetEmployeeSupervisorByQueryCriteria criteria);

    IEnumerable<Supervisor> GetDisciplineSettingByEmployeeId(int employeeId);

    Task<IEnumerable<Supervisor>> GetDisciplineSettingByEmployeeIdAsync(int employeeId);

    Task<IList<SupervisorNameModel>> GetHRAndSupervisorByEmpIdAsync(int employeeId);

    Task<IList<Employee>> GetEmployeeByGender(string query, int gender);

    Task<IList<EmployeeFullProfile>> GetEmployeeFullProfile(GetEmployeeFullProfileCriteria criteria);

    CommandResult EditEmployeeSupervisor(int actorId, int employeeId, short roleId, IEnumerable<EditTeamSupervisor> supervisors);

    Task<CommandResult> EditEmployeeSupervisorAsync(int actorId, int employeeId, short roleId,
        IEnumerable<EditTeamSupervisor> supervisors);

    Task<CommandResult> EditPasswordAsync(int actorId, EditPassword model);

    Task<CommandResult> CreateAsync(int actorId, CreateEmployeeParam param);

    Task<CommandResult> EditAsync(int actorId, EditEmployeeParam param);

    Task<CommandResult> DeleteAsync(int actorId, DeleteEmployeeParam param);

    string CaculateLengthOfService(DateTime? joinedDate, DateTime? militaryFromDate, DateTime? militaryToDate);

    Task<CommandResult> UpdateUserIdAsync(UpdateUserIdCommand command);
}