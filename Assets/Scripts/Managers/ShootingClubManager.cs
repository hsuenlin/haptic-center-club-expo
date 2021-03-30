using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using DG.Tweening;

public class ShootingClubManager : SceneManager<ShootingClubManager>
{
    
    public SceneState currentClub;
    public Device requiredDevice;
    
    public ClubState currentClubState;
    public ClubState nextClubState;
    public PropState currentPropState;
    public PropState nextPropState;

    private Dictionary<ClubState, Action> clubStateInits;
    private Dictionary<ClubState, Action> clubStateExits;

    private Dictionary<PropState, Action> propStateInits;
    private Dictionary<PropState, Action> propStateExits;

    /* Idle State */
    public Text welcomeText2d;
    public float welcomeTextTime; //3f

    public GameObject addTargetDemoBtn;

    private GunScript gun;
    private PropSupport gunSupport;

    private GameObject controllerRoot;
    private GameObject controllerCartridgeRoot;
    public float propSupportMinHeight;
    public float propSupportMaxHeight;
    public float propSupportAnimationTime;
    public AnimationCurve propSupportAnimationCurve;
    public GameObject fetchTrigger;
    public GameObject fetchText3d;

    public GameObject returnText3d;
    public GameObject readyTrigger;
    public GameObject readyAnchor;

    public Text readyText2d;
    public Text startText2d;
    public float readyTextTime; // 1f
    public float startTextTime; // 1.75f
    public Image progressBarImage;

    public Image healthBarImage;

    public Text finishText2d;
    public float finishTextTime; // 2f

    /* Variables for Quest mode */
    public float deliveringTime;
    public float returningTime;
    public GameObject questGunSupportAnchor;
    public GameObject readyArea;

    public GameObject aimImage;

    public GameObject putBackTrigger;
    public GameObject putBackText3d;

    private bool isPropStateMachineRunning;

    public GameObject arenaSign;

    private bool isReadyTextShowed;
    private bool isStartTextShowed;

    private Camera playerCamera;

    protected override void OnAwake()
    {

        Assert.IsNotNull(welcomeText2d);
        Assert.AreNotApproximatelyEqual(0f, welcomeTextTime);
        
        Assert.IsNotNull(addTargetDemoBtn);

        Assert.AreNotApproximatelyEqual(0f, propSupportAnimationTime);
        
        Assert.IsNotNull(propSupportAnimationCurve);
        Assert.IsNotNull(fetchTrigger);
        Assert.IsNotNull(fetchText3d);
    
        Assert.IsNotNull(returnText3d);
        Assert.IsNotNull(readyTrigger);

        Assert.IsNotNull(readyText2d);
        Assert.IsNotNull(startText2d);
        Assert.AreNotApproximatelyEqual(0f, readyTextTime);
        Assert.AreNotApproximatelyEqual(0f, startTextTime);
        Assert.IsNotNull(progressBarImage);

        Assert.IsNotNull(healthBarImage);

        Assert.IsNotNull(finishText2d);
        Assert.AreNotApproximatelyEqual(0f, finishTextTime);

        Assert.IsNotNull(putBackTrigger);
        Assert.IsNotNull(putBackText3d);

        Assert.IsNotNull(arenaSign);

        welcomeText2d.gameObject.SetActive(false);
        addTargetDemoBtn.SetActive(false);
        fetchTrigger.SetActive(false);
        fetchText3d.SetActive(false);
        returnText3d.SetActive(false);
        readyText2d.gameObject.SetActive(false);
        startText2d.gameObject.SetActive(false);
        progressBarImage.gameObject.SetActive(false);
        healthBarImage.gameObject.SetActive(false);
        finishText2d.gameObject.SetActive(false);

        putBackTrigger.SetActive(false);
        putBackText3d.SetActive(false);

        isPropStateMachineRunning = false;

        arenaSign.SetActive(false);
        
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
            {PropState.RETURNING, ()=>{ InitReturning(); }},
            {PropState.PUTBACK, ()=>{ InitPutBack(); }}
        };
        propStateExits = new Dictionary<PropState, Action>() {
            {PropState.DELIVERING, ()=>{ ExitDelivering(); }},
            {PropState.FETCHING, ()=>{ ExitFetching(); }},
            {PropState.RETURNING, ()=>{ ExitReturning(); }},
            {PropState.PUTBACK, ()=>{ ExitPutBack(); }}
        };

        
        
    }

    public override void Init() {
        // TODO: Init all variables
        playerCamera = DataManager.instance.playerCamera;
        //ReadyZone
        DataManager.instance.requestDevice = Device.CONTROLLER;
        gun = DataManager.instance.gunObj.GetComponent<GunScript>();
        gunSupport = DataManager.instance.gunSupportObj.GetComponent<PropSupport>();
        controllerRoot = DataManager.instance.controllerRoot;
        controllerCartridgeRoot = DataManager.instance.controllerCartridgeRoot;
        gun.gameObject.SetActive(false);
        readyAnchor.SetActive(false);
        readyTrigger.SetActive(false);
        gunSupport.gameObject.SetActive(false);
        gunSupport.gameObject.transform.localPosition = new Vector3(0f, propSupportMinHeight, 0f);
        gunSupport.minHeight = propSupportMinHeight;
        gunSupport.maxHeight = propSupportMaxHeight;
        gunSupport.animationTime = propSupportAnimationTime;
        gunSupport.curve = propSupportAnimationCurve;

        if (GameManager.instance.gameMode == GameMode.QUEST)
        {
            ClubUtil.Attach(gun.gameObject, gunSupport.gameObject);
            ClubUtil.Attach(gunSupport.gameObject, questGunSupportAnchor);
            Assert.AreNotApproximatelyEqual(0f, deliveringTime);
            Assert.AreNotApproximatelyEqual(0f, returningTime);
            returningTime = 2f;
        } else {
            ClubUtil.Attach(gun.gameObject, controllerRoot);
            ClubUtil.Attach(gunSupport.gameObject, controllerCartridgeRoot);
        }

        //PropStand
        InitIdle();
        StartCoroutine(UpdateClubState());
    }
    public override void Exit() {
        StopCoroutine(UpdateClubState());
        Debug.Log("Shooting Club End");
    }

    /* Club States Methods */

    public void InitIdle() {
        welcomeText2d.gameObject.SetActive(true);
        StartCoroutine(Timer.StartTimer(welcomeTextTime, ()=>{ nextClubState = ClubState.WAITING; }));
    }

    public void OnIdle() {
    }

    public void ExitIdle() {
        welcomeText2d.gameObject.SetActive(false);
    }

    public void InitWaiting() {
        addTargetDemoBtn.SetActive(true);
        TargetMachine.instance.AddTargetDemo();
        DataManager.instance.isInReadyZone = false;
        StartPropStateMachine(PropState.DELIVERING, InitDelivering);
    }

    public void OnWaiting() {
        if(DataManager.instance.isInReadyZone && !isPropStateMachineRunning) {
            nextClubState = ClubState.READY;
        }
    }

    public void ExitWaiting() {
        addTargetDemoBtn.SetActive(false);
        TargetMachine.instance.DestroyTargetDemos();
        StopPropStateMachine();
    }
    
    public void InitReady() {
        aimImage.SetActive(true);
        readyText2d.gameObject.SetActive(true);
        progressBarImage.gameObject.SetActive(true);
        progressBarImage.fillAmount = 0f;

        isReadyTextShowed = false;
        isStartTextShowed = false;

        gunSupport.Drop(() => { gunSupport.gameObject.SetActive(false); });
    }

    public void OnReady() {
        if(isStartTextShowed) {
            nextClubState = ClubState.GAME;
        }
        else 
        {
            Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit)) {
                if(hit.transform.gameObject.name == "GazeTrigger") {
                    progressBarImage.fillAmount += 1f / readyTextTime * Time.deltaTime;
                } else {
                    progressBarImage.fillAmount = 0f;
                }
            }
            if(Mathf.Approximately(1f, progressBarImage.fillAmount)) {
                isReadyTextShowed = true;
             }

            if(readyText2d.IsActive() && isReadyTextShowed) {
                aimImage.SetActive(false);
                readyText2d.gameObject.SetActive(false);
                progressBarImage.gameObject.SetActive(false);
                startText2d.gameObject.SetActive(true);
                StartCoroutine(Timer.StartTimer(startTextTime, ()=>{
                    isStartTextShowed = true;
                }));
            }
        }
    }

    public void ExitReady() {
        startText2d.gameObject.SetActive(false);
    }

    public void InitGame() {
        healthBarImage.gameObject.SetActive(true);
        StartCoroutine(TargetMachine.instance.StartShooting());
        if(GameManager.instance.gameMode == GameMode.QUEST) {
            StartCoroutine(gun.StartAutoShooting());
        }
    }

    public void OnGame() {
        if(TargetMachine.instance.AllTargetsDie()) {
            nextClubState = ClubState.RESULT;
        }
    }

    public void ExitGame() {
        healthBarImage.gameObject.SetActive(false);
        StopCoroutine(TargetMachine.instance.StartShooting());
        if(GameManager.instance.gameMode == GameMode.QUEST) {
            StopCoroutine(gun.StartAutoShooting());
        }
        gun.appearance = DeviceAppearance.REAL;
    }

    public void InitResult() {
        DataManager.instance.isInReadyZone = false;
        arenaSign.SetActive(false);
        finishText2d.gameObject.SetActive(true);
        StartCoroutine(Timer.StartTimer(finishTextTime, () =>
        {
            finishText2d.gameObject.SetActive(false);
            StartPropStateMachine(PropState.PUTBACK, InitPutBack);
        }));
    }

    public void OnResult() {
        if(DataManager.instance.isInReadyZone && !isPropStateMachineRunning) {
            StopPropStateMachine();
            arenaSign.SetActive(true);
        }
    }

    public void ExitResult() {
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
        if(DataManager.instance.isDeviceReady[(int)requiredDevice]) {
            gun.gameObject.SetActive(false);
            nextPropState = PropState.FETCHING;
            gunSupport.gameObject.SetActive(true);
            gunSupport.Rise(() => { });
        }
    }

    public void ExitDelivering() {
        
    }

    public void InitFetching() {
        // fetchArea
        StartCoroutine(Timer.StartTimer(1.5f, () =>
        {
            fetchTrigger.SetActive(true);
            fetchText3d.gameObject.SetActive(true);
            gun.gameObject.SetActive(true);
        }));
        
        if(GameManager.instance.gameMode == GameMode.QUEST) {
            DataManager.instance.isDeviceFollowHand = false;
        }
    }

    public void OnFetching() {
        if(DataManager.instance.isDeviceFetched[(int)requiredDevice]) {
            nextPropState = PropState.RETURNING;
        }
        ClubUtil.TextLookAt(fetchText3d, playerCamera.gameObject);
    }

    public void ExitFetching() {
        fetchTrigger.SetActive(false);
        fetchText3d.gameObject.SetActive(false);
    }

    public void InitReturning() {
        DataManager.instance.isInReadyZone = false;
        returnText3d.gameObject.SetActive(true);
        readyAnchor.SetActive(true);
        readyTrigger.SetActive(true);
        if(GameManager.instance.gameMode == GameMode.QUEST) {
            StartCoroutine(Timer.StartTimer(returningTime, ()=>{
                DataManager.instance.isInReadyZone = true;
            }));
        }
    }

    public void OnReturning() {
        ClubUtil.TextLookAt(returnText3d, playerCamera.gameObject);
        if(DataManager.instance.isInReadyZone) {
            ExitReturning();
        }
    }

    public void ExitReturning() {
        returnText3d.gameObject.SetActive(false);
        readyAnchor.SetActive(false);
        readyTrigger.SetActive(false);
        isPropStateMachineRunning = false;
    }

    public void InitPutBack() {
        gunSupport.gameObject.SetActive(true);
        gunSupport.Rise(() =>
            {
                putBackTrigger.SetActive(true);
                putBackText3d.gameObject.SetActive(true);
            });
    }

    public void OnPutBack() {
        ClubUtil.TextLookAt(putBackText3d, playerCamera.gameObject);
        
        if(DataManager.instance.isPropPutBack[(int)requiredDevice]) {
            if(GameManager.instance.gameMode == GameMode.HAPTIC_CENTER) {
                ClientSend.ReleaseDevice();
            }
            nextPropState = PropState.RETURNING;
        }
    }

    public void ExitPutBack() {
        putBackTrigger.SetActive(false);
        putBackText3d.gameObject.SetActive(false);
        gunSupport.Drop(() =>
        {
            gunSupport.gameObject.SetActive(false);
        });
    }

    private void StartPropStateMachine(PropState initState, Action InitFunction) {
        currentPropState = initState;
        nextPropState = initState;
        InitFunction();
        isPropStateMachineRunning = true;
        StartCoroutine(UpdatePropState());
    }

    private void StopPropStateMachine() {
        isPropStateMachineRunning = false;
        StopCoroutine(UpdatePropState());
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

    private IEnumerator UpdateClubState() {
        while(true) {
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
            yield return null;
        }
    }

    private IEnumerator UpdatePropState() {
        while(true) {
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
                case PropState.PUTBACK:
                    OnPutBack();
                    break;
                default:
                    break;
            }
            yield return null;
        }
    }
}
