using System.Collections.Generic;
using System.Threading.Tasks;
using HRM.Models;

namespace HRM.Services.Interfaces
{
    public interface ICompanyService
    {
        Task<CompanySite> GetCompanySiteByDomainAsync();

        Task<IEnumerable<Company>> GetAllCompany();

        Task<string> GetDomain(short companyId);

        Task<IEnumerable<SubCompany>> GetSubCompanies(short? companyId);

        Task<bool> GetIsShowSubCompanyValue();

        Task<IEnumerable<CompanySetting>> GetSettingCompanyAsync();

        Task<IEnumerable<Currency>> GetCurrencyAsync();

        Task<CompanySetting> GetByIdAsync(short companyId);

        Task<string> GetWhitelistIps();

        Task<IEnumerable<CompanySite>> GetAllDomainAsync(short? companyId = null);
    }
}