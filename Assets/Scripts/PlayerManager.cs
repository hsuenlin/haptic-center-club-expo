using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public float health = 50f;
    public float damage = 5f;
    void OnTriggerEnter(Collider collider) {
        if(collider.tag == "Target") {
            health -= damage;
            Debug.Log($"HP: {health}");
        }
    }
    
}
