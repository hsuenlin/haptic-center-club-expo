using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;

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

public enum DeviceName
{
    Shifty = 40,
    Panel = 50,
    Controller = 60,
    Gun = 70,
    Shield = 80
}
public enum ServerGameState
{
    NONE = 0,
    INIT = 1,
    STORY = 2,
    BRANCH = 3,
    TENNIS_CLUB = 4,
    SHOOTING_CLUB = 5,
    MUSIC_CLUB = 6
}
public enum ServerStageState
{
    NONE = 0,
    DISTRIBUTE_DEVICE = 1,
    WAITING_DEVICE_READY = 2,
    STAGE_START = 3,
    STAGE_END = 4
}

public enum SceneState
{
    CALIBRATION = -2,
    ARENA = -1,
    SHOOTING_CLUB = 0,
    TENNIS_CLUB = 1,
    MUSICGAME_CLUB = 2
}
public enum Device
{
    GUN = 0,
    RACKET = 1,
    PANEL = 2
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



public enum ServerCommand
{
    Welcome = 1,
    SendTrackerInfo = 2,
    SendDeviceStatus = 3,
    SendRequestIsSuccess = 4,
    SendDeviceReady = 5,
    SendControllerTriggerPress = 60,
    SendPanelInfo = 50,
    PlayerDisconnected = 100,

}
public enum ClientCommand
{
    SendWelcomeBack = 1,
    NotifyGameStateChange = 2,
    NotifyStageStateChange = 3,
    RequestDevicesStatus = 4,
    RequestDevice = 5
}

public enum DeviceAppearance {
    REAL = 0,
    VIRTUAL = 1
}

public class DataManager : Singleton<DataManager> {
    /* Updated by server */
    public Transform forestIslandRoot;

    public GameObject player;
    public Camera playerCamera;
    
    public GameObject hapticCenter;
    public GameObject gun;
    public GameObject gunSupport;
    public GameObject racket;
    public GameObject racketSupport;
    public GameObject panel;
    public bool isCalibrated;
    public bool[] isDeviceFree;
    public bool[] isDeviceReady;
    public bool[] isDeviceFetched;
    public bool[] isClubReady;
    public bool[] isClubPlayed;
    public bool[] isInReadyZone;
    public bool[] isReadyTextShowed;
    public bool[] isStartTextShowed;
    public bool canShoot;
    public DeviceAppearance gunAppearance;
    public bool isDeviceFollowHand;
    
    protected override void OnAwake() {
        Assert.IsNotNull(forestIslandRoot);

        Assert.IsNotNull(hapticCenter);
        Assert.IsNotNull(player);
        Assert.IsNotNull(playerCamera);
        Assert.IsNotNull(gun);
        Assert.IsNotNull(gunSupport);
        Assert.IsNotNull(racket);
        Assert.IsNotNull(racketSupport);
        Assert.IsNotNull(panel);

        isCalibrated = false;
        isDeviceFree = new bool[3];
        isDeviceReady = new bool[3];
        isDeviceFetched = new bool[3];
        isClubReady = new bool[3];
        isClubPlayed = new bool[3];
        isInReadyZone = new bool[3];
        isReadyTextShowed = new bool[3];
        isStartTextShowed = new bool[3];
        canShoot = true;
        gunAppearance = DeviceAppearance.REAL;
        isDeviceFollowHand = false;
    }
}