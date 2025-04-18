namespace HRM.Core.Services;

using System.Collections.Generic;
using System.Threading.Tasks;
using Fanex.Data.Repository;
using Models;
using Repositories;

public class BaseService(
    IDynamicRepository dynamicRepository,
    IBaseRepository baseRepository) : IBaseService
{
    public async Task<IEnumerable<T>> GetDataAsync<T>(ICriteria criteria)
    {
        if (criteria != null && criteria.IsValid())
        {
             var result = await dynamicRepository.FetchAsync<T>(criteria);
            return result;
        }

        return new List<T>();
    }

    public async Task<CommandResult> ExecuteNonQueryAsync(ICriteria command)
    {
        var commandResult = new CommandResult();
        if (command != null && command.IsValid())
        {
            commandResult = await baseRepository.ExecuteNonQueryAsync(command);
        }

        return commandResult;
    }

    public async Task<T> GetDataEntityAsync<T>(ICriteria criteria)
    {
        if (criteria != null && criteria.IsValid())
        {
            return await dynamicRepository.GetAsync<T>(criteria);
        }

        return default;
    }

    public async Task<(IEnumerable<TFirst>, IEnumerable<TSecond>)> FetchMultipleAsync<TFirst, TSecond>(ICriteria criteria)
    {
        if (criteria != null && criteria.IsValid())
        {
            return await dynamicRepository.FetchMultipleAsync<TFirst, TSecond>(criteria);
        }

        return (new List<TFirst>(), new List<TSecond>());
    }

    public async Task<(IEnumerable<TFirst>, IEnumerable<TSecond>, IEnumerable<TThird>)> FetchMultipleAsync<TFirst, TSecond, TThird>(ICriteria criteria)
    {
        if (criteria != null && criteria.IsValid())
        {
            return await dynamicRepository.FetchMultipleAsync<TFirst, TSecond, TThird>(criteria);
        }

        return (new List<TFirst>(), new List<TSecond>(), new List<TThird>());
    }
}