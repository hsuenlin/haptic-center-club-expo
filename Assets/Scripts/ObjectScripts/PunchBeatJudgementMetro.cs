using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchBeatJudgementMetro : Metronome {

    public bool isPerfact;
    private int[] beatScore;
    Dictionary<Half, PunchBeatScript> punchBeatDict;
    public void Init(PunchBeatGame pbGame)
    {
        InitMetro(pbGame.beatTime, pbGame.firstBeatTime, pbGame.judgementBeatWidth);
        punchBeatDict = pbGame.punchBeatDict;
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