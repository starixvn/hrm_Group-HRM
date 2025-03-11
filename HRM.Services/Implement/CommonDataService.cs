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
using GO.UserControl.Client.Core.Session;
using HRM.Core.Constants;
using HRM.Core.Models;
using HRM.Core.Services;
using HRM.Services.Interfaces;
using Models;
using Resources;

public interface ICommonDataService
{
    Task<IEnumerable<LookupData>> GetLookupDataByCodeAsync(string code, int companyId);

    Task<List<LookupData>> GetJobTitle(string jobTitle, int companyId);

    Task<IList<LeaveType>> GetLeaveTypeAsync(bool excludeNotShown = true);

    Task<IEnumerable<Team>> GetAllTeamAsync(bool isDeleted = false);

    Task<List<Country>> GetAllCountry();

    Task<IList<Country>> GetAllCountryAsync();

    IEnumerable<EntityBase<byte>> GetMarriageStatus();

    IEnumerable<EntityBase<byte>> GetGender();

    Task<SystemSetting> GetSystemSettingAsync();

    Task<bool> IsEnableGroupDateAsync();
}

public class CommonDataService(
    IUserControlSession session,
    IBaseService baseService,
    ITeamService teamService) : ICommonDataService
{
    public async Task<IEnumerable<LookupData>> GetLookupDataByCodeAsync(string code, int companyId)
    {
        var criteria = new GetAllLookupDataCriteria
        {
            LookupCode = code,
            CompanyId = companyId
        };
        return await baseService.GetDataAsync<LookupData>(criteria);
    }

    public async Task<List<LookupData>> GetJobTitle(string jobTitle, int companyId)
    {
        var criteria = new GetJobTitleCriteria
        {
            JobTitle = jobTitle,
            CompanyId = companyId
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

    public async Task<List<Country>> GetAllCountry()
    {
        var criteria = new GetAllCountryCriteria();
        return (await baseService.GetDataAsync<Country>(criteria)).ToList();
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
            CompanyId = session.Get<short>(SessionKey.CurrentCompanyId),
            Key = "EnableGroupDate"
        };

        return await baseService.GetDataEntityAsync<bool>(criteria);
    }
}