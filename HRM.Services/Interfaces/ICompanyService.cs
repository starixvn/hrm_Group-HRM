using System.Collections.Generic;
using System.Threading.Tasks;
using HRM.Models;

namespace HRM.Services.Interfaces;

public interface ICompanyService
{
    Task<IEnumerable<CompanySite>> GetAllCompanySiteAsync();

    Task<CompanySite> GetCompanySiteByDomainAsync();

    Task<IEnumerable<Company>> GetAllCompany();

    Task<IEnumerable<SubCompany>> GetSubCompanies(short? companyId);

    Task<IEnumerable<CompanySetting>> GetSettingCompanyAsync();

    Task<IEnumerable<Currency>> GetCurrencyAsync();

    Task<string> GetCompanyCurrencyId();

    Task<CompanySetting> GetByIdAsync(short companyId);

    Task<string> GetWhitelistIps();

    Task<IEnumerable<CompanySite>> GetAllDomainAsync(short? companyId = null);
}