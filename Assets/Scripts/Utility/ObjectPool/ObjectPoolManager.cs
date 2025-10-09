// File: ObjectPoolManager.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : MonoSingleton<ObjectPoolManager>
{
    [System.Serializable]
    public class PoolConfig
    {
        public GameObject prefab;
        public int initialSize = 10;
    }

    [Header("∂‘œÛ≥ÿ≈‰÷√")]
    [SerializeField] private ObjectPoolConfig objectPoolConfig;

    private Dictionary<GameObject, ObjectPool> _prefabPoolMap = new Dictionary<GameObject, ObjectPool>();
    protected override void Awake()
    {
        base.Awake();
        InitializePools(objectPoolConfig.poolConfigs);
    }
    private void InitializePools(List<ObjectPoolConfig.PoolConfig> configs)
    {
        foreach (var config in configs)
        {
            if (config.prefab == null)
            {
                Debug.LogError($"Pool config error: prefab is null.");
                continue;
            }

            if (_prefabPoolMap.ContainsKey(config.prefab))
            {
                Debug.LogWarning($"Pool for prefab {config.prefab.name} already exists. Skipping.");
                continue;
            }

            GameObject poolParent = new GameObject($"Pool_{config.prefab.name}");
            poolParent.transform.SetParent(transform);
            _prefabPoolMap.Add(config.prefab, new ObjectPool(config.prefab, config.initialSize, poolParent.transform));
        }
    }

    /// <summary>
    /// Retrieves a GameObject from the appropriate pool.
    /// </summary>
    public GameObject GetObject(GameObject prefab)
    {
        if (prefab == null)
        {
            Debug.LogError("Attempted to get an object from a null prefab!");
            return null;
        }

        // Check if the pool exists. If not, create it dynamically (optional but good practice).
        if (!_prefabPoolMap.ContainsKey(prefab))
        {
            Debug.LogWarning($"No pool found for {prefab.name}. Creating a new pool.");
            GameObject dynamicPoolParent = new GameObject($"Pool_{prefab.name}_Dynamic");
            dynamicPoolParent.transform.SetParent(this.transform);
            _prefabPoolMap.Add(prefab, new ObjectPool(prefab, 1, dynamicPoolParent.transform));
        }

        return _prefabPoolMap[prefab].GetObject();
    }

    /// <summary>
    /// Returns a GameObject to its original pool.
    /// </summary>
    public void ReturnObject(GameObject prefab, GameObject obj)
    {
        if (prefab == null || obj == null)
        {
            Debug.LogError("Attempted to return a null object to the pool!");
            return;
        }

        if (!_prefabPoolMap.ContainsKey(prefab))
        {
            GameObject dynamicPoolParent = new GameObject($"Pool_{prefab.name}_Dynamic");
            dynamicPoolParent.transform.SetParent(this.transform);

            _prefabPoolMap.Add(prefab, new ObjectPool(prefab, 1, dynamicPoolParent.transform));
        }

        _prefabPoolMap[prefab].ReturnObject(obj);
    }
}