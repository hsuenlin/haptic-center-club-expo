using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameManager : Singleton
{

    public SceneState currentScene;

    /* Scene Managers */
    public CalibrationManager calibrationManager;
    public ArenaManager arenaManager;
    public ShootingClubManager shootingClubManager;
    public TennisClubManager tennisClubManager;
    public MusicGameClubManager musicGameClubManager;

    private Dictionary<SceneState, Component> sceneManagers;

    // Start is called before the first frame update
    void Awake() {

        Assert.IsNotNull(cabibrationManager);
        Assert.IsNotNull(arenaManager);
        Assert.IsNotNull(shootingClubManager);
        Assert.IsNotNull(tennisClubManager);
        Assert.IsNotNull(musicGameClubManager);

        sceneManagerDict = new Dictionary<SceneState, Component>() {
            {SceneState.CALIBRATION, cabibrationManager},
            {SceneState.ARENA, arenaManager},
            {SceneState.SHOOTING_CLUB, shootingClubManager},
            {SceneState.TENNIS_CLUB, tennisClubManager},
            {SceneState.MUSICGAME_CLUB, musicGameClubManager}
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
                for(int i = 0; i < 3; ++i) {
                    if(DataManager.instance.isClubReady[i]) {
                        ChangeSceneTo((SceneState)i))
                        break;
                    }
                }
                break;
            case SceneState.SHOOTING_CLUB:
                if(DataManager.instance.isClubPlayed[(int)SceneState.SHOOTING_CLUB]) {
                    ChangeSceneTo(SceneState.ARENA);
                }
                break;
            case SceneState.TENNIS_CLUB:
                if(DataManager.instance.isClubPlayed[(int)SceneState.TENNIS_CLUB]) {
                    ChangeSceneTo(SceneState.ARENA);
                }
                break;
            case SceneState.MUSICGAME_CLUB:
                if(DataManager.instance.isClubPlayed[(int)SceneState.MUSICGAME_CLUB]) {
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
