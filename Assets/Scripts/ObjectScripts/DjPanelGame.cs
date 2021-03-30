using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class DjPanelGame : MonoBehaviour {

    /*
    public AudioSource audioSource;
    public AudioClip gameAudio;
    public GameObject punchBeatPrefab;
    public Transform punchBeatLeftTransform;
    public Transform punchBeatRightTransform;
    public Dictionary<Half, PunchBeatScript> punchBeatDict;

    public PunchBeatAnimationMetro pbAnimationMetro;
    public PunchBeatJudgementMetro pbJudgementMetro;

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

    public GameObject pbAnimationMetroObj;
    public GameObject pbJudgementMetroObj;

    private DjPanelScript djPanel;
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
        0, 0, 0, 0, 0, 1, 2, 3, // 1 beat

        0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0,
    };

    public void Init() {
        djPanel = DataManager.instance.djPanelObj.GetComponent<DjPanelScript>();

        dpAnimationMetro = dpAnimationMetroObj.GetComponent<DjPanelAnimationMetro>();
        dpJudgementMetro = dpJudgementMetroObj.GetComponent<DjPanelJudgementMetro>();

        dpAnimationMetro.Init(this);
        dpJudgementMetro.Init(this);

    }
    public void Run() {
        audioSource.clip = gameAudio;
        audioSource.PlayScheduled(AudioSettings.dspTime + 1.0);
        StartCoroutine(DelayStart(dpAnimationMetro.InfDspTick(), 1.0f));
        StartCoroutine(DelayStart(dpJudgementMetro.InfDspTick(), 1.0f));
    }

    public void End() {
        StopCoroutine(dpAnimationMetro.InfDspTick());
        StopCoroutine(dpJudgementMetro.InfDspTick());
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

    public HitType JudgeSlide() {
        
    }

    public IEnumerator DelayStart(IEnumerator task, float delay) {
        yield return new WaitForSecondsRealtime(delay);
        StartCoroutine(task);
    }
    */
}