using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalibrationManager : StateSingleton
{
    //public Transform toCalibrationTransform;

    public Text calibrationText;
    public float calibrationTime;
    public bool isCalibrated;
    protected override OnAwake() {
        Assert.IsNotNull(calibrationText);
        Assert.IsNotNull(calibrationTime);
        isCalibrated = false;
    }

    protected override Init() {
        calibrationText.SetActive(true);
        StartCoroutine(CalibrationCountDown);
    }

    private IEnumerator CalibrationCountDown()
    {
        float timer = 0f;
        while (timer < calibrationTime)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        isCalibrated = true;
    }

    protected override End() {
        calibrationText.SetActive(false);
    }
}
