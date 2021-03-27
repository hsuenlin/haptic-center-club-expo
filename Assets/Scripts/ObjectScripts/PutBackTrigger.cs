using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class PutBackTrigger : MonoBehaviour
{

    public GameObject propStand;
    public Device requiredDevice;
    public float waitingTime;
    private float timer = 0f;
    public void OnTriggerEnter(Collider other)
    {
        if(GameManager.instance.currentSceneState == SceneState.SHOOTING_CLUB && other.name == "Gun"
            || GameManager.instance.currentSceneState == SceneState.TENNIS_CLUB && other.tag == "Racket"
            || GameManager.instance.currentSceneState == SceneState.MUSICGAME_CLUB && other.tag == "Hand") 
        {
            timer = 0f;
        }
        /*
        if (other.tag == "Gun" || other.tag == "Racket")
        {
            other.gameObject.GetComponent<GunScript>().appearance = DeviceAppearance.REAL;
        }
        */
    }
    public void OnTriggerStay(Collider other) {
        
        if (GameManager.instance.currentSceneState == SceneState.SHOOTING_CLUB && other.tag == "Gun"
            || GameManager.instance.currentSceneState == SceneState.TENNIS_CLUB && other.tag == "Racket"
            || GameManager.instance.currentSceneState == SceneState.MUSICGAME_CLUB && other.tag == "Hand")
        {
            if (timer > waitingTime)
            {
                DataManager.instance.isPropPutBack[(int)requiredDevice] = true;
                if (GameManager.instance.gameMode == GameMode.QUEST)
                {
                    DataManager.instance.isDeviceFollowHand = false;
                }
                if (other.tag == "Gun" || other.tag == "Racket")
                {
                    other.gameObject.transform.parent = propStand.transform;
                }
                DataManager.instance.leftHandPrefab.SetActive(true);
                DataManager.instance.rightHandPrefab.SetActive(true);
                foreach (GameObject rayToolObj in DataManager.instance.rayTools)
                {
                    rayToolObj.SetActive(true);
                }
            }
            timer += Time.deltaTime;
        }
        
    }
}