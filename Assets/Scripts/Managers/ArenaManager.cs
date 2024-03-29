﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class ArenaManager : StateSingleton<ArenaManager>
{
    public Transform toArenaTransform;

    public GameObject floatingGun;
    public GameObject floatingRacket;
    public GameObject floatingSpeaker;

    public GameObject shootingClubSign;
    public GameObject tennisClubSign;
    public GameObject musicGameClubSign;

    protected override void OnAwake() {
        Assert.IsNotNull(toArenaTransform);

        Assert.IsNotNull(floatingGun);
        Assert.IsNotNull(floatingRacket);
        Assert.IsNotNull(floatingSpeaker);

        Assert.IsNotNull(shootingClubSign);
        Assert.IsNotNull(tennisClubSign);
        Assert.IsNotNull(musicGameClubSign);
    }

    protected override void Init() {
        DataManager.instance.forestIslandRoot.position = toArenaTransform.position;
        DataManager.instance.forestIslandRoot.eulerAngles = toArenaTransform.eulerAngles;

        floatingGun.SetActive(true);
        floatingRacket.SetActive(true);
        floatingSpeaker.SetActive(true);

        shootingClubSign.SetActive(true);
        tennisClubSign.SetActive(true);
        musicGameClubSign.SetActive(true);
    }

    protected override void Exit() {
        floatingGun.SetActive(false);
        floatingRacket.SetActive(false);
        floatingSpeaker.SetActive(false);

        shootingClubSign.SetActive(false);
        tennisClubSign.SetActive(false);
        musicGameClubSign.SetActive(false);
    }

}
