using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TargetManager : MonoBehaviour
{
    public int[] targetSequence = { 1, 0, 0, 0, 1, 0, 0, 0, 1, 1, 1, 1 };
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

    public Queue<GameObject> dashingQueue;
    public Queue<bool> shootingQueue;
    
    private int shootingIndex = 0;
    private int risingCount = 0;
    private int dashReady = 0;

    private enum TargetState {
        Idle,
        Ready,
        OnShoot,
        Shooting,  
    };

    private TargetState targetState = TargetState.Idle;

    // Start is called before the first frame update
    void Start()
    {
        dashingQueue = new Queue<GameObject>();
        List<Transform> targetGeneratorsList = new List<Transform>();
        foreach(Transform child in gameObject.transform) {
            targetGeneratorsList.Add(child);
        }
        targetGenerators = targetGeneratorsList.ToArray();
    }
    
    private GameObject target;

    void OnRisingComplete() {
        risingCount--;
        dashReady++;
        if(risingCount == 0) {
            targetState = TargetState.Ready;
        }
    }

    void OnTargetHitOnPlayer(GameObject target) {
        Debug.Log("HP--");
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.S)) {
            if(targetState == TargetState.Idle) {
                targetState = TargetState.Ready;
            }   
        }
        if (targetState == TargetState.Ready)
        {
            if(shootingIndex < targetSequence.Length) {
                targetState = TargetState.OnShoot;
            }
            else {
                shootingIndex %= targetSequence.Length;
                targetState = TargetState.Idle;
            }
        }
        if(targetState == TargetState.OnShoot) {
            
            for(int i = 0; i < targetGenerators.Length; i++) {
                if(targetSequence[shootingIndex + i] == 1) {
                    risingCount++;
                    // Instantiate target
                    target = Instantiate(targetPrefab, Vector3.one, Quaternion.identity);
                    target.transform.parent = targetGenerators[i];
                    target.transform.localPosition = new Vector3(0, minHeight, 0);
                    dashingQueue.Enqueue(target);
                    // Tween
                    Sequence risingSequence = DOTween.Sequence();
                    risingSequence.Append(target.transform.DOLocalMoveY(maxHeight, risingTime))
                        .SetEase(targetCurve)
                        .OnUpdate(() => { target.transform.LookAt(Camera.main.transform); })
                        .AppendInterval(airTime)
                        .AppendCallback(() => {OnRisingComplete();});
                    risingSequence.Play();
                }
            }
            shootingIndex += targetGenerators.Length;
            targetState = TargetState.Shooting;
        }
        while(dashReady > 0 && dashingQueue.Count > 0) {
            dashReady--;
            GameObject target = dashingQueue.Dequeue();
            Sequence dashingSequence = DOTween.Sequence();
            dashingSequence.Append(target.transform.DOMove(Camera.main.transform.position, dashingTime))
                .OnUpdate(() => { target.transform.LookAt(Camera.main.transform); });
            dashingSequence.Play();
        }
    }
}
