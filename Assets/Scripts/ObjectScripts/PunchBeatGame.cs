using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private PunchBeatAnimationMetro pbAnimationMetro;
    private PunchBeatJudgementMetro pbJudgementMetro;

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


    public void Init() {
        GameObject punchBeatLeft = ClubUtil.InstantiateOn(punchBeatPrefab, punchBeatLeftTransform);
        punchBeatLeft.GetComponent<PunchBeatScript>().Init(this, punchBeatLeftTransform);
        punchBeatLeft.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

        GameObject punchBeatRight = ClubUtil.InstantiateOn(punchBeatPrefab, punchBeatRightTransform);
        punchBeatRight.GetComponent<PunchBeatScript>().Init(this, punchBeatRightTransform);
        punchBeatRight.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

        punchBeatDict = new Dictionary<Half, PunchBeatScript>() {
            {Half.LEFT, punchBeatLeft.GetComponent<PunchBeatScript>()},
            {Half.RIGHT, punchBeatRight.GetComponent<PunchBeatScript>()},
        };

        pbAnimationMetro = new PunchBeatAnimationMetro(this);
        pbJudgementMetro = new PunchBeatJudgementMetro(this);
    }

    
    public void Run() {
        audioSource.PlayOneShot(gameAudio);
        StartCoroutine(pbAnimationMetro.InfTick());
        StartCoroutine(pbJudgementMetro.InfTick());
    }

    public void End() {
        StopCoroutine(pbAnimationMetro.InfTick());
        StopCoroutine(pbJudgementMetro.InfTick());
        audioSource.Pause();
    }

    public HitType JudgeHit() {
        if(pbAnimationMetro.isActivated) {
            return pbAnimationMetro.isActivated ? HitType.PERFECT : HitType.GOOD;
        }
        else {
            return HitType.MISS;
        }
    }

    public Half GetActivatedHalf() {
        return pbAnimationMetro.activatedHalf;
    }
}