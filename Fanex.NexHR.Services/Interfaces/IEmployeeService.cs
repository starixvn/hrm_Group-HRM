namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections.Generic;
    using Fanex.NexHR.Core.Models;
    using Fanex.NexHR.Criteria.AnnualLeave;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Criteria.Supervisor;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.EmployeeInformation;
    using Fanex.NexHR.Models.Group;
    using Fanex.NexHR.Models.Supervisor;
    using Fanex.NexHR.ViewModels.AnnualLeave;

    public interface IEmployeeService
    {
        string GenerateEmployeeCode();

        IList<SupervisorNameModel> GetListSupervisorName(GetListSupervisorNameCriteria criteria);

        IList<SupervisorModel> GetEmployeeSupervisorByRoleId(int employeeId, short roleId);

        Employee GetEmployeeById(int id);

        IList<Employee> GetEmployees(GetAllEmployeeCriteria criteria);

        IList<Employee> GetMemberByManager(GetMemberByManagerCriteria criteria);

        IList<Employee> GetEmployeesByRole(GetEmployeeByRoleCriteria criteria);

        EmployeeShift GetShiftByEmployeeId(int id);

        IList<AnnualLeaveTransactionViewModel> GetAnnualLeaveTransactions(GetAnnualLeaveTransactionCriteria criteria);

        AnnualLeaveTransactionViewModel GetAnnualLeaveTransaction(int leaveTransId);

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