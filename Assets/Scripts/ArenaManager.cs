using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaManager : StateSingleton
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
        GameObject.instance.forestIslandRoot.position = toArenaTransform.position;
        GameObject.instance.forestIslandRoot.eulerAngles = toArenaTransform.eulerAngles;

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
