using HRM.Core.Constants;
using HRM.Core.Filters;
using HRM.Core;
using HRM.Models.Supervisor;
using HRM.Services;
using System.Collections.Generic;
using GO.UserControl.Client.Core.Enumerables;
using GO.UserControl.Client.Core.Session;
using HRM.Models.Group;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using HRM.Core.Enums;

namespace HRM.Controllers;

public class TeamController(IUserControlSession userControlSession,
    ITeamService teamService) : BaseController(userControlSession)
{

    [HttpGet]
    public CustomJsonResult GetDisciplineManagerByTeamId(int teamId)
    {
        var supervisors = teamService.GetDisciplineManagerByTeamId(teamId);
        return new CustomJsonResult(supervisors);
    }

    [HrmPermission(Policy.Host, FunctionKey.TeamManagement)]
    public CustomJsonResult GetTeamSupervisorByRoleId(int teamId, short roleId)
    {
        IEnumerable<SupervisorModel> supervisors = teamService.GetTeamSupervisorByRoleId(teamId, roleId);

        return new CustomJsonResult(supervisors);
    }

    [HttpPost]
    [HrmPermission(Policy.Host, FunctionKey.TeamManagement)]
    public CustomJsonResult EditTeamSupervisor(int teamId, short roleId, IEnumerable<EditTeamSupervisor> supervisors)
    {
        var commandResult = teamService.EditTeamSupervisor(EmployeeSession.EmpId, teamId, roleId, supervisors);

        return new CustomJsonResult(commandResult);
    }

    [HrmPermission(Policy.Host, FunctionKey.TeamManagement)]
    public CustomJsonResult GetShiftManagerByTeamId(int teamId)
    {
        var supervisors = teamService.GetTeamSupervisorByRoleId(teamId, (short)RoleSystem.ShiftManager);
        var data = supervisors
            .Select(x => new Supervisor
            {
                OwnerId = x.OwnerId,
                Nickname = x.NickName,
                FullName = x.FullName
            });
        return new CustomJsonResult(data);
    }
}