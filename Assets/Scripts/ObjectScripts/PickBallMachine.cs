using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using OculusSampleFramework;
public class PickBallMachine : MonoBehaviour
{
    public float spawnTime;
    public GameObject regionPrefab;
    public PickBallRegion[] regions;

    public int maxBallNum;
    public float spawnThreshold;

    public int spawnNum;
    public float sideLength;

    public float throwTime;

    public GameObject ballContainerPrefab;

    void Awake()
    {
        Assert.AreNotApproximatelyEqual(0f, spawnTime);
        if(spawnThreshold < 0f || spawnThreshold > 1f) {
            Debug.LogError("spawnThreshold is out of range [0, 1].");
        }
        if(spawnNum > maxBallNum) {
            Debug.LogError("SpawnNum is not less than maxBallNum");
        }
    }

    public void Init() {
        regions = new PickBallRegion[8];

        // Regions are generated clockwise
        float d = sideLength / 2;
        float[] xs = new float[8] {0f, d, d, d, 0, -d, -d, -d};
        float[] zs = new float[8] {d, d, 0, -d, -d, -d, 0, d};
        for(int i = 0; i < 8; ++i) {
            GameObject regionObj = Instantiate(regionPrefab);
            PickBallRegion region = regionObj.GetComponent<PickBallRegion>();
            region.transform.parent = transform;
            region.transform.localPosition = new Vector3(xs[i], 0f, zs[i]);
            region.transform.localRotation = Quaternion.identity;
            region.maxBallNum = maxBallNum;
            region.sideLength = sideLength;
            region.throwTime = throwTime;
            region.spawnThreshold = spawnThreshold;
            region.spawnNum = spawnNum;
            GameObject ballContainer = Instantiate(ballContainerPrefab, DataManager.instance.playerCamera.gameObject.transform);
            ballContainer.transform.localPosition = new Vector3(0f, -0.5f, 0.5f);
            region.ballContainer = ballContainer;
            regions[i] = region;
        }

    }
    public void End() {
        foreach(PickBallRegion region in regions) {
            Destroy(region.gameObject);
        }
    }
}
