using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class BallScript : MonoBehaviour
{
    public float trackSpeed;
    private string trackTargetTag;
    public string racketName;
    private bool isTracking;

    public int id;
    public bool isHit;

    void Awake() {
        Assert.AreNotApproximatelyEqual(0f, trackSpeed);
        trackTargetTag = "BallContainer";
        racketName = "Racket";
        isTracking = false;
        isHit = false;   
        gameObject.tag = "Ball";
    }
    public IEnumerator Track(GameObject target) {
        gameObject.layer = 8;
        trackTargetTag = target.tag;
        isTracking = true;
        //gameObject.GetComponent<Rigidbody>().useGravity = false;
        //gameObject.GetComponent<Rigidbody>().isKinematic = false;
        Destroy(gameObject.GetComponent<Rigidbody>());
        
        trackSpeed = 0.05f;
        while(true) {
            // Ball goes straight toward the target
            Vector3 direction = target.transform.position - gameObject.transform.position;
            Vector3.Normalize(direction);
            gameObject.transform.position = gameObject.transform.position + direction * trackSpeed;
            yield return null;
        }
    }

    void OnTriggerEnter(Collider other) {
        if(other.tag == trackTargetTag) {
            if(isTracking) {
                StopCoroutine("Track");
                //Destroy(gameObject);
            }
        }
    }

    void OnCollisiontEnter(Collision collision) {
        if(collision.gameObject.tag == "RacketFace") {
            isHit = true;
        }
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
