// File: ObjectPool.cs
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages a pool of a single type of GameObject.
/// </summary>
[System.Serializable]
public class ObjectPool
{
    private Queue<GameObject> pool = new Queue<GameObject>();
    private GameObject prefab;
    private Transform parent;

    public ObjectPool(GameObject prefab, int initialSize, Transform parent)
    {
        this.prefab = prefab;
        this.parent = parent;
        Expand(initialSize);
    }

    /// <summary>
    /// Gets a GameObject from the pool.
    /// </summary>
    public GameObject GetObject()
    {
        if (pool.Count == 0)
        {
            Debug.LogWarning($"Pool for {prefab.name} is empty. Expanding...");
            Expand(3); // Default expansion size
        }

        GameObject obj = pool.Dequeue();
        obj.transform.SetParent(null); // Unparent from the pool container
        obj.SetActive(true);
        return obj;
    }

    /// <summary>
    /// Returns a GameObject to the pool.
    /// </summary>
    public void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);
        obj.transform.SetParent(parent); // Re-parent to the pool container
        pool.Enqueue(obj);
        //Debug.Log($"pool count for{obj} is{pool.Count}");
    }

    /// <summary>
    /// Creates and adds new objects to the pool.
    /// </summary>
    private void Expand(int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject obj = GameObject.Instantiate(prefab, parent);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
    }
}