using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public abstract class StateSingleton<T> : MonoBehaviour where T : Component
{
    public static T instance;
    protected virtual void Awake() {
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

    // Start is called before the first frame update
    protected virtual void Init() {}
    protected virtual void Exit() {}

    protected virtual void OnEnabled()
    {
        Init();
    }
    protected virtual void OnDisable()
    {
        Exit();
    }
}
