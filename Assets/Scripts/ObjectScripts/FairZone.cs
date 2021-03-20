using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Half {
    LEFT = 0,
    RIGHT = 1
}
public class FairZone : MonoBehaviour
{
    public int currentBallId;
    public Half half;
    public GameObject left;
    public GameObject right;

    public Action OnGetPoint;

    void Awake() {
        left.SetActive(false);
        right.SetActive(false);
    }
    public void ChangeFairZoneHalf() {
        if(UnityEngine.Random.Range(0f, 1f) > 0.5f) {
            left.SetActive(true);
            right.SetActive(false);
            half = Half.LEFT;
        } else {
            left.SetActive(false);
            right.SetActive(true);
            half = Half.RIGHT;
        }
    }

    public void OnBallHitEnter(Half hitHalf, int ballId) {
        if(ballId == currentBallId && half == hitHalf) {
            OnGetPoint();
        }
    }
}
