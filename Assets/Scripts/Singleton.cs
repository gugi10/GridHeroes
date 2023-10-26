using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : class
{
    private static T instance;
    public static T Instance { get { return instance; } }
    public static event Action<T> OnInstantiate;

    protected virtual void Awake()
    {
        if (instance != null && instance != this as T)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this as T;
            StartCoroutine(SendOnInstantiateInNextFrame());
        }

        DontDestroyOnLoad(gameObject);
    }

    private IEnumerator SendOnInstantiateInNextFrame()
    {
        yield return new WaitForEndOfFrame();
        OnInstantiate?.Invoke(instance);
    }
}
