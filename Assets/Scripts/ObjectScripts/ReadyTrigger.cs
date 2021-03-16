using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadyTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.name == DataManager.instance.player.name) {
            DataManager.instance.isInReadyZone[(int)GameManager.instance.currentSceneState] = true;
        }
    }
}
