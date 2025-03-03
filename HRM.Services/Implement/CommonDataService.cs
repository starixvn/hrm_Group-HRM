namespace HRM.Services.Implement;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Enums;
using Core.Helpers;
using Criteria.Country;
using Criteria.LeaveType;
using Criteria.Lookup;
using Criteria.SystemSetting;
using HRM.Core.Models;
using HRM.Core.Services;
using Models;
using Resources;
using HRM.Services.Interfaces;

public interface ICommonDataService
{
    Task<IEnumerable<LookupData>> GetLookupDataByCodeAsync(string code);

    Task<List<LookupData>> GetJobTitle(string jobTitle);

    Task<IList<LeaveType>> GetLeaveTypeAsync(bool excludeNotShown = true);

    Task<IEnumerable<Team>> GetAllTeamAsync(bool isDeleted = false);

    IList<Country> GetAllCountry();

    Task<IList<Country>> GetAllCountryAsync();

    IEnumerable<EntityBase<byte>> GetMarriageStatus();

    IEnumerable<EntityBase<byte>> GetGender();

    Task<SystemSetting> GetSystemSettingAsync();

    Task<bool> IsEnableGroupDateAsync();

    bool IsEnableGroupDate();
}

public class CommonDataService(
    IBaseService baseService,
    ITeamService teamService) : ICommonDataService
{
    public async Task<IEnumerable<LookupData>> GetLookupDataByCodeAsync(string code)
    {
        var criteria = new GetAllLookupDataCriteria
        {
            LookupCode = code
        };
        return await baseService.GetDataAsync<LookupData>(criteria);
    }

    public async Task<List<LookupData>> GetJobTitle(string jobTitle)
    {
        var criteria = new GetJobTitleCriteria
        {
            JobTitle = jobTitle
        };
        return (await baseService.GetDataAsync<LookupData>(criteria)).ToList();
    }

    public async Task<IList<LeaveType>> GetLeaveTypeAsync(bool excludeNotShown = true)
    {
        var criteria = new GetLeaveTypeCriteria
        {
            ExcludeNotShown = excludeNotShown
        };
        var leaveTypes = (await baseService.GetDataAsync<LeaveType>(criteria)).ToList();
        leaveTypes.Insert(0, new LeaveType { Id = (ushort)LeaveTypes.GeneralLeave, Name = Labels.GeneralLeave });

        return leaveTypes;
    }

    public async Task<IEnumerable<Team>> GetAllTeamAsync(bool isDeleted = false)
    {
        return await teamService.GetAllAsync(isDeleted);
    }

    public IList<Country> GetAllCountry()
    {
        var criteria = new GetAllCountryCriteria();
        return baseService.GetData<Country>(criteria).ToList();
    }

    public async Task<IList<Country>> GetAllCountryAsync()
    {
        var criteria = new GetAllCountryCriteria();
        var data = await baseService.GetDataAsync<Country>(criteria);
        return data.ToList();
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

    public async Task<SystemSetting> GetSystemSettingAsync()
    {
        var criteria = new GetSystemSettingCriteria();
        return await baseService.GetDataEntityAsync<SystemSetting>(criteria);
    }

    public async Task<bool> IsEnableGroupDateAsync()
    {
        var criteria = new GetSystemConfigsByKeyCriteria
        {
            Key = "EnableGroupDate"
        };

        return await baseService.GetDataEntityAsync<bool>(criteria);
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