namespace ElusiveWorld.Core.Assets.Scripts.Systems.Game.Services
{
    public interface IServiceLocator
    {
        static IServiceLocator Default { get; set; }

        static IServiceLocator() => Default = new DefaultServiceLocator();

        T GetService<T>() where T : IService;

        bool TryRegisterService<T>(T service) where T : class, IService
            => TryRegisterService<T, T>(service);
        bool TryRegisterService<TContract, TImplementation>(TImplementation service)
            where TContract : class, IService
            where TImplementation : class, TContract;

        bool TryUnregisterService<T>(T service) where T : class, IService
            => TryUnregisterService<T, T>(service);
        bool TryUnregisterService<TContract, TImplementation>(TImplementation service)
            where TContract : class, IService
            where TImplementation : class, TContract;
    }
}
