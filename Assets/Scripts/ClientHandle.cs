using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class ClientHandle : MonoBehaviour
{
    public enum TrackerType {
        HC_Origin = 0,
        Player1 = 1,
        Player2 = 2,
        Player3 = 3,
        Shifty = 40,
        Shifty_Cartridge = 41,
        Panel = 50,
        Vive_Controller_Left = 60,
        Vive_Controller_Right = 61,
        Controller_Cartridge = 62,
        Gun = 70,
        Gun_Cartridge = 71,
        Shield = 80,
        Shield_Cartridge = 81,
    }

    public enum DeviceName {
        Shifty = 40,
        Panel = 50,
        Controller = 60,
        Gun = 70,
        Shield = 80
    }
    
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
    public static void Welcome(Packet _packet)
    {
        int _myId = _packet.ReadInt();
        string _msg = _packet.ReadString();
        Debug.Log($"My id from server: {_myId}");
        Debug.Log($"Message from server: {_msg}");
        Client.instance.myId = _myId;
        ClientSend.WelcomeReceived();

        // Now that we have the client's id, connect UDP
        Client.instance.udp.Connect(((IPEndPoint)Client.instance.tcp.socket.Client.LocalEndPoint).Port);
    }

    private static Vector3 refPosition;
    private static bool isFirstPosition = true;

    public Camera player;
    public Transform gun;

    private static int cnt = 0;

    public static void TrackerInfoHandle(Packet _packet) {

        int trackerNum = _packet.ReadInt();
        //Debug.Log($"trackerNum = {trackerNum}");
        for (int i = 0; i < trackerNum; ++i)
        {
            TrackerType type = (TrackerType)_packet.ReadInt();
            Vector3 pos = _packet.ReadVector3();
            Quaternion rot = _packet.ReadQuaternion();
            
            if (type == TrackerType.HC_Origin)
            {
                // Pass
                //Debug.Log("Get origin");
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
        }
    }

    public static void DeviceStatusHandle(Packet _packet) {
        
    }

    public static void RequestResultHandle(Packet _packet) {

    }

    public static void DeviceReadyHandle(Packet _packet) {

    }

    public static void TriggerHandle(Packet _packet)
    {
        // Trigger
        cnt++;
        Debug.Log($"射 {cnt}");
        ShootingScript ss = instance.player.GetComponent<ShootingScript>();
        ss.isTrigger = true;
    }
    public static void PanelInfoHandle(Packet _packet) {
        
    }
    public static void PlayerDisconnected(Packet _packet)
    {
        int _id = _packet.ReadInt();
    }
}
