using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public abstract class StateSingleton<T> : MonoBehaviour where T : Component
{
    public static T instance;
    protected virtual void Awake() {
        Debug.Log("Hi");
        if (instance == null)
        {
            instance = this as T;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
        OnAwake();
    }

    protected virtual void OnAwake() {}

    protected virtual void Init() {}
    protected virtual void Exit() {}

    protected virtual void OnEnabled()
    {
        Debug.Log("Init");
        Init();
    }
    protected virtual void OnDisable()
    {
        Debug.Log("Exit");
        Exit();
    }
}
