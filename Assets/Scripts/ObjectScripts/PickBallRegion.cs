using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using OculusSampleFramework;
public class PickBallRegion : HandsInteractable
{
    public GameObject ballContainer;
    public GameObject ballPrefab;
    public int maxBallNum;
    public float sideLength;
    public Stack<GameObject> ballStack;
    public float throwTime;
    public float spawnTime;
    public float spawnThreshold;
    public int spawnNum;

    private bool isSpawning;
    private bool isThrowing;
    void Awake()
    {
        ballStack = new Stack<GameObject>();
        isSpawning = false;
        isThrowing = false;
    }

    public void Init() {
        for (int i = 0; i < maxBallNum; ++i)
        {
            ballStack.Push(InstantiateBall());
        }
    }

    private GameObject InstantiateBall() {
        GameObject ball = Instantiate(ballPrefab);
        ball.transform.parent = transform;
        ball.transform.localPosition = GetSquareRandomPosition();
        ball.transform.localRotation = Quaternion.identity;
        return ball;
    }

    private Vector3 GetSquareRandomPosition() {
        float x = Random.Range(-sideLength / 2, sideLength / 2);
        float z = Random.Range(-sideLength / 2, sideLength / 2);
        return new Vector3(x, 0f, z);
    }
    public IEnumerator Throw() {
        while(true) {
            if(ballStack.Count > 0) {
                GameObject ball = ballStack.Pop();
                Debug.Log("THROW!!!");
                StartCoroutine(ball.GetComponent<BallScript>().Track(ballContainer));
            }
            yield return new WaitForSeconds(throwTime);
        }
    }

    public IEnumerator Spawn(int nSpawn) {
        while(true) {
            if (ballStack.Count / maxBallNum < spawnThreshold) {
                if(ballStack.Count + nSpawn >= maxBallNum) {
                    nSpawn = maxBallNum - ballStack.Count;
                }
                for(int i = 0; i < nSpawn; ++i) {
                    ballStack.Push(InstantiateBall());
                }
            }
            yield return new WaitForSeconds(spawnTime);
        }
    }

    public override void OnNoInput() {
        if (isThrowing)
        {
            StopCoroutine(Throw());
            isThrowing = false;
        }
        if(!isSpawning) {
            StartCoroutine(Spawn(spawnNum));
            isSpawning = true;
        }
    }
    public override void OnPrimaryInputDown() {
        if(isSpawning) {
            StopCoroutine(Spawn(spawnNum));
            isSpawning = false;
        }
        if(!isThrowing) {
            Debug.Log("Throw start");
            StartCoroutine(Throw());
            isThrowing = true;
        }
    }
    void OnDestroy() {
        if(isThrowing) {
            StopCoroutine(Throw());
        }
        if(isSpawning) {
            StopCoroutine(Spawn(spawnNum));
        }
        while(ballStack.Count != 0) {
            GameObject ball = ballStack.Pop();
            Destroy(ball);
        }
        Destroy(ballContainer);
    }
}
