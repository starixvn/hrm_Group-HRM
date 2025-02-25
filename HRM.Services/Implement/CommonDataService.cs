namespace HRM.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using HRM.Core.Enums;
    using HRM.Core.Helpers;
    using HRM.Core.Models;
    using HRM.Core.Services;
    using HRM.Criteria.Country;
    using HRM.Criteria.LeaveType;
    using HRM.Criteria.Lookup;
    using HRM.Criteria.SystemSetting;
    using HRM.Models;
    using HRM.Resources;

    public interface ICommonDataService
    {
        IList<LookupData> GetLookupDataByCode(string code);

        IList<LookupData> GetJobTitle(string jobTitle);

        IList<LeaveType> GetLeaveType(bool excludeNotShown = true);

        IList<Team> GetAllTeam(bool isDeleted = false);

        IList<Country> GetAllCountry();

        IEnumerable<EntityBase<byte>> GetMarriageStatus();

        IEnumerable<EntityBase<byte>> GetGender();

        SystemSetting GetSystemSetting();

        bool IsEnableGroupDate();
    }

    public class CommonDataService(
        IBaseService baseService,
        ITeamService teamService) : ICommonDataService
    {
        public IList<LookupData> GetLookupDataByCode(string code)
        {
            var criteria = new GetAllLookupDataCriteria
            {
                LookupCode = code
            };
            return baseService.GetData<LookupData>(criteria).ToList();
        }

        public IList<LookupData> GetJobTitle(string jobTitle)
        {
            var criteria = new GetJobTitleCriteria
            {
                JobTitle = jobTitle
            };
            return baseService.GetData<LookupData>(criteria).ToList();
        }

        public IList<LeaveType> GetLeaveType(bool excludeNotShown = true)
        {
            var criteria = new GetLeaveTypeCriteria
            {
                ExcludeNotShown = excludeNotShown
            };
            var leaveTypes = baseService.GetData<LeaveType>(criteria).ToList();
            leaveTypes.Insert(0, new LeaveType { Id = (ushort)LeaveTypes.GeneralLeave, Name = Labels.GeneralLeave });

            return leaveTypes;
        }

        public IList<Team> GetAllTeam(bool isDeleted = false)
        {
            return teamService.GetAll(isDeleted);
        }

        public IList<Country> GetAllCountry()
        {
            var criteria = new GetAllCountryCriteria();
            return baseService.GetData<Country>(criteria).ToList();
        }

        public IEnumerable<EntityBase<byte>> GetMarriageStatus()
        {
            return new List<EntityBase<byte>>
            {
                new EntityBase<byte>
                {
                    Id = (byte)MarriageStatus.Default,
                    Name = EnumHelper.GetDescription(MarriageStatus.Default)
                },
                new EntityBase<byte>
                {
                    Id = (byte)MarriageStatus.Married,
                    Name = EnumHelper.GetDescription(MarriageStatus.Married)
                },
                new EntityBase<byte>
                {
                    Id = (byte)MarriageStatus.Divorced,
                    Name = EnumHelper.GetDescription(MarriageStatus.Divorced)
                },
                new EntityBase<byte>
                {
                    Id = (byte)MarriageStatus.Single,
                    Name = EnumHelper.GetDescription(MarriageStatus.Single)
                },
                new EntityBase<byte>
                {
                    Id = (byte)MarriageStatus.Widow,
                    Name = EnumHelper.GetDescription(MarriageStatus.Widow)
                }
            };
        }

        public IEnumerable<EntityBase<byte>> GetGender()
        {
            return new List<EntityBase<byte>>
            {
                new EntityBase<byte>
                {
                    Id = (byte)Gender.Female,
                    Name = EnumHelper.GetDescription(Gender.Female)
                },
                new EntityBase<byte>
                {
                    Id = (byte)Gender.Male,
                    Name = EnumHelper.GetDescription(Gender.Male)
                },
                new EntityBase<byte>
                {
                    Id = (byte)Gender.Unknown,
                    Name = EnumHelper.GetDescription(Gender.Unknown)
                }
            };
        }

        public SystemSetting GetSystemSetting()
        {
            var criteria = new GetSystemSettingCriteria();
            return baseService.GetDataEntity<SystemSetting>(criteria);
        }

        public bool IsEnableGroupDate()
        {
            var criteria = new GetSystemConfigsByKeyCriteria
            {
                Key = "EnableGroupDate"
            };

            return baseService.GetDataEntity<bool>(criteria);
        }
    }
}