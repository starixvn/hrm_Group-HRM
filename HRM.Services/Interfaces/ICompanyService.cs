using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HRM.Models;

namespace HRM.Services.Interfaces
{
    public interface ICompanyService
    {
        Task<CompanySite> GetCompanySiteByDomainAsync();
    }
}
