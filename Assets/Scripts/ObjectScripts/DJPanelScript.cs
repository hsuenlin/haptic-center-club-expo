using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DjPanelScript : MonoBehaviour
{

    public PanelInfo panelInfo;
    public Action[] OnButtonPressDown;

    private DjPanelButtonScript[] buttonScripts;

    void Awake() {
        panelInfo = new PanelInfo();
    }

    public void Init(DjPanelGame _dpGame) {
        OnButtonPressDown = new Action[2];
        buttonScripts = new DjPanelButtonScript[2];
        buttonScripts[0] = _dpGame.buttonScripts[0];
        buttonScripts[1] = _dpGame.buttonScripts[1];
        buttonScripts[0].Init(_dpGame, this, gameObject.transform, Half.LEFT); // Register OnButtonPressDown
        buttonScripts[1].Init(_dpGame, this, gameObject.transform, Half.RIGHT);
        StartCoroutine(ButtonListener());
    }

    public void Exit() {
        StopCoroutine(ButtonListener());
    }

    public IEnumerator ButtonListener() {
        bool[] isLastPress = new bool[2];
        bool[] isNowPress = new bool[2];
        
        while(true) {
            for(int i = 0; i < 2; ++i) {
                isLastPress[i] = isNowPress[i];
                isNowPress[i] = panelInfo.buttons[i] == 1;
                if(!isLastPress[i] && isNowPress[i]) {
                    OnButtonPressDown[i]();
                }
            }
            yield return null;
        }
    }
}

public class PanelInfo
{
    public PanelInfo()
    {
        buttons = new int[2];
        sliders = new int[4];
    }
    // Only use red, blue and sliders
    public int[] buttons;
    public int[] sliders;

    public int x, y;
    public int deg;
}

