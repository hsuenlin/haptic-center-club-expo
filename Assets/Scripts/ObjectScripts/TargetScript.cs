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

    void Awake() {
        attackImage.gameObject.SetActive(false);
        hpImage.gameObject.SetActive(false);
    }

    public IEnumerator UpdateAbilitiesView() {
        isDisplay = true;
        attackImage.gameObject.SetActive(true);
        hpImage.gameObject.SetActive(true);
        while(true) {
            hpImage.fillAmount = ((float)hp) / 5;
            attackImage.fillAmount = ((float)attack) / 5;
            yield return null;
        }
    }

    void OnDestroy() {
        if(isDisplay) {
            StopCoroutine(UpdateAbilitiesView());
        }
    }
}
