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

    public DeviceAppearance appearance {
        get {
            return appearance;
        }
        set {
            appearance = value;
            switch(appearance) {
                case DeviceAppearance.REAL:
                    viveModel.SetActive(true);
                    gunModel.SetActive(false);
                    break;
                case DeviceAppearance.VIRTUAL:
                    viveModel.SetActive(false);
                    gunModel.SetActive(true);
                    break;
            }
        }
    }
    public GameObject viveModel;
    public GameObject gunModel;

    void Awake() {
        Assert.IsNotNull(muzzle);
        Assert.IsNotNull(lineRenderer);
        lineRenderer.enabled = false;
        appearance = DeviceAppearance.REAL;
        isCollideHand = false;
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

    public IEnumerator StartListenToFetchTrigger()
    {
        while (true)
        {
            if (DataManager.instance.isDeviceFetched[(int)SceneState.SHOOTING_CLUB])
            {
                appearance = DeviceAppearance.VIRTUAL;
            }
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

    void OnTriggerEnter(Collider other) {
        if(other.tag == "Hand" && DataManager.instance.isDeviceFollowHand) {
            transform.position = other.gameObject.transform.position;
            transform.rotation = other.gameObject.transform.rotation;
        }
    }
}