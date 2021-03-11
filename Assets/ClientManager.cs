using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ClientState {
    
}

public class ClientManager : MonoBehaviour
{
    
    // Start is called before the first frame update
    void Start()
    {
        // Connect to server
        // Start polling thread
    }

    public void UpdateTrackersTransform() {
    }
    public void GetDevicesStatus() {
        // Clear status flag (isDeviceStatusReady)
        // Send request (set callback)
        // StartCoroutine(UpdateDevicesStatus());
    }  

    public void RequestDeviceStatus() {
        
    }

    public void RequestDevice() {
        
    }

    public void GetTrackerData() {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
