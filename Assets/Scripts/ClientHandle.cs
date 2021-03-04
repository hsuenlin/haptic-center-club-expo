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
public class Panel {
    public Panel() {
        sliders = new int[4];
    }
    public int red, blue;
    public int[] sliders;
    public int x, y;
    public int deg;
}

public class ClientHandle : MonoBehaviour
{
    public static ClientHandle instance;

    /* Server Data */
    public static Tracker[] trackers;
    public static Panel panel;
    public static bool[] deviceStatusArray;
    public static bool deviceReady;
    public static bool requestResult;



    private void Awake() {
        if (instance == null) {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }

        trackers = new Tracker[14];
        panel = new Panel();
        deviceStatusArray = new bool[5];
    }
    
    // TODO: Move the following part to tennis club.
    //private static Vector3 refPosition;
    //private static bool isFirstPosition = true;

    //public Camera player;
    //public Transform gun;

    //private static int cnt = 0;

    public static void WelcomeHandle(Packet _packet)
    {
        string _msg = _packet.ReadString();
        Debug.Log($"Welcome message from server: {_msg}");
        ClientSend.SendWelcomeBack();

        // Now that we have the client's id, connect UDP
        Client.instance.udp.Connect(((IPEndPoint)Client.instance.tcp.socket.Client.LocalEndPoint).Port);
    }

    public static void TrackerInfoHandle(Packet _packet) {
        Dictionary<TrackerType, int> typeToIndex = new Dictionary<TrackerType, int>()
        {
            {TrackerType.HC_Origin, 0},
            {TrackerType.Player1, 1},
            {TrackerType.Player2, 2},
            {TrackerType.Player3, 3},
            {TrackerType.Shifty, 4},
            {TrackerType.Shifty_Cartridge, 5},
            {TrackerType.Panel, 6},
            {TrackerType.Vive_Controller_Left, 7}, 
            {TrackerType.Vive_Controller_Right, 8},
            {TrackerType.Controller_Cartridge, 9},
            {TrackerType.Gun, 10}, 
            {TrackerType.Gun_Cartridge, 11},
            {TrackerType.Shield, 12}, 
            {TrackerType.Shield_Cartridge, 13}
        };
        int trackerNum = _packet.ReadInt();
        
        for (int i = 0; i < trackerNum; ++i)
        {
            TrackerType type = (TrackerType)_packet.ReadInt();
            Vector3 pos = _packet.ReadVector3();
            Quaternion rot = _packet.ReadQuaternion();
            trackers[typeToIndex[type]] = new Tracker(pos, rot);
            
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
        GameManager.OnTrackerInfoReady();
    }
    public static void DeviceStatusHandle(Packet _packet) {
        // Shifty, Shiled, Gun, Controller, Panel
        // TODO: Inform game manager.
        for(int i = 0; i < 5; ++i) {
            deviceStatusArray[i] = (_packet.ReadInt() == 1);
        }
        GameManager.OnDeviceStatusReady();
    }

    public static void RequestResultHandle(Packet _packet) {
        requestResult = (_packet.ReadInt() == 1);
        GameManager.OnRequestResultReady();
    }

    public static void DeviceReadyHandle(Packet _packet) {
        deviceReady = true;
        GameManager.OnDeviceReady();
    }

    public static void TriggerHandle(Packet _packet)
    {
        // TODO: Notify gun
        GameManager.OnTriggered();
    }
    public static void PanelInfoHandle(Packet _packet) {
        // RedBtn, BlueBtn, Slider1, Slider2, Slider3, Slider4, x, y, degree
        panel.red = _packet.ReadInt();
        panel.blue = _packet.ReadInt();
        panel.sliders[0] = _packet.ReadInt();
        panel.sliders[1] = _packet.ReadInt();
        panel.sliders[2] = _packet.ReadInt();
        panel.sliders[3] = _packet.ReadInt();
        panel.x = _packet.ReadInt();
        panel.y = _packet.ReadInt();
        panel.deg = _packet.ReadInt();
        GameManager.OnPanelInfoReady();
    }
    public static void PlayerDisconnected(Packet _packet)
    {
        int _id = _packet.ReadInt();
    }
}
