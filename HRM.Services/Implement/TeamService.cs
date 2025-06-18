using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GO.UserControl.Client.Core.Session;
using HRM.Core.Constants;
using HRM.Core.Models;
using HRM.Core.Services;
using HRM.Core.XMLExtensions;
using HRM.Criteria.Group;
using HRM.Criteria.Supervisor;
using HRM.Models;
using HRM.Models.Group;
using HRM.Models.Supervisor;
using HRM.Repositories.Interfaces;
using HRM.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;
using OfficeOpenXml;

namespace HRM.Services.Implement;

public class TeamService(
    ITeamRepository teamRepository,
    IBaseService baseService,
    IUserControlSession userControlSession,
    IWebHostEnvironment hostingEnvironment) : ITeamService
{
    private const string GroupTeamTemplatePath = "ExcelTemplates/GroupTeams.xlsx";

    public async Task<Team> GetById(int id)
    {
        var companyId = userControlSession.Get<short>(SessionKey.CurrentCompanyId);
        var criteria = new GetGroupByIdCriteria
        {
            TeamId = id,
            CompanyId = companyId
        };
        return await teamRepository.GetById(criteria);
    }

    public async Task<int> GetNextGroupTeamIdAsync()
    {
        var criteria = new GetNextGroupTeamIdCriteria();
        return await baseService.GetDataEntityAsync<int>(criteria);
    }

    public async Task<GroupTeamModel> GetGroupTeamInUnitScopeById(int id)
    {
        var criteria = new GetTeamInUnitScopeCriteria
        {
            TeamId = id
        };
        return await baseService.GetDataEntityAsync<GroupTeamModel>(criteria);
        
    }
    
    public async Task<int> GetGroupIdByGroupTeamId(int groupTeamId)
    {
        var criteria = new GetGroupIdByGroupTeamIdCriteria
        {
            GroupTeamId = groupTeamId
        };
        return await baseService.GetDataEntityAsync<int>(criteria);
    }

    public async Task<IEnumerable<Team>> GetAllAsync(bool isDeleted = false, string keyword = null)
    {
        var companyId = userControlSession.Get<short>(SessionKey.CurrentCompanyId);
        var criteria = new GetAllGroupCriteria
        {
            IsDeleted = isDeleted,
            Keyword = keyword,
            CompanyId = companyId
        };
        var teams = await teamRepository.GetAllAsync(criteria);

        return teams ?? new List<Team>();
    }

    public async Task<IEnumerable<GroupTeamModel>> GetAllTeamByUnitScopeAsync(string keyword = null)
    {
        var criteria = new GetTeamInUnitScopeCriteria
        {
            Keyword = keyword
        };
        var teams = await baseService.GetDataAsync<GroupTeamModel>(criteria);

        return teams ?? new List<GroupTeamModel>();
    }

    public async Task<CommandResult> Delete(int actorId, int id)
    {
        var criteria = new DeleteTeamCriteria
        {
            TeamId = id,
            ModifiedBy = actorId
        };
        return await baseService.ExecuteNonQueryAsync(criteria);
    }

    public async Task<IEnumerable<Team>> GetBySupervisorAsync(int supervisorId)
    {
        var criteria = new GetGroupBySupervisorCriteria
        {
            SupervisorId = supervisorId
        };
        return await baseService.GetDataAsync<Team>(criteria);
    }

    public async Task<IList<SupervisorModel>> GetTeamSupervisorByRoleId(int teamId, short roleId)
    {
        var criteria = new GetTeamSupervisorByRoleIdCriteria
        {
            TeamId = teamId,
            Role = roleId
        };
        return (await baseService.GetDataAsync<SupervisorModel>(criteria)).ToList();
    }

    public async Task<IEnumerable<TeamSupervisorModel>> GetSupervisorByTeamId(int id, bool isGroupSpace)
    {
        int? companyId = null;
        if (!isGroupSpace)
        {
            companyId = userControlSession.Get<short>(SessionKey.CurrentCompanyId);
        }

        var criteria = new GetSupervisorByTeamIdCriteria
        {
            TeamId = id,
            IsGroupSpace = isGroupSpace,
            CompanyId = companyId
        };
        return (await baseService.GetDataAsync<TeamSupervisorModel>(criteria)).ToList();
    }

    public async Task<IEnumerable<Supervisor>> GetDisciplineManagerByTeamId(int teamId)
    {
        var criteria = new GetDisciplineManagerByTeamIdCriteria
        {
            TeamId = teamId
        };
        return (await baseService.GetDataAsync<Supervisor>(criteria)).ToList();
    }

    public async Task<CommandResult> EditTeamSupervisor(int actorId, int teamId, short roleId, IEnumerable<EditTeamSupervisor> supervisors)
    {
        var criteria = new EditTeamSupervisorCriteria
        {
            TeamId = teamId,
            ActorId = actorId,
            RoleId = roleId,
            SupervisorXml = supervisors.ToXML().ToString()
        };
        return await baseService.ExecuteNonQueryAsync(criteria);
    }

    public async Task<CommandResult> InsertTeamSupervisorByPIC(int actorId, int teamId, IEnumerable<EditTeamSupervisor> supervisors)
    {
        var criteria = new InsertTeamSupervisorByPICCriteria
        {
            TeamId = teamId,
            ActorId = actorId,
            SupervisorXml = supervisors.ToXML().ToString()
        };
        return await baseService.ExecuteNonQueryAsync(criteria);
    }

    public async Task<IEnumerable<DisciplineRule>> GetDisciplineRuleByTeam(int teamId)
    {
        var criteria = new GetDisciplineRuleByTeamCriteria { TeamId = teamId };
        return await baseService.GetDataAsync<DisciplineRule>(criteria);
    }

    public async Task<CommandResult> EditDisciplineRule(int actorId, int teamId, IEnumerable<DisciplineRule> disciplineRules)
    {
        var criteria = new EditDisciplineRuleCriteria
        {
            TeamId = teamId,
            ActorId = actorId,
            DisciplineRuleXml = disciplineRules.ToXML().ToString()
        };
        return await baseService.ExecuteNonQueryAsync(criteria);
    }

    public async Task<byte[]> GenerateGroupTeamExcel(IEnumerable<GroupTeamModel> teams)
    {
        const int totalColumns = 13;
        const int rowStartBody = 3;
        var templatePath = Path.Combine(hostingEnvironment.ContentRootPath, GroupTeamTemplatePath);

        await using var templateStream = new FileStream(templatePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, useAsync: true);
        using var templatePackage = new ExcelPackage(templateStream);
        using var xlPackage = new ExcelPackage();

        var template = templatePackage.Workbook.Worksheets.FirstOrDefault();
        if (template == null)
        {
            throw new InvalidOperationException("The template Excel file does not contain any worksheet.");
        }

        var worksheet = xlPackage.Workbook.Worksheets.Add("Group Team List");
        var rowIndex = 1;
        var colIndex = 1;

        // title
        template.Cells[rowIndex, colIndex, rowIndex, totalColumns].Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, totalColumns]);
        rowIndex++;

        // header
        template.Cells[rowIndex, colIndex, rowIndex, totalColumns].Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, totalColumns]);
        rowIndex++;

        // data
        var rowNum = 1;
        foreach (var team in teams)
        {
            colIndex = 1;
            template.Cells[rowStartBody, 1, rowStartBody, totalColumns].Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, totalColumns]);

            worksheet.Cells[rowIndex, colIndex++].Value = rowNum++;
            worksheet.Cells[rowIndex, colIndex++].Value = team.GroupTeamId.ToString();
            worksheet.Cells[rowIndex, colIndex++].Value = team.UnitCode;
            worksheet.Cells[rowIndex, colIndex++].Value = team.DepartmentName;
            worksheet.Cells[rowIndex, colIndex++].Value = team.DepartmentCode;
            worksheet.Cells[rowIndex, colIndex++].Value = team.DepartmentPIC;
            worksheet.Cells[rowIndex, colIndex++].Value = team.TeamName;
            worksheet.Cells[rowIndex, colIndex++].Value = team.TeamCode;
            worksheet.Cells[rowIndex, colIndex++].Value = team.PICDisplayName;
            worksheet.Cells[rowIndex, colIndex++].Value = team.AccountableArea;
            worksheet.Cells[rowIndex, colIndex++].Value = team.CompanyName;
            worksheet.Cells[rowIndex, colIndex++].Value = team.IsOutsource ? Constants.Yes : Constants.No;
            worksheet.Cells[rowIndex, colIndex++].Value = team.NoOfMembers.ToString();
            rowIndex++;
        }

        worksheet.Cells.AutoFitColumns();
        worksheet.View.FreezePanes(rowStartBody, 1);

        await using var ms = new MemoryStream();
        await xlPackage.SaveAsAsync(ms);
        return ms.ToArray();
    }
}