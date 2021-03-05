using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TargetMachine : MonoBehaviour
{
    public Image healthBarImage;
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
    
    private float timer = 0f;
    private float readyTextTime = 1f;
    private float startTextTime = 1.75f;
    public Text readyText;
    public Text startText;
    public Text finishText;
    public Image aim;
        
    // TODO: Polish states
    public enum ShootingState {
        Idle,
        Intro,
        IntroEnd,
        Ready,
        OnShoot,
        Shooting,
        Result
    };

    public ShootingState state = ShootingState.Idle;
    private int cnt = 0;
    public int nTarget = 0;

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
        foreach(int x in shootingOrder) {
            nTarget += x;
        }
        nTarget++;
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
            .OnUpdate(() => { target.transform.LookAt(Camera.main.transform); })
            .AppendCallback(() => { 
                Destroy(target);
                if (nTarget == 0 && state != ShootingState.Result)
                {
                    state = ShootingState.Result;
                }
                else if(target.gameObject.activeInHierarchy) {
                    nTarget--;
                    Debug.Log($"Damage: {nTarget}");
                    
                }
            });
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

    public void DestroyTargetDemos() {
        if (targetDemo != null)
        {
            Destroy(targetDemo);
            foreach (GameObject target in targetDemoList)
            {
                Destroy(target);
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

    // Update is called once per frame
    void Update()
    {   
        // TODO: Move this part to ShootingClubManager
        if(Input.GetKeyDown(KeyCode.DownArrow)) {
            if(state == ShootingState.Idle) {
                // Change state
                state = ShootingState.Intro;
                GenerateTargetDemo();
            }
            else if(state == ShootingState.IntroEnd) {
                timer = 0f;
                // Change state
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
                DestroyTargetDemos();
                // InitIntroEnd()
                targetColors = targetColorsList.ToArray();
                // Change state
                state = ShootingState.IntroEnd;
            }
        }
        if (state == ShootingState.Ready)
        {
            // Rewrite timer (startTextTime, readyTextTime)
            if(timer > startTextTime) {
                if(shootingIndex < shootingOrder.Length) {
                    // InitOnShoot()
                    startText.gameObject.SetActive(false);
                    healthBarImage.gameObject.SetActive(true);
                    aim.gameObject.SetActive(true);
                    GameObject.Find("ShootingClubManager").GetComponent<SmoothMouseLook>().enabled = true;
                    
                    // Change state
                    state = ShootingState.OnShoot;
                }
                else {
                    shootingIndex %= shootingOrder.Length;
                    state = ShootingState.IntroEnd;
                }
            } else if(timer > readyTextTime && timer < startTextTime){
                readyText.gameObject.SetActive(false);
                startText.gameObject.SetActive(true);
            } else if(timer < readyTextTime){
                readyText.gameObject.SetActive(true);
            }
            timer += Time.deltaTime;
        }
        if(state == ShootingState.OnShoot) {
            ShootTargets();
            // Change state
            state = ShootingState.Shooting;
        }
        if (state == ShootingState.Result) {
            GameObject.Find("ShootingClubManager").GetComponent<SmoothMouseLook>().enabled = false;
            Debug.Log("Finish!");
            healthBarImage.gameObject.SetActive(false);
            aim.gameObject.SetActive(false);
            finishText.gameObject.SetActive(true);

            // Change state
            state = ShootingState.IntroEnd;
        }
    }
}