using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using OculusSampleFramework;

public class TennisClubManager : SceneManager<TennisClubManager> {
    public SceneState currentClub;
    public Device requiredDevice;
    public GameObject racket;
    
    public ClubState currentClubState;
    public ClubState nextClubState;
    public PropState currentPropState;
    public PropState nextPropState;

    public Dictionary<ClubState, Action> clubStateInits;
    public Dictionary<ClubState, Action> clubStateExits;

    public Dictionary<PropState, Action> propStateInits;
    public Dictionary<PropState, Action> propStateExits;

    public Text welcomeText;
    public float welcomeTextTime;

    public float deliveringTime;
    public GameObject returnText;
    public float returningTime;

    public PickBallMachine pickBallMachine;
    public GameObject propStand;

    public float propStandMaxHeight;
    public float propStandMinHeight;
    public float propStandAnimationTime;
    public AnimationCurve propStandAnimationCurve;

    public GameObject fetchTrigger;
    public GameObject fetchText;

    public GameObject readyTrigger;
    public GameObject readyText;
    
    protected override void OnAwake() {
    //TODO:
    // Assert
    // Dict init

     clubStateInits = new Dictionary<ClubState,Action>() {

     };
     clubStateExits = new Dictionary<ClubState,Action>() {

     };

     propStateInits = new Dictionary<PropState,Action>() {

     };
     propStateExits = new Dictionary<PropState,Action>() {

     };
    }

    public override void Init() {
        InitIdle();
        StartCoroutine(UpdateClubState());
    }
    public override void Exit() {
        StopCoroutine(UpdateClubState());
        Debug.Log("Tennis Club End");
    }

    public void InitIdle() {
        welcomeText.gameObject.SetActive(true);
        StartCoroutine(Timer.StartTimer(welcomeTextTime, ()=>{ nextClubState = ClubState.WAITING; }));
    }

    public void OnIdle() {}
    
    public void ExitIdle() {
        welcomeText.gameObject.SetActive(false);
    }

    public void InitWaiting() {
        pickBallMachine.gameObject.SetActive(true);
        pickBallMachine.Init();
        InitDelivering();
        StartCoroutine(UpdatePropState());
    }

    public void ExitWaiting() {
        pickBallMachine.End();
        pickBallMachine.gameObject.SetActive(false);
    }

    public void InitDelivering() {
        if(GameManager.instance.gameMode == GameMode.QUEST) {
            StartCoroutine(Timer.StartTimer(deliveringTime, () => {
                DataManager.instance.isDeviceReady[(int)requiredDevice] = true;
            }));
        }
    }

    public void OnDelivering() {
        if(DataManager.instance.isDeviceReady[(int)requiredDevice] && !propStand.activeInHierarchy) {
            propStand.SetActive(true);
            Sequence propStandRisingSequence = DOTween.Sequence();
            propStandRisingSequence.Append(propStand.transform.DOLocalMoveY(propStandMaxHeight, propStandAnimationTime))
                .SetEase(propStandAnimationCurve)
                .AppendCallback(() => {
                    nextPropState = PropState.FETCHING;
                    racket.SetActive(true);
                    });
            propStandRisingSequence.Play();
        }
    }
    
    public void ExitDelivering() {}

    public void InitFetching() {
        //TODO: Set trigger and text
        fetchTrigger.SetActive(true);
        fetchText.gameObject.SetActive(true);
        // TODO: Do we need to change appearance?
        if(GameManager.instance.gameMode == GameMode.QUEST) {
            DataManager.instance.isDeviceFollowHand = true;
        }
    }

    public void OnFetching() {
        if(DataManager.instance.isDeviceFetched[(int)requiredDevice]) {
            nextPropState = PropState.RETURNING;
        }
        fetchText.transform.LookAt(DataManager.instance.playerCamera.transform.position);
        Vector3 tmp = fetchText.transform.eulerAngles;
        if(GameManager.instance.gameMode == GameMode.QUEST) {
            tmp.x = -tmp.x;
            tmp.y += 180f;
            fetchText.transform.eulerAngles = tmp;
        }   
    }

    public void ExitFetching() {
        fetchTrigger.SetActive(false);
        fetchText.gameObject.SetActive(false);
        if(GameManager.instance.gameMode == GameMode.QUEST) {
            DataManager.instance.isDeviceFollowHand = false;
        }
    }

    public void InitReturning() {
        returnText.gameObject.SetActive(true);
        readyTrigger.SetActive(true);
        if(GameManager.instance.gameMode == GameMode.QUEST) {
            StartCoroutine(Timer.StartTimer(returningTime, ()=>{
                DataManager.instance.isInReadyZone[(int)currentClub] = true;
            }));
        }
    }

    public void OnReturning() {
        if(DataManager.instance.isInReadyZone[(int)currentClub]) {
            ExitReturning();
            //nextClubState = ClubState.READY;
        }
    }

    public void ExitReturning() {
        returnText.gameObject.SetActive(false);
        readyTrigger.SetActive(false);
    }

    public IEnumerator UpdateClubState() {
        yield return null;
    }
    public IEnumerator UpdatePropState() {
        yield return null;
    }
}