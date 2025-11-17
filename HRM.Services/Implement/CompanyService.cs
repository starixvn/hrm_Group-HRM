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
    IUserControlSession session) : ICompanyService
{
    public async Task<IEnumerable<CompanySite>> GetAllCompanySiteAsync()
    {
        var criteria = new GetAllCompanySiteCriteria();
        return await baseService.GetDataAsync<CompanySite>(criteria);
    }

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

    public async Task<IEnumerable<SubCompany>> GetSubCompanies(short? companyId)
    {
        var criteria = new GetSubCompaniesCriteria
        {
            CompanyId = companyId
        };

        return await baseService.GetDataAsync<SubCompany>(criteria);
    }

    public async Task<string> GetWhitelistIps()
    {
        var currentCompanyId = session.Get<short>(SessionKey.CurrentCompanyId);
        var criteria = new GetSystemConfigsByKeyCriteria
        {
            CompanyId = currentCompanyId,
            Key = SystemConfigKey.WhitelistIPs
        };

        return await baseService.GetDataEntityAsync<string>(criteria);
    }

    public async Task<IEnumerable<CompanySetting>> GetSettingCompanyAsync()
    {
        var criteria = new GetCompanyCriteria();
        return await baseService.GetDataAsync<CompanySetting>(criteria);
    }

    public async Task<IEnumerable<Currency>> GetCurrencyAsync()
    {
        var criteria = new GetCurrencyCriteria();
        var currencies = (await baseService.GetDataAsync<Currency>(criteria)).ToList();

        var companyCurrencyId = await GetCompanyCurrencyId();

        if (string.IsNullOrEmpty(companyCurrencyId))
            return currencies;

        return [.. currencies
            .OrderBy(c => c.Id == int.Parse(companyCurrencyId) ? 0 : 1)
            .ThenBy(c => c.Id)];
    }

    public async Task<string> GetCompanyCurrencyId()
    {
        var companyId = session.Get<short>(SessionKey.CurrentCompanyId);

        var criteria = new GetSystemConfigsByKeyCriteria
        {
            CompanyId = companyId,
            Key = SystemConfigKey.Currency
        };

        return await baseService.GetDataEntityAsync<string>(criteria);
    }

    public async Task<CompanySetting> GetByIdAsync(short companyId)
    {
        var criteria = new GetByIdCriteria
        {
            CompanyId = companyId
        };
        var (companies, subCompanyEmployees) = await baseService.FetchMultipleAsync<CompanySetting, SubCompanyEmployee>(criteria);
        var company = companies.FirstOrDefault();
        if (company != null)
        {
            company.SubCompanyEmployees = subCompanyEmployees.ToList() ?? [];

        }

        return company;
    }

    public async Task<IEnumerable<CompanySite>> GetAllDomainAsync(short? companyId = null)
    {
        var criteria = new GetDomainCriteria
        {
            CompanyId = companyId
        };
        return await baseService.GetDataAsync<CompanySite>(criteria);
    }

    private string GetDomainName()
    {
        var httpContext = httpContextAccessor.HttpContext
                          ?? throw new InvalidOperationException("HttpContext is unavailable.");
        var protocol = httpContext.Request.Headers["x-forwarded-proto"].ToString();
        return $"{(!string.IsNullOrWhiteSpace(protocol) ? protocol : httpContext.Request.Scheme)}://{httpContext.Request.Host.Value}";
    }
}