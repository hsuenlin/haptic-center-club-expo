using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;

public class CalibrationManager : SceneManager<CalibrationManager>
{
    //public Transform toCalibrationTransform;

    public Image transparentBlack;
    public Text calibrationText;
    public float calibrationTime;
    
    protected override void OnAwake() {
        Assert.IsNotNull(transparentBlack);
        Assert.IsNotNull(calibrationText);
    }

    public override void Init() {
        transparentBlack.gameObject.SetActive(true);
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
        DataManager.instance.isCalibrated = true;
    }

    public override void Exit() {
        transparentBlack.gameObject.SetActive(false);
        calibrationText.gameObject.SetActive(false);
    }
}
