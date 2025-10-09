// File: ObjectPoolConfig.cs
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ObjectPoolConfig", menuName = "Configs/Object Pool Config")]
public class ObjectPoolConfig : ScriptableObject
{
    [System.Serializable]
    public class PoolConfig
    {
        public GameObject prefab;
        public int initialSize = 10;
    }

    public List<PoolConfig> poolConfigs = new List<PoolConfig>();
}