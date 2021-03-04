using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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

public enum SceneState {
    ARENA = 0,
    SHOOTING_CLUB = 1,
    TENNIS_CLUB = 2,
    MUSICGAME_CLUB = 3
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
public class GameManager : MonoBehaviour
{

    /* Singleton */
    public GameManager instance;

    /* Client State */
    public SceneState sceneState;

    /* Server States */
    public ServerGameState serverGameState;
    public ServerStageState serverStageState;
    
    /* Actions */
    public static Action OnTrackerInfoReady;
    public static Action OnDeviceStatusReady;
    public static Action OnRequestResultReady;
    public static Action OnPanelInfoReady;
    public static Action OnTriggered;
    public static Action OnDeviceReady;

    /* Origins */
    public Transform shootingClubOrigin;
    public Transform tennisClubOrigin;
    public Transform musicGameClubOrigin;

    /* Scene Managers */
    public GameObject arenaManager;
    public GameObject shootingClubManager;

    /* Init and Exit Functions */
    private Dictionary<SceneState, Action> inits;
    private Dictionary<SceneState, Action> exits;
    
    // Start is called before the first frame update
    void Awake() {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }
    void Start()
    {
        inits = new Dictionary<SceneState, Action>() {
            {SceneState.ARENA, () => InitArena() },
            {SceneState.SHOOTING_CLUB, () => InitShootingClub() },
            {SceneState.TENNIS_CLUB, () => InitTennisClub() },
            {SceneState.MUSICGAME_CLUB, () => InitMusicGameClub() }
        };
        
        exits = new Dictionary<SceneState, Action>() {
            {SceneState.ARENA, () => ExitArena() },
            {SceneState.SHOOTING_CLUB, () => ExitShootingClub() },
            {SceneState.TENNIS_CLUB, () => ExitTennisClub() },
            {SceneState.MUSICGAME_CLUB, () => ExitMusicGameClub() }
        };

        arenaManager = transform.GetChild(0).gameObject;
        shootingClubManager = transform.GetChild(1).gameObject;
        
        sceneState = SceneState.ARENA;
        InitArena();
    }

    public void TeleportTo(Transform dest) {
        // TODO: Add teleportation animation
        Camera.main.transform.position = dest.position;
        Camera.main.transform.rotation = dest.rotation;
    }

    public void ToSceneState(SceneState to) {
        exits[sceneState].Invoke();
        inits[to].Invoke();
    }

    public void InitArena() {
        arenaManager.SetActive(true);
    }

    public void ExitArena() {
        arenaManager.SetActive(false);
    }

    public void InitShootingClub() {
        TeleportTo(shootingClubOrigin);
        shootingClubManager.SetActive(true);
    }
    public void ExitShootingClub() {
        shootingClubManager.SetActive(false);
    }

    public void InitTennisClub() {
        
    }
    public void ExitTennisClub() {

    }

    public void InitMusicGameClub() {

    }
    public void ExitMusicGameClub() {

    }

    // Update is called once per frame
    void Update()
    {
    }
}
