using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class BallScript : MonoBehaviour
{
    public float trackSpeed;
    private string trackTargetName;
    public string racketName;
    private bool isTracking;

    public int id;

    void Awake() {
        Assert.AreNotApproximatelyEqual(0f, trackSpeed);
        trackTargetName = "";
        racketName = "Racket";
        isTracking = false;
    }
    public IEnumerator Track(GameObject target) {
        trackTargetName = target.name;
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
        Debug.Log("E?");
        if(other.name == trackTargetName) {
            if(isTracking) {
                StopCoroutine("Track");
                Destroy(gameObject);
            }
        }
    }

    void OnCollisiontEnter(Collision collision) {
        /*
        Debug.Log("Hi");
        if (collision.gameObject.name == racketName || collision.gameObject.name == "Tennis Field")
        {
            Debug.Log("Hieee");
            Vector3 newDir = Vector3.Reflect(transform.forward, collision.GetContact(0).normal);
            transform.rotation = Quaternion.FromToRotation(Vector3.forward, newDir);
            Rigidbody rb = gameObject.GetComponent<Rigidbody>();
            rb.velocity = rb.velocity.magnitude * newDir.normalized;
        }
        */
    }
}
