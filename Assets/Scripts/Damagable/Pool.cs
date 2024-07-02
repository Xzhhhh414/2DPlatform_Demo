using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

public class BulletPool : MonoSingleton<BulletPool>
{
    
    private Dictionary<string,IObjectPool<Projectile>> pool;
    public bool collectionChecks = true;
    public int maxPoolSize = 10;
    public List<GameObject> projectiles;


    private void Awake()
    {
        pool = new Dictionary<string, IObjectPool<Projectile>>();
    }

    public static IObjectPool<Projectile> Pool(string id)
    {
        if (!Instance.pool.ContainsKey(id))
        {
            Instance.pool.Add(id,new ObjectPool<Projectile>(()=>Instance.CreatePooledItem(id), Instance.OnTakeFromPool, Instance.OnReturnedToPool, Instance.OnDestroyPoolObject, Instance.collectionChecks, 10, Instance.maxPoolSize)) ;
        }
        return Instance.pool[id];
        
    }
    
    Projectile CreatePooledItem(string bullet)
    {
        foreach (var projectile in projectiles)
        {
            if (projectile.name == bullet)
            {
                var go = Instantiate(projectile).GetComponent<Projectile>();
                return go;
            }
        }
        return null;
    }

    // Called when an item is returned to the pool using Release
    void OnReturnedToPool(Projectile bullet)
    {
        bullet.gameObject.SetActive(false);
    }

    // Called when an item is taken from the pool using Get
    void OnTakeFromPool(Projectile bullet)
    {
        bullet.gameObject.SetActive(true);
    }

    // If the pool capacity is reached then any items returned will be destroyed.
    // We can control what the destroy behavior does, here we destroy the GameObject.
    void OnDestroyPoolObject(Projectile bullet)
    {
        Destroy(bullet.gameObject);
    }
}
