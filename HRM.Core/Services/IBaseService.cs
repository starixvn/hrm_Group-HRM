namespace HRM.Core.Services
{
    using System;
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

        Task<T> GetDataEntityAsync<T>(ICriteria criteria);

        ValueTuple<IEnumerable<TFirst>, IEnumerable<TSecond>> FetchMultiple<TFirst, TSecond>(ICriteria criteria);

        ValueTuple<IEnumerable<TFirst>, IEnumerable<TSecond>, IEnumerable<TThird>> FetchMultiple<TFirst, TSecond, TThird>(ICriteria criteria);
    }
}