using System.Collections.Generic;
using UnityEngine;

namespace VHS
{
    public class PoolManager : Singleton<PoolManager>
    {
        readonly Dictionary<IPoolable, List<IPoolable>> pool = new();

        public static IPoolable SpawnPoolable(
            IPoolable poolable, Vector3 position, Quaternion rotation,
            Vector3 scale, Transform parent = null)
        {
            var poolableInstance = Instance.GetPoolable(poolable);
            if (poolableInstance == null) return null;
            poolableInstance.Transform.SetPositionAndRotation(position, rotation);
            poolableInstance.Transform.SetParent(parent);
            poolableInstance.Transform.localScale = scale;
            poolableInstance.Transform.gameObject.SetActive(true);
            return poolableInstance.OnReuse();
        }

        IPoolable GetPoolable(IPoolable poolable)
        {
            List<IPoolable> poolableList;
            if (!pool.ContainsKey(poolable))
            {
                CreateNewPool();

                void CreateNewPool()
                {
                    poolableList = new List<IPoolable>();
                    pool[poolable] = poolableList;

                    CreateNewPoolable();
                }
            }
            else
            {
                poolableList = pool[poolable];
                for (var i = poolableList.Count - 1; i >= 0; i--)
                    if (poolableList[i].IsPoolable)
                        return poolableList[i];
                
                CreateNewPoolable();
            }

            IPoolable poolableInstance;
            void CreateNewPoolable()
            {
                poolableInstance = Instantiate(poolable.Transform.gameObject)
                    .GetComponentInChildren<IPoolable>();
                poolableList.Add(poolableInstance);
            }
            return poolableInstance;
        }
    }
}
