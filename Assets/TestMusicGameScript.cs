using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TestMusicGameScript : MonoBehaviour
{
    public PunchBeatGame pbGame;
    void Awake() {
        pbGame.Init();
        pbGame.Run();
    }

    void OnDisable() {
        pbGame.End();
    }

}
    /*
    public float initTime;
    public float beatTime;
    public float hintTime;
    public float waitTime;
    public Renderer cube;
    public bool toggle;

    public AudioClip clip;
    public AudioSource audiosource;
    
    public Transform leftSpawnTransform;
    public Transform rightSpawnTransform;
    public Transform beatoPrefab;

    public float perfectRange;

    // Start is called before the first frame update
    void Start()
    {
        //audiosource.Play();
        hintTime = beatTime / 4;
        waitTime = beatTime - 2 * hintTime;

        //StartCoroutine(ToggleCube());

        //StartCoroutine(SpawnBeat());
    }

    IEnumerator ToggleCube() {
        cube.material.DOFade(0, 0);
        yield return new WaitForSeconds(initTime - hintTime);
        cube.material.DOFade(1, hintTime);
        while(true) {
            // 1 beat
            cube.material.DOFade(0, hintTime);
            yield return new WaitForSeconds(waitTime);
            cube.material.DOFade(1, hintTime);
            yield return new WaitForSeconds(waitTime);
        }
    }

    public int punchOrder = {1, 0, 2, 0, 1, 0, 2, 0};
    
    IEnumerator PunchAnimationBeat() {
        //cube.material.DOFade(0.2, 0);
        yield return new WaitForSeconds(initTime - hintTime);
        yield return new WaitForSeconds(hintTime);
        //cube.material.DOFade(1, hintTime);
        for(int i = 0; i < punchOrder.Length; ++i) {
            //cube.material.DOFade(0.2, hintTime);
            yield return new WaitForSeconds(hintTime);
            // Down
            yield return new WaitForSeconds(waitTime);
            // Up
            yield return new WaitForSeconds(hintTime);
            //cube.material.DOFade(1, hintTime);
        }
    }

    public float firstBeatTime;

    public IEnumerator Metronome(float beatWidth, Action OnBeatEnter, Action OnBeat, Action OnBeatExit) {
        float idleTime = beatTime - beatWidth;
        float halfWidth = beatWidth / 2;
        yield return new WaitForSeconds(firstBeatTime - halfWidth);
        while(true) {
            OnBeatEnter();
            yield return new WaitForSeconds(halfWidth);
            OnBeat();
            yield return new WaitForSeconds(halfWidth);
            OnBeatExit();
            yield return new WaitForSeconds(idleTime);
        }
    }

    public 

    IEnumerator PunchJudgeBeat() {
        yield return new WaitForSeconds(initTime - hintTime);
        for(int i = 0; i < punchOrder.Length; ++i) {
            
        }
    }

    IEnumerator DjPanelBeat() {
        yield return null;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    */