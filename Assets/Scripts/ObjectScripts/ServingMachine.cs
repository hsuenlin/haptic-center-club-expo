using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServingMachine : MonoBehaviour
{

    public int serveNum;
    
    public FairZone fairZone;

    public GameObject ballPrefab;
    public Transform ballOrigin;
    
    public float elevation;// = 10;
    public float maxElevationDeviation;// = 15;
    public float thrust;// = 400;
    public float maxThrustDeviation;// = 50;
    public float maxAzimuth;// = 30;

    public bool isGameOver;

    void Awake() {
        isGameOver = false;
    }

    public void Init() {
        fairZone.gameObject.SetActive(true);
    }

    public void Serve(int id) {
        GameObject ball = Instantiate(ballPrefab);
        ball.GetComponent<BallScript>().id = id;
        ball.transform.parent = ballOrigin;
        ball.transform.localPosition = Vector3.zero;
        ball.transform.localRotation = Quaternion.identity;
        Rigidbody rb = ball.GetComponent<Rigidbody>();

        Vector3 forwardToPlayer = new Vector3(0, 0, -1);
        elevation += Random.Range(0, maxElevationDeviation);
        thrust += Random.Range(0, maxThrustDeviation);
        float azimuth = Random.Range(-maxAzimuth, maxAzimuth);
        Vector3 direction = Quaternion.Euler(elevation, azimuth, 0) * forwardToPlayer;
        rb.AddForce(direction * thrust);
    }
    
    public IEnumerator ServeBalls() {
        for(int i = 0; i < serveNum; --i) {
            while(!DataManager.instance.isSenpaiSwing) {
                yield return null;
            }
            Serve(i);
            //ServeBall
            //fairZone.leftOrRight = Random.Range(0f, 1f) > 0.5f ? CourtType.LEFT: CourtType.RIGHT;
            fairZone.ChangeFairZoneHalf();
            fairZone.currentBallId = i;
            yield return null;
        }
    }

    public void End() {
        fairZone.gameObject.SetActive(false);
    }
}
