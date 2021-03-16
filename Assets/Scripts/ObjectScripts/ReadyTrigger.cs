using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class ReadyTrigger : MonoBehaviour
{
    public float delay;
    private bool isCountDown;

    void Awake() {
        Assert.IsFalse(isCountDown);
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.name == DataManager.instance.player.name) {
            if(GameManager.instance.gameMode == GameMode.QUEST && !isCountDown) {
                StartCoroutine(Timer.StartTimer(delay, ()=>{
                    DataManager.instance.isInReadyZone[(int)GameManager.instance.currentSceneState] = true;
                }));
                isCountDown = true;
            }
            else {
                DataManager.instance.isInReadyZone[(int)GameManager.instance.currentSceneState] = true;
            }
        }
    }
}
