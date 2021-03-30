using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PunchBeatAnimationMetro : Metronome {
    public Half activatedHalf;
    public bool isActivated;
    public float minAlpha;
    public Dictionary<Half, PunchBeatScript> punchBeatDict;
    private int[] beatScore;
    private int beatIdx;
    
    public void Init(PunchBeatGame pbGame) {
        InitMetro(pbGame.beatTime, pbGame.firstBeatTime, pbGame.animationBeatWidth);

        punchBeatDict = pbGame.punchBeatDict;
        activatedHalf = Half.NONE;
        isActivated = false;
        minAlpha = 0.2f;

        beatScore = pbGame.beatScore;
        beatIdx = 0;
        activatedHalf = (Half)beatScore[beatIdx];
    }

    public override void OnBeatEnter() {
        activatedHalf = (Half)beatScore[beatIdx];
        if (activatedHalf < Half.WHOLE)
        {
            punchBeatDict[activatedHalf].gameObject.GetComponent<Renderer>().material.DOFade(1f, (float)halfWidth);
        }
        else if (activatedHalf == Half.WHOLE)
        {
            punchBeatDict[Half.LEFT].gameObject.GetComponent<Renderer>().material.DOFade(1f, (float)halfWidth);
            punchBeatDict[Half.RIGHT].gameObject.GetComponent<Renderer>().material.DOFade(1f, (float)halfWidth);
        }
        isActivated = true;
    }
    public override void OnBeat() {
        if (activatedHalf < Half.WHOLE)
        {
            punchBeatDict[activatedHalf].gameObject.GetComponent<Renderer>().material.DOFade(minAlpha, (float)halfWidth);
        }
        else if (activatedHalf == Half.WHOLE)
        {
            punchBeatDict[Half.LEFT].gameObject.GetComponent<Renderer>().material.DOFade(minAlpha, (float)halfWidth*2);
            punchBeatDict[Half.RIGHT].gameObject.GetComponent<Renderer>().material.DOFade(minAlpha, (float)halfWidth*2);
        }
    }
    public override void OnBeatExit() {
        isActivated = false;
        beatIdx++;
    }
}