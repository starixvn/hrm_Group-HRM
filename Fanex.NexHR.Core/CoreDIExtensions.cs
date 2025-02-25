namespace Fanex.NexHR.Core
{
    using Fanex.Data.Repository;
    using Fanex.NexHR.Core.Repositories;
    using Fanex.NexHR.Core.Services;
    using Unity;

    public static class CoreDIExtensions
    {
        public static void ResolveCoreDI(this IUnityContainer container)
        {
            container.RegisterType<IDynamicRepository, DynamicRepository>(TypeLifetime.Singleton);
            container.RegisterType<IBaseRepository, BaseRepository>(TypeLifetime.Singleton);
            container.RegisterType<IBaseService, BaseService>(TypeLifetime.Singleton);
        }
    }
}