using UnityEngine;
using Object = UnityEngine.Object;

public static class GameObjectExtensions
{
    public static T GetOrAdd<T>(this GameObject gameObject) where T : Component
    {
        T component = gameObject.GetComponent<T>();
        return component != null ? component : gameObject.AddComponent<T>();
    }

    public static T OrNull<T>(this T obj) where T : Object => obj ? obj : null;

    public static GameObject Spawn(this GameObject prefab, Vector3 worldPosition = default, Transform parent = null, PoolManager.PoolCategory category = PoolManager.PoolCategory.Others)
    {
        return PoolManager.Instance.Spawn(prefab, worldPosition, parent, category);
    }

    public static void Despawn(this GameObject obj)
    {
        PoolManager.Instance.Despawn(obj);
    }

}