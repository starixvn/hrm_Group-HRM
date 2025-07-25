using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Threading.Tasks;
using GO.UserControl.Client.Core.Session;
using HRM.Core.Constants;
using HRM.Core.Services;
using HRM.Criteria.Company;
using HRM.Criteria.CompanySite;
using HRM.Criteria.SystemSetting;
using HRM.Models;
using HRM.Services.Interfaces;
using Microsoft.AspNetCore.Http;

namespace HRM.Services.Implement;

public class CompanyService(
    IBaseService baseService,
    IHttpContextAccessor httpContextAccessor,
    IUserControlSession userControlSession) : ICompanyService
{
    public async Task<CompanySite> GetCompanySiteByDomainAsync()
    {
        var domainName = GetDomainName();
        var criteria = new GetCompanySiteByDomainCriteria
        {
            Domain = domainName
        };
        return await baseService.GetDataEntityAsync<CompanySite>(criteria);
    }

    public async Task<IEnumerable<Company>> GetAllCompany()
    {
        var criteria = new GetAllCompanyCriteria();
        return await baseService.GetDataAsync<Company>(criteria);
    }

    public async Task<string> GetDomain(int companyId)
    {
        GetDomainByCompanyIdCriteria criteria = new()
        {
            CompanyId = companyId
        };
        var mailConfig = await baseService.GetDataEntityAsync<string>(criteria);

        return mailConfig;
    }

    public async Task<IEnumerable<SubCompany>> GetSubCompanies(int? companyId)
    {
        var criteria = new GetSubCompaniesCriteria
        {
            CompanyId = companyId
        };

        return await baseService.GetDataAsync<SubCompany>(criteria);
    }

    public async Task<bool> GetIsShowSubCompanyValue()
    {
        var currentCompanyId = userControlSession.Get<short>(SessionKey.CurrentCompanyId);
        var criteria = new GetSystemConfigsByKeyCriteria
        {
            CompanyId = currentCompanyId,
            Key = SystemConfigKey.IsShowSubCompany
        };

        return await baseService.GetDataEntityAsync<bool>(criteria);
    }

    public async Task<string> GetWhitelistIps()
    {
        var currentCompanyId = userControlSession.Get<short>(SessionKey.CurrentCompanyId);
        var criteria = new GetSystemConfigsByKeyCriteria
        {
            CompanyId = currentCompanyId,
            Key = SystemConfigKey.WhitelistIPs
        };

        return await baseService.GetDataEntityAsync<string>(criteria);
    }

    public string GetDomainName()
    {
        var httpContext = httpContextAccessor.HttpContext
                          ?? throw new InvalidOperationException("HttpContext is unavailable.");
        var protocol = httpContext.Request.Headers["x-forwarded-proto"].ToString();
        return $"{(!string.IsNullOrWhiteSpace(protocol) ? protocol : httpContext.Request.Scheme)}://{httpContext.Request.Host.Value}";
    }

    public async Task<IEnumerable<CompanySetting>> GetSettingCompanyAsync()
    {
        var criteria = new GetCompanyCriteria();
        return await baseService.GetDataAsync<CompanySetting>(criteria);
    }

    public async Task<List<Currency>> GetCurrencyAsync()
    {
        var criteria = new GetCurrencyCriteria();
        var result = await baseService.GetDataAsync<Currency>(criteria);
        return result.ToList();
    }

    public async Task<CompanySetting> GetByIdAsync(int companyId)
    {
        var criteria = new GetByIdCriteria
        {
            CompanyId = companyId
        };
        return await baseService.GetDataEntityAsync<CompanySetting>(criteria);
    }

    public async Task<List<SubCompanyEmployee>> CountEmployeeById(int companyId)
    {
        var criteria = new CountEmployeeByIdCriteria
        {
            CompanyId = companyId
        };
        var result = await baseService.GetDataAsync<SubCompanyEmployee>(criteria);
        return result.ToList();
    }
}