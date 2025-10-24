using Fanex.Data;

namespace HRM.Core.Services;

public interface IObjectDbFactory
{
    IObjectDb CreateInstance(string key);
}

public class ObjectDbFactoryWrapper : IObjectDbFactory
{
    public IObjectDb CreateInstance(string key)
        => ObjectDbFactory.CreateInstance(key);
}
