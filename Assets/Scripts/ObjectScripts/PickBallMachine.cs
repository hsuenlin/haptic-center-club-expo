using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class PickBallMachine : MonoBehaviour
{

    public float spawnTime;

    void Awake()
    {
        Assert.AreNotApproximatelyEqual(0f, spawnTime);
    }

    public void Init() {
        // Generate balls
        // Setup pickBallRegion
        StartCoroutine(Run());
        StartCoroutine(Spawn());
    }

    public IEnumerator Run() {
        while(true) {
            // TODO:
            // Check if any region activated between this frame and last frome
            // If so, start picking balls
            // Check if any region deactivated between this frame and last frame
            // If so, stop picking balls
            // region.Throw(ball) ->  From ball.gameObject.position to region.hand.transform.position;
        }
    }

    public IEnumerator Spawn() {
        while(true) {
            // TODO:
            // Check the number of balls in each region
            // If less than a threshold, spawn balls.
            yield return new WaitForSeconds(spawnTime);
        }
    }

    public void End() {
        // Detroy all balls
    }
}
