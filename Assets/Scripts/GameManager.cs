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
    CALIBRATION = -1,
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
public class GameManager : Singleton
{

    public SceneState currentScene;
    
    public Transform forestIslandRoot;

    /* Scene Managers */
    public CalibrationManager CalibrationManager;
    public ArenaManager arenaManager;
    public ShootingClubManager shootingClubManager;
    public TennisClubManager tennisClubManager;
    public MusicGameClubManager musicGameClubManager;

    // Updated by each club's End()
    public bool isShootingClubPlayed;
    public bool isTennisClubPlayed;
    public bool isMusicGameClubPlayed;

    public Dictionary<SceneState, Action> isClubPlayedDict;

    private Dictionary<SceneState, Component> sceneManagerDict;
    
    // Start is called before the first frame update
    protected override void Awake() {
        Assert.IsNotNull(forestIslandRoot);

        Assert.IsNotNull(cabibrationManager);
        Assert.IsNotNull(arenaManager);
        Assert.IsNotNull(shootingClubManager);
        Assert.IsNotNull(tennisClubManager);
        Assert.IsNotNull(musicGameClubManager);
        
        isShootingClubPlayed = false;
        isTennisClubPlayed = false;
        isMusicGameClubPlayed = false;

        sceneManagerDict = new Dictionary<SceneState, Component>() {
            {SceneState.CALIBRATION, cabibrationManager},
            {SceneState.ARENA, arenaManager},
            {SceneState.SHOOTING_CLUB, shootingClubManager},
            {SceneState.TENNIS_CLUB, tennisClubManager},
            {SceneState.MUSICGAME_CLUB, musicGameClubManager}
        }

        isClubPlayedDict = new Dictionary<SceneState, Action>() {
            {SceneState.SHOOTING_CLUB, ()=>{ return isShootingClubPlayed; } },
            {SceneState.TENNIS_CLUB, ()=>{ return isTennisClubPlayed; } },
            {SceneState.MUSICGAME_CLUB, ()=>{ return isMusicGameClubPlayed; }}
        }

        sceneManagerDict[currentScene].gameObject.SetActive(true);

        // TODO: 
        // Make connection with server. 
        // -> Server will keep sending haptic center tracker data and player tracker data.
    }


    void Update() {
        switch(currentScene) 
        {
            case SceneState.CALIBRATION:
                if(calibrationManager.isCalibrated) {
                    ChangeSceneTo(SceneState.ARENA);
                }
                break;
            case SceneState.ARENA:
                if(arenaManager.isShootingClubReady) {
                    ChangeSceneTo(SceneState.SHOOTING_CLUB);
                }
                else if(arenaManager.isTennisClubReady) {
                    ChangeSceneTo(SceneState.TENNIS_CLUB);
                }
                else if(arenaManager.isMusicGameClubReady) {
                    ChangeSceneTo(SceneState.MUSICGAME_CLUB);
                }
                break;
            case SceneState.SHOOTING_CLUB:
                if(isShootingClubPlayed) {
                    ChangeSceneTo(SceneState.ARENA);
                }
                break;
            case SceneState.TENNIS_CLUB:
                if(isTennisClubPlayed) {
                    ChangeSceneTo(SceneState.ARENA);
                }
                break;
            case SceneState.MUSICGAME_CLUB:
                if(isMusicGameClubPlayed) {
                    ChangeSceneTo(SceneState.ARENA);
                }
                break;
            default:
                break;
        }
    }

    public void ChangeSceneTo(SceneState nextScene) {
        sceneManagerDict[currentScene].gameObject.SetActive(false);
        sceneManagerDict[nextScene].gameObject.SetActive(true);
    }
}
