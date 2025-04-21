using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HRM.Core.Services;
using HRM.Criteria.Company;
using HRM.Criteria.CompanySite;
using HRM.Models;
using HRM.Services.Interfaces;
using Microsoft.AspNetCore.Http;

namespace HRM.Services.Implement;

public class CompanyService(IBaseService baseService,
    IHttpContextAccessor httpContextAccessor) : ICompanyService
{
    public async Task<CompanySite> GetCompanySiteByDomainAsync()
    {
        var httpContext = httpContextAccessor.HttpContext
                          ?? throw new InvalidOperationException("HttpContext is unavailable.");
        var domainName = GetDomainName(httpContext);
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

    private static string GetDomainName(HttpContext httpContext)
    {
        var protocol = httpContext.Request.Headers["x-forwarded-proto"].ToString();
        return $"{(!string.IsNullOrWhiteSpace(protocol) ? protocol : httpContext.Request.Scheme)}://{httpContext.Request.Host.Value}";
    }
}