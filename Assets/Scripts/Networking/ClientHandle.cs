using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.Events;

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

    private void Awake() {
        if (instance == null) {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }
    
    // TODO: Move the following part to tennis club.
    //private static Vector3 refPosition;
    //private static bool isFirstPosition = true;

    //public Camera player;
    //public Transform gun;

    //private static int cnt = 0;

    public void WelcomeHandle(Packet _packet)
    {
        string _msg = _packet.ReadString();
        Debug.Log($"Welcome message from server: {_msg}");
        ClientSend.SendWelcomeBack();

        // Now that we have the client's id, connect UDP
        Client.instance.udp.Connect(((IPEndPoint)Client.instance.tcp.socket.Client.LocalEndPoint).Port);
    }

    public void TrackerInfoHandle(Packet _packet) {
        int trackerNum = _packet.ReadInt();
        
        for (int i = 0; i < trackerNum; ++i)
        {
            TrackerType type = (TrackerType)_packet.ReadInt();
            Vector3 pos = _packet.ReadVector3();
            Quaternion rot = _packet.ReadQuaternion();

            switch(type) {
                case TrackerType.HC_Origin:
                    DataManager.instance.hapticCenter.transform.position = pos;
                    DataManager.instance.hapticCenter.transform.rotation = rot;
                    break;
                case TrackerType.Player1:
                    DataManager.instance.playerCamera.transform.position = pos;
                    DataManager.instance.playerCamera.transform.rotation = rot;
                    break;
                case TrackerType.Vive_Controller_Right:
                    DataManager.instance.controller.transform.position = pos;
                    DataManager.instance.controller.transform.rotation = rot;
                    break;
                case TrackerType.Controller_Cartridge:
                    DataManager.instance.controllerCartridge.transform.position = pos;
                    DataManager.instance.controllerCartridge.transform.rotation = rot;
                    break;
                case TrackerType.Shifty:
                    DataManager.instance.shifty.transform.position = pos;
                    DataManager.instance.shifty.transform.rotation = rot;
                    break;
                case TrackerType.Shifty_Cartridge:
                    DataManager.instance.shiftyCartridge.transform.position = pos;
                    DataManager.instance.shiftyCartridge.transform.rotation = rot;
                    break;
                case TrackerType.Panel:
                    DataManager.instance.panel.transform.position = pos;
                    DataManager.instance.panel.transform.rotation = rot;
                    break;
                default:
                    Debug.Log($"Not handle tracker type {type}");
                    break;
            }
            
            // TODO: Move following parts to tennis club.
            /*
            if (type == TrackerType.HC_Origin)
            {
            }
            else if (type == TrackerType.Player1) {
                // Camera
                Vector3 trackerEuler = rot.eulerAngles;
                //Debug.Log("Get player1 transform");
                Vector3 cameraEuler = new Vector3(0f, 0f, 0f);
                cameraEuler[0] = trackerEuler[2];
                cameraEuler[1] = trackerEuler[1];
                cameraEuler[2] = -trackerEuler[0];
                instance.player.transform.rotation = Quaternion.Euler(cameraEuler);
            }
            else if (type == TrackerType.Vive_Controller_Left)
            {
                // Controller
                //Debug.Log("Get controller transform");
                if(isFirstPosition) {
                    refPosition = pos;
                    isFirstPosition = false;
                } else {
                    Vector3 dPos = pos - refPosition;
                    instance.gun.position += dPos;
                    instance.gun.rotation = rot;
                }
            }
            */
        }
        //GameManager.instance.OnTrackerInfoReady();
    }
    public void DeviceStatusHandle(Packet _packet) {
        DataManager.instance.isDeviceFree[(int)Device.SHIFTY] = (_packet.ReadInt() == 0);
        _packet.ReadInt(); // Shield
        _packet.ReadInt(); // Gun
        DataManager.instance.isDeviceFree[(int)Device.CONTROLLER] = (_packet.ReadInt() == 0);
        DataManager.instance.isDeviceFree[(int)Device.PANEL] = (_packet.ReadInt() == 0);
    }

    public void RequestResultHandle(Packet _packet) {
        if(_packet.ReadInt() == 1) {
            DataManager.instance.isClubReady[(int)DataManager.instance.requestDevice] = true;
        }
    }

    public void DeviceReadyHandle(Packet _packet) {
        DataManager.instance.isDeviceReady[(int)DataManager.instance.requestDevice] = true;
    }

    public void TriggerHandle(Packet _packet)
    {
        if(ShootingClubManager.instance.currentClubState == ClubState.GAME) {
            DataManager.instance.gun.GetComponent<GunScript>().Shoot();
        }
    }
    public void PanelInfoHandle(Packet _packet) {
        // RedBtn, BlueBtn, Slider1, Slider2, Slider3, Slider4, x, y, degree
        DataManager.instance.djPanel.GetComponent<DJPanelScript>().panelInfo.red = _packet.ReadInt();
        DataManager.instance.djPanel.GetComponent<DJPanelScript>().panelInfo.blue = _packet.ReadInt();
        DataManager.instance.djPanel.GetComponent<DJPanelScript>().panelInfo.sliders[0] = _packet.ReadInt();
        DataManager.instance.djPanel.GetComponent<DJPanelScript>().panelInfo.sliders[1] = _packet.ReadInt();
        DataManager.instance.djPanel.GetComponent<DJPanelScript>().panelInfo.sliders[2] = _packet.ReadInt();
        DataManager.instance.djPanel.GetComponent<DJPanelScript>().panelInfo.sliders[3] = _packet.ReadInt();
        DataManager.instance.djPanel.GetComponent<DJPanelScript>().panelInfo.x = _packet.ReadInt();
        DataManager.instance.djPanel.GetComponent<DJPanelScript>().panelInfo.y = _packet.ReadInt();
        DataManager.instance.djPanel.GetComponent<DJPanelScript>().panelInfo.deg = _packet.ReadInt();
        //GameManager.instance.OnPanelInfoReady();
    }
    public void PlayerDisconnected(Packet _packet)
    {
        int _id = _packet.ReadInt();
    }
}
