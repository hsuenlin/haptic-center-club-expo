using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class FetchTrigger : MonoBehaviour
{
    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other) {
        switch(other.name) {
            case "Gun":
                DataManager.instance.isDeviceFetched[(int)SceneState.SHOOTING_CLUB] = true;
            case "Racket":
                DataManager.instance.isDeviceFetched[(int)SceneState.TENNIS_CLUB] = true;
            case "LeftHandAnchor" || "RightHandAnchor":
                DataManager.instance.isDeviceFetched[(int)SceneState.MUSICGAME_CLUB] = true;
                
        }       
    }
}
