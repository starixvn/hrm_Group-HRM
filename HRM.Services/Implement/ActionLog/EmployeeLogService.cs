namespace HRM.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using HRM.Core.Constants;
    using HRM.Models;
    using HRM.Models.Log;
    using HRM.Resources;
    using HRM.Services.Interfaces;
    using GO.UserControl.Client.Core.Models.Role;
    using GO.UserControl.Client.Core.Services;
    using GO.UserControl.Client.Core.Session;

    public interface IEmployeeLogService
    {
        IEnumerable<LogObjectDetail> GetLogDetailById(int employeeId);
    }

    public class EmployeeLogService : IEmployeeLogService
    {
        private readonly IEnumerable<MappingFieldConfig> mappingFields = new List<MappingFieldConfig>
        {
            new MappingFieldConfig { FieldName = "Code", DisplayName = Labels.EmployeeID },
            new MappingFieldConfig { FieldName = "EmployeeCard", DisplayName = Labels.EmployeeCard },
            new MappingFieldConfig { FieldName = "DomainUsername", DisplayName = Labels.DomainUsername },
            new MappingFieldConfig { FieldName = "FullName", DisplayName = Labels.FullName },
            new MappingFieldConfig { FieldName = "Nickname", DisplayName = Labels.NickName },
            new MappingFieldConfig { FieldName = "DOB", DisplayName = Labels.DateOfBirth, DataType = MappingDataType.Date },
            new MappingFieldConfig { FieldName = "PlaceOfBirth", DisplayName = Labels.PlaceOfBirth },
            new MappingFieldConfig { FieldName = "Ethnic", DisplayName = Labels.Ethnic },
            new MappingFieldConfig { FieldName = "Native", DisplayName = Labels.Native },
            new MappingFieldConfig { FieldName = "Religion", DisplayName = Labels.Religion },
            new MappingFieldConfig { FieldName = "Gender", DisplayName = Labels.Gender },
            new MappingFieldConfig { FieldName = "Marriage", DisplayName = Labels.Marriage },
            new MappingFieldConfig { FieldName = "CountryCode", DisplayName = Labels.Nationality },
            new MappingFieldConfig { FieldName = "IdCardNumber", DisplayName = Labels.IDCardNo },
            new MappingFieldConfig { FieldName = "DateOfIdCard", DisplayName = Labels.IDCardIssueDate, DataType = MappingDataType.Date },
            new MappingFieldConfig { FieldName = "PlaceOfIdCard", DisplayName = Labels.IDCardPlaceOfIssue },
            new MappingFieldConfig { FieldName = "PassportNo", DisplayName = Labels.PassportNo },
            new MappingFieldConfig { FieldName = "PassportExp", DisplayName = Labels.PassportExp },
            new MappingFieldConfig { FieldName = "Qualification", DisplayName = Labels.Qualification },
            new MappingFieldConfig { FieldName = "Major", DisplayName = Labels.Major },
            new MappingFieldConfig { FieldName = "MilitaryFromDate", DisplayName = Labels.MilitaryFromDate, DataType = MappingDataType.Date },
            new MappingFieldConfig { FieldName = "MilitaryToDate", DisplayName = Labels.MilitaryToDate, DataType = MappingDataType.Date },
            new MappingFieldConfig { FieldName = "Phone", DisplayName = Labels.Cellphone },
            new MappingFieldConfig { FieldName = "HomePhone", DisplayName = Labels.HomePhone },
            new MappingFieldConfig { FieldName = "Email", DisplayName = Labels.WorkEmail },
            new MappingFieldConfig { FieldName = "PermanentAddress", DisplayName = Labels.Address },
            new MappingFieldConfig { FieldName = "CurrentAddress", DisplayName = Labels.CurrentAddress },
            new MappingFieldConfig { FieldName = "PickupRemark", DisplayName = Labels.PickupRemark },
            new MappingFieldConfig { FieldName = "Skype", DisplayName = Labels.Skype },
            new MappingFieldConfig { FieldName = "LetsTalk", DisplayName = Labels.LetsTalk },
            new MappingFieldConfig { FieldName = "Unit", DisplayName = Labels.Unit },
            new MappingFieldConfig { FieldName = "TeamId", DisplayName = Labels.Team },
            new MappingFieldConfig { FieldName = "TeamDate", DisplayName = Labels.TeamDate, DataType = MappingDataType.Date },
            new MappingFieldConfig { FieldName = "JobTitleId", DisplayName = Labels.JobTitle },
            new MappingFieldConfig { FieldName = "JobLevelId", DisplayName = Labels.JobLevel },
            new MappingFieldConfig { FieldName = "ExpertGradeId", DisplayName = Labels.ExpertGrade },
            new MappingFieldConfig { FieldName = "PromotionDate", DisplayName = Labels.PromotionDate, DataType = MappingDataType.Date },
            new MappingFieldConfig { FieldName = "JoinedDate", DisplayName = Labels.GroupDate },
            new MappingFieldConfig { FieldName = "CompanyDate", DisplayName = Labels.CompanyDate },
            new MappingFieldConfig { FieldName = "ConfirmedDate", DisplayName = Labels.ConfirmedDate },
            new MappingFieldConfig { FieldName = "ResignedDate", DisplayName = Labels.ResignedDate },
            new MappingFieldConfig { FieldName = "ResignedReason", DisplayName = Labels.ResignedReason },
            new MappingFieldConfig { FieldName = "CanOT", DisplayName = Labels.AllowClaimOT, DataType = MappingDataType.Boolean },
            new MappingFieldConfig { FieldName = "SpecialOT", DisplayName = Labels.SpecialOT },
            new MappingFieldConfig { FieldName = "WorkingDays", DisplayName = Labels.WorkingDaysPerMonth },
            new MappingFieldConfig { FieldName = "WorkingHours", DisplayName = Labels.WorkingHoursPerDay },
            new MappingFieldConfig { FieldName = "OffDaysbyWeek", DisplayName = Labels.NumberOfOffday },
            new MappingFieldConfig { FieldName = "RoleIds", DisplayName = Labels.Roles },
            new MappingFieldConfig { FieldName = "IsTopManager", DisplayName = Labels.SpecialUser, DataType = MappingDataType.Boolean },
            new MappingFieldConfig { FieldName = "IsLocal", DisplayName = Labels.Category, DataType = MappingDataType.Boolean },
            new MappingFieldConfig { FieldName = "IsExpatUseHoliday", DisplayName = Labels.ExpatApplicableHoliday, DataType = MappingDataType.Boolean },
            new MappingFieldConfig { FieldName = "Enabled", DisplayName = Labels.EnabledUser, DataType = MappingDataType.Boolean },
            new MappingFieldConfig { FieldName = "IsDeleted", DisplayName = Labels.Deleted, DataType = MappingDataType.Boolean },
            new MappingFieldConfig { FieldName = "ChineseName", DisplayName = Labels.ChineseName },
            new MappingFieldConfig { FieldName = "WorkingHoursbyWeek", DisplayName = Labels.WorkingHoursPerWeek }
        };

        private readonly IUserControlSession userControlSession;
        private readonly ICommonDataService commonDataService;
        private readonly IActionLogService actionLogService;
        private readonly IJobLevelService jobLevelService;
        private readonly IRoleService roleService;

        private IEnumerable<Team> Teams { get; set; }
        private IEnumerable<LookupData> JobTitles { get; set; }
        private IEnumerable<JobLevel> JobLevel { get; set; }
        private IEnumerable<Country> Countries { get; set; }
        private IEnumerable<Role> SystemRoles { get; set; }
        private IEnumerable<EntityBase<byte>> Genders { get; set; }
        private IEnumerable<EntityBase<byte>> MarriageStatuses { get; set; }

        public EmployeeLogService(
            IUserControlSession userControlSession,
            ICommonDataService commonDataService,
            IActionLogService actionLogService,
            IJobLevelService jobLevelService,
            IRoleService roleService)
        {
            this.userControlSession = userControlSession;
            this.commonDataService = commonDataService;
            this.actionLogService = actionLogService;
            this.jobLevelService = jobLevelService;
            this.roleService = roleService;
        }

        public IEnumerable<LogObjectDetail> GetLogDetailById(int employeeId)
        {
            Teams = commonDataService.GetAllTeam();
            JobTitles = commonDataService.GetLookupDataByCode(Constants.JobTitle);
            JobLevel = jobLevelService.GetJobLevel();
            Countries = commonDataService.GetAllCountry();
            SystemRoles = roleService.GetBySiteId(userControlSession.SiteId);
            Genders = commonDataService.GetGender();
            MarriageStatuses = commonDataService.GetMarriageStatus();
            IEnumerable<LogObjectDetail> logs = actionLogService.GetLogDetailById(LogObjectType.Employee, employeeId);
            MappingField(logs);

            return logs;
        }

        private void MappingField(IEnumerable<LogObjectDetail> logs)
        {
            foreach (LogObjectDetail log in logs)
            {
                foreach (FieldValue field in log.Fields)
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

                case "ExpertGradeId":
                    MappingJobLevel(field);
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

                default:
                    // nothing
                    break;
            }
        }

        private void MappingTeam(FieldValue field)
        {
            int.TryParse(field.OldValue, out int teamId);
            field.OldValue = Teams.FirstOrDefault(x => x.Id == teamId)?.Name;

            int.TryParse(field.Value, out teamId);
            field.Value = Teams.FirstOrDefault(x => x.Id == teamId)?.Name;
        }

        private void MappingRole(FieldValue field)
        {
            var roles = !string.IsNullOrWhiteSpace(field.OldValue) ? field.OldValue.Split(',') : new string[] { };
            field.OldValue = string.Join(", ", SystemRoles.Where(x => roles.Contains(x.RoleId.ToString())).Select(x => x.Name));

            roles = !string.IsNullOrWhiteSpace(field.Value) ? field.Value.Split(',') : new string[] { };
            field.Value = string.Join(", ", SystemRoles.Where(x => roles.Contains(x.RoleId.ToString())).Select(x => x.Name));
        }

        private void MappingJobTitle(FieldValue field)
        {
            int.TryParse(field.OldValue, out int jobTitleId);
            field.OldValue = JobTitles.FirstOrDefault(x => x.Id == jobTitleId)?.Name;

            int.TryParse(field.Value, out jobTitleId);
            field.Value = JobTitles.FirstOrDefault(x => x.Id == jobTitleId)?.Name;
        }

        private void MappingJobLevel(FieldValue field)
        {
            int.TryParse(field.OldValue, out int jobLevelId);
            field.OldValue = JobLevel.FirstOrDefault(x => x.Id == jobLevelId)?.Name;

            int.TryParse(field.Value, out jobLevelId);
            field.Value = JobLevel.FirstOrDefault(x => x.Id == jobLevelId)?.Name;
        }

        private void MappingCountry(FieldValue field)
        {
            field.OldValue = Countries.FirstOrDefault(x => string.Equals(x.CountryCode, field.OldValue, StringComparison.OrdinalIgnoreCase))?.CountryName;
            field.Value = Countries.FirstOrDefault(x => string.Equals(x.CountryCode, field.Value, StringComparison.OrdinalIgnoreCase))?.CountryName;
        }

        private void MappingGender(FieldValue field)
        {
            byte.TryParse(field.OldValue, out byte gender);
            field.OldValue = Genders.FirstOrDefault(x => x.Id == gender)?.Name;

            byte.TryParse(field.Value, out gender);
            field.Value = Genders.FirstOrDefault(x => x.Id == gender)?.Name;
        }

        private void MappingMarriage(FieldValue field)
        {
            byte.TryParse(field.OldValue, out byte gender);
            field.OldValue = MarriageStatuses.FirstOrDefault(x => x.Id == gender)?.Name;

            byte.TryParse(field.Value, out gender);
            field.Value = MarriageStatuses.FirstOrDefault(x => x.Id == gender)?.Name;
        }

        private static void MappingCategory(FieldValue field)
        {
            field.OldValue = field.OldValue == "1" ? Labels.Local : Labels.Expat;
            field.Value = field.Value == "1" ? Labels.Local : Labels.Expat;
        }
    }
}