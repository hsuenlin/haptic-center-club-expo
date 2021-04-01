using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DjPanelAnimationMetro : Metronome {
    public Half activatedHalf;
    public bool isActivated;
    public float minAlpha;
    public Dictionary<Half, GameObject> buttonDict;
    public GameObject sliderIndicator;
    private int[] beatScore;
    private int[] sliderScore;
    private int beatIdx;

    //private bool isSliderShowed;
    
    public void Init(DjPanelGame dpGame) {
        InitMetro(dpGame.beatTime, dpGame.firstBeatTime, dpGame.animationBeatWidth);

        buttonDict = dpGame.djPanelButtonDict;
        //slider = dpGame.slider;
        activatedHalf = Half.NONE;
        isActivated = false;
        minAlpha = 0.1f;

        beatScore = dpGame.beatScore;
        //sliderScore = dpGame.sliderScore;
        beatIdx = 0;
        activatedHalf = (Half)beatScore[beatIdx];

        //isSliderShowed = false;
    }

    public override void OnBeatEnter() {
        // Button
        activatedHalf = (Half)beatScore[beatIdx];
        if (activatedHalf < Half.WHOLE)
        {
            buttonDict[activatedHalf].GetComponent<Renderer>().material.DOFade(1f, (float)halfWidth);
        }
        else if (activatedHalf == Half.WHOLE)
        {
            buttonDict[Half.LEFT].gameObject.GetComponent<Renderer>().material.DOFade(1f, (float)halfWidth);
            buttonDict[Half.RIGHT].gameObject.GetComponent<Renderer>().material.DOFade(1f, (float)halfWidth);
        }
        isActivated = true;
        
        // Slider
        /*
        if(sliderScore[beatIdx] != 0) {
            sliderIndicator.transform.GetChild(0).GetComponent<Renderer>().material.DOFade(1f, (float)halfWidth);
            isSliderShowed = true;
        }
        */
    }
    public override void OnBeat() {
        // Button
        if (activatedHalf < Half.WHOLE)
        {
            buttonDict[activatedHalf].gameObject.GetComponent<Renderer>().material.DOFade(minAlpha, (float)halfWidth);
        }
        else if (activatedHalf == Half.WHOLE)
        {
            buttonDict[Half.LEFT].gameObject.GetComponent<Renderer>().material.DOFade(minAlpha, (float)halfWidth);
            buttonDict[Half.RIGHT].gameObject.GetComponent<Renderer>().material.DOFade(minAlpha, (float)halfWidth);
        }

        // Slider
        /*
        if(isSliderShowed) {
            //isNext
        } else {

        }
        */
    }
    public override void OnBeatExit() {
        isActivated = false;
        beatIdx++;
    }
}