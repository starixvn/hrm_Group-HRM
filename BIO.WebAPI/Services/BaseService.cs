using System.Collections.Generic;
using System.Threading.Tasks;
using Fanex.Data.Repository;

namespace BIO.WebAPI.Services;

public interface IBaseService
{
    Task<IEnumerable<T>> GetDataAsync<T>(ICriteria criteria);
}

public class BaseService(IDynamicRepository dynamicRepository) : IBaseService
{
    public async Task<IEnumerable<T>> GetDataAsync<T>(ICriteria criteria)
    {
        if (criteria.IsValid())
        {
            return await dynamicRepository.FetchAsync<T>(criteria);
        }

        return new List<T>();
    }
}