using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadyTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.name == "OVRCameraRig") {
            DataManager.instance.isInReadyZone[(int)GameManager.instance.currentSceneState] = true;
        }
    }
}
