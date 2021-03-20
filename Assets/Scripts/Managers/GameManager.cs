using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public enum GameMode {
    QUEST = 0,
    HAPTIC_CENTER = 1
}

public class GameManager : Singleton<GameManager>
{

    public Client client;

    public SceneState currentSceneState;
    public SceneState nextSceneState;

    public GameMode gameMode;

    /* Scene Managers */
    public CalibrationManager calibrationManager;
    public ArenaManager arenaManager;
    public ShootingClubManager shootingClubManager;
    public TennisClubManager tennisClubManager;
    public MusicGameClubManager musicGameClubManager;

    public Transform calibrationSceneTransform;
    public Transform arenaSceneTransform;
    public Transform shootingClubSceneTransform;
    public Transform tennisClubSceneTransform;
    public Transform musicGameClubSceneTransform;

    private Dictionary<SceneState, IState> sceneManagers;
    private Dictionary<SceneState, Transform> sceneTransforms;

    // Start is called before the first frame update
    protected override void OnAwake() {

        Assert.IsNotNull(client);

        Assert.IsNotNull(calibrationManager);
        Assert.IsNotNull(arenaManager);
        Assert.IsNotNull(shootingClubManager);
        Assert.IsNotNull(tennisClubManager);
        Assert.IsNotNull(musicGameClubManager);

        Assert.IsNotNull(calibrationSceneTransform);
        Assert.IsNotNull(arenaSceneTransform);
        Assert.IsNotNull(shootingClubSceneTransform);
        Assert.IsNotNull(tennisClubSceneTransform);
        Assert.IsNotNull(musicGameClubSceneTransform);

        calibrationManager.gameObject.SetActive(true);
        arenaManager.gameObject.SetActive(true);
        shootingClubManager.gameObject.SetActive(true);
        tennisClubManager.gameObject.SetActive(true);
        musicGameClubManager.gameObject.SetActive(true);

        sceneManagers = new Dictionary<SceneState, IState>() {
            {SceneState.CALIBRATION, calibrationManager},
            {SceneState.ARENA, arenaManager},
            {SceneState.SHOOTING_CLUB, shootingClubManager},
            {SceneState.TENNIS_CLUB, tennisClubManager},
            {SceneState.MUSICGAME_CLUB, musicGameClubManager}
        };

        sceneTransforms = new Dictionary<SceneState, Transform>() {
            {SceneState.CALIBRATION, calibrationSceneTransform},
            {SceneState.ARENA, arenaSceneTransform},
            {SceneState.SHOOTING_CLUB, shootingClubSceneTransform},
            {SceneState.TENNIS_CLUB, tennisClubSceneTransform},
            {SceneState.MUSICGAME_CLUB, musicGameClubSceneTransform}
        };

        if(gameMode == GameMode.HAPTIC_CENTER) {
            client.ConnectToServer();
        }
        
        sceneManagers[currentSceneState].Init();
    }

    void Update() {
        if(currentSceneState != nextSceneState) {
            ChangeSceneState();
        }
        switch(currentSceneState) 
        {
            case SceneState.CALIBRATION:
                if(DataManager.instance.isCalibrated) {
                    nextSceneState = SceneState.ARENA;
                    ClientSend.NotifyServerStateChange(ServerState.ARENA);
                }
                break;
            case SceneState.ARENA:
                for(int sceneIdx = 0; sceneIdx < 3; ++sceneIdx) {
                    if(DataManager.instance.isClubReady && !DataManager.instance.isClubPlayed[sceneIdx]) {
                        nextSceneState = (SceneState) sceneIdx;
                        ClientSend.NotifyServerStateChange((ServerState)sceneIdx);
                        break;
                    }
                }
                break;
            case SceneState.SHOOTING_CLUB:
            case SceneState.TENNIS_CLUB:
            case SceneState.MUSICGAME_CLUB:
                if(DataManager.instance.isClubPlayed[(int)currentSceneState]) {
                    nextSceneState = SceneState.ARENA;
                    ClientSend.NotifyServerStateChange(ServerState.ARENA);
                }
                break;
            default:
                break;
        }
    }

    public void ChangeSceneState() {
        Component currentSceneManager = (Component)sceneManagers[currentSceneState];
        Component nextSceneManager = (Component)sceneManagers[nextSceneState];

        sceneManagers[currentSceneState].Exit();
        currentSceneManager.gameObject.SetActive(false);
        nextSceneManager.gameObject.SetActive(true);
        sceneManagers[nextSceneState].Init();
        DataManager.instance.forestIslandRoot.localPosition = sceneTransforms[nextSceneState].localPosition;
        DataManager.instance.forestIslandRoot.localRotation = sceneTransforms[nextSceneState].localRotation;
        currentSceneState = nextSceneState;
    }
}
