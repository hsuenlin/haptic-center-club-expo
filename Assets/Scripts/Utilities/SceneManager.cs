using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public abstract class SceneManager<T> : Singleton<T>, IState where T : Component
{
    public virtual void Init() {}
    public virtual void Exit() {}
}
