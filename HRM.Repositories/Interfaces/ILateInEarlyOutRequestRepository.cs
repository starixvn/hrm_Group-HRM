using System.Collections.Generic;
using System.Threading.Tasks;
using Fanex.Data.Repository;
using HRM.Models.Request;

namespace HRM.Repositories.Interfaces;

public interface ILateInEarlyOutRequestRepository
{
    Task<IList<LateInEarlyOutRequest>> GetByDateAsync(ICriteria criteria);
    Task<LateInEarlyOutRequest> GetByIdAsync(ICriteria criteria);
    Task<IList<LateInEarlyOutRequest>> GetHistoryByIdAsync(ICriteria criteria);
}