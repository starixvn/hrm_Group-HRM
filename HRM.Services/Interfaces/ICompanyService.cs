using System.Collections.Generic;
using System.Threading.Tasks;
using HRM.Models;

namespace HRM.Services.Interfaces
{
    public interface ICompanyService
    {
        Task<CompanySite> GetCompanySiteByDomainAsync();

        Task<IEnumerable<Company>> GetAllCompany();

        Task<string> GetDomain(int companyId);

        Task<IEnumerable<SubCompany>> GetSubCompanies(int companyId);
    }
}
