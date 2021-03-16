using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;

public class PlayerManager : MonoBehaviour
{
    public int health; //7
    public Image healthBarImage;
    public Sprite[] healthBarSprites;

    public OldCinemaEffect damageAnimationScript;
    public float damageAnimationTime; //0.5f
    

    void Awake() {
        Assert.AreNotEqual(0, health);
        Assert.IsNotNull(healthBarImage);
        
        Assert.IsNotNull(healthBarSprites);
        Assert.IsNotNull(damageAnimationScript);
        Assert.AreNotApproximatelyEqual(0f, damageAnimationTime);

        damageAnimationScript.enabled = false;
    }

    private IEnumerator StartDamageAnimation() {
        float timer = 0f;
        damageAnimationScript.enabled = true;
        while(timer < damageAnimationTime) {
            timer += Time.deltaTime;
            yield return null;
        }
        damageAnimationScript.enabled = false;
    }

    void OnTriggerEnter(Collider collider) {
        if(collider.tag == "Target" 
            && GameManager.instance.currentSceneState == SceneState.SHOOTING_CLUB) {
            health -= (health > 0) ? 1 : 0;
            healthBarImage.sprite = healthBarSprites[health];
            StartCoroutine(StartDamageAnimation());
        }
    }
}
