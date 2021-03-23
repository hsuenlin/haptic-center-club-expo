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
        Debug.Log($"Enter: {other.name}");
        if(GameManager.instance.currentSceneState == SceneState.SHOOTING_CLUB && other.name == "Gun"
            || GameManager.instance.currentSceneState == SceneState.TENNIS_CLUB && other.name == "Racket"
            || GameManager.instance.currentSceneState == SceneState.MUSICGAME_CLUB && other.tag == "Hand") 
        {
            timer = 0f;
            foreach (GameObject rayToolObj in DataManager.instance.rayTools)
            {
                rayToolObj.SetActive(true);
            }
        }
        if (other.name == "Gun" || other.name == "Racket")
        {
            other.gameObject.GetComponent<GunScript>().appearance = DeviceAppearance.REAL;
        }
    }
    public void OnTriggerStay(Collider other) {
        Debug.Log($"Stay: {other.name}, {timer}");
        
        if (GameManager.instance.currentSceneState == SceneState.SHOOTING_CLUB && other.name == "Gun"
            || GameManager.instance.currentSceneState == SceneState.TENNIS_CLUB && other.name == "Racket"
            || GameManager.instance.currentSceneState == SceneState.MUSICGAME_CLUB && other.tag == "Hand")
        {
            if (timer > waitingTime)
            {
                DataManager.instance.isPropPutBack[(int)requiredDevice] = true;
                if (DataManager.instance.isDeviceFollowHand)
                {
                    Debug.Log("Not follow hand");
                    DataManager.instance.isDeviceFollowHand = false;
                    if(other.name == "Gun" || other.name == "Racket") {
                        other.gameObject.transform.parent = propStand.transform;
                    }
                }
            }
            timer += Time.deltaTime;
        }
        
    }
}