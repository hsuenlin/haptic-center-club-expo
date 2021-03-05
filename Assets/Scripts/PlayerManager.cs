using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    public Image healthBarImage;
    public Sprite[] healthBarSprites;
    public float health = 50f;
    public float damage = 5f;
    public int cnt = 7;

    void Start() {
    }
    void OnTriggerEnter(Collider collider) {
        if(collider.tag == "Target") {
            health -= damage;
            Debug.Log($"HP: {health}");
        }
    }
    void Update() {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            cnt += (cnt < 7) ? 1 : 0;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            cnt -= (cnt > 0) ? 1 : 0;
        }
        healthBarImage.sprite = healthBarSprites[cnt];
    }
    
}
