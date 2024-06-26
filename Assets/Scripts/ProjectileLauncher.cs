using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Pool;

public class ProjectileLauncher : MonoBehaviour
{
    public List<Transform> launchPoint=new List<Transform>();
    public GameObject projectilePrefab;
    protected IObjectPool<Projectile> pool;
    public bool collectionChecks = true;
    public int maxPoolSize = 10;
    [Label("多发时发射子弹数量")]
    public int num;
    [Label("多发时发射子弹扇形角度")]
    public int range;

    public IObjectPool<Projectile> Pool
    {
        get
        {
            if (pool == null)
            {
                pool = new ObjectPool<Projectile>(CreatePooledItem, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, collectionChecks, 10, maxPoolSize);
            }
            return pool;
        }
    }
    
    Projectile CreatePooledItem()
    {
        var go = Instantiate(projectilePrefab, Vector3.zero, projectilePrefab.transform.rotation).GetComponent<Projectile>();
        go.Launcher = this;
        return go;
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

    public void FireProjectile(int index=1)
    {
        if (index <= 0) index = 1;
        if (launchPoint.Count < index) return;
        Projectile projectile = Pool.Get();
        projectile.transform.position = launchPoint[index-1].position;
        projectile.transform.rotation = launchPoint[index-1].transform.rotation;
        projectile.Spawn((transform.localScale.x>0?1:-1)*launchPoint[index-1].right,transform.localScale.x>0);
    }

    public void FireMultiProjectile(int index)
    {
        if (index <= 0) index = 1;
        if (launchPoint.Count < index||num<=1) return;
        for (float i = 0; i < num; i++)
        {
            Projectile projectile = Pool.Get();
            projectile.transform.position = launchPoint[index-1].position;
            projectile.transform.rotation = launchPoint[index-1].transform.rotation;
            var angle=range / (num - 1);
            projectile.transform.Rotate(new(0,0,range/2-i*angle),Space.Self);
            var dir = (transform.localScale.x>0?1:-1)*projectile.transform.right;
            projectile.Spawn(dir,transform.localScale.x>0);
        }
        
        
    }

    private void OnDestroy()
    {
        Pool.Clear();
    }

    void OnDrawGizmos()
    {
        for(int i=0;i<launchPoint.Count;i++)
        {
            Gizmos.color = new Color((float)(i+1)/launchPoint.Count,0,0); 
            Vector3 pointA = launchPoint[i].TransformPoint(new Vector3(0, -0.2f, 0));
            Vector3 pointB = launchPoint[i].TransformPoint(new Vector3(0, 0.2f, 0));
            Vector3 pointC = launchPoint[i].TransformPoint(new Vector3(0.4f, 0, 0));
            Gizmos.DrawLine(pointA, pointB);
            Gizmos.DrawLine(pointB, pointC);
            Gizmos.DrawLine(pointC, pointA);
        }
    }

}
