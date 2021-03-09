using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;
    
    public bool isHit;
    public GameObject hitObject;

    public Transform handPose;
    
    void Awake() {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        // TODO: 
        // Pinch raycast
        // Update isHit and Hit
        
        
        
        
    }
}
