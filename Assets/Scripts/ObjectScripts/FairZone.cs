using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class FairZone : MonoBehaviour
{
    public int currentBallId;
    public GameObject left;
    public GameObject right;

    void Awake() {
        left.SetActive(false);
        right.SetActive(false);
    }
    public void ChangeFairZoneHalf() {
        if(Random.Range(0f, 1f) > 0.5f) {
            left.SetActive(true);
            right.SetActive(false);
        } else {
            left.SetActive(false);
            right.SetActive(true);
        }
    }

    // TODO: OnTriggerEnter Left and Right
}
