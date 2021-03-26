using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class FairZoneHalf : MonoBehaviour
{
    public Half half;
    public FairZone parentFairZone;
    
    void Awake() {
        Assert.IsNotNull(parentFairZone);
    }
    void OnTriggerEnter(Collider other) {
        Debug.Log($"Fair {other.name}");
        if(other.tag == "Ball") {
            if(other.gameObject.GetComponent<BallScript>().isHit) {
                parentFairZone.OnBallHitEnter(half, other.gameObject.GetComponent<BallScript>().id);
            }
        }
    }
}
