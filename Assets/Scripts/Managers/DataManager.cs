using System;
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
    RESULT = 4,
}

public enum PropState
{
    DELIVERING = 0,
    FETCHING = 1,
    RETURNING = 2,
    PUTBACK = 3
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

public class DataManager : Singleton<DataManager> {
    
    public Transform forestIslandRoot;

    public Camera playerCamera;

    /* Tracker Object */
    public GameObject playerTracker;
    public GameObject hapticCenterTracker;
    public GameObject controllerTracker;
    public GameObject controllerCartridgeTracker;
    public GameObject shiftyTracker;
    public GameObject shiftyCartridgeTracker;
    public GameObject panelTracker;

    /* Tracker Roots */
    public GameObject playerRoot;
    public GameObject hapticCenterRoot;
    public GameObject controllerRoot;
    public GameObject controllerCartridgeRoot;
    public GameObject shiftyRoot;
    public GameObject shiftyCartridgeRoot;
    public GameObject panelRoot;

    /* Objects Relate to Different Trackers */
    public GameObject scenesObj;
    public GameObject ovrCameraObj;
    public GameObject gunObj;
    public GameObject gunSupportObj;
    public GameObject racketObj;
    public GameObject racketSupportObj;
    public GameObject djPanelObj;
    public GameObject djPanelSupportObj;

    public bool isCalibrated;
    public bool[] isDeviceFree;
    public bool[] isDeviceReady;
    public bool[] isDeviceFetched;
    public Device requestDevice;
    public bool isClubReady;
    public bool[] isClubPlayed;
    public bool isInReadyZone;
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
    
    public List<GameObject> rayTools;

    public bool[] isPropPutBack;
    public Action OnServeStateEnd;
    
    public GameObject leftHandPrefab;
    public GameObject rightHandPrefab;

    public AudioClip arrivedSound;
    public AudioClip shootSound;
    public AudioClip pickUpSound;
    public AudioSource audioSource;

    private void PlaySound(AudioClip clip) {
        audioSource.PlayOneShot(clip);
    }
    public void PlayArrivedSound() {
        PlaySound(arrivedSound);
    }

    public void PlayShootSound() {
        PlaySound(shootSound);
    }

    public void PlayPickUpSound() {
        PlaySound(pickUpSound);
    }

    public void Pause() {
        audioSource.Pause();
    }

    public void StopSound() {
        audioSource.Stop();
    }
    
    protected override void OnAwake() {
        Assert.IsNotNull(forestIslandRoot);

        Assert.IsNotNull(hapticCenterTracker);
        Assert.IsNotNull(playerTracker);
        Assert.IsNotNull(playerCamera);
        Assert.IsNotNull(controllerTracker);
        Assert.IsNotNull(controllerCartridgeTracker);
        Assert.IsNotNull(shiftyTracker);
        Assert.IsNotNull(shiftyCartridgeTracker);
        Assert.IsNotNull(panelTracker);

        Assert.IsNotNull(contactText);
        Assert.IsNotNull(clubPromptTransforms);

        Assert.IsNotNull(ovrRig);

        isCalibrated = false;
        isDeviceFree = new bool[3];
        isDeviceReady = new bool[3];
        isDeviceFetched = new bool[3];
        //isClubReady = new bool[3];
        isClubPlayed = new bool[3];
        canShoot = true;
        gunAppearance = DeviceAppearance.REAL;
        isDeviceFollowHand = false;

        isRequestResultReady = false;

        isSenpaiSwing= false;

        isPropPutBack = new bool[3];

        rayTools = new List<GameObject>();
    }
}