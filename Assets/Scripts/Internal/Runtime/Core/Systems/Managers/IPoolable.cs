using UnityEngine;

namespace Assets.Scripts.Internal.Runtime.Core.Systems.Managers
{
    public interface IPoolable
    {
        bool IsPoolable { get; }
        Transform Transform { get; }
        
        IPoolable OnReuse();
    }
}
