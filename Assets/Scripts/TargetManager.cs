using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TargetManager : MonoBehaviour
{

    public enum TargetMachineState
    {
        IDLE = 0,
        READY = 1,
        SHOOT = 2,
        RISING = 3
    }

    public static TargetManager instance;
    public int[] shootingOrder = { 
        1, 0, 0,
        1, 0, 0,
        0, 1, 0, 
        0, 0, 1,
        0, 0, 1, 
        1, 0, 1,
        0, 1, 0,
        1, 0, 0,
        0, 1, 1,
        0, 0, 1,
        1, 1, 0,
        1, 0, 1,
        0, 1, 1,
        1, 0, 0,
        1, 1, 1
        };

    public float occurrenceFrequency = 4.0f;
    public float risingTime = 1.0f;
    public float airTime = 0.5f;
    public float dashingTime = 5.0f;
    public float maxHeight = 1.5f;
    public float minHeight = -0.1f;
    public GameObject targetPrefab;
    public AnimationCurve targetCurve;
    public Transform handbookCenter;

    
    /* Initialized in Start() */
    public Transform[] targetGenerators;
    public List<float> targetColorsList;
    public List<GameObject> targetDemoList;
    private int nAliveTarget;
    
    /* ====================================== */

    private float[] targetColors;
    private GameObject[] targetDemos;
    
    private int shootingIndex = 0;
    private int risingCount = 0;

    private GameObject targetDemo;
    private GameObject target;
    private TargetMachineState state = TargetMachineState.IDLE;
     void Awake()
    {
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
        targetColorsList = new List<float>();
        targetDemoList = new List<GameObject>();
        List<Transform> targetGeneratorsList = new List<Transform>();
        foreach(Transform child in gameObject.transform) {
            targetGeneratorsList.Add(child);
        }
        targetGenerators = targetGeneratorsList.ToArray();
        foreach(int x in shootingOrder) {
            nAliveTarget += x;
        }
        nAliveTarget++;
    }

    public void KillTarget(GameObject _target)
    {
        _target.SetActive(false);
        nAliveTarget--;
    }

    public bool AllTargetsDie()
    {
        return nAliveTarget == 0;
    }

    public bool AllRisingComplete()
    {
        return risingCount == 0;
    }

    public void GetReady() {
        state = TargetMachineState.READY;
    }

    public void Reset() {
        state = TargetMachineState.IDLE;
    }

    public void AddTargetDemo() {
        targetDemo = Instantiate(targetPrefab, Vector3.one, Quaternion.identity);
        targetDemo.transform.parent = handbookCenter;
        targetDemo.transform.localPosition = Vector3.zero;
        targetDemo.transform.rotation = Quaternion.LookRotation(new Vector3(0f, 0f, -1f), Vector3.up);
        //targetDemo.transform.LookAt(Camera.main.transform);
        targetDemoList.Add(targetDemo);

        float hue = Random.Range(0f, 1f);
        targetColorsList.Add(hue);

        Renderer renderer = targetDemo.transform.GetChild(1).GetComponent<Renderer>();
        renderer.material.color = Color.HSVToRGB(hue, 0.6f, 1f);
    }

    public void UpdateHandbook() {
        targetDemos = targetDemoList.ToArray();
        Vector3 upLeftCorner = new Vector3(2f, -0.5f, 0f);
        
        int nCol = 11;
        int nRow = targetDemos.Length % nCol + 1;
        float scale = 0.3f;

        float hSpacing = upLeftCorner.x * 2 / (nCol - 1);
        float vSpacing = 0.25f;

        for(int i = 0; i < targetDemos.Length; ++i) {
            int col = i % nCol;
            int row = i / nCol;
            targetDemos[row * nCol + col].transform.localPosition 
                = new Vector3(upLeftCorner.x - hSpacing * col, upLeftCorner.y - vSpacing * row, upLeftCorner.z);
            targetDemos[row * nCol + col].transform.localScale = new Vector3(scale, scale, scale);
        }
    }

    public void DestroyTargetDemos() {
        if (targetDemo != null)
        {
            Destroy(targetDemo);
            foreach (GameObject targetDemo in targetDemoList)
            {
                Destroy(targetDemo);
            }
        }
    }

    public void ShootTargets() {
        for (int i = 0; i < targetGenerators.Length; i++)
        {
            if (shootingOrder[shootingIndex + i] == 1)
            {
                risingCount++;
                // Instantiate target
                target = Instantiate(targetPrefab, Vector3.one, Quaternion.identity);

                // Set target position
                target.transform.parent = targetGenerators[i];
                target.transform.localPosition = new Vector3(0, minHeight, 0);

                //Set target color
                Renderer renderer = target.transform.GetChild(1).GetComponent<Renderer>();
                int colorIndex = (shootingIndex + i) % targetColors.Length;
                renderer.material.color = Color.HSVToRGB(targetColors[colorIndex], 0.6f, 1f);

                TargetRise(target);
            }
        }
        shootingIndex += targetGenerators.Length;
    }

    private void TargetRise(GameObject target)
    {
        Sequence risingSequence = DOTween.Sequence();
        risingSequence.Append(target.transform.DOLocalMoveY(maxHeight, risingTime))
            .SetEase(targetCurve)
            .OnUpdate(() => { target.transform.LookAt(Camera.main.transform); })
            .AppendInterval(airTime)
            .AppendCallback(() => { OnRisingComplete(target); });
        risingSequence.Play();
    }

    private void OnRisingComplete(GameObject target)
    {
        risingCount--;
        TargetDash(target);
    }

    private void TargetDash(GameObject target)
    {
        Sequence dashingSequence = DOTween.Sequence();
        dashingSequence.Append(target.transform.DOMove(Camera.main.transform.position, dashingTime))
            .OnUpdate(() => { target.transform.LookAt(Camera.main.transform); })
            .AppendCallback(() => { OnDashingComplete(target); });
        dashingSequence.Play();
    }

    private void OnDashingComplete(GameObject target) {
        if (target.gameObject.activeInHierarchy)
        {
            KillTarget(target.gameObject);
        }
        Destroy(target);
    }
}