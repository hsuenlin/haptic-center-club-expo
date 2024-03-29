/* Created by Yi Chen, modified by Hsu-En Lin */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Timer
{
    public static IEnumerator StartTimer(float maxTime, System.Action end) {
        yield return StartTimer(maxTime, (x)=>{ }, end);
    }

    public static IEnumerator StartTimer(float maxTime, System.Action<float> tick) {
        yield return StartTimer(maxTime, tick, () => { });
    }

    public static IEnumerator StartTimer(float maxTime, System.Action<float> tick, System.Action end) {
        if (maxTime <= 0f) {
            yield break;
        }

        float timer = 0f;
        while (timer < maxTime) {
            tick(timer);
            yield return null;
            timer = timer + Time.deltaTime;
        }
        tick(maxTime);

        end();
    }
}