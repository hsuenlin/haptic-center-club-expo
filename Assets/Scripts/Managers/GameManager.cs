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

    public SceneState currentSceneState;
    public SceneState nextSceneState;

    public GameMode gameMode;

    /* Scene Managers */
    public CalibrationManager calibrationManager;
    public ArenaManager arenaManager;
    public ShootingClubManager shootingClubManager;
    public TennisClubManager tennisClubManager;
    public MusicGameClubManager musicGameClubManager;

    private Dictionary<SceneState, IState> sceneManagers;

    // Start is called before the first frame update
    protected override void OnAwake() {

        Assert.IsNotNull(calibrationManager);
        Assert.IsNotNull(arenaManager);
        Assert.IsNotNull(shootingClubManager);
        Assert.IsNotNull(tennisClubManager);
        Assert.IsNotNull(musicGameClubManager);

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

        //sceneManagers[currentSceneState]().gameObject.SetActive(true);
        
        sceneManagers[currentSceneState].Init();

        // TODO: 
        // Make connection with server. 
        // -> Server will keep sending haptic center tracker data and player tracker data.
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
                }
                break;
            case SceneState.ARENA:
                for(int sceneIdx = 0; sceneIdx < 3; ++sceneIdx) {
                    if(DataManager.instance.isClubReady[sceneIdx]) {
                        nextSceneState = (SceneState) sceneIdx;
                        break;
                    }
                }
                break;
            case SceneState.SHOOTING_CLUB:
            case SceneState.TENNIS_CLUB:
            case SceneState.MUSICGAME_CLUB:
                if(DataManager.instance.isClubPlayed[(int)currentSceneState]) {
                    nextSceneState = SceneState.ARENA;
                }
                break;
            default:
                break;
        }
    }

    public void ChangeSceneState() {
        //sceneManagers[currentSceneState]().gameObject.SetActive(false);
        sceneManagers[currentSceneState].Exit();
        //sceneManagers[nextSceneState]().gameObject.SetActive(true);
        sceneManagers[nextSceneState].Init();
        currentSceneState = nextSceneState;
    }
}
