using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class Metronome : MonoBehaviour
{
    
    public double beatTime;
    public double firstBeatTime;
    public double beatWidth;

    protected double idleTime;
    protected double halfWidth;
    
    public virtual void OnBeatEnter() {}
    public virtual void OnBeat() {}
    public virtual void OnBeatExit() {}

    public void InitMetro(double _beatTime, double _firstBeatTime, double _beatWidth) {
        beatTime = _beatTime;
        firstBeatTime = _firstBeatTime;
        beatWidth = _beatWidth;
        halfWidth = _beatWidth / 2;
        idleTime = _beatTime - _beatWidth;
    }

    /*
    public IEnumerator Tick(int beatNum, Action OnTickEnd) {
        yield return new WaitForSeconds(firstBeatTime - halfWidth);
        for(int i = 0; i < beatNum; ++i) {
            OnBeatEnter();
            yield return new WaitForSeconds(halfWidth);
            OnBeat();
            yield return new WaitForSeconds(halfWidth);
            OnBeatExit();
            yield return new WaitForSeconds(idleTime);
        }
        OnTickEnd();
    }
    */

    /*
    public IEnumerator InfTick() {
        double idleTime = beatTime - beatWidth;
        yield return new WaitForSeconds(firstBeatTime - halfWidth);
        while(true) {
            OnBeatEnter();
            yield return new WaitForSeconds(halfWidth);
            OnBeat();
            yield return new WaitForSeconds(halfWidth);
            OnBeatExit();
            yield return new WaitForSeconds(idleTime);
        }
    }
    public IEnumerator DspTick(int beatNum, Action OnTickEnd) {
        double startTime = AudioSettings.dspTime;
        double onBeatEnterTime = startTime + firstBeatTime - halfWidth;
        double onBeatTime = onBeatEnterTime + halfWidth;
        double onBeatExitTime = onBeatTime + halfWidth;
        double timer;

        for(int i = 0; i < beatNum; ++i) {
            timer = AudioSettings.dspTime;
            while(timer < onBeatEnterTime) { 
                yield return null;
                timer = AudioSettings.dspTime;
            }
            OnBeatEnter();
            timer = AudioSettings.dspTime;
            while (timer < onBeatTime) {
                yield return null;
                timer = AudioSettings.dspTime;
            }
            OnBeat();
            timer = AudioSettings.dspTime;
            while (timer < onBeatExitTime) {
                yield return null;
                timer = AudioSettings.dspTime;
            }
            OnBeatExit();
            onBeatEnterTime = onBeatExitTime += idleTime;
            onBeatTime = onBeatEnterTime + halfWidth;
            onBeatExitTime = onBeatTime + halfWidth;
        }
        OnTickEnd();       
    }
    */

    public IEnumerator InfDspTick()
    {
        double startTime = AudioSettings.dspTime;
        double onBeatEnterTime = startTime + firstBeatTime - halfWidth;
        double onBeatTime = startTime + firstBeatTime;
        double onBeatExitTime = startTime + firstBeatTime + halfWidth;
        double timer;

        while(true)
        {
            timer = AudioSettings.dspTime;
            while (timer < onBeatEnterTime)
            {
                yield return null;
                timer = AudioSettings.dspTime;
            }
            OnBeatEnter();
            while (timer < onBeatTime)
            {
                yield return null;
                timer = AudioSettings.dspTime;
            }
            OnBeat();
            while (timer < onBeatExitTime)
            {
                yield return null;
                timer = AudioSettings.dspTime;
            }
            OnBeatExit();
            onBeatEnterTime = onBeatExitTime + idleTime;
            onBeatTime = onBeatEnterTime + halfWidth;
            onBeatExitTime = onBeatTime + halfWidth;
        }
    }
}
