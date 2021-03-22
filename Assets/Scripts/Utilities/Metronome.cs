using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class Metronome : MonoBehaviour
{
    
    public float beatWidth;
    public virtual void OnBeatEnter() {}
    public virtual void OnBeat() {}
    public virtual void OnBeatExit() {}

    protected float idleTime;
    protected float halfWidth;

    protected float beatTime;
    protected float firstBeatTime;

    void Awake() {
        idleTime = beatTime - beatWidth;
        halfWidth = beatWidth / 2;
    }

    public IEnumerator Tick(int beatNum, Action OnTickEnd) {
        yield return new WaitForSecondsRealtime(firstBeatTime - halfWidth);
        for(int i = 0; i < beatNum; ++i) {
            OnBeatEnter();
            yield return new WaitForSecondsRealtime(halfWidth);
            OnBeat();
            yield return new WaitForSecondsRealtime(halfWidth);
            OnBeatExit();
            yield return new WaitForSecondsRealtime(idleTime);
        }
        OnTickEnd();
    }

    public IEnumerator InfTick() {
        float idleTime = beatTime - beatWidth;
        yield return new WaitForSecondsRealtime(firstBeatTime - halfWidth);
        while(true) {
            OnBeatEnter();
            yield return new WaitForSecondsRealtime(halfWidth);
            OnBeat();
            yield return new WaitForSecondsRealtime(halfWidth);
            OnBeatExit();
            yield return new WaitForSecondsRealtime(idleTime);
        }
    }
}
