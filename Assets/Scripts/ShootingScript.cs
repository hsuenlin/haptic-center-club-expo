using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingScript : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform muzzle;
    public float gunColdDownTime = 0.25f;
    public float shootingRange = 100f;
    private float timer = 0f;
    private LineRenderer trajectory;
    private Vector3 cameraCenter;
    private WaitForSeconds shotDuration = new WaitForSeconds(.07f);
    
    void Start()
    {
        cameraCenter = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0f));
        trajectory = GetComponent<LineRenderer>();
        timer = gunColdDownTime + 1;
    }

    // Update is called once per frame
    void Update()
    {
        if(timer < gunColdDownTime) {
            timer += Time.deltaTime;
        }
        if(Input.GetKeyDown(KeyCode.Space)) {
            if(timer >= gunColdDownTime) {
                // Fire
                StartCoroutine(ShotEffect());
                RaycastHit hit;
                if(Physics.Raycast(cameraCenter, Camera.main.transform.forward, out hit, shootingRange)) {
                    trajectory.SetPosition(0, muzzle.position);
                    trajectory.SetPosition(1, hit.point);
                    if(hit.collider.gameObject.tag == "Target") {
                        Destroy(hit.collider.gameObject);
                    }
                }
            }
        }
    }
    private IEnumerator ShotEffect()
    {

        // Turn on our line renderer
        trajectory.enabled = true;

        //Wait for .07 seconds
        yield return shotDuration;

        // Deactivate our line renderer after waiting
        trajectory.enabled = false;
    }
}
