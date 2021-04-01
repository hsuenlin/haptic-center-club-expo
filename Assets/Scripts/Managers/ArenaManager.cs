using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class ArenaManager : SceneManager<ArenaManager>
{
    public GameObject[] floatingArenas;
    
    public GameObject floatingGun;
    public GameObject floatingRacket;
    public GameObject floatingSpeaker;

    public GameObject shootingClubSign;
    public GameObject tennisClubSign;
    public GameObject musicGameClubSign;

    public SceneState requestClub;

    public AudioSource audioSource;

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
        audioSource.Play();

        floatingArenas[0].SetActive(false);
        floatingArenas[1].SetActive(false);
        floatingArenas[2].SetActive(false);

        floatingGun.SetActive(true);
        floatingRacket.SetActive(true);
        floatingSpeaker.SetActive(true);

        shootingClubSign.SetActive(true);
        tennisClubSign.SetActive(true);
        musicGameClubSign.SetActive(true);

        requestClub = SceneState.ARENA;

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
        audioSource.Stop();
        floatingGun.SetActive(false);
        floatingRacket.SetActive(false);
        floatingSpeaker.SetActive(false);

        shootingClubSign.SetActive(false);
        tennisClubSign.SetActive(false);
        musicGameClubSign.SetActive(false);
    }

}
