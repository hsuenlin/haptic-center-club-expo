using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public abstract class Singleton<T> : MonoBehaviour where T : Component
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
}
