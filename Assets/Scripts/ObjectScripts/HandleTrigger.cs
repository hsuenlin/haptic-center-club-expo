using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleTrigger : MonoBehaviour
{
    public GameObject racketObj;
    private bool isFetched;
    // Start is called before the first frame update

    void Start() {
        racketObj = DataManager.instance.racketObj;
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Hand")
        {
            //DataManager.instance.handSDK.SetActive(false);
            if (GameManager.instance.gameMode == GameMode.QUEST && !DataManager.instance.isDeviceFollowHand)
            {
                racketObj.transform.parent = other.transform;
                racketObj.transform.localPosition = new Vector3(-0.09f, -0.04f, 0f);
                racketObj.transform.localEulerAngles = new Vector3(90f, -70f, 90f);
            }
        }
    }

    void OnDisable()
    {
        if(DataManager.instance.isDeviceFollowHand) {
            foreach (GameObject rayToolObj in DataManager.instance.rayTools)
            {      
                rayToolObj.SetActive(true);
            }
        }
    }
}
