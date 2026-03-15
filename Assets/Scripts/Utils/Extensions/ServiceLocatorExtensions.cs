using ElusiveWorld.Core.Assets.Scripts.Systems.Game.Services;
using System;

public static class ServiceLocatorExtensions
{
    public static bool TryRegisterService(this IServiceLocator locator, Type serviceType, object service)
    {
        var method = typeof(IServiceLocator).GetMethod(nameof(IServiceLocator.TryRegisterService))
            .MakeGenericMethod(serviceType);

        return (bool)method.Invoke(locator, new[] { service });
    }

    public static bool TryUnregisterService(this IServiceLocator locator, Type serviceType, object service)
    {
        var method = typeof(IServiceLocator).GetMethod(nameof(IServiceLocator.TryUnregisterService))
            .MakeGenericMethod(serviceType);

        return (bool)method.Invoke(locator, new[] { service });
    }
}