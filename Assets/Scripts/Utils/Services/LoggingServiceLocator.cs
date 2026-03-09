using UnityEngine;

namespace Assets.Scripts.Internal.Runtime.Core.Utils.Services
{
    public class LoggingServiceLocator : DefaultServiceLocator
    {
        public override T GetService<T>()
        {
            Debug.Log($"Requesting service of type: {typeof(T).Name}");
            return base.GetService<T>();
        }

        public override bool TryRegisterService<TContract, TImplementation>(TImplementation service)
        {
            Debug.Log($"Registering service of type: {typeof(TContract).Name}");
            return base.TryRegisterService<TContract, TImplementation>(service);
        }

        public override bool TryUnregisterService<TContract, TImplementation>(TImplementation service)
        {
            Debug.Log($"Unrequesting service of type: {typeof(TContract).Name}");
            return base.TryUnregisterService<TContract, TImplementation>(service);
        }
    }
}
