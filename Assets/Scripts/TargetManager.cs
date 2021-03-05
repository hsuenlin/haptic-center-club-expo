using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TargetManager : MonoBehaviour
{
    public int[] shootingOrder = { 1, 0, 0, 0, 1, 0, 0, 0, 1, 1, 1, 1 };
    public float occurrenceFrequency = 2.0f;
    public float risingTime = 1.0f;
    public float airTime = 0.5f;
    public float dashingTime = 5.0f;
    public float maxHeight = 1.5f;
    public float minHeight = -0.1f;
    public GameObject targetPrefab;
    [SerializeField]
    public AnimationCurve targetCurve;

    public Transform[] targetGenerators;
    public List<float> targetColorsList;
    public List<GameObject> targetDemoList;
    public float[] targetColors;
    
    private int shootingIndex = 0;
    private int risingCount = 0;

    private GameObject targetDemo;
    private GameObject target;
    

    public enum ShootingState {
        Idle,
        Intro,
        IntroEnd,
        Ready,
        OnShoot,
        Shooting,
    };

    public ShootingState state = ShootingState.Idle;

    // Start is called before the first frame update
    void Start()
    {
        targetColorsList = new List<float>();
        targetDemoList = new List<GameObject>();
        List<Transform> targetGeneratorsList = new List<Transform>();
        foreach(Transform child in gameObject.transform) {
            targetGeneratorsList.Add(child);
        }
        targetGenerators = targetGeneratorsList.ToArray();
    }

    void OnRisingComplete(GameObject target) {
        risingCount--;
        TargetDash(target);
        if(risingCount == 0) {
            state = ShootingState.Ready;
        }
    }

    void TargetRise(GameObject target) {
        Sequence risingSequence = DOTween.Sequence();
        risingSequence.Append(target.transform.DOLocalMoveY(maxHeight, risingTime))
            .SetEase(targetCurve)
            .OnUpdate(() => {target.transform.LookAt(Camera.main.transform);})
            .AppendInterval(airTime)
            .AppendCallback(() => {OnRisingComplete(target);});
        risingSequence.Play();
    }

    void TargetDash(GameObject target) {
        Sequence dashingSequence = DOTween.Sequence();
        dashingSequence.Append(target.transform.DOMove(Camera.main.transform.position, dashingTime))
            .OnUpdate(() => { target.transform.LookAt(Camera.main.transform); });
        dashingSequence.Play();
    }

    void GenerateTargetDemo() {
        targetDemo = Instantiate(targetPrefab, Vector3.one, Quaternion.identity);
        targetDemo.transform.parent = Camera.main.transform;
        targetDemo.transform.localPosition = new Vector3(0, 0, 5);
        targetDemo.transform.LookAt(Camera.main.transform);
        targetDemoList.Add(targetDemo);

        float hue = Random.Range(0f, 1f);
        targetColorsList.Add(hue);

        Renderer renderer = targetDemo.transform.GetChild(1).GetComponent<Renderer>();
        renderer.material.color = Color.HSVToRGB(hue, 0.6f, 1f);
    }

    void UpdateHandbook() {
        GameObject[] targetDemos = targetDemoList.ToArray();
        Vector3 upLeftCorner = new Vector3(-2f, -0.5f, 5f);
        
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

    // Update is called once per frame
    void Update()
    {   
        // TODO: Move this part to ShootingClubManager
        if(Input.GetKeyDown(KeyCode.DownArrow)) {
            if(state == ShootingState.Idle) {
                state = ShootingState.Intro;
                GenerateTargetDemo();
            }
            else if(state == ShootingState.IntroEnd) {
                state = ShootingState.Ready;
            }
        }
        if(state == ShootingState.Intro) {
            // Choose color
            if(Input.GetKeyDown(KeyCode.RightArrow)) {
                if(targetDemo != null) {
                    //Destroy(targetDemo);
                    UpdateHandbook();
                }
                GenerateTargetDemo();
            }
            else if(Input.GetKeyDown(KeyCode.Space)) {
                if(targetDemo != null) {
                    Destroy(targetDemo);
                    foreach(GameObject target in targetDemoList) {
                        Destroy(target);
                    }
                }
                targetColors = targetColorsList.ToArray();
                state = ShootingState.IntroEnd;
            }
        }
        if (state == ShootingState.Ready)
        {
            if(shootingIndex < shootingOrder.Length) {
                state = ShootingState.OnShoot;
            }
            else {
                shootingIndex %= shootingOrder.Length;
                state = ShootingState.IntroEnd;
            }
        }
        if(state == ShootingState.OnShoot) {
            
            for(int i = 0; i < targetGenerators.Length; i++) {
                Debug.Log(shootingIndex + i);
                if(shootingOrder[shootingIndex + i] == 1) {
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
            state = ShootingState.Shooting;
        }
    }
}