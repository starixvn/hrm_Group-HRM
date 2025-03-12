using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fanex.Data.Repository;

namespace HRM.AutoJobs.Services.Interfaces;

public interface IBaseService
{
    Task<IEnumerable<T>> GetDataAsync<T>(ICriteria criteria);

    Task<T> GetDataEntityAsync<T>(ICriteria criteria);

    Task<ValueTuple<IEnumerable<TFirst>, IEnumerable<TSecond>>> FetchMultipleAsync<TFirst, TSecond>(ICriteria criteria);

    Task<ValueTuple<IEnumerable<TFirst>, IEnumerable<TSecond>, IEnumerable<TThird>>> FetchMultipleAsync<TFirst, TSecond, TThird>(ICriteria criteria);

    Task ExecuteNonQueryAsync(INonQueryCommand command);
}