using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;

public class CalibrationManager : StateSingleton<CalibrationManager>
{
    //public Transform toCalibrationTransform;

    public Text calibrationText;
    public float calibrationTime;
    public bool isCalibrated;
    protected override void OnAwake() {
        Assert.IsNotNull(calibrationText);
        isCalibrated = false;
    }

    protected override void Init() {
        calibrationText.gameObject.SetActive(true);
        StartCoroutine(CalibrationCountDown());
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

    protected override void Exit() {
        calibrationText.gameObject.SetActive(false);
    }
}
