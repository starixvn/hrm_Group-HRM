using HRM.ViewModels.Report;

namespace HRM.Repositories;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fanex.Data;
using Fanex.Data.Repository;
using GO.UserControl.Client.Core.Models.Role;
using Models;
using Models.Group;
using Models.Request;
using Models.Supervisor;

public class ReportRepository : Interfaces.IReportRepository
{
    public async Task<IEnumerable<EmployeeAttendanceModel>> GetEmployeeAttendanceReportAsync(ICriteria criteria)
    {
        using var objectDb = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());
        using var multi = await objectDb.QueryMultipleAsync(criteria);
        var attendances = (await multi.ReadAsync<EmployeeAttendanceModel>()).ToList();
        var requests = multi.Read<EmployeeAttendanceRequest, RequestTypeModel, LeaveType, EmployeeAttendanceRequest>(
            (request, requestType, leaveType) =>
            {
                request.RequestType = requestType;
                request.LeaveType = leaveType;
                return request;
            },
            splitOn: "RequestType, LeaveTypeId").ToList();

        foreach (var attendance in attendances)
        {
            attendance.RequestList = requests.Where(t => t.WorkDate == attendance.AttendanceDate && t.EmpId == attendance.EmpId).ToList();
        }

        return attendances;
    }

    public async Task<IList<SupervisorReportModel>> GetSupervisorReport(ICriteria criteria)
    {
        using var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());
        using var multi = await db.QueryMultipleAsync(criteria);
        var supervisors = multi.Read<SupervisorReportModel, TeamModel, SupervisorReportModel>(
            (su, team) =>
            {
                su.Team = team;
                return su;
            },
            splitOn: "TeamId").ToList();

        var roles = multi.Read<EmployeeRole, Role, EmployeeRole>(
            (er, r) =>
            {
                er.Role = r;
                return er;
            },
            "RoleId").ToList();

        var employees = multi.Read<SupervisorModel, Employee, SupervisorModel>(
            (su, e) =>
            {
                su.Supervisor = e;
                return su;
            },
            splitOn: "EmpId").ToList();

        foreach (var supervisor in supervisors)
        {
            supervisor.Supervisors = employees.Where(x => x.Supervisor.Id == supervisor.Id).ToList();
            supervisor.Roles = roles.Where(i => i.EmployeeId == supervisor.Id).Select(i => i.Role).ToList();
        }

        return supervisors;
    }

    public async Task<IList<TeamSupervisorReportModel>> GetTeamSupervisor(ICriteria criteria)
    {
        using var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());
        using var multi = await db.QueryMultipleAsync(criteria);
        var supervisors = (await multi.ReadAsync<TeamSupervisorReportModel>()).ToList();

        IList<SupervisorModel> employees = multi.Read<SupervisorModel, TeamModel, SupervisorModel>(
            (su, team) =>
            {
                su.Team = team;
                return su;
            },
            splitOn: "TeamId").ToList();

        foreach (var supervisor in supervisors)
        {
            supervisor.Supervisors = employees.Where(x => x.Team.TeamId == supervisor.TeamId).ToList();
        }

        return supervisors.ToList();
    }
}