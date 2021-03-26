using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Half {
    LEFT = 0,
    RIGHT = 1,
    WHOLE = 2,
    NONE = 3
}
public class FairZone : MonoBehaviour
{
    public int currentBallId;
    public Half half;
    public GameObject left;
    public GameObject right;

    public Action OnBallIn;
    public Action OnBallOut;

    void Awake() {
        left.GetComponent<Renderer>().enabled = false;
        right.GetComponent<Renderer>().enabled = false;
    }
    public void ChangeFairZoneHalf() {
        if(UnityEngine.Random.Range(0f, 1f) > 0.5f) {
            left.GetComponent<Renderer>().enabled = true;
            right.GetComponent<Renderer>().enabled = false;
            half = Half.LEFT;
        } else {
            left.GetComponent<Renderer>().enabled = false;
            right.GetComponent<Renderer>().enabled = true;
            half = Half.RIGHT;
        }
    }

    public void OnBallHitEnter(Half hitHalf, int ballId) {
        Debug.Log("BALL HIT");
        if(ballId == currentBallId && half == hitHalf) {
            OnBallIn();
        } else {
            OnBallOut();
        }
    }
}
