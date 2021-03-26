using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public enum HitType {
    MISS = 0,
    GOOD = 1,
    PERFECT = 2
}

public class PunchBeatGame : MonoBehaviour {

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
    public AudioSource audioSource;
    public GameObject missText;
    public GameObject goodText;
    public GameObject perfectText;

    public float textRiseHeight;
    public float textRiseTime;
    public float textFadeTime;

    public GameObject pbAnimationMetroObj;
    public GameObject pbJudgementMetroObj;
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
        GameObject punchBeatLeft = ClubUtil.InstantiateOn(punchBeatPrefab, punchBeatLeftTransform);
        punchBeatLeft.GetComponent<PunchBeatScript>().Init(this, punchBeatLeftTransform, Half.LEFT);
        punchBeatLeft.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        punchBeatLeft.GetComponent<Renderer>().material.DOFade(0.2f, 0f);

        GameObject punchBeatRight = ClubUtil.InstantiateOn(punchBeatPrefab, punchBeatRightTransform);
        punchBeatRight.GetComponent<PunchBeatScript>().Init(this, punchBeatRightTransform, Half.RIGHT);
        punchBeatRight.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        punchBeatRight.GetComponent<Renderer>().material.DOFade(0.2f, 0f);

        punchBeatDict = new Dictionary<Half, PunchBeatScript>() {
            {Half.LEFT, punchBeatLeft.GetComponent<PunchBeatScript>()},
            {Half.RIGHT, punchBeatRight.GetComponent<PunchBeatScript>()},
        };

        pbAnimationMetro = pbAnimationMetroObj.GetComponent<PunchBeatAnimationMetro>();
        pbJudgementMetro = pbJudgementMetroObj.GetComponent<PunchBeatJudgementMetro>();

        pbAnimationMetro.Init(this);
        pbJudgementMetro.Init(this);

    }
    public void Run() {
        audioSource.clip = gameAudio;
        audioSource.PlayScheduled(AudioSettings.dspTime + 1.07);
        //audioSource.PlayOneShot(gameAudio);
        StartCoroutine(pbAnimationMetro.InfDspTick());
        StartCoroutine(pbJudgementMetro.InfDspTick());
        //audioSource.PlayOneShot(gameAudio);
        Debug.Log("Hi");
    }

    public void End() {
        StopCoroutine(pbAnimationMetro.InfTick());
        StopCoroutine(pbJudgementMetro.InfTick());
        audioSource.Pause();
    }

    public HitType JudgeHit(Half half) {
        if(half == pbAnimationMetro.activatedHalf || pbAnimationMetro.activatedHalf == Half.WHOLE) {
            if(pbAnimationMetro.isActivated) {
                return pbJudgementMetro.isPerfact ? HitType.PERFECT : HitType.GOOD;
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
}