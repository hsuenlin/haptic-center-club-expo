using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class DeviceStatusSign: MonoBehaviour
{
    public Material busyMaterial;
    public Material freeMaterial;
    public Renderer meshRenderer;
    public Device signifiedDevice;
    
    void Awake() {
        Assert.IsNotNull(busyMaterial);
        Assert.IsNotNull(freeMaterial);
        Assert.IsNotNull(meshRenderer);
    }

    void Update() {
        // Fetching device status
        if(DataManager.instance.isDeviceReady[(int)signifiedDevice]) {
            meshRenderer.material = freeMaterial;
        }
        else {
            meshRenderer.material = busyMaterial;
        }
    }

    
}