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
using HRM.Models.PayrollManagement;
using HRM.Models.Supervisor;
using HRM.ViewModels.AnnualLeave;

namespace HRM.Services.Interfaces;

public interface IEmployeeService
{
    Task<string> GenerateEmployeeCode(short? companyId);

    Task<IEnumerable<SupervisorNameModel>> GetListSupervisorNameAsync(GetListSupervisorNameCriteria criteria);

    Task<IEnumerable<SupervisorModel>> GetEmployeeSupervisorByRoleIdAsync(int employeeId, short roleId);

    Task<Employee> GetEmployeeByIdAsync(int id);

    Task<Employee> GetEmployeeTransferByIdAsync(int id, short companyId);

    Task<Employee> GetEmployeeByIdInGroupScopeAsync(int id);

    Task<IEnumerable<EmployeePayrollAllowance>> GetEmployeePayrollAllowanceByEmpIdAsync(int empId);

    Task<IEnumerable<Employee>> GetEmployeeAsync(GetAllEmployeeCriteria criteria);

    Task<IEnumerable<GroupEmployeeModel>> GetEmployeesInGroupAsync();

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

    Task<CommandResult> CheckExistAsync(string code, string nickname);

    Task<CommandResult> CreateAsync(CreateEmployeeParam param);

    Task<CommandResult> EditAsync(int actorId, EditEmployeeParam param, short companyId);

    Task<CommandResult> DeleteAsync(int actorId, DeleteEmployeeParam param);

    (double Years, string LengthOfServiceText) CalculateLengthOfService(DateTime? joinedDate, DateTime? militaryFromDate, DateTime? militaryToDate);

    Task<IList<SupervisorNameModel>> CheckAndAddMissingSupervisor(IList<SupervisorNameModel> supervisors, int? supervisorId);

    Task<EmployeeBioInfoModel> GetEmployeeBioInfoAsync(int employeeId);

    Task<IEnumerable<int>> GetCompaniesBelongToSupervisor();

    Task<IEnumerable<T>> FilterByEmployeeDataPermission<T>(IEnumerable<T> data, Func<T, int> employeeIdSelector);

    Task<GO.UserControl.Client.Core.Models.CommandResult> UpdateUserInfoAsync(int userId, EditEmployeeParam param);

    Task<int> TransferEmployeeAsync(EmployeeTransfer model);

    Task<IEnumerable<EmployeeTransferModel>> GetEmployeeTransferListAsync(GetEmployeeTransferCriteria criteria);
}