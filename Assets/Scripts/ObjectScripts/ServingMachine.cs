using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServingMachine : MonoBehaviour
{

    public GameObject ballPrefab;
    public Transform ballOrigin;
    
    public float lobElevation = 60;
    public float volleyElevation = 10;
    public float maxElevationDeviation = 15;
    public float lobThrust = 200;
    public float volleyThrust = 400;
    public float maxThrustDeviation = 50;
    public float maxAzimuth = 30;
    public int[] ballTypeOrder;
    private int orderIndex = 0;
    
    // Start is called before the first frame update

    public enum BallType {
        Lob = 0,
        Volley = 1
    }
    void Start()
    {
        ballTypeOrder = new int[] {0, 1};
    }

    // Update is called once per frame
    void Update()
    {
        
        if(Input.GetKeyDown(KeyCode.S)) {
            GameObject ball = Instantiate(ballPrefab, Vector3.zero, Quaternion.identity);
            ball.transform.parent = ballOrigin;
            ball.transform.localPosition = Vector3.zero;
            Rigidbody rb = ball.GetComponent<Rigidbody>();
            
            orderIndex %= ballTypeOrder.Length;
            float elevation, thrust;
            Vector3 forwardToPlayer = new Vector3(0, 0, -1);
            BallType ballType = (BallType) ballTypeOrder[orderIndex];
            
            if(ballType == BallType.Lob) {
                elevation = lobElevation + Random.Range(0, maxElevationDeviation);
                thrust = lobThrust + Random.Range(0, maxThrustDeviation);
            } else {
                elevation = volleyElevation + Random.Range(0, maxElevationDeviation);
                thrust = volleyThrust + Random.Range(0, maxThrustDeviation);
            }

            float azimuth = Random.Range(-maxAzimuth, maxAzimuth);
            Vector3 direction = Quaternion.Euler(elevation, azimuth, 0) * forwardToPlayer;
            rb.AddForce(direction * thrust);
            orderIndex++;
        }
    }
}
