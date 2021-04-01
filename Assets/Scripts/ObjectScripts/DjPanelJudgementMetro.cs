using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DjPanelJudgementMetro : Metronome {

    public bool isPerfact;
    private int[] beatScore;
    public void Init(DjPanelGame dpGame)
    {
        InitMetro(dpGame.beatTime, dpGame.firstBeatTime, dpGame.judgementBeatWidth);
        isPerfact = false;
    }
    
    public override void OnBeatEnter() {
        isPerfact = true;  
    }
    public override void OnBeat() {
    }
    public override void OnBeatExit() {
        isPerfact = false;
    }
}