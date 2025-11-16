using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class Pool
{
    private enum PoolEvent { SPAWN, DESPAWN, CREATE }

    [SerializeField] private GameObject prefab;
    public GameObject Prefab => prefab;

    [SerializeField] private int initialPoolSize = 3;
    private Stack<GameObject> pooledInstances;
    private List<GameObject> activeInstances;
    private int count = 0; // for naming objects
    public Transform Transform { get; set; }

    public Pool(GameObject prefab, int initialPoolSize)
    {
        this.prefab = prefab;
        this.initialPoolSize = initialPoolSize;
    }

    public void InitPool()
    {
        pooledInstances = new Stack<GameObject>();
        activeInstances = new List<GameObject>();

        for (var i = 0; i < initialPoolSize; i++)
        {
            var instance = Object.Instantiate(prefab, Transform, true);
            instance.name = $"{prefab.name}_{count++}";
            InvokeEvent(instance, PoolEvent.CREATE);

            //todo
            instance.transform.localPosition = Vector3.zero;
            instance.transform.localScale = Vector3.one;
            instance.transform.localEulerAngles = Vector3.zero;
            instance.SetActive(false);
            pooledInstances.Push(instance);
            InvokeEvent(instance, PoolEvent.DESPAWN);
        }
    }

    //todo: xem xét lại cái của nợ này
    public GameObject Spawn(Vector3 position,
        Quaternion rotation = default,
        Vector3 scale = default,
        Transform parent = null,
        bool useLocalPosition = false,
        bool useLocalRotation = false,
        bool isActive = true)
    {
        if (pooledInstances.Count <= 0) // Every game object has been spawned!
        {
            var freshObject = Object.Instantiate(prefab);
            freshObject.name = $"{prefab.name}_{count++}";
            InvokeEvent(freshObject, PoolEvent.CREATE);
            pooledInstances.Push(freshObject);
        }

        var obj = pooledInstances.Pop();
        if (obj != null)
        {
            obj.transform.SetParent(parent ?? Transform);

            if (useLocalPosition)
            {
                obj.transform.localPosition = position;
            }
            else
            {
                obj.transform.position = position;
            }

            if (rotation.eulerAngles == Quaternion.identity.eulerAngles)
            {
                rotation = prefab.transform.rotation;
            }

            if (useLocalRotation)
            {
                obj.transform.localRotation = rotation;
            }
            else
            {
                obj.transform.rotation = rotation;
            }

            if (scale == default)
            {
                scale = Vector3.one;
            }

            obj.transform.localScale = scale;

            SetActiveSafe(obj, isActive);

            activeInstances.Add(obj);

            InvokeEvent(obj, PoolEvent.SPAWN);
        }

        return obj;
    }

    /// <summary>
    /// Deactivate an object and add it back to the pool, given that it's
    /// in alive objects array.
    /// </summary>
    /// <param name="obj"></param>
    public void Despawn(GameObject obj)
    {
        var index = activeInstances.FindIndex(o => obj == o);
        if (index == -1)
        {
            Object.Destroy(obj);
            return;
        }

        InvokeEvent(obj, PoolEvent.DESPAWN);

        //Todo: xét lại cái đoạn này xem nên để vào IPoolObject.OnDespawn không
        obj.transform.SetParent(Transform);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localScale = Vector3.one;
        obj.transform.localEulerAngles = Vector3.zero;

        pooledInstances.Push(obj);
        activeInstances.RemoveAt(index);

        SetActiveSafe(obj, false);
    }

    private void SetActiveSafe(GameObject obj, bool value) //todo: check xem có thực sự improve performance k
    {
        if (obj.activeSelf != value)
        {
            obj.SetActive(value);
        }
    }

    private void InvokeEvent(GameObject instance, PoolEvent ev)
    {
        var poolScripts = instance.GetComponentsInChildren<IPoolObject>();

        switch (ev)
        {
            case PoolEvent.SPAWN:
                {
                    foreach (var poolScript in poolScripts)
                    {
                        poolScript.OnSpawn();
                    }

                    break;
                }
            case PoolEvent.DESPAWN:
                {
                    foreach (var poolScript in poolScripts)
                    {
                        poolScript.OnDeSpawn();
                    }

                    break;
                }
            case PoolEvent.CREATE:
                {
                    foreach (var poolScript in poolScripts)
                    {
                        poolScript.OnCreated();
                    }

                    break;
                }
            default:
                throw new ArgumentOutOfRangeException(nameof(ev), ev, null);
        }
    }

    public void ClearInactive()
    {
        while (pooledInstances.Count > 0)
        {
            var obj = pooledInstances.Pop();
            if (obj != null)
            {
                Object.Destroy(obj);
            }
        }

        // Clear stack để đảm bảo không có reference nào còn lại
        pooledInstances.Clear();
    }
    
    /// <summary>
    /// Inactive toàn bộ objects trong pool (chuyển tất cả active objects về inactive)
    /// Sau đó có thể giới hạn số lượng objects còn lại trong pool
    /// </summary>
    /// <param name="remainInstance">Số lượng object muốn giữ lại sau khi inactive tất cả, default = -1 tức là giữ cả. 
    public void DespawnAll(int remainInstance = -1)
    {
        // Bước 1: Inactive tất cả active objects
        // Tạo copy của list để tránh modification during iteration
        var activeObjectsCopy = new List<GameObject>(activeInstances);
        
        foreach (var activeObj in activeObjectsCopy) //todo: xem có nên gọi thẳng Despawn cho đồng bộ hay k
        {
            if (activeObj != null)
            {
                // Invoke despawn event
                InvokeEvent(activeObj, PoolEvent.DESPAWN);
                
                // Reset transform về trạng thái ban đầu
                activeObj.transform.SetParent(Transform);
                activeObj.transform.localPosition = Vector3.zero;
                activeObj.transform.localScale = Vector3.one;
                activeObj.transform.localEulerAngles = Vector3.zero;
                
                // Set inactive và add vào pool
                SetActiveSafe(activeObj, false);
                pooledInstances.Push(activeObj);
            }
        }
        
        // Clear danh sách active objects
        activeInstances.Clear();
        
        // Bước 2: Nếu remainInstance >= 1, destroy bớt objects để chỉ còn lại số lượng mong muốn
        if (remainInstance >= 1)
        {
            // Chỉ destroy bớt nếu số objects hiện tại nhiều hơn số lượng muốn giữ lại
            if (pooledInstances.Count > remainInstance)
            {
                int destroyTimes = pooledInstances.Count - remainInstance;
                
                // Destroy từng object một cho đến khi còn lại đúng số lượng mong muốn
                for (int i = 0; i < destroyTimes; i++)
                {
                    var obj = pooledInstances.Pop();
                    if (obj != null)
                    {
                        Object.Destroy(obj);
                    }
                }
            }
        }
    }
}

public interface IPoolObject
{
    void OnSpawn();
    void OnDeSpawn();
    void OnCreated();
}