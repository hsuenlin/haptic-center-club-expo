using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchBeatJudgementMetro : Metronome {

    public bool isPerfact;
    Dictionary<Half, PunchBeatScript> punchBeatDict;
    public PunchBeatJudgementMetro(PunchBeatGame pbGame)
    {
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