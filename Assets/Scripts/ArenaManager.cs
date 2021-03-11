using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaManager : StateSingleton
{
    public Transform toArenaTransform;

    // Updated by ClientHandler
    public bool isShootingClubDeviceReady;
    public bool isTennisClubDeviceReady;
    public bool isMusicGameClubDeviceReady;

    public bool isShootingClubReady;
    public bool isTennisClubReady;
    public bool isMusicGameClubReady;

    public Dictionary<SceneState, Action> isDeviceReadyDict;
    
    protected override void OnAwake() {
        Assert.IsNotNull(toArenaTransform);

        Assert.IsNotNull(shootingClubSign);
        Assert.IsNotNull(tennisClubSign);
        Assert.IsNotNull(musicGameClubSign);

        isDeviceReadyDict = new Dictionary<SceneState, Action>() {
            {SceneState.SHOOTING_CLUB, ()=>{ return isShootingClubDeviceReady; }},
            {SceneState.TENNIS_CLUB, ()=>{ return isTennisDeviceReady; }},
            {SceneState.MUSICGAME_CLUB, ()=>{ return isMusicGameClubDeviceReady; }},
        }
    }

    protected override void Init() {
        GameObject.instance.forestIslandRoot.position = toArenaTransform.position;
        GameObject.instance.forestIslandRoot.eulerAngles = toArenaTransform.eulerAngles;
        
        isShootingClubReady = false;
        isTennisClubReady = false;
        isMusicGameClubReady = false;
    }

    public void SetIsClubReady(SceneState state, bool value) {
        switch(state) {
            case SceneState.SHOOTING:
                isShootingClubReady = value;
                break;
            case SceneState.TENNIS_CLUB:
                isTennisClubReady = value;
                break;
            case SceneState.MUSICGAME_CLUB:
                isMusicGameClubReady = value;
                break;
            default:
                break;
        }
    }

    protected override void End() {
        
    }
}
