using System.Collections.Generic;
using UnityEngine;

public static class EntityManager
{
    private static Dictionary<EntityType, List<Entity>> entities = new Dictionary<EntityType, List<Entity>>();

    public static void Register(EntityType type, Entity entity)
    {
        if (!entities.ContainsKey(type))
            entities[type] = new List<Entity>();

        if (!entities[type].Contains(entity))
            entities[type].Add(entity);
    }

    public static void Unregister(EntityType type, Entity entity)
    {
        if (entities.ContainsKey(type))
        {
            entities[type].Remove(entity);
            if (entities[type].Count == 0)
                entities.Remove(type);
        }
    }

    // Get the first entity of this type (e.g. Player)
    public static T GetSingle<T>(EntityType type) where T : Entity
    {
        if (entities.TryGetValue(type, out var list) && list.Count > 0)
            return list[0] as T;

        //Debug.LogWarning($"[EntityManager] Entity of type {type} not found!");
        return null;
    }

    // Get all entities of this type (e.g. NPCs)
    public static List<T> GetAll<T>(EntityType type) where T : Entity
    {
        if (entities.TryGetValue(type, out var list))
            return list.ConvertAll(e => e as T);

        return new List<T>();
    }

    public static void Clear()
    {
        entities.Clear();
    }
}
