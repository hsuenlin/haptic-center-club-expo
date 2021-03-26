using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DjPanelScript : MonoBehaviour
{
    public PanelInfo panelInfo;
    void Awake() {
        panelInfo = new PanelInfo();
    }
}

public class PanelInfo
{
    public PanelInfo()
    {
        sliders = new int[4];
    }
    // Only use red, blue and sliders
    public int red, blue;
    public int[] sliders;

    public int x, y;
    public int deg;
}

