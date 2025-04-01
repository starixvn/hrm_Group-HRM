using System.Threading.Tasks;
using HRM.Models;

namespace HRM.Services.Interfaces
{
    public interface ICompanyService
    {
        Task<CompanySite> GetCompanySiteByDomainAsync();
    }
}
