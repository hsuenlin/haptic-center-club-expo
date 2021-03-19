using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class BallScript : MonoBehaviour
{
    public float trackSpeed;
    private string targetName;
    private bool isTracking;

    void Awake() {
        Assert.AreNotApproximatelyEqual(0f, trackSpeed);
        isTracking = false;
    }
    public IEnumerator Track(GameObject target) {
        targetName = target.name;
        isTracking = true;
        while(true) {
            // Ball goes straight toward the target
            Vector3 direction = target.transform.position - gameObject.transform.position;
            Vector3.Normalize(direction);
            gameObject.transform.position = gameObject.transform.position + direction * trackSpeed;
            yield return null;
        }
    }

    void OnTriggerEnter(Collider other) {
        if(other.name == targetName) {
            if(isTracking) {
                StopCoroutine("Track");
            }
            Destroy(gameObject);
        }
    }
}
