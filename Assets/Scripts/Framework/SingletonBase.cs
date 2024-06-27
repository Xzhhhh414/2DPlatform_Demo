using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 单例模式的基类
/// </summary>
public abstract class Singleton<T> where T : Singleton<T>, new()
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new T();
            }
            return instance;
        }
    }
}