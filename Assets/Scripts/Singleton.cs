using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Monobehaviorシングルトン
/// </summary>
/// <typeparam name="T"></typeparam>
public class Singleton<T>: MonoBehaviour where T: Singleton<T>
{
    public static T Instance;
    public bool isPesistant = true;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this as T;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (isPesistant)
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}

