using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.Events;
public enum ServerCommand
{
    Welcome = 1,
    SendTrackerInfo = 2,
    SendDeviceStatus = 3,
    SendRequestIsSuccess = 4,
    SendDeviceReady = 6,
    SendControllerTriggerPress = 60,
    SendPanelInfo = 50,
    PlayerDisconnected = 100,
}
public class Tracker {
    public Tracker() {
        pos = Vector3.zero;
        rot = Quaternion.identity;
    }
    public Tracker (Vector3 _pos, Quaternion _rot) {
        pos = _pos;
        rot = _rot;
    }
    public Vector3 pos;
    public Quaternion rot;
}

public class ClientHandle : MonoBehaviour
{
    public static ClientHandle instance;
    public float calibrationTime = 0f;

    public bool[] isCalibrationStart;
    public bool[] isCalibrationEnd;


    private void Awake() {
        if (instance == null) {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }

        isCalibrationStart = new bool[7];
        isCalibrationEnd = new bool[7];
    }
    public void WelcomeHandle(Packet _packet)
    {
        string _msg = _packet.ReadString();
        Debug.Log($"Welcome message from server: {_msg}");
        ClientSend.SendWelcomeBack();

        // Now that we have the client's id, connect UDP
        Client.instance.udp.Connect(((IPEndPoint)Client.instance.tcp.socket.Client.LocalEndPoint).Port);
    }

    public void TrackerInfoHandle(Packet _packet) {
        Debug.Log("Call TrackerInfoHandle");
        int trackerNum = _packet.ReadInt();
        
        for (int i = 0; i < trackerNum; ++i)
        {
            
            TrackerType type = (TrackerType)_packet.ReadInt();
            Debug.Log(type);
            Vector3 pos = _packet.ReadVector3();
            Quaternion rot = _packet.ReadQuaternion();

            switch(type) {
                case TrackerType.HC_Origin:
                    if(!isCalibrationStart[0]) {
                        isCalibrationStart[0] = true;
                        StartCoroutine(Timer.StartTimer(calibrationTime, ()=>{ isCalibrationEnd[0] = true; }));
                    }
                    else if(!isCalibrationEnd[0]) { 
                        DataManager.instance.hapticCenterTracker.transform.position = pos;
                        DataManager.instance.hapticCenterTracker.transform.rotation = rot;
                    }
                    break;
                case TrackerType.Player1:
                    if (!isCalibrationStart[1])
                    {
                        isCalibrationStart[1] = true;
                        StartCoroutine(Timer.StartTimer(calibrationTime, () => { isCalibrationEnd[1] = true; }));
                    }
                    else if (!isCalibrationEnd[1])
                    {
                        DataManager.instance.playerTracker.transform.position = pos;
                        DataManager.instance.playerTracker.transform.rotation = rot;
                    }
                    break;
                case TrackerType.Vive_Controller_Right:
                    /*
                    if (!isCalibrationStart[2])
                    {
                        isCalibrationStart[2] = true;
                        StartCoroutine(Timer.StartTimer(calibrationTime, () => { isCalibrationEnd[0] = true; }));
                    }
                    */
                    //else if (!isCalibrationEnd[2])
                    //{
                    DataManager.instance.controllerTracker.transform.position = pos;
                    DataManager.instance.controllerTracker.transform.rotation = rot;
                    //}
                    break;
                case TrackerType.Controller_Cartridge:
                    if (!isCalibrationStart[3])
                    {
                        isCalibrationStart[3] = true;
                        StartCoroutine(Timer.StartTimer(calibrationTime, () => { /*isCalibrationEnd[3] = true;*/ }));
                    }
                    else if (!isCalibrationEnd[3])
                    {
                        DataManager.instance.controllerCartridgeTracker.transform.position = pos;
                        DataManager.instance.controllerCartridgeTracker.transform.rotation = rot;
                    }
                    break;
                case TrackerType.Shifty:
                    /*
                    if (!isCalibrationStart[4])
                    {
                        isCalibrationStart[4] = true;
                        StartCoroutine(Timer.StartTimer(calibrationTime, () => { isCalibrationEnd[0] = true; }));
                    }
                    */
                    //else if (!isCalibrationEnd[4])
                    //{
                    DataManager.instance.shiftyTracker.transform.position = pos;
                    DataManager.instance.shiftyTracker.transform.rotation = rot;
                    //}
                    break;
                case TrackerType.Shifty_Cartridge:
                    Debug.Log("Get Shifty Cartridge");
                    if (!isCalibrationStart[5])
                    {
                        isCalibrationStart[5] = true;
                        StartCoroutine(Timer.StartTimer(calibrationTime, () => { /*isCalibrationEnd[5] = true;*/ }));
                    }
                    else if (!isCalibrationEnd[5])
                    {
                        DataManager.instance.shiftyCartridgeTracker.transform.position = pos;
                        DataManager.instance.shiftyCartridgeTracker.transform.rotation = rot;
                    }
                    break;
                case TrackerType.Panel:
                    if (!isCalibrationStart[6])
                    {
                        isCalibrationStart[6] = true;
                        StartCoroutine(Timer.StartTimer(calibrationTime, () => { /*isCalibrationEnd[6] = true;*/ }));
                    }
                    else if (!isCalibrationEnd[6])
                    {
                        DataManager.instance.panelTracker.transform.position = pos;
                        DataManager.instance.panelTracker.transform.rotation = rot;
                    }
                    break;
                default:
                    Debug.Log($"Not handle tracker type {type}");
                    break;
            }
        }
    }
    public void DeviceStatusHandle(Packet _packet) {
        DataManager.instance.isDeviceFree[(int)Device.SHIFTY] = (_packet.ReadInt() == 0);
        _packet.ReadInt(); // Shield
        _packet.ReadInt(); // Gun
        DataManager.instance.isDeviceFree[(int)Device.CONTROLLER] = (_packet.ReadInt() == 0);
        DataManager.instance.isDeviceFree[(int)Device.PANEL] = (_packet.ReadInt() == 0);
    }

    public void RequestResultHandle(Packet _packet) {
        DataManager.instance.isRequestResultReady = true;
        DataManager.instance.isClubReady = (_packet.ReadInt() == 1);
    }

    public void DeviceReadyHandle(Packet _packet) {
        DataManager.instance.isDeviceReady[(int)DataManager.instance.requestDevice] = true;
    }

    public void TriggerHandle(Packet _packet)
    {
        if(ShootingClubManager.instance.currentClubState == ClubState.GAME) {
            DataManager.instance.gunObj.GetComponent<GunScript>().Shoot();
        }
    }
    public void PanelInfoHandle(Packet _packet) {
        // RedBtn, BlueBtn, Slider1, Slider2, Slider3, Slider4, x, y, degree
        DataManager.instance.djPanelObj.GetComponent<DjPanelScript>().panelInfo.buttons[0] = _packet.ReadInt();
        DataManager.instance.djPanelObj.GetComponent<DjPanelScript>().panelInfo.buttons[1] = _packet.ReadInt();
        DataManager.instance.djPanelObj.GetComponent<DjPanelScript>().panelInfo.sliders[0] = _packet.ReadInt();
        DataManager.instance.djPanelObj.GetComponent<DjPanelScript>().panelInfo.sliders[1] = _packet.ReadInt();
        DataManager.instance.djPanelObj.GetComponent<DjPanelScript>().panelInfo.sliders[2] = _packet.ReadInt();
        DataManager.instance.djPanelObj.GetComponent<DjPanelScript>().panelInfo.sliders[3] = _packet.ReadInt();
        DataManager.instance.djPanelObj.GetComponent<DjPanelScript>().panelInfo.x = _packet.ReadInt();
        DataManager.instance.djPanelObj.GetComponent<DjPanelScript>().panelInfo.y = _packet.ReadInt();
        DataManager.instance.djPanelObj.GetComponent<DjPanelScript>().panelInfo.deg = _packet.ReadInt();
    }
    public void PlayerDisconnected(Packet _packet)
    {
        int _id = _packet.ReadInt();
    }
}
