namespace Fanex.NexHR.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Group;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Models.Supervisor;
    using Fanex.NexHR.ViewModels;
    using GO.UserControl.Client.Core.Models.Role;

    public partial class ReportRepository : IReportRepository
    {
        public IList<EmployeeAttendanceModel> GetEmployeeAttendanceReport(ICriteria criteria)
        {
            using (var objectDb = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (var multi = objectDb.QueryMultiple(criteria))
                {
                    var attendances = multi.Read<EmployeeAttendanceModel>().ToList();
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
            }
        }

        public IList<SupervisorReportModel> GetSupervisorReport(ICriteria criteria)
        {
            using (IObjectDb db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (var multi = db.QueryMultiple(criteria))
                {
                    IList<SupervisorReportModel> supervisors = multi.Read<SupervisorReportModel, TeamModel, SupervisorReportModel>(
                    (su, team) =>
                    {
                        su.Team = team;
                        return su;
                    },
                    splitOn: "TeamId").ToList();

                    IEnumerable<EmployeeRole> roles = multi.Read<EmployeeRole, Role, EmployeeRole>(
                        (er, r) =>
                        {
                            er.Role = r;
                            return er;
                        },
                        "RoleId");

                    IList<SupervisorModel> employees = multi.Read<SupervisorModel, Employee, SupervisorModel>(
                    (su, e) =>
                    {
                        su.Supervisor = e;
                        return su;
                    },
                    splitOn: "EmpId").ToList();

                    foreach (SupervisorReportModel supervisor in supervisors)
                    {
                        supervisor.Supervisors = employees.Where(x => x.Supervisor.Id == supervisor.Id).ToList();
                        supervisor.Roles = roles.Where(i => i.EmployeeId == supervisor.Id).Select(i => i.Role).ToList();
                    }

                    return supervisors;
                }
            }
        }

        public IList<TeamSupervisorReportModel> GetTeamSupervisor(ICriteria criteria)
        {
            using (IObjectDb db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (var multi = db.QueryMultiple(criteria))
                {
                    IList<TeamSupervisorReportModel> supervisors = multi.Read<TeamSupervisorReportModel>().ToList();

                    IList<SupervisorModel> employees = multi.Read<SupervisorModel, TeamModel, SupervisorModel>(
                    (su, team) =>
                    {
                        su.Team = team;
                        return su;
                    },
                    splitOn: "TeamId").ToList();

                    foreach (TeamSupervisorReportModel supervisor in supervisors)
                    {
                        supervisor.Supervisors = employees.Where(x => x.Team.TeamId == supervisor.TeamId).ToList();
                    }

                    return supervisors;
                }
            }
        }
    }
}