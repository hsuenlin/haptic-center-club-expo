using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using DG.Tweening;

public class ShootingClubManager : StateSingleton<ShootingClubManager>
{
    
    public SceneState currentClub;
    public Device requiredDevice;
    
    public ClubState currentClubState;
    public PropState currentPropState;

    public ClubState nextClubState;
    public PropState nextPropState;

    private Dictionary<ClubState, Action> clubStateInits;
    private Dictionary<ClubState, Action> clubStateExits;

    private Dictionary<PropState, Action> propStateInits;
    private Dictionary<PropState, Action> propStateExits;

    public Text welcomeText;
    public float welcomeTextTime; //3f

    public GameObject addTargetDemoBtn;

    public GameObject propStand;
    public float propStandMinHeight;
    public float propStandMaxHeight;
    public float propStandAnimationTime;
    public AnimationCurve propStandAnimationCurve;
    public GameObject fetchTrigger;
    public Text fetchText;

    public Text returnText;
    public GameObject readyPositionIndicator;

    public Text readyText;
    public Text startText;
    public float readyTextTime; // 1f
    public float startTextTime; // 1.75f
    public Image progressBar;

    public Image healthBarImage;

    public Text finishText;
    public float finishTextTime; // 2f

    /* Variables for Quest mode */
    public float deliveringTime;

    /* Timer */
    private float timer = 0f;

    protected override void OnAwake()
    {

        Assert.IsNotNull(welcomeText);
        Assert.AreNotApproximatelyEqual(0f, welcomeTextTime);

        Assert.IsNotNull(propStand);
        Assert.AreNotApproximatelyEqual(0f, propStandAnimationTime);
        Assert.IsNotNull(propStandAnimationCurve);
        Assert.IsNotNull(fetchTrigger);
        Assert.IsNotNull(fetchText);
    
        Assert.IsNotNull(returnText);
        Assert.IsNotNull(readyPositionIndicator);

        Assert.IsNotNull(readyText);
        Assert.IsNotNull(startText);
        Assert.AreNotApproximatelyEqual(0f, readyTextTime);
        Assert.AreNotApproximatelyEqual(0f, startTextTime);
        Assert.IsNotNull(progressBar);

        Assert.IsNotNull(healthBarImage);

        Assert.IsNotNull(finishText);
        Assert.AreNotApproximatelyEqual(0f, finishTextTime);
        
        clubStateInits = new Dictionary<ClubState, Action>() {
            {ClubState.IDLE, ()=>{ InitIdle(); }},
            {ClubState.WAITING, ()=>{ InitWaiting(); }},
            {ClubState.READY, ()=>{ InitReady(); }},
            {ClubState.GAME, ()=>{ InitGame(); }},
            {ClubState.RESULT, ()=>{ InitResult(); }}
        };
        clubStateExits = new Dictionary<ClubState, Action>() {
            {ClubState.IDLE, ()=>{ ExitIdle(); }},
            {ClubState.WAITING, ()=>{ ExitWaiting(); }},
            {ClubState.READY, ()=>{ ExitReady(); }},
            {ClubState.GAME, ()=>{ ExitGame(); }},
            {ClubState.RESULT, ()=>{ ExitResult(); }}
        };
        propStateInits = new Dictionary<PropState, Action>() {
            {PropState.DELIVERING, ()=>{ InitDelivering(); }},
            {PropState.FETCHING, ()=>{ InitFetching(); }},
            {PropState.RETURNING, ()=>{ InitReturning(); }}
        };
        propStateExits = new Dictionary<PropState, Action>() {
            {PropState.DELIVERING, ()=>{ ExitDelivering(); }},
            {PropState.FETCHING, ()=>{ ExitFetching(); }},
            {PropState.RETURNING, ()=>{ ExitReturning(); }}
        };
    }

    /* Club States Methods */

    public void InitIdle() {
        timer = 0f;
        welcomeText.gameObject.SetActive(true);
        StartCoroutine(Timer.StartTimer(welcomeTextTime, ()=>{ nextClubState = ClubState.WAITING; }));
    }

    public void OnIdle() {
    }

    public void ExitIdle() {
        welcomeText.gameObject.SetActive(false);
    }

    public void InitWaiting() {
        timer = 0f;
        addTargetDemoBtn.SetActive(true);
        TargetMachineScript.instance.AddTargetDemo();
    }

    public void OnWaiting() {
        UpdatePropState();
    }

    public void ExitWaiting() {
        addTargetDemoBtn.SetActive(false);
        TargetMachineScript.instance.DestroyTargetDemos();
    }
    
    public void InitReady() {
        readyText.gameObject.SetActive(true);
        progressBar.fillAmount = 0f;
    }

    public void OnReady() {
        Ray ray = DataManager.instance.playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit)) {
            if(hit.transform.gameObject.name == "GazeTrigger") {
                progressBar.fillAmount += 1f / readyTextTime * Time.deltaTime;
            } else {
                progressBar.fillAmount = 0f;
            }
        }
        if(progressBar.fillAmount >= 1f) {
            DataManager.instance.isReadyTextShowed[(int)currentClub] = true;
        }

        if(readyText.IsActive() && DataManager.instance.isReadyTextShowed[(int)currentClub]) {
            readyText.gameObject.SetActive(false);
            startText.gameObject.SetActive(true);
        }
        if(startText.IsActive() && DataManager.instance.isStartTextShowed[(int)currentClub]) {
            nextClubState = ClubState.GAME;
        }
    }

    public void ExitReady() {
        startText.gameObject.SetActive(false);
    }

    public void InitGame() {
        DataManager.instance.player.SetActive(true);
        StartCoroutine(TargetMachineScript.instance.StartShooting());
        if(GameManager.instance.gameMode == GameMode.QUEST) {
            StartCoroutine(DataManager.instance.gun.GetComponent<GunScript>().StartAutoShooting());
        }
    }

    public void OnGame() {
        if(TargetMachineScript.instance.AllTargetsDie()) {
            nextClubState = ClubState.RESULT;
        }
        
    }

    public void ExitGame() {
        healthBarImage.gameObject.SetActive(false);
        if(GameManager.instance.gameMode == GameMode.QUEST) {
            StopCoroutine(DataManager.instance.gun.GetComponent<GunScript>().StartAutoShooting());
        }
    }

    public void InitResult() {
        timer = 0f;
    }

    public void OnResult() {
        if(timer >= finishTextTime) {
            ExitResult();
            DataManager.instance.isClubPlayed[(int)currentClub] = true;
        }
        timer += Time.deltaTime;
    }

    public void ExitResult() {
        finishText.gameObject.SetActive(false);
    }

    /* Prop States Methods */

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
            propStandRisingSequence.Append(propStand.transform.DOMoveY(propStandMaxHeight, propStandAnimationTime))
                .SetEase(propStandAnimationCurve)
                .AppendCallback(() => { nextPropState = PropState.FETCHING; });
            propStandRisingSequence.Play();
        }
    }

    public void ExitDelivering() {
        
    }

    public void InitFetching() {
        // TODO:
        // 1. Gun switch
        // 2. Gun follows hand
        // 3. Using real hand to grab (no need to do)
        fetchTrigger.SetActive(true);
        fetchText.gameObject.SetActive(true);
    }

    public void OnFetching() {
        if(DataManager.instance.isDeviceFetched[(int)requiredDevice]) {
            nextPropState = PropState.RETURNING;
        }
        fetchText.transform.LookAt(DataManager.instance.playerCamera.transform);
    }

    public void ExitFetching() {
        fetchTrigger.SetActive(false);
        fetchText.gameObject.SetActive(false);
        Sequence propStandDropingSequence = DOTween.Sequence();
        propStandDropingSequence.Append(propStand.transform.DOMoveY(propStandMinHeight, propStandAnimationTime))
                .SetEase(propStandAnimationCurve);
        propStandDropingSequence.Play();
    }

    public void InitReturning() {
        returnText.gameObject.SetActive(true);
        readyPositionIndicator.SetActive(true);
    }

    public void OnReturning() {
        if(DataManager.instance.isInReadyZone[(int)currentClub]) {
            ExitReturning();
            nextClubState = ClubState.READY;
        }
    }

    public void ExitReturning() {
        returnText.gameObject.SetActive(false);
        readyPositionIndicator.SetActive(false);
    }

    /* Change State Functions */

    private void ChangeClubState() {
        clubStateExits[currentClubState]();
        clubStateInits[nextClubState]();
        currentClubState = nextClubState;
    }
    
    private void ChangePropState() {
        propStateExits[currentPropState]();
        propStateInits[nextPropState]();
        currentPropState = nextPropState;
    }

    /* Update State Functions */

    private void UpdateClubState() {
        if(currentClubState != nextClubState) {
            ChangeClubState();
        }
        switch(currentClubState) {
            case ClubState.IDLE:
                OnIdle();
                break;
            case ClubState.WAITING:
                OnWaiting();
                break;
            case ClubState.READY:
                OnReady();
                break;
            case ClubState.GAME:
                OnGame();
                break;
            case ClubState.RESULT:
                OnResult();
                break;
            default:
                break;
        }
    }

    private void UpdatePropState() {
        if(currentPropState != nextPropState) {
            ChangePropState();
        }
        switch(currentPropState) {
            case PropState.DELIVERING:
                OnDelivering();
                break;
            case PropState.FETCHING:
                OnFetching();
                break;
            case PropState.RETURNING:
                OnReturning();
                break;
            default:
                break;
        }
    }

    void Update()
    {
        UpdateClubState();
    }
}
