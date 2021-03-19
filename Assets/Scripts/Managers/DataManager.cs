using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;

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

public enum ClientCommand
{
    SendWelcomeBack = 1,
    NotifyServerStateChange = 2,
    RequestDevice = 5
}

public enum TrackerType
{
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

public enum SceneState
{
    CALIBRATION = -2,
    ARENA = -1,
    SHOOTING_CLUB = 0,
    TENNIS_CLUB = 1,
    MUSICGAME_CLUB = 2
}

public enum ClubState
{
    IDLE = 0,
    WAITING = 1,
    READY = 2,
    GAME = 3,
    RESULT = 4
}

public enum PropState
{
    DELIVERING = 0,
    FETCHING = 1,
    RETURNING = 2
}

public enum ServerState
{
    CALIBRATION = -2,
    ARENA = -1,
    TENNIS_CLUB = 0,
    SHOOTING_CLUB = 1,
    MUSICGAME_CLUB = 2
}

public enum DeviceAppearance {
    REAL = 0,
    VIRTUAL = 1
}

public enum Device
{
    CONTROLLER = 0,
    SHIFTY = 1,
    PANEL = 2
}

public enum ServerDevice
{
    Shifty = 40,
    Panel = 50,
    Controller = 60,
    Gun = 70,
    Shield = 80
}

public class PanelInfo {
    public PanelInfo() {
        sliders = new int[4];
    }
    public int red, blue;
    public int[] sliders;
    public int x, y;
    public int deg;
}

public class DataManager : Singleton<DataManager> {
    /* Updated by server */
    public Transform forestIslandRoot;

    public GameObject player;
    public Camera playerCamera;

    public GameObject handSDK;
    
    public GameObject hapticCenter;
    public GameObject controller;
    public GameObject controllerCartridge;
    public GameObject shifty;
    public GameObject shiftyCartridge;
    public GameObject panel;

    public GameObject gun;
    public GameObject gunSupport;
    public GameObject racket;
    public GameObject racketSupport;
    public GameObject djPanel;

    public bool isCalibrated;
    public bool[] isDeviceFree;
    public bool[] isDeviceReady;
    public bool[] isDeviceFetched;
    public Device requestDevice;
    public bool isClubReady;
    public bool[] isClubPlayed;
    public bool[] isInReadyZone;
    public bool[] isReadyTextShowed;
    public bool[] isStartTextShowed;
    public bool canShoot;
    public DeviceAppearance gunAppearance;
    public bool isDeviceFollowHand;

    public GameObject contactText;
    public GameObject failedText;
    public Transform[] clubPromptTransforms;
    public bool isRequestResultReady;

    public OVRCameraRig ovrRig;
    
    public Transform rightHandAnchor;
    public Transform leftHandAnchor;

    public bool isSenpaiSwing;
    
    protected override void OnAwake() {
        Assert.IsNotNull(forestIslandRoot);

        Assert.IsNotNull(hapticCenter);
        Assert.IsNotNull(player);
        Assert.IsNotNull(playerCamera);
        Assert.IsNotNull(handSDK);
        Assert.IsNotNull(controller);
        Assert.IsNotNull(controllerCartridge);
        Assert.IsNotNull(shifty);
        Assert.IsNotNull(shiftyCartridge);
        Assert.IsNotNull(panel);

        Assert.IsNotNull(contactText);
        Assert.IsNotNull(clubPromptTransforms);

        Assert.IsNotNull(ovrRig);

        isCalibrated = false;
        isDeviceFree = new bool[3];
        isDeviceReady = new bool[3];
        isDeviceFetched = new bool[3];
        //isClubReady = new bool[3];
        isClubReady = false;
        isClubPlayed = new bool[3];
        isInReadyZone = new bool[3];
        isReadyTextShowed = new bool[3];
        isStartTextShowed = new bool[3];
        canShoot = true;
        gunAppearance = DeviceAppearance.REAL;
        isDeviceFollowHand = false;

        isRequestResultReady = false;

        isSenpaiSwing= false;
    }
}

public class UIFunc {
    public static void AttachObjects(GameObject src, GameObject dest) {
        src.transform.parent = dest.transform;
        src.transform.localPosition = Vector3.zero;
        src.transform.localRotation = Quaternion.identity;
    }
}