using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class GunScript : MonoBehaviour {
    
    public float coldDownTime;
    public float shotDuration;
    public float autoShootingTime;
    public Transform muzzle; // Forward vector align with gun body
    public LineRenderer lineRenderer;

    void Awake() {
        Assert.IsNotNull(muzzle);
        Assert.IsNotNull(lineRenderer);
        lineRenderer.enabled = false;
    }

    private IEnumerator StartColdDown() {
        DataManager.instance.canShoot = false;
        yield return coldDownTime;
        DataManager.instance.canShoot = true;
    }

    public IEnumerator StartShotEffect() {
        //gunAudio.Play();
        lineRenderer.enabled = true;
        yield return shotDuration;
        lineRenderer.enabled = false;
    }

    public IEnumerator StartAutoShooting() {
        while(true) {
            Shoot();
            yield return autoShootingTime;
        }
    }
    public void Shoot() {
        if(DataManager.instance.canShoot) {
            lineRenderer.SetPosition(0, muzzle.position);
            RaycastHit hit; 
            if(Physics.Raycast(muzzle.position, muzzle.forward, out hit)) {
                if(hit.collider) {
                    lineRenderer.SetPosition(1, hit.point);
                }
                if(hit.collider.tag == "Target") {
                    TargetMachineScript.instance.KillTarget(hit.collider.gameObject);
                }
            } else {
                lineRenderer.SetPosition(1, transform.forward * 5000);
            }
            StartCoroutine(StartColdDown());
            StartCoroutine(StartShotEffect());
        }
    }
}