using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;

public class CalibrationManager : SceneManager<CalibrationManager>
{

    public Image transparentBlack;
    public Text calibrationText;
    public float calibrationTime;
    private Vector3 negatePosition;
    private Quaternion negateRotation;

    private GameObject scenesObj;
    
    protected override void OnAwake() {
        Assert.IsNotNull(transparentBlack);
        Assert.IsNotNull(calibrationText);
    }

    public override void Init() {
        scenesObj = DataManager.instance.scenesObj;
        transparentBlack.gameObject.SetActive(true);
        calibrationText.gameObject.SetActive(true);
        negatePosition = Vector3.zero;
        negateRotation = Quaternion.identity;
        
        StartCoroutine(CalibrationCountDown());
        if(GameManager.instance.gameMode == GameMode.HAPTIC_CENTER) {
            DataManager.instance.ovrRig.GetComponent<OVRCameraRig>().UpdatedAnchors += NegateCameraTransform;
        }
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

    void NegateCameraTransform(OVRCameraRig cameraRig)
    {
        
        if(GameManager.instance.currentSceneState == SceneState.CALIBRATION) {
            Matrix4x4 m = cameraRig.centerEyeAnchor.parent.worldToLocalMatrix * cameraRig.centerEyeAnchor.localToWorldMatrix;
            negateRotation = m.inverse.rotation;
            negatePosition.x = m.inverse.m03;
            negatePosition.y = m.inverse.m13;
            negatePosition.z = m.inverse.m23;
        }
        
        
        DataManager.instance.playerCamera.transform.parent.localRotation = negateRotation;
        DataManager.instance.playerCamera.transform.parent.localPosition = negatePosition;
        
    }

    public override void Exit() {
        transparentBlack.gameObject.SetActive(false);
        calibrationText.gameObject.SetActive(false);
    }
}
