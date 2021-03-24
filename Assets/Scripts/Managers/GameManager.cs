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

    public GameMode gameMode;
    
    public SceneState currentSceneState;
    public SceneState nextSceneState;
    

    /* Scene Managers */
    public CalibrationManager calibrationManager;
    public ArenaManager arenaManager;
    public ShootingClubManager shootingClubManager;
    public TennisClubManager tennisClubManager;
    public MusicGameClubManager musicGameClubManager;


    /* Scene Transforms */
    public Transform calibrationSceneTransform;
    public Transform arenaSceneTransform;
    public Transform shootingClubSceneTransform;
    public Transform tennisClubSceneTransform;
    public Transform musicGameClubSceneTransform;

    
    private Dictionary<SceneState, IState> sceneManagers;
    private Dictionary<SceneState, Transform> sceneTransforms;

    
    /* Client */
    private Client client;

    /* Tracker Roots */
    private Transform playerRoot;
    private Transform hapticCenterRoot;
    private Transform controllerRoot;
    private Transform controllerCartridgeRoot;
    private Transform shiftyRoot;
    private Transform shiftyCartridgeRoot;
    private Transform panelRoot;
    
    /* Game Roots */
    private Transform forestIslandRoot;
    private Transform ovrCameraRoot;
    private Transform sceneRoot;
    private Transform gunRoot;
    private Transform gunSupportRoot;
    private Transform racketRoot;
    private Transform racketSupportRoot;
    private Transform djPanelRoot;

    protected override void OnAwake() {
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
    }

    void Start() {
        playerRoot = DataManager.instance.playerRoot.transform;
        hapticCenterRoot = DataManager.instance.hapticCenterRoot.transform;
        controllerRoot = DataManager.instance.controllerRoot.transform;
        controllerCartridgeRoot = DataManager.instance.controllerCartridgeRoot.transform;
        shiftyRoot = DataManager.instance.shiftyRoot.transform;
        shiftyCartridgeRoot = DataManager.instance.shiftyCartridgeRoot.transform;
        panelRoot = DataManager.instance.panelRoot.transform;

        forestIslandRoot = DataManager.instance.forestIslandRoot;
        ovrCameraRoot = DataManager.instance.ovrCameraObj.transform;
        sceneRoot = DataManager.instance.scenesObj.transform;
        gunRoot = DataManager.instance.gunObj.transform;
        gunSupportRoot = DataManager.instance.gunSupportObj.transform;
        racketRoot = DataManager.instance.racketObj.transform;
        racketSupportRoot = DataManager.instance.racketSupportObj.transform;
        djPanelRoot = DataManager.instance.djPanelObj.transform;
        
        calibrationManager.gameObject.SetActive(false);
        arenaManager.gameObject.SetActive(false);
        shootingClubManager.gameObject.SetActive(false);
        tennisClubManager.gameObject.SetActive(false);
        musicGameClubManager.gameObject.SetActive(false);

        Component initSceneManager = (Component)sceneManagers[currentSceneState];
        initSceneManager.gameObject.SetActive(true);
        sceneManagers[currentSceneState].Init();

        if (gameMode == GameMode.HAPTIC_CENTER)
        {
            client.ConnectToServer();
            ClubUtil.Attach(ovrCameraRoot, playerRoot);
            ClubUtil.Attach(sceneRoot, hapticCenterRoot);
            ClubUtil.Attach(gunRoot, controllerRoot);
            ClubUtil.Attach(gunSupportRoot, controllerCartridgeRoot);
            ClubUtil.Attach(racketRoot, shiftyRoot);
            ClubUtil.Attach(racketSupportRoot, shiftyCartridgeRoot);
            ClubUtil.Attach(djPanelRoot, panelRoot);
        }
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
                    if(gameMode == GameMode.HAPTIC_CENTER) {
                        ClientSend.NotifyServerStateChange(ServerState.ARENA);
                    }
                }
                break;
            case SceneState.ARENA:
                for(int clubIdx = 0; clubIdx < 3; ++clubIdx) {
                    int requestClubIdx = (int)ArenaManager.instance.requestClub;
                    if(clubIdx == requestClubIdx && DataManager.instance.isClubReady && !DataManager.instance.isClubPlayed[clubIdx]) 
                        {
                        nextSceneState = (SceneState) clubIdx;
                        if(gameMode == GameMode.HAPTIC_CENTER) {
                            ClientSend.NotifyServerStateChange((ServerState)clubIdx);
                        }
                        break;
                    }
                }
                break;
            case SceneState.SHOOTING_CLUB:
            case SceneState.TENNIS_CLUB:
            case SceneState.MUSICGAME_CLUB:
                if(DataManager.instance.isClubPlayed[(int)currentSceneState]) {
                    nextSceneState = SceneState.ARENA;
                    if(gameMode == GameMode.HAPTIC_CENTER) {
                        ClientSend.NotifyServerStateChange(ServerState.ARENA);
                    }
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
        forestIslandRoot.localPosition = sceneTransforms[nextSceneState].localPosition;
        forestIslandRoot.localRotation = sceneTransforms[nextSceneState].localRotation;
        currentSceneState = nextSceneState;
    }

    
}
