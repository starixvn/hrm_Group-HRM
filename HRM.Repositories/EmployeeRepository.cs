namespace HRM.Repositories;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fanex.Data;
using Fanex.Data.Repository;
using GO.UserControl.Client.Core.Models.Role;
using Interfaces;
using Models;
using Models.EmployeeInformation;
using Models.Section;
using Models.Supervisor;

public class EmployeeRepository : IEmployeeRepository
{
    public async Task<Employee> GetEmployeeByIdAsync(ICriteria criteria)
    {
        using var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());
        return await db.QueryEntityAsync<Employee, Team, Employee>(
            (e, t) =>
            {
                e.Team = t;
                return e;
            },
            splitOn: "TeamId",
            parameterSet: criteria);
    }

    public async Task<IEnumerable<Employee>> GetEmployeeAsync(ICriteria criteria)
    {
        using var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());
        using var multi = await db.QueryMultipleAsync(criteria);
        var employees = multi.Read<Employee, dynamic, Employee>(
            (e, t) =>
            {
                e.Team = new Team
                {
                    Id = t.TeamId,
                    Code = t.TeamCode,
                    Name = t.TeamName,
                    Section = new Section
                    {
                        Id = t.SectionId,
                        Code = t.SectionCode,
                        Name = t.SectionName,
                        Department = new Department
                        {
                            Id = t.DepartmentId,
                            Code = t.DepartmentCode,
                            Name = t.DepartmentName
                        }
                    }
                };
                return e;
            },
            splitOn: "TeamId");

        var roles = multi.Read<EmployeeRole, Role, EmployeeRole>(
            (er, r) =>
            {
                er.Role = r;
                return er;
            },
            "RoleId");

        employees = GroupByRoles(employees, roles);

        return employees.ToList();
    }

    public async Task<IList<Employee>> GetEmployeesByRoleAsync(ICriteria criteria)
    {
        using var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());
        using var multi = await db.QueryMultipleAsync(criteria);
        var employees = multi.Read<Employee, dynamic, Employee>(
            (e, t) =>
            {
                e.Team = new Team { Id = t.TeamID, Name = t.Team };
                return e;
            },
            splitOn: "TeamID");

        var roles = multi.Read<EmployeeRole, Role, EmployeeRole>(
            (er, r) =>
            {
                er.Role = r;
                return er;
            },
            "RoleId");

        employees = GroupByRoles(employees, roles);

        return employees.ToList();
    }

    private static IEnumerable<Employee> GroupByRoles(IEnumerable<Employee> employees, IEnumerable<EmployeeRole> roles)
    {
        foreach (var employee in employees)
        {
            employee.Roles = roles.Where(i => i.EmployeeId == employee.Id).Select(i => i.Role).ToList();
        }

        return employees;
    }

    public async Task<List<EmployeeFullProfile>> GetEmployeeFullInformation(ICriteria criteria)
    {
        using var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());
        using var multi = await db.QueryMultipleAsync(criteria);
        var profiles = multi.Read<EmployeeFullProfile>();
        var supervisors = multi.Read<SupervisorNameModel>();

        var roles = multi.Read<EmployeeRole, Role, EmployeeRole>(
            (er, r) =>
            {
                er.Role = r;
                return er;
            },
            "RoleId");

        GroupData(profiles, supervisors, roles);

        return profiles.ToList();
    }

    private static void GroupData(
        IEnumerable<EmployeeFullProfile> profiles,
        IEnumerable<SupervisorNameModel> supervisors,
        IEnumerable<EmployeeRole> roles)
    {
        foreach (var profile in profiles)
        {
            profile.Supervisors = supervisors.Where(i => i.EmpId == profile.EmpId).ToList();
            profile.Roles = roles.Where(i => i.EmployeeId == profile.EmpId).Select(i => i.Role).ToList();
        }
    }
}