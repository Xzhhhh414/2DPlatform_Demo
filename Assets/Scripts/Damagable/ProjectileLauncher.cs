using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Pool;

public class ProjectileLauncher : MonoBehaviour
{
    public List<Transform> launchPoint=new List<Transform>();
    [Label("多发时发射子弹数量")]
    public int num;
    [Label("多发时发射子弹扇形角度")]
    public int range;

  
    

    public void FireProjectileAt1(GameObject bullet)
    {
        FireProjectile(1,bullet);
    }
    
    public void FireProjectileAt2(GameObject bullet)
    {
        FireProjectile(2,bullet);
    }
    
    public void FireProjectileAt3(GameObject bullet)
    {
        FireProjectile(3,bullet);
    }
    
    
    public void FireProjectile(int index,GameObject bullet)
    {
        if (launchPoint.Count < index) return;
        Projectile projectile = BulletPool.Pool(bullet.gameObject.name).Get();
        projectile.transform.position = launchPoint[index-1].position;
        projectile.transform.rotation = launchPoint[index-1].transform.rotation;
        projectile.Spawn((transform.localScale.x>0?1:-1)*launchPoint[index-1].right,transform.localScale.x>0);
    }

    public void FireMultiProjectile(int index,GameObject bullet)
    {
        if (index <= 0) index = 1;
        if (launchPoint.Count < index||num<=1) return;
        for (float i = 0; i < num; i++)
        {
            Projectile projectile = BulletPool.Pool(bullet.gameObject.name).Get();
            projectile.transform.position = launchPoint[index-1].position;
            projectile.transform.rotation = launchPoint[index-1].transform.rotation;
            var angle=range / (num - 1);
            projectile.transform.Rotate(new(0,0,range/2.0f-i*angle),Space.Self);
            var dir = (transform.localScale.x>0?1:-1)*projectile.transform.right;
            projectile.Spawn(dir,transform.localScale.x>0);
        }
        
        
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
