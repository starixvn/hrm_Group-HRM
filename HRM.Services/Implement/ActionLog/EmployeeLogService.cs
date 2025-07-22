namespace HRM.Services.Implement.ActionLog;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Constants;
using Fanex.Data;
using GO.UserControl.Client.Core.Enumerables;
using GO.UserControl.Client.Core.Models.Role;
using GO.UserControl.Client.Core.Services;
using GO.UserControl.Client.Core.Session;
using HRM.Models.SalaryStructure;
using Interfaces;
using Models;
using Models.Log;
using Resources;

public interface IEmployeeLogService
{
    Task<IEnumerable<LogObjectDetail>> GetLogDetailByIdAsync(int employeeId);
}

public class EmployeeLogService(
    IUserControlSession session,
    ICommonDataService commonDataService,
    IActionLogService actionLogService,
    IJobLevelService jobLevelService,
    IManagementLevelService managementLevelService,
    IRoleService roleService,
    ISalaryStructureService salaryStructureService) : IEmployeeLogService
{
    private IEnumerable<MappingFieldConfig> mappingFields = new List<MappingFieldConfig>
    {
        new() { FieldName = "Code", DisplayName = Labels.EmployeeID },
        new() { FieldName = "EmployeeCard", DisplayName = Labels.EmployeeCard },
        new() { FieldName = "DomainUsername", DisplayName = Labels.DomainUsername },
        new() { FieldName = "FullName", DisplayName = Labels.FullName },
        new() { FieldName = "Nickname", DisplayName = Labels.NickName },
        new() { FieldName = "DOB", DisplayName = Labels.DateOfBirth, DataType = MappingDataType.Date },
        new() { FieldName = "PlaceOfBirth", DisplayName = Labels.PlaceOfBirth },
        new() { FieldName = "Ethnic", DisplayName = Labels.Ethnic },
        new() { FieldName = "Native", DisplayName = Labels.Native },
        new() { FieldName = "Religion", DisplayName = Labels.Religion },
        new() { FieldName = "Gender", DisplayName = Labels.Gender },
        new() { FieldName = "Marriage", DisplayName = Labels.Marriage },
        new() { FieldName = "CountryCode", DisplayName = Labels.Nationality },
        new() { FieldName = "IdCardNumber", DisplayName = Labels.IDCardNo },
        new() { FieldName = "DateOfIdCard", DisplayName = Labels.IDCardIssueDate, DataType = MappingDataType.Date },
        new() { FieldName = "PlaceOfIdCard", DisplayName = Labels.IDCardPlaceOfIssue },
        new() { FieldName = "PassportNo", DisplayName = Labels.PassportNo },
        new() { FieldName = "PassportExp", DisplayName = Labels.PassportExp },
        new() { FieldName = "Qualification", DisplayName = Labels.Qualification },
        new() { FieldName = "Major", DisplayName = Labels.Major },
        new() { FieldName = "MilitaryFromDate", DisplayName = Labels.MilitaryFromDate, DataType = MappingDataType.Date },
        new() { FieldName = "MilitaryToDate", DisplayName = Labels.MilitaryToDate, DataType = MappingDataType.Date },
        new() { FieldName = "Phone", DisplayName = Labels.Cellphone },
        new() { FieldName = "HomePhone", DisplayName = Labels.HomePhone },
        new() { FieldName = "Email", DisplayName = Labels.WorkEmail },
        new() { FieldName = "PermanentAddress", DisplayName = Labels.Address },
        new() { FieldName = "CurrentAddress", DisplayName = Labels.CurrentAddress },
        new() { FieldName = "PickupRemark", DisplayName = Labels.PickupRemark },
        new() { FieldName = "LetsTalk", DisplayName = Labels.LetsTalk },
        new() { FieldName = "Unit", DisplayName = Labels.Unit },
        new() { FieldName = "TeamId", DisplayName = Labels.Team },
        new() { FieldName = "TeamDate", DisplayName = Labels.TeamDate, DataType = MappingDataType.Date },
        new() { FieldName = "JobTitleId", DisplayName = Labels.JobTitle },
        new() { FieldName = "JobLevelId", DisplayName = Labels.JobLevel },
        new() { FieldName = "ManagementLevelId", DisplayName = Labels.ManagementLevel },
        new() { FieldName = "PromotionDate", DisplayName = Labels.PromotionDate, DataType = MappingDataType.Date },
        new() { FieldName = "JoinedDate", DisplayName = Labels.GroupDate },
        new() { FieldName = "CompanyDate", DisplayName = Labels.CompanyDate },
        new() { FieldName = "ConfirmedDate", DisplayName = Labels.ConfirmedDate },
        new() { FieldName = "ResignedDate", DisplayName = Labels.ResignedDate },
        new() { FieldName = "ResignedReason", DisplayName = Labels.ResignedReason },
        new() { FieldName = "CanOT", DisplayName = Labels.AllowClaimOT, DataType = MappingDataType.Boolean },
        new() { FieldName = "SpecialOT", DisplayName = Labels.SpecialOT },
        new() { FieldName = "WorkingDays", DisplayName = Labels.WorkingDaysPerMonth },
        new() { FieldName = "WorkingHours", DisplayName = Labels.WorkingHoursPerDay },
        new() { FieldName = "OffDaysbyWeek", DisplayName = Labels.NumberOfOffday },
        new() { FieldName = "RoleIds", DisplayName = Labels.Roles },
        new() { FieldName = "IsTopManager", DisplayName = Labels.SpecialUser, DataType = MappingDataType.Boolean },
        new() { FieldName = "IsLocal", DisplayName = Labels.Category, DataType = MappingDataType.Boolean },
        new() { FieldName = "IsExpatUseHoliday", DisplayName = Labels.ExpatApplicableHoliday, DataType = MappingDataType.Boolean },
        new() { FieldName = "Enabled", DisplayName = Labels.EnabledUser, DataType = MappingDataType.Boolean },
        new() { FieldName = "IsDeleted", DisplayName = Labels.Deleted, DataType = MappingDataType.Boolean },
        new() { FieldName = "ChineseName", DisplayName = Labels.ChineseName },
        new() { FieldName = "WorkingHoursbyWeek", DisplayName = Labels.WorkingHoursPerWeek },
        new() { FieldName = "SalaryStructureId", DisplayName = Labels.SalaryStructure },
        new() { FieldName = "SubCompanyName", DisplayName = Labels.SubCompany }
    };

    private IEnumerable<Team> Teams { get; set; }
    private IEnumerable<LookupData> JobTitles { get; set; }
    private IEnumerable<JobLevel> JobLevels { get; set; }
    private IEnumerable<ManagementLevel> ManagementLevels { get; set; }
    private IEnumerable<Country> Countries { get; set; }
    private IEnumerable<Role> SystemRoles { get; set; }
    private IEnumerable<EntityBase<byte>> Genders { get; set; }
    private IEnumerable<EntityBase<byte>> MarriageStatuses { get; set; }
    private IEnumerable<SalaryStructure> SalaryStructures { get; set; }

    public async Task<IEnumerable<LogObjectDetail>> GetLogDetailByIdAsync(int employeeId)
    {
        var companyId = session.Get<int>(SessionKey.CurrentCompanyId);
        Teams = await commonDataService.GetAllTeamAsync();
        JobTitles = await commonDataService.GetLookupDataByCodeAsync(Constants.JobTitle, companyId);
        JobLevels = await jobLevelService.GetJobLevel();
        ManagementLevels = await managementLevelService.GetManagementLevel();
        Countries = await commonDataService.GetAllCountry();
        SystemRoles = await roleService.GetBySiteIdAsync(session.SiteId);
        Genders = commonDataService.GetGender();
        MarriageStatuses = commonDataService.GetMarriageStatus();
        SalaryStructures = await salaryStructureService.GetSalaryStructureAsync(companyId);

        var logs = await actionLogService.GetLogDetailByIdAsync(LogObjectType.Employee, employeeId);
        MappingField(logs);

        return logs;
    }

    private void MappingField(IEnumerable<LogObjectDetail> logs)
    {
        if (session.HasPermissionOnFunction(FunctionKey.SalaryStructureConfiguration, Policy.Read))
        {
            mappingFields = mappingFields.Append(new MappingFieldConfig { FieldName = "Salary", DisplayName = Labels.NonStandardSalaryStructure });
        }

        foreach (var log in logs)
        {
            foreach (var field in log.Fields)
            {
                var mappingField = mappingFields.FirstOrDefault(x => string.Equals(x.FieldName, field.FieldName, StringComparison.OrdinalIgnoreCase));
                if (mappingField != null)
                {
                    field.DisplayName = mappingField.DisplayName;
                }

                MappingValue(field);
            }
        }
    }

    private void MappingValue(FieldValue field)
    {
        switch (field.FieldName)
        {
            case "TeamId":
                MappingTeam(field);
                break;

            case "RoleIds":
                MappingRole(field);
                break;

            case "CountryCode":
                MappingCountry(field);
                break;

            case "JobTitleId":
                MappingJobTitle(field);
                break;

            case "JobLevelId":
                MappingJobLevel(field);
                break;

            case "ManagementLevelId":
                MappingManagementLevel(field);
                break;

            case "Gender":
                MappingGender(field);
                break;

            case "Marriage":
                MappingMarriage(field);
                break;

            case "IsLocal":
                MappingCategory(field);
                break;

            case "SalaryStructureId":
                MappingSalaryStructure(field);
                break;

            case "Salary":
                MappingSalary(field);
                break;

            default:
                // Handle other fields
                break;
        }
    }

    private void MappingTeam(FieldValue field)
    {
        _ = int.TryParse(field.OldValue, out var teamId);
        field.OldValue = Teams.FirstOrDefault(x => x.Id == teamId)?.Name;

        _ = int.TryParse(field.Value, out teamId);
        field.Value = Teams.FirstOrDefault(x => x.Id == teamId)?.Name;
    }

    private void MappingRole(FieldValue field)
    {
        var roles = !string.IsNullOrWhiteSpace(field.OldValue) ? field.OldValue.Split(',') : [];
        field.OldValue = string.Join(", ", SystemRoles.Where(x => roles.Contains(x.RoleId.ToString())).Select(x => x.Name));

        roles = !string.IsNullOrWhiteSpace(field.Value) ? field.Value.Split(',') : [];
        field.Value = string.Join(", ", SystemRoles.Where(x => roles.Contains(x.RoleId.ToString())).Select(x => x.Name));
    }

    private void MappingJobTitle(FieldValue field)
    {
        _ = int.TryParse(field.OldValue, out var jobTitleId);
        field.OldValue = JobTitles.FirstOrDefault(x => x.Id == jobTitleId)?.Name;

        _ = int.TryParse(field.Value, out jobTitleId);
        field.Value = JobTitles.FirstOrDefault(x => x.Id == jobTitleId)?.Name;
    }

    private void MappingJobLevel(FieldValue field)
    {
        _ = int.TryParse(field.OldValue, out var jobLevelId);
        field.OldValue = JobLevels.FirstOrDefault(x => x.Id == jobLevelId)?.Name;

        _ = int.TryParse(field.Value, out jobLevelId);
        field.Value = JobLevels.FirstOrDefault(x => x.Id == jobLevelId)?.Name;
    }

    private void MappingManagementLevel(FieldValue field)
    {
        _ = short.TryParse(field.OldValue, out var managementLevelId);
        field.OldValue = ManagementLevels.FirstOrDefault(x => x.Id == managementLevelId)?.Name;

        _ = short.TryParse(field.Value, out managementLevelId);
        field.Value = ManagementLevels.FirstOrDefault(x => x.Id == managementLevelId)?.Name;
    }

    private void MappingCountry(FieldValue field)
    {
        field.OldValue = Countries.FirstOrDefault(x => string.Equals(x.CountryCode, field.OldValue, StringComparison.OrdinalIgnoreCase))?.CountryName;
        field.Value = Countries.FirstOrDefault(x => string.Equals(x.CountryCode, field.Value, StringComparison.OrdinalIgnoreCase))?.CountryName;
    }

    private void MappingGender(FieldValue field)
    {
        _ = byte.TryParse(field.OldValue, out var gender);
        field.OldValue = Genders.FirstOrDefault(x => x.Id == gender)?.Name;

        _ = byte.TryParse(field.Value, out gender);
        field.Value = Genders.FirstOrDefault(x => x.Id == gender)?.Name;
    }

    private void MappingMarriage(FieldValue field)
    {
        _ = byte.TryParse(field.OldValue, out var gender);
        field.OldValue = MarriageStatuses.FirstOrDefault(x => x.Id == gender)?.Name;

        _ = byte.TryParse(field.Value, out gender);
        field.Value = MarriageStatuses.FirstOrDefault(x => x.Id == gender)?.Name;
    }

    private static void MappingCategory(FieldValue field)
    {
        field.OldValue = field.OldValue == "1" ? Labels.Local : Labels.Expat;
        field.Value = field.Value == "1" ? Labels.Local : Labels.Expat;
    }

    private void MappingSalaryStructure(FieldValue field)
    {
        _ = int.TryParse(field.OldValue, out var salaryStructureId);
        var salaryStructure = SalaryStructures.FirstOrDefault(x => x.SalaryStructureId == salaryStructureId);
        field.OldValue = salaryStructure?.Name;

        _ = int.TryParse(field.Value, out salaryStructureId);
        salaryStructure = SalaryStructures.FirstOrDefault(x => x.SalaryStructureId == salaryStructureId);
        field.Value = salaryStructure?.Name;
    }

    private void MappingSalary(FieldValue field)
    {
        if (session.HasPermissionOnFunction(FunctionKey.SalaryStructureConfiguration, Policy.Read))
        {
            field.OldValue = !string.IsNullOrWhiteSpace(field.OldValue) ? Cryptography.DecryptPassword(field.OldValue) : string.Empty;
            field.Value = !string.IsNullOrWhiteSpace(field.Value) ? Cryptography.DecryptPassword(field.Value) : string.Empty;
        }
    }
}