using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class ArenaManager : SceneManager<ArenaManager>
{
    public GameObject floatingGun;
    public GameObject floatingRacket;
    public GameObject floatingSpeaker;

    public GameObject shootingClubSign;
    public GameObject tennisClubSign;
    public GameObject musicGameClubSign;

    public SceneState requestClub;

    protected override void OnAwake() {

        Assert.IsNotNull(floatingGun);
        Assert.IsNotNull(floatingRacket);
        Assert.IsNotNull(floatingSpeaker);

        Assert.IsNotNull(shootingClubSign);
        Assert.IsNotNull(tennisClubSign);
        Assert.IsNotNull(musicGameClubSign);

        floatingGun.SetActive(false);
        floatingRacket.SetActive(false);
        floatingSpeaker.SetActive(false);

        shootingClubSign.SetActive(false);
        tennisClubSign.SetActive(false);
        musicGameClubSign.SetActive(false);
    }

    public override void Init() {

        floatingGun.SetActive(true);
        floatingRacket.SetActive(true);
        floatingSpeaker.SetActive(true);

        shootingClubSign.SetActive(true);
        tennisClubSign.SetActive(true);
        musicGameClubSign.SetActive(true);

        requestClub = SceneState.ARENA;
        Debug.Log(requestClub);

        DataManager.instance.isRequestResultReady = false;
        DataManager.instance.isClubReady = false;

        if (GameManager.instance.gameMode == GameMode.QUEST)
        {
            DataManager.instance.isDeviceFree[0] = true;
            DataManager.instance.isDeviceFree[1] = true;
            DataManager.instance.isDeviceFree[2] = true;
        }
    }

    public override void Exit() {
        floatingGun.SetActive(false);
        floatingRacket.SetActive(false);
        floatingSpeaker.SetActive(false);

        shootingClubSign.SetActive(false);
        tennisClubSign.SetActive(false);
        musicGameClubSign.SetActive(false);
    }

}
