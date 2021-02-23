using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TargetManager : MonoBehaviour
{
    public int[] shootingSequence = { 1, 0, 0, 0, 1, 0, 0, 0, 1, 1, 1, 1 };
    public float occurrenceFrequency = 2.0f;
    public float airTimeRatio = 0.5f;
    public float maxHeight = 1.5f;
    public float minHeight = -0.1f;
    public GameObject targetPrefab;
    [SerializeField]
    public AnimationCurve targetCurve;

    public Transform[] targetGenerators;
    
    private int shootingIndex = 0;
    private int shootingCount = 0;

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
        List<Transform> targetGeneratorsList = new List<Transform>();
        foreach(Transform child in gameObject.transform) {
            targetGeneratorsList.Add(child);
        }
        targetGenerators = targetGeneratorsList.ToArray();
    }
    
    private GameObject target;

    TweenCallback OnShootingComplete(GameObject target) {
        shootingCount--;
        Destroy(target);
        if(shootingCount == 0) {
            targetState = TargetState.Ready;
        }
        return null;
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
            if(shootingIndex < shootingSequence.Length) {
                targetState = TargetState.OnShoot;
            }
            else {
                shootingIndex %= shootingSequence.Length;
                targetState = TargetState.Idle;
            }
        }
        if(targetState == TargetState.OnShoot) {
            
            for(int i = 0; i < targetGenerators.Length; i++) {
                if(shootingSequence[shootingIndex + i] == 1) {
                    shootingCount++;
                    // Instantiate target
                    target = Instantiate(targetPrefab, Vector3.one, Quaternion.identity);
                    target.transform.parent = targetGenerators[i];
                    target.transform.localPosition = new Vector3(0, minHeight, 0);
                    // Tween
                    float movingTime = occurrenceFrequency * ((1 - airTimeRatio) / 2);
                    float airTime = occurrenceFrequency * airTimeRatio;
                    int test;
                    Sequence movingSequence = DOTween.Sequence();
                    movingSequence.Append(target.transform.DOLocalMoveY(maxHeight, movingTime))
                        .SetEase(targetCurve)
                        .OnUpdate(() => { target.transform.LookAt(Camera.main.transform); })
                        .AppendInterval(airTime)
                        .Append(target.transform.DOLocalMoveY(minHeight, movingTime))
                        .AppendCallback(() => {OnShootingComplete(target);});
                    movingSequence.Play();
                } 
            }
            shootingIndex += targetGenerators.Length;
            targetState = TargetState.Shooting;
        }
    }
}
