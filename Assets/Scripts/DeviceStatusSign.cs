using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public virtual class DeviceStatusSign: MonoBehaviour
{
    public Material busyMaterial;
    public Material freeMaterial;
    public MeshRenderer renderer;
    public SceneState signifiedScene;
    
    void Awake() {
        Assert.IsNotNull(busyMaterial);
        Assert.IsNotNull(freeMaterial);
        Assert.IsNotNull(renderer);
        Assert.IsNotNull(signifiedScene);
    }

    void Update() {
        // Fetching device status
        if(ArenaManager.instance.isDeviceReadyDict[signifiedScene]) {
            renderer.material = freeMaterial;
        }
        else {
            renderer.material = busyMaterial;
        }
    }

    
}