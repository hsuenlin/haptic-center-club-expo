using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class DjPanelGame : MonoBehaviour {

    public AudioSource audioSource;
    public AudioClip gameAudio;
    public Dictionary<Half, GameObject> djPanelButtonDict;

    public DjPanelAnimationMetro dpAnimationMetro;
    public DjPanelJudgementMetro dpJudgementMetro;

    public float beatTime;
    public float firstBeatTime;
    public float animationBeatWidth;
    public float judgementBeatWidth;

    public AudioClip missSound;
    public AudioClip goodSound;
    public AudioClip perfectSound;
    public Animation missAnimation;
    public Animation goodAnimation;
    public Animation perfectAnimation;
    public GameObject missText;
    public GameObject goodText;
    public GameObject perfectText;

    public float textRiseHeight;
    public float textRiseTime;
    public float textFadeTime;

    public GameObject dpAnimationMetroObj;
    public GameObject dpJudgementMetroObj;

    private DjPanelScript djPanel;
    private GameObject sliderIndicator;
    public int[] beatScore = {
        3, 1, 3, 0, 3, 1, 3, 0,
        3, 1, 3, 0, 3, 1, 3, 0,
        3, 1, 3, 0, 3, 1, 3, 0,
        3, 1, 3, 0, 3, 1, 3, 0,
        
        3, 1, 0, 1, 3, 1, 0, 1,
        3, 1, 0, 1, 3, 1, 0, 1,
        3, 1, 3, 1, 3, 1, 3, 1,
        1, 0, 1, 0, 2, 3, 3, 3,

        1, 0, 0, 1, 1, 0, 0, 1,
        1, 0, 0, 1, 1, 0, 1, 1,
        2, 0, 0, 1, 1, 0, 0, 1,
        1, 0, 0, 1, 1, 0, 1, 2
    };

    // Slider on the beginning
    public int[] sliderScore = {
        0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0,

        0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 3, 0, 0, 0,

        0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0,
    };

    public DjPanelButtonScript[] buttonScripts;
    public GameObject[] buttons;

    public Action OnGameOver;

    private bool isAnimationMetroEnd;
    private bool isJudgementMetroEnd;

    public void Init() {
        djPanel = DataManager.instance.djPanelObj.GetComponent<DjPanelScript>();
        djPanel.Init(this);
        djPanelButtonDict = new Dictionary<Half, GameObject>();
        djPanelButtonDict[Half.LEFT] = buttons[0];
        djPanelButtonDict[Half.RIGHT] = buttons[1];
        //sliderIndicator.transform.GetChild(0).GetComponent<Renderer>().material.DOFade(0f, 0f);

        dpAnimationMetro = dpAnimationMetroObj.GetComponent<DjPanelAnimationMetro>();
        dpJudgementMetro = dpJudgementMetroObj.GetComponent<DjPanelJudgementMetro>();

        dpAnimationMetro.Init(this);
        dpJudgementMetro.Init(this);
    }
    public void Run() {
        audioSource.clip = gameAudio;
        audioSource.PlayScheduled(AudioSettings.dspTime + 1.0);
        StartCoroutine(DelayStart(dpAnimationMetro.DspTick(beatScore.Length, ()=>{ isAnimationMetroEnd = true; }), 1.0f));
        StartCoroutine(DelayStart(dpJudgementMetro.DspTick(beatScore.Length, ()=>{ isJudgementMetroEnd = true; }), 1.0f));
        StartCoroutine(Game());
    }

    public IEnumerator Game() {
        while(!isAnimationMetroEnd && !isJudgementMetroEnd) {yield return null;}
        OnGameOver();
        yield return null;
    }

    public void End() {
        //StopCoroutine(DelayStart(dpAnimationMetro.DspTick(beatNum, OnGameOver), 1.0f));
        //StopCoroutine(DelayStart(dpJudgementMetro.DspTick(beatNum, OnGameOver), 1.0f));
        audioSource.Pause();
    }

    public HitType JudgeHit(Half half) {
        if(half == dpAnimationMetro.activatedHalf || dpAnimationMetro.activatedHalf == Half.WHOLE) {
            if(dpAnimationMetro.isActivated) {
                return dpJudgementMetro.isPerfact ? HitType.PERFECT : HitType.GOOD;
            }
            else {
                return HitType.MISS;
            }
        }
        else
        {
            return HitType.MISS;
        }
    }

    public void JudgeSlide() {
        
    }

    public IEnumerator DelayStart(IEnumerator task, float delay) {
        yield return new WaitForSecondsRealtime(delay);
        StartCoroutine(task);
    }
    
}