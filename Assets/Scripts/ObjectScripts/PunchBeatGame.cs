using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HitType {
    MISS = 0,
    GOOD = 1,
    PERFECT = 2
}

public class PunchBeatGame : MonoBehaviour {
    
    public int beatNum;
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

    void Init() {
        GameObject punchBeatLeft = Instantiate(punchBeatPrefab);
        punchBeatLeft.transform.parent = punchBeatLeftTransform;
        punchBeatLeft.transform.position = Vector3.zero;
        punchBeatLeft.transform.rotation = Quaternion.identity;

        GameObject punchBeatRight = Instantiate(punchBeatPrefab);
        punchBeatRight.transform.parent = punchBeatRightTransform;
        punchBeatRight.transform.position = Vector3.zero;
        punchBeatRight.transform.rotation = Quaternion.identity;

        punchBeatDict = new Dictionary<Half, PunchBeatScript>() {
            {Half.LEFT, punchBeatLeft.GetComponent<PunchBeatScript>()},
            {Half.RIGHT, punchBeatRight.GetComponent<PunchBeatScript>()},
        };

        pbAnimationMetro = new PunchBeatAnimationMetro(this);
        pbJudgementMetro = new PunchBeatJudgementMetro(this);
    }

    
    public void Run() {
        StartCoroutine(pbAnimationMetro.InfTick());
        StartCoroutine(pbJudgementMetro.InfTick());
    }

    public void End() {
        StopCoroutine(pbAnimationMetro.InfTick());
        StopCoroutine(pbJudgementMetro.InfTick());
    }

    public HitType JudgeHit() {
        if(pbAnimationMetro.isActivated) {
            return pbAnimationMetro.isActivated ? HitType.PERFECT : HitType.GOOD;
        }
        else {
            return HitType.MISS;
        }
    }
}