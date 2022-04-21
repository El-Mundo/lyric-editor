using UnityEngine;
using Lyrics.Game;
using System;
using System.Collections.Generic;
using System.Linq;

public class ObjectPool<T>
{
    [Header("Direct Setup")]
    [Tooltip("When loadChildren is false, this array will be used to load the stored objects.")]
    public List<T> objects;
    [Tooltip("A prefab to be loaded when no available object was found in the pool.")]
    public GameObject prefab;

    //[Header("Hierachical Setup")]
    //public bool loadChildren;
    [Tooltip("The parent object containing the pooled objects as its chidlren.")]
    public Transform container;

    /// <summary>
    /// Attempt to spawn an instance from the object pool. Will spawn nothing if no available object was found in the object pool.
    /// </summary>
    /// <returns>WARNING: might return NULL.</returns>
    public T AttemptToSpawn()
    {
        foreach(IObjectPoolHandler<T> obj in objects)
        {
            if (obj.IsAvailable())
            {
                return obj.Spawn();
            }
        }
        return default(T);
    }

    /// <summary>
    /// <para>
    /// A prefeb will be generated if no available object was found in the object pool.
    /// </para>
    /// <para>Please use SpawnMultipleWithPrefab when spawning numerous objects together.</para>
    /// </summary>
    public T SpawnWithPrefab(Transform parent)
    {
        foreach (IObjectPoolHandler<T> obj in objects)
        {
            if (obj.IsAvailable())
            {
                return obj.Spawn();
            }
        }

        if(prefab != null)
        {
            T obj = GameObject.Instantiate(prefab, parent).GetComponent<T>();
            objects.Add(obj);
            return obj;
        }
        else
        {
            throw new Exception("Please set a prefab for this pool before calling this function.");
        }
    }

    /// <summary>
    /// An optimized function specialized for large-sclae spawning.
    /// </summary>
    public T[] SpawnMultipleWithPrefab(Transform parent, int size)
    {
        int num = 0;
        T[] output = new T[size];

        foreach (IObjectPoolHandler<T> obj in objects)
        {
            if (obj.IsAvailable())
            {
                output[num] = obj.Spawn();
                num++;
                if (num >= size) return output;
            }
        }

        if (prefab != null)
        {
            for(; num < size; num++)
            {
                T obj = GameObject.Instantiate(prefab, parent).GetComponent<T>();
                objects.Add(obj);
                output[num] = obj;
            }
            return output;
        }
        else
        {
            throw new Exception("Please set a prefab for this pool before calling this function.");
        }
    }

    public void RecycleAll()
    {
        foreach (IObjectPoolHandler<T> obj in objects)
        {
            if (!obj.IsAvailable())
            {
                obj.Recycle();
            }
        }
    }

    public void DEBUG_PrintOccupancy()
    {
        int total = 0, used = 0;
        foreach (IObjectPoolHandler<T> obj in objects)
        {
            if (!obj.IsAvailable())
            {
                used++;
            }
            total++;
        }
        Debug.Log("Pool used: " + used + "/" + total);
    }

    public ObjectPool(Transform container, GameObject prefab)
    {
        objects = container.GetComponentsInChildren<T>().ToList();
        this.prefab = prefab;
    }

}
