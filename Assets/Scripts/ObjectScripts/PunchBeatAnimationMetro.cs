using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PunchBeatAnimationMetro : Metronome {

    public Half activatedHalf;
    public bool isActivated;
    public float minAlpha;
    public Dictionary<Half, PunchBeatScript> punchBeatDict;
    
    public PunchBeatAnimationMetro(PunchBeatGame pbGame) {

        beatTime = pbGame.beatTime;
        firstBeatTime = pbGame.firstBeatTime;
        beatWidth = pbGame.animationBeatWidth;
        halfWidth = beatWidth / 2;

        punchBeatDict = pbGame.punchBeatDict;
        activatedHalf = Half.NONE;
        isActivated = false;
        minAlpha = 0.2f;
    }

    public override void OnBeatEnter() {
        //activatedHalf = (Half)(int)Random.Range(0, 3);
        activatedHalf = Half.LEFT;
        if(activatedHalf < Half.WHOLE) {
            punchBeatDict[activatedHalf].gameObject.GetComponent<Renderer>().material.DOFade(1f, halfWidth);
        }
        else if(activatedHalf == Half.WHOLE){
            punchBeatDict[Half.LEFT].gameObject.GetComponent<Renderer>().material.DOFade(1f, halfWidth);
            punchBeatDict[Half.RIGHT].gameObject.GetComponent<Renderer>().material.DOFade(1f, halfWidth);
        }
        isActivated = true;
    }
    public override void OnBeat() {
        if (activatedHalf < Half.WHOLE)
        {
            punchBeatDict[activatedHalf].gameObject.GetComponent<Renderer>().material.DOFade(minAlpha, halfWidth);
        }
        else if(activatedHalf == Half.WHOLE)
        {
            punchBeatDict[Half.LEFT].gameObject.GetComponent<Renderer>().material.DOFade(minAlpha, halfWidth);
            punchBeatDict[Half.RIGHT].gameObject.GetComponent<Renderer>().material.DOFade(minAlpha, halfWidth);
        }
    }
    public override void OnBeatExit() {
        isActivated = false;
    }
}