using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class ReadyTrigger : MonoBehaviour
{

    private void OnTriggerStay(Collider other)
    {
        DataManager.instance.isInReadyZone[(int)GameManager.instance.currentSceneState] = true;
    }
}
