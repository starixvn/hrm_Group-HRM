using System.Collections.Generic;
using System.Threading.Tasks;
using Fanex.Data.Repository;
using HRM.Models.Request;

namespace HRM.Repositories.Interfaces;

public interface ICheckInOutRequestRepository
{
    Task<IList<CheckInOutRequest>> GetByDateAsync(ICriteria criteria);
    Task<CheckInOutRequest> GetByIdAsync(ICriteria criteria);
    Task<IList<CheckInOutRequest>> GetHistoryByIdAsync(ICriteria criteria);
}