using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DjPanelAnimationMetro : Metronome {
    public Half activatedHalf;
    public bool isActivated;
    public float minAlpha;
    public Dictionary<Half, DjPanelScript> djPanelDict;
    private int[] beatScore;
    private int beatIdx;
    
    public void Init(DjPanelGame dpGame) {
        InitMetro(dpGame.beatTime, dpGame.firstBeatTime, dpGame.animationBeatWidth);

        djPanelDict = dpGame.djPanelDict;
        activatedHalf = Half.NONE;
        isActivated = false;
        minAlpha = 0f;

        beatScore = dpGame.beatScore;
        beatIdx = 0;
        activatedHalf = (Half)beatScore[beatIdx];
    }

    public override void OnBeatEnter() {
        activatedHalf = (Half)beatScore[beatIdx];
        if (activatedHalf < Half.WHOLE)
        {
            djPanelDict[activatedHalf].gameObject.GetComponent<Renderer>().material.DOFade(1f, (float)halfWidth);
            djPanelDict[activatedHalf].transform.DOScale(0.2f, (float)halfWidth);
        }
        else if (activatedHalf == Half.WHOLE)
        {
            djPanelDict[Half.LEFT].gameObject.GetComponent<Renderer>().material.DOFade(1f, (float)halfWidth);
            djPanelDict[Half.RIGHT].gameObject.GetComponent<Renderer>().material.DOFade(1f, (float)halfWidth);
            djPanelDict[Half.LEFT].transform.DOScale(0.2f, (float)halfWidth);
            djPanelDict[Half.RIGHT].transform.DOScale(0.2f, (float)halfWidth);
        }
        isActivated = true;
    }
    public override void OnBeat() {
        if (activatedHalf < Half.WHOLE)
        {
            djPanelDict[activatedHalf].gameObject.GetComponent<Renderer>().material.DOFade(minAlpha, (float)halfWidth);
            //djPanelDict[activatedHalf].transform.DOScale(0.05f, (float)halfWidth);
        }
        else if (activatedHalf == Half.WHOLE)
        {
            djPanelDict[Half.LEFT].gameObject.GetComponent<Renderer>().material.DOFade(minAlpha, (float)halfWidth);
            djPanelDict[Half.RIGHT].gameObject.GetComponent<Renderer>().material.DOFade(minAlpha, (float)halfWidth);
            //djPanelDict[Half.LEFT].transform.DOScale(0.05f, (float)halfWidth);
            //djPanelDict[Half.RIGHT].transform.DOScale(0.05f, (float)halfWidth);
        }
    }
    public override void OnBeatExit() {
        isActivated = false;
        beatIdx++;
        djPanelDict[Half.LEFT].transform.DOScale(0.05f, 0f);
        djPanelDict[Half.RIGHT].transform.DOScale(0.05f, 0f);
    }
}