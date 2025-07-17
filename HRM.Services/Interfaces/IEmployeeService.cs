using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HRM.Core.Models;
using HRM.Criteria.AnnualLeave;
using HRM.Criteria.Employee;
using HRM.Criteria.Supervisor;
using HRM.Models;
using HRM.Models.EmployeeInformation;
using HRM.Models.Employees;
using HRM.Models.Group;
using HRM.Models.Supervisor;
using HRM.ViewModels.AnnualLeave;

namespace HRM.Services.Interfaces;

public interface IEmployeeService
{
    Task<string> GenerateEmployeeCode();

    Task<IEnumerable<SupervisorNameModel>> GetListSupervisorNameAsync(GetListSupervisorNameCriteria criteria);

    Task<IEnumerable<SupervisorModel>> GetEmployeeSupervisorByRoleIdAsync(int employeeId, short roleId);

    Task<Employee> GetEmployeeByIdAsync(int id);

    Task<IEnumerable<Employee>> GetEmployeeAsync(GetAllEmployeeCriteria criteria);

    Task<IEnumerable<Employee>> GetMemberByManagerAsync(GetMemberByManagerCriteria criteria);

    Task<IEnumerable<EmployeePIC>> GetEmployeeForPICAsync(GetEmployeeForPICCriteria criteria);

    Task<IEnumerable<Employee>> GetEmployeesByRoleAsync(GetEmployeeByRoleCriteria criteria);

    Task<EmployeeShift> GetShiftByEmployeeId(int id);

    Task<IEnumerable<AnnualLeaveTransactionViewModel>> GetAnnualLeaveTransactionAsync(GetAnnualLeaveTransactionCriteria criteria);

    Task<AnnualLeaveTransactionViewModel> GetAnnualLeaveTransactionAsync(int leaveTransId);

    Task<IEnumerable<SupervisorNameModel>> GetSupervisorByRequestTypeAsync(GetSupervisorByRequestTypeCriteria criteria);

    Task<IEnumerable<SupervisorNameModel>> GetSupervisorByQueryAsync(GetEmployeeSupervisorByQueryCriteria criteria);

    Task<IEnumerable<Supervisor>> GetDisciplineSettingByEmployeeIdAsync(int employeeId);

    Task<IEnumerable<SupervisorNameModel>> GetHRAndSupervisorByEmpIdAsync(int employeeId);

    Task<IEnumerable<Employee>> GetEmployeeByGender(string query, int gender, short companyId);

    Task<IEnumerable<EmployeeFullProfile>> GetEmployeeFullProfile(GetEmployeeFullProfileCriteria criteria);

    Task<CommandResult> EditEmployeeSupervisorAsync(int actorId, int employeeId, short roleId, IEnumerable<EditTeamSupervisor> supervisors);

    Task<CommandResult> EditPasswordAsync(int actorId, EditPassword model);

    Task<CommandResult> CheckExistAsync(CreateEmployeeParam param);

    Task<CommandResult> CreateAsync(CreateEmployeeParam param);

    Task<CommandResult> EditAsync(int actorId, EditEmployeeParam param, short companyId);

    Task<CommandResult> DeleteAsync(int actorId, DeleteEmployeeParam param);

    string CalculateLengthOfService(DateTime? joinedDate, DateTime? militaryFromDate, DateTime? militaryToDate);

    Task<CommandResult> UpdateUserIdAsync(UpdateUserIdCommand command);

    Task<IList<SupervisorNameModel>> CheckAndAddMissingSupervisor(IList<SupervisorNameModel> supervisors, int supervisorId);

    Task<EmployeeBioInfoModel> GetEmployeeBioInfoAsync(int employeeId, string clientIp);

    Task<IEnumerable<int>> GetCompaniesBelongToSupervisor();
}