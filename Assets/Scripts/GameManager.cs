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
    public static GameManager instance;

    /* Client State */
    public SceneState sceneState;

    /* Server States */
    public ServerGameState serverGameState;
    public ServerStageState serverStageState;
    
    /* Actions */
    public Action OnTrackerInfoReady;
    public Action OnDeviceStatusReady;
    public Action OnRequestResultReady;
    public Action OnPanelInfoReady;
    public Action OnTriggered;
    public Action OnDeviceReady;

    /* Roots */
    public Transform arenaRoot;
    public Transform shootingClubRoot;
    public Transform tennisClubRoot;
    public Transform musicGameClubRoot;

    /* Scene Managers */
    public ArenaManager arenaManager;
    public ShootingClubManager shootingClubManager;

    /* Init and Exit Functions */
    private Dictionary<SceneState, Action> inits;
    private Dictionary<SceneState, Action> exits;
    private Dictionary<SceneState, Transform> roots;

    /* Prefabs */
    public GameObject gunPrefab;
    
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

        roots = new Dictionary<SceneState, Transform>() {
            {SceneState.ARENA, arenaRoot},
            {SceneState.SHOOTING_CLUB, shootingClubRoot },
            {SceneState.TENNIS_CLUB, tennisClubRoot },
            {SceneState.MUSICGAME_CLUB, musicGameClubRoot }
        };

        arenaManager = transform.GetChild(0).GetComponent<ArenaManager>();
        shootingClubManager = transform.GetChild(1).GetComponent<ShootingClubManager>();
        
        arenaManager.gameObject.SetActive(false);
        shootingClubManager.gameObject.SetActive(false);
        
        arenaRoot.gameObject.SetActive(false);
        shootingClubRoot.gameObject.SetActive(false);

        sceneState = SceneState.ARENA;
        InitArena();
    }

    private void TeleportTo(Transform dest) {
        // TODO: Add teleportation animation
        Camera.main.transform.position = dest.GetChild(0).position;
        Camera.main.transform.rotation = dest.GetChild(0).rotation;

        // LookAt
        // Height
    }

    public void ChangeSceneTo(SceneState dest) {
        // TODO: Exit and inits during teleportation
        TeleportTo(roots[dest]);
        exits[sceneState].Invoke();
        instance.inits[dest].Invoke();
    }
    public void InitArena() {
        arenaManager.gameObject.SetActive(true);
        arenaRoot.gameObject.SetActive(true);
    }

    public void ExitArena() {
        arenaManager.gameObject.SetActive(false);
        arenaRoot.gameObject.SetActive(false);
    }

    public void InitShootingClub() {
        TeleportTo(shootingClubRoot);
        GameObject gun = Instantiate(gunPrefab, Vector3.one, Quaternion.identity);
        gun.transform.parent = Camera.main.transform;
        gun.transform.localPosition = new Vector3(0.2f, -0.9f, 0.7f);
        shootingClubManager.gun = gun;
        shootingClubManager.muzzle = gun.transform.GetChild(1).transform;
        shootingClubManager.cameraCenter = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0f));
        shootingClubManager.gameObject.SetActive(true);
        shootingClubRoot.gameObject.SetActive(true);
    }
    public void ExitShootingClub() {
        shootingClubManager.gameObject.SetActive(false);
        shootingClubRoot.gameObject.SetActive(false);
        Destroy(ShootingClubManager.instance.gun);
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
