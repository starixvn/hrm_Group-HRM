namespace HRM.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using HRM.Models;
    using HRM.Models.EmployeeInformation;
    using HRM.Models.Supervisor;
    using GO.UserControl.Client.Core.Models.Role;

    public class EmployeeRepository : IEmployeeRepository
    {
        public Employee GetEmployeeById(ICriteria criteria)
        {
            using (IObjectDb db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.QueryEntity<Employee, Team, Employee>(
                        (e, t) =>
                        {
                            e.Team = t;
                            return e;
                        },
                        splitOn: "TeamId",
                        parameterSet: criteria);
            }
        }

        public Employee GetEmployeeByCode(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.QueryEntity<Employee, dynamic, Employee>(
                        (e, t) =>
                        {
                            e.Team = new Team { Id = t.TeamID, Name = t.Team };
                            return e;
                        },
                        splitOn: "TeamId",
                        parameterSet: criteria);
            }
        }

        public IList<Employee> GetEmployees(ICriteria criteria)
        {
            using (IObjectDb db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (var multi = db.QueryMultiple(criteria))
                {
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
            }
        }

        public IList<Employee> GetEmployeesByRole(ICriteria criteria)
        {
            using (IObjectDb db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (var multi = db.QueryMultiple(criteria))
                {
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
            }
        }

        private static IEnumerable<Employee> GroupByRoles(IEnumerable<Employee> employees, IEnumerable<EmployeeRole> roles)
        {
            foreach (var employee in employees)
            {
                employee.Roles = roles.Where(i => i.EmployeeId == employee.Id).Select(i => i.Role).ToList();
            }

            return employees;
        }

        public IList<EmployeeFullProfile> GetEmployeeFullInformation(ICriteria criteria)
        {
            using (IObjectDb db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (var multi = db.QueryMultiple(criteria))
                {
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
            }
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
}