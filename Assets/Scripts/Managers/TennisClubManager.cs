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
    public Text readyText;
    public float readyTextTime;

    public Image progressBar;

    public Text startText;
    public float startTextTime;
    
    public ServingMachine servingMachine;

    
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
        StopCoroutine(UpdatePropState());
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

    public void InitReady()
    {
        readyText.gameObject.SetActive(true);
        progressBar.gameObject.SetActive(true);
        if (propStand.activeInHierarchy)
        {
            Sequence propStandDropingSequence = DOTween.Sequence();
            propStandDropingSequence.Append(propStand.transform.DOLocalMoveY(propStandMinHeight, propStandAnimationTime))
                    .SetEase(propStandAnimationCurve);
            propStandDropingSequence.Play();
        }
        progressBar.fillAmount = 0f;
    }

    public void OnReady()
    {
        if (DataManager.instance.isStartTextShowed[(int)currentClub])
        {
            nextClubState = ClubState.GAME;
        }
        else
        {
            Ray ray = DataManager.instance.playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.gameObject.name == "GazeTrigger")
                {
                    progressBar.fillAmount += 1f / readyTextTime * Time.deltaTime;
                }
                else
                {
                    progressBar.fillAmount = 0f;
                }
            }
            if (Mathf.Approximately(1f, progressBar.fillAmount))
            {
                DataManager.instance.isReadyTextShowed[(int)currentClub] = true;
            }

            if (readyText.IsActive() && DataManager.instance.isReadyTextShowed[(int)currentClub])
            {
                readyText.gameObject.SetActive(false);
                progressBar.gameObject.SetActive(false);
                startText.gameObject.SetActive(true);
                StartCoroutine(Timer.StartTimer(startTextTime, () =>
                {
                    DataManager.instance.isStartTextShowed[(int)currentClub] = true;
                }));
            }
        }
    }

    public void ExitReady()
    {
        startText.gameObject.SetActive(false);
    }

    public void InitGame() {
        // Player initialization
        servingMachine.gameObject.SetActive(true);
        servingMachine.Init();
    }

    public void OnGame() {
        if(servingMachine.isGameOver) {
            nextClubState = ClubState.RESULT;
        }
    }

    public void ExitGame() {
        servingMachine.End();
        servingMachine.gameObject.SetActive(false);
    }

    public IEnumerator UpdateClubState() {
        yield return null;
    }
    public IEnumerator UpdatePropState() {
        yield return null;
    }
}