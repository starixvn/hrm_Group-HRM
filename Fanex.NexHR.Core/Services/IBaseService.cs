namespace Fanex.NexHR.Core.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Core.Models;

    public interface IBaseService
    {
        IEnumerable<T> GetData<T>(ICriteria criteria);

        T GetDataEntity<T>(ICriteria criteria);

        CommandResult ExecuteNonQuery(ICriteria criteria);

        Task<T> GetDataEntityAsync<T>(ICriteria criteria);

        ValueTuple<IEnumerable<TFirst>, IEnumerable<TSecond>> FetchMultiple<TFirst, TSecond>(ICriteria criteria);

        ValueTuple<IEnumerable<TFirst>, IEnumerable<TSecond>, IEnumerable<TThird>> FetchMultiple<TFirst, TSecond, TThird>(ICriteria criteria);
    }
}