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
        if(other.name == "TennisBall") {
            parentFairZone.OnBallHitEnter(half, other.gameObject.GetComponent<BallScript>().id);
        }
    }
}
