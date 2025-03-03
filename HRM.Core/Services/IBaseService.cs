namespace HRM.Core.Services;

using System.Collections.Generic;
using System.Threading.Tasks;
using Fanex.Data.Repository;
using HRM.Core.Models;

public interface IBaseService
{
    IEnumerable<T> GetData<T>(ICriteria criteria);

    Task<IEnumerable<T>> GetDataAsync<T>(ICriteria criteria);

    T GetDataEntity<T>(ICriteria criteria);

    CommandResult ExecuteNonQuery(ICriteria criteria);

    Task<CommandResult> ExecuteNonQueryAsync(ICriteria command);

    Task<T> GetDataEntityAsync<T>(ICriteria criteria);

    Task<(IEnumerable<TFirst>, IEnumerable<TSecond>)> FetchMultipleAsync<TFirst, TSecond>(ICriteria criteria);

    Task<(IEnumerable<TFirst>, IEnumerable<TSecond>, IEnumerable<TThird>)> FetchMultipleAsync<TFirst, TSecond, TThird>(ICriteria criteria);
}