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

    public DeviceAppearance appearance;

    public GameObject viveModel;
    public GameObject gunModel;

    void Awake() {
        Assert.IsNotNull(muzzle);
        Assert.IsNotNull(lineRenderer);
        lineRenderer.enabled = false;
        appearance = DeviceAppearance.REAL;
    }

    private IEnumerator StartColdDown() {
        DataManager.instance.canShoot = false;
        yield return new WaitForSeconds(coldDownTime);
        DataManager.instance.canShoot = true;
    }

    public IEnumerator StartShotEffect() {
        //gunAudio.Play();
        lineRenderer.enabled = true;
        yield return new WaitForSeconds(shotDuration);
        lineRenderer.enabled = false;
    }

    public IEnumerator StartAutoShooting() {
        while(true) {
            Shoot();
            yield return new WaitForSeconds(autoShootingTime);
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
            yield return null;
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
                    TargetScript ts = hit.collider.gameObject.GetComponent<TargetScript>();
                    ts.hp--;
                    if(ts.hp == 0) {
                        TargetMachine.instance.KillTarget(hit.collider.gameObject);
                    }
                }
            } else {
                lineRenderer.SetPosition(1, transform.forward * 5000);
            }
            StartCoroutine(StartColdDown());
            StartCoroutine(StartShotEffect());
        }
    }

    void OnTriggerEnter(Collider other) {
        if(other.tag == "Hand") {
            //DataManager.instance.handSDK.SetActive(false);
            
            if(GameManager.instance.gameMode == GameMode.QUEST && !DataManager.instance.isDeviceFollowHand) {
                transform.parent = other.transform;
                transform.localPosition = new Vector3(-0.15f, -0.02f, 0.03f);
                transform.localEulerAngles = new Vector3(0f, -65f, 90f);
            }
        }
    }

    void OnDisable() {
        if (DataManager.instance.isDeviceFollowHand)
        {
            foreach (GameObject rayToolObj in DataManager.instance.rayTools)
            {
                rayToolObj.SetActive(true);
            }
        }
    }

    void Update() {
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