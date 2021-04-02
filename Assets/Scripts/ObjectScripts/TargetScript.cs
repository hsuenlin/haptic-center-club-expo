using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TargetScript : MonoBehaviour
{
    public int attack;
    public int hp;

    public Image attackImage;
    public Image hpImage;
    private bool isDisplay = false;

    public IEnumerator UpdateAbilitiesView() {
        isDisplay = true;
        while(true) {
            yield return null;
        }
    }

    void OnDestroy() {
        if(isDisplay) {
            StopCoroutine(UpdateAbilitiesView());
        }
    }
}
