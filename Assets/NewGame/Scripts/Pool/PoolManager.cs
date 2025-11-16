using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using NTWUtils;

public class PoolManager : PersistentSingleton<PoolManager>
{
    // Pool categories for better organization
    public enum PoolCategory { Unit, UI_Panel, HUD, VFX, Others }

    private Dictionary<GameObject, Pool> prefabToPools = new();
    private Dictionary<GameObject, Pool> instanceToPools = new();

    private Dictionary<PoolCategory, Transform> categoryParents = new();

    protected override void Awake()
    {
        base.Awake();
        InitializeCategoryParents();
    }

    private void InitializeCategoryParents()
    {
        foreach (PoolCategory category in System.Enum.GetValues(typeof(PoolCategory)))
        {
            var parent = new GameObject($"Category_{category}");
            parent.transform.SetParent(transform);
            categoryParents[category] = parent.transform;
        }
    }

    public GameObject Spawn(GameObject prefab, Vector3 position = default, Transform parent = null, PoolCategory category = PoolCategory.Others)
    {
        if (!prefabToPools.ContainsKey(prefab))
        {
            AddNewPool(new Pool(prefab, 3), category); //Todo: build pool size default config mapping from category or config
        }

        var spawnedObject = prefabToPools[prefab].Spawn(position, parent: parent);

        // Track object to pool mapping for fast Despawn operations
        if (spawnedObject != null)
        {
            instanceToPools[spawnedObject] = prefabToPools[prefab];
        }

        return spawnedObject;
    }

    private void AddNewPool(Pool pool, PoolCategory category)
    {
        prefabToPools[pool.Prefab] = pool;

        var categoryTransform = categoryParents[category];
        var poolObject = new GameObject($"Pool_{pool.Prefab.name}");
        poolObject.transform.SetParent(categoryTransform);
        pool.Transform = poolObject.transform; //todo: check if this is correct

        pool.InitPool();
    }

    public void Despawn(GameObject obj)
    {
        if (obj == null || !obj.activeSelf) return;

        // O(1) lookup 
        if (instanceToPools.TryGetValue(obj, out Pool pool))
        {
            instanceToPools.Remove(obj);
            pool.Despawn(obj);
        }
        else // obj này không được tạo và quản lý bởi PoolManager
        {
            Debug.LogWarning($"Object {obj.name} is not managed by PoolManager, auto destroy.");
            Destroy(obj);
        }
    }

    // Cleanup method for scene transitions
    public void ClearCategory(PoolCategory category)
    {
        var poolsToRemove = new List<GameObject>();

        foreach (var kvp in prefabToPools)
        {
            if (kvp.Value.Transform.parent == categoryParents[category])
            {
                poolsToRemove.Add(kvp.Key);
            }
        }

        foreach (var prefab in poolsToRemove)
        {
            prefabToPools.Remove(prefab);
        }

        // Clear object mappings for this category
        var objectsToRemove = instanceToPools.Where(kvp =>
            poolsToRemove.Any(prefab => prefabToPools.ContainsKey(prefab) &&
            prefabToPools[prefab] == kvp.Value)).Select(kvp => kvp.Key).ToList();

        foreach (var obj in objectsToRemove)
        {
            instanceToPools.Remove(obj);
        }
    }
}