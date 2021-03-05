using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    public Image healthBarImage;
    public Sprite[] healthBarSprites;
    public int health = 7;
    public float damageAnimationTime = 0.5f;
    public float timer = 0f;
    public bool isDamaged = false;
    public OldCinemaEffect damageAnimationScript;

    void Start() {
        damageAnimationScript = Camera.main.GetComponent<OldCinemaEffect>();
    }
    void OnTriggerEnter(Collider collider) {
        if(collider.tag == "Target") {
            health -= (health > 0) ? 1 : 0;
            damageAnimationScript.enabled = true;
            isDamaged = true;
            timer = 0f;
        }
    }
    void Update() {
        healthBarImage.sprite = healthBarSprites[health];
        if(isDamaged) {
            timer += Time.deltaTime;
            if(timer > damageAnimationTime) {
                damageAnimationScript.enabled = false;
                isDamaged = false;
            }
        }
    }
    
}
