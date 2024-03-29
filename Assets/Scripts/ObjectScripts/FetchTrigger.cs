﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class FetchTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) {
        switch(other.name) {
            case "Gun":
                DataManager.instance.isDeviceFetched[(int)SceneState.SHOOTING_CLUB] = true;
                break;
            case "Racket":
                DataManager.instance.isDeviceFetched[(int)SceneState.TENNIS_CLUB] = true;
                break;
            case "LeftHandAnchor":
            case "RightHandAnchor":
                DataManager.instance.isDeviceFetched[(int)SceneState.MUSICGAME_CLUB] = true;
                break;
        }       
    }
}
