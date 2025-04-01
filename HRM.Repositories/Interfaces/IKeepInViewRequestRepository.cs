using System.Collections.Generic;
using System.Threading.Tasks;
using Fanex.Data.Repository;
using HRM.Models.Request;

namespace HRM.Repositories.Interfaces;

public interface IKeepInViewRequestRepository
{
    Task<IList<KeepInViewRequest>> GetByDateAsync(ICriteria criteria);
    Task<KeepInViewRequest> GetByIdAsync(ICriteria criteria);
    Task<IList<KeepInViewRequest>> GetHistoryByIdAsync(ICriteria criteria);
}