using System;
using System.Collections.Generic;

namespace ElusiveWorld.Core.Assets.Scripts.Utils.Services
{
    public class DefaultServiceLocator : IServiceLocator
    {
        readonly Dictionary<Type, IService> services = new();

        public virtual T GetService<T>() where T : IService
        {
            var type = typeof(T);
            if (services.TryGetValue(type, out var value)) return (T)value;
            return default;
        }

        public virtual bool TryRegisterService<TContract, TImplementation>(TImplementation service)
            where TContract : class, IService
            where TImplementation : class, TContract
        {
            var contractType = typeof(TContract);
            if (!services.TryAdd(contractType, service)) return false;

            var implementationType = typeof(TImplementation);
            if (contractType == implementationType) return true;

            if (!services.TryAdd(implementationType, service))
            {
                services.Remove(contractType);
                return false;
            }

            return true;
        }

        public virtual bool TryUnregisterService<TContract, TImplementation>(TImplementation service)
            where TContract : class, IService
            where TImplementation : class, TContract
        {
            var contractType = typeof(TContract);
            var isRegistrationExists = services.TryGetValue(contractType, out var existing);
            if (!isRegistrationExists || !ReferenceEquals(existing, service)) return false;

            var isContractRemoved = services.Remove(contractType);
            var implementationType = typeof(TImplementation);
            if (isContractRemoved && implementationType == contractType) return true;

            var isImplementationRemoved = TryUnregisterService<TContract, TImplementation>(service);
            if (!isImplementationRemoved)
            {
                services.Add(contractType, service);
                return false;
            }

            return true;
        }
    }
}
