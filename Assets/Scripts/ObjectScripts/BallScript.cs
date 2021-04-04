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

    private float cnt = 0;

    void Awake() {
        Assert.AreNotApproximatelyEqual(0f, trackSpeed);
        trackTargetTag = "BallContainer";
        racketName = "Racket";
        isTracking = false;
        isHit = false;   
        gameObject.tag = "Ball";
    }
    public IEnumerator Track(GameObject target) {
        ClubUtil.SetLayerRecursively(gameObject, 8);
        trackTargetTag = target.tag;
        isTracking = true;
        gameObject.GetComponent<Rigidbody>().useGravity = false;
        //gameObject.GetComponent<Rigidbody>().isKinematic = true;
        //Destroy(gameObject.GetComponent<Rigidbody>());
        
        trackSpeed = 0.05f;
        while(true) {
            // Ball goes straight toward the target
            Vector3 direction = target.transform.position - gameObject.transform.position;
            Vector3.Normalize(direction);
            gameObject.transform.position = gameObject.transform.position + direction * trackSpeed + new Vector3(0.01f, 0.01f, 0.01f);
            yield return null;
            cnt += Time.deltaTime;
            if(cnt > 3f) {
                //StopCoroutine("Track");
                gameObject.GetComponent<Rigidbody>().useGravity = true;
                yield break;
            }
        }
    }

    void OnTriggerEnter(Collider other) {
        if(other.tag == trackTargetTag) {
            Debug.Log($"{other.name}");
            if(isTracking) {
                StopCoroutine("Track");
                ClubUtil.SetLayerRecursively(gameObject, 13);
                gameObject.GetComponent<Rigidbody>().useGravity = true;
                
                //gameObject.GetComponent<Rigidbody>().isKinematic = true;

                //gameObject.AddComponent<Rigidbody>();
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
