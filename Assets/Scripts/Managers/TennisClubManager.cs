using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using OculusSampleFramework;
using UnityEngine.Assertions;

public class TennisClubManager : SceneManager<TennisClubManager> {
    public SceneState currentClub;
    public Device requiredDevice;
    private RacketScript racket;
    private Camera playerCamera; 
    
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
    public GameObject returnText3d;
    public float returningTime;

    public PickBallMachine pickBallMachine;
    private PropSupport racketSupport;

    public float propSupportMaxHeight;
    public float propSupportMinHeight;
    public float propSupportAnimationTime;
    public AnimationCurve propSupportAnimationCurve;

    public GameObject fetchTrigger;
    public GameObject fetchText3d;

    public GameObject readyTrigger;
    public GameObject readyAnchor;
    public Text readyText2d;
    public float readyTextTime;

    public Image progressBarImage;

    public Text startText2d;
    public float startTextTime;
    
    public ServingMachine servingMachine;
    

    public GameObject senpai;

    private bool isStartTextShowed;
    private bool isReadyTextShowed;
    
    private bool isPropStateMachineRunning;

    public Image aimImage;

    public GameObject arenaSign;
    public Text finishText2d;
    public float finishTextTime;

    public GameObject putBackTrigger;
    public GameObject putBackText3d;
    private GameObject shiftyRoot;
    private GameObject shiftyCartridgeRoot;
    public GameObject questRacketSupportAnchor;

    private int remainBallNum;
    public Text remainBallText;

    public GameObject ballInText3d;
    public GameObject ballOutText3d;
    public float serveEndWaitingTime;

    public GameObject tennisField;

    public GameObject racketHookObj;

    public GameObject turnRightText3d;
    public AudioSource audioSource;
    protected override void OnAwake() {

     clubStateInits = new Dictionary<ClubState,Action>() {
         {ClubState.IDLE, ()=> { InitIdle(); } },
         {ClubState.WAITING, ()=>{ InitWaiting();} },
         {ClubState.READY, ()=>{ InitReady(); } },
         {ClubState.GAME, ()=>{ InitGame(); } },
         {ClubState.RESULT, ()=>{ InitResult(); } }
     };
     clubStateExits = new Dictionary<ClubState,Action>() {
         {ClubState.IDLE, ()=> { ExitIdle(); } },
         {ClubState.WAITING, ()=>{ ExitWaiting();} },
         {ClubState.READY, ()=>{ ExitReady(); } },
         {ClubState.GAME, ()=>{ ExitGame(); } },
         {ClubState.RESULT, ()=>{ ExitResult(); } }
     };

     propStateInits = new Dictionary<PropState,Action>() {
         {PropState.DELIVERING, ()=>{ InitDelivering(); }},
         {PropState.FETCHING, ()=>{ InitFetching(); }},
         {PropState.RETURNING, ()=>{ InitReturning(); }},
         {PropState.PUTBACK, ()=>{ InitPutBack(); }}
     };
     propStateExits = new Dictionary<PropState,Action>() {
         {PropState.DELIVERING, ()=>{ ExitDelivering(); }},
         {PropState.FETCHING, ()=>{ ExitFetching(); }},
         {PropState.RETURNING, ()=>{ ExitReturning(); }},
         {PropState.PUTBACK, ()=>{ ExitPutBack(); }}

     };
    }

    public override void Init() {
        audioSource.Play();
        Debug.Log("Tennis Club Init");
        DataManager.instance.requestDevice = Device.SHIFTY;
        playerCamera = DataManager.instance.playerCamera;
        racket = DataManager.instance.racketObj.GetComponent<RacketScript>();
        racket.gameObject.SetActive(false);
        racketSupport = DataManager.instance.racketSupportObj.GetComponent<PropSupport>();
        racketSupport.gameObject.SetActive(false);
        racketSupport.minHeight = propSupportMinHeight;
        racketSupport.maxHeight = propSupportMaxHeight;
        racketSupport.animationTime = propSupportAnimationTime;
        racketSupport.curve = propSupportAnimationCurve;
        fetchTrigger.SetActive(false);
        fetchText3d.SetActive(false);
        readyTrigger.SetActive(false);
        readyAnchor.SetActive(false);
        readyText2d.gameObject.SetActive(false);
        progressBarImage.gameObject.SetActive(false);
        startText2d.gameObject.SetActive(false);
        servingMachine.gameObject.SetActive(false);
        
        senpai.SetActive(false);
        isReadyTextShowed = false;
        isStartTextShowed = false;
        isPropStateMachineRunning = false;
        aimImage.gameObject.SetActive(false);
        arenaSign.SetActive(false);
        finishText2d.gameObject.SetActive(false);
        pickBallMachine.gameObject.SetActive(false);
        servingMachine.gameObject.SetActive(false);
        shiftyRoot = DataManager.instance.shiftyRoot;
        shiftyCartridgeRoot = DataManager.instance.shiftyCartridgeRoot;
        remainBallText.gameObject.SetActive(false);
        ballInText3d.SetActive(false);
        ballOutText3d.SetActive(false);
        if (GameManager.instance.gameMode == GameMode.QUEST)
        {
            ClubUtil.Attach(racket.gameObject, racketSupport.gameObject);
            racket.gameObject.transform.localEulerAngles = new Vector3(90f, 0f, 0f);
            ClubUtil.Attach(racketSupport.gameObject, questRacketSupportAnchor);
            Assert.AreNotApproximatelyEqual(0f, deliveringTime);
            Assert.AreNotApproximatelyEqual(0f, returningTime);
            returningTime = 2f;
        }
        else
        {
            ClubUtil.Attach(racket.gameObject, shiftyRoot);
            ClubUtil.Attach(racketSupport.gameObject, shiftyCartridgeRoot);
        }
        InitIdle();
        StartCoroutine(UpdateClubState());
        Debug.Log($"{tennisField.transform.position}, {tennisField.transform.rotation}");
    }
    public override void Exit() {
        audioSource.Stop();
        Debug.Log("Tennis Club Exit");
        StopCoroutine(UpdateClubState());
    }

    public void InitIdle() {
        turnRightText3d.SetActive(true);
        welcomeText.gameObject.SetActive(true);
        StartCoroutine(Timer.StartTimer(welcomeTextTime, ()=>{ nextClubState = ClubState.WAITING; }));
    }

    public void OnIdle() {
    }
    
    public void ExitIdle() {
        turnRightText3d.SetActive(false);
        welcomeText.gameObject.SetActive(false);
    }

    public void InitWaiting() {
        pickBallMachine.gameObject.SetActive(true);
        pickBallMachine.Init();
        Debug.Log("Inits waiting");
        DataManager.instance.isInReadyZone = false;
        StartPropStateMachine(PropState.DELIVERING, InitDelivering);
    }

    public void OnWaiting() {
        if (DataManager.instance.isInReadyZone && !isPropStateMachineRunning)
        {
            nextClubState = ClubState.READY;
        }
    }

    public void ExitWaiting() {
        Debug.Log("Exit waiting");
        pickBallMachine.End();
        pickBallMachine.gameObject.SetActive(false);
        StopPropStateMachine();
    }

    public void InitDelivering() {
        if(GameManager.instance.gameMode == GameMode.QUEST) {
            StartCoroutine(Timer.StartTimer(deliveringTime, () => {
                DataManager.instance.isDeviceReady[(int)requiredDevice] = true;
            }));
        }
        Debug.Log("Init Delivering");
    }

    public void OnDelivering() {
        if(DataManager.instance.isDeviceReady[(int)requiredDevice]) {
            Debug.Log("Shifty is ready");
            nextPropState = PropState.FETCHING;
            racket.gameObject.SetActive(false);
            racketSupport.gameObject.SetActive(true);
            racketSupport.Rise(() => { });
        }
    }
    
    public void ExitDelivering() {}

    public void InitFetching() {
        //TODO: Set trigger and text
        fetchTrigger.SetActive(false);
        fetchText3d.SetActive(false);
        Debug.Log("Init Fetching");
        StartCoroutine(Timer.StartTimer(2.5f, ()=>{
            racket.gameObject.SetActive(true);
            racketHookObj.SetActive(true);
            fetchTrigger.SetActive(true);
            fetchText3d.gameObject.SetActive(true);
            DataManager.instance.PlayArrivedSound();
        }));
        // TODO: Do we need to change appearance?
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
        racketHookObj.SetActive(false);
    }

    public void InitReturning() {
        DataManager.instance.isInReadyZone = false;
        returnText3d.gameObject.SetActive(true);
        readyTrigger.SetActive(true);
        readyAnchor.SetActive(true);
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
        readyTrigger.SetActive(false);
        readyAnchor.SetActive(false);
        isPropStateMachineRunning = false;
    }

    public void InitReady()
    {
        aimImage.gameObject.SetActive(true);
        readyText2d.gameObject.SetActive(true);
        progressBarImage.gameObject.SetActive(true);
        progressBarImage.fillAmount = 0f;

        isReadyTextShowed = false;
        isStartTextShowed = false;

        racketSupport.Drop(() => { racketSupport.gameObject.SetActive(false); });
    }

    public void OnReady()
    {
        if (isStartTextShowed)
        {
            nextClubState = ClubState.GAME;
        }
        else
        {
            //LayerMask layerMask = 1;
            Ray ray = DataManager.instance.playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {   
                Debug.Log(hit.transform.gameObject.name);
                if (hit.transform.gameObject.name == "GazeTrigger")
                {
                    progressBarImage.fillAmount += 1f / readyTextTime * Time.deltaTime;
                }
                else
                {
                    progressBarImage.fillAmount = 0f;
                }
            }
            if (Mathf.Approximately(1f, progressBarImage.fillAmount))
            {
                isReadyTextShowed = true;
            }

            if (readyText2d.IsActive() && isReadyTextShowed)
            {
                readyText2d.gameObject.SetActive(false);
                progressBarImage.gameObject.SetActive(false);
                aimImage.gameObject.SetActive(false);
                startText2d.gameObject.SetActive(true);
                StartCoroutine(Timer.StartTimer(startTextTime, () =>
                {
                    isStartTextShowed = true;
                }));
            }
        }
    }

    public void ExitReady()
    {
        
        startText2d.gameObject.SetActive(false);
    }

    public void InitGame() {
        //Debug.Log("Game start, please wait for 3 sec.");
        //StartCoroutine(Timer.StartTimer(3f, ()=>{ nextClubState = ClubState.RESULT; }));
        
        senpai.SetActive(true);
        servingMachine.gameObject.SetActive(true);
        servingMachine.Init();
        remainBallNum = servingMachine.serveNum;
        remainBallText.text = $"{remainBallNum}";
        remainBallText.gameObject.SetActive(true);
        servingMachine.OnServeEnd = () => {
            remainBallNum--;
            remainBallText.text = $"{remainBallNum}";
        };
        servingMachine.fairZone.OnBallIn = () => {
            // TODO: Ball in sound
            ballInText3d.SetActive(true);
            ballInText3d.GetComponent<Text3dAnimation>().RiseFadeInOut(()=>{ ballInText3d.SetActive(false); });

        };
        servingMachine.fairZone.OnBallOut = () =>
        {
            // TODO: Ball out sound
            ballOutText3d.SetActive(true);
            ballOutText3d.GetComponent<Text3dAnimation>().RiseFadeInOut(() => { ballOutText3d.SetActive(false); });
        };
    }

    public void OnGame() {
        
        if(servingMachine.isServeOver) {
            DataManager.instance.OnServeStateEnd += () => {
                senpai.SetActive(false);
                StartCoroutine(Timer.StartTimer(serveEndWaitingTime, ()=>{
                    nextClubState = ClubState.RESULT;
                    DataManager.instance.OnServeStateEnd = null;
                }));
            };
        }
    }

    public void ExitGame() {
        Debug.Log("Game over");
        servingMachine.End();
        servingMachine.gameObject.SetActive(false);
        remainBallText.gameObject.SetActive(false);
        senpai.SetActive(false);
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
        if (DataManager.instance.isInReadyZone && !isPropStateMachineRunning)
        {
            StopPropStateMachine();
            arenaSign.SetActive(true);
        }
    }
    public void ExitResult() {
    }
    public void InitPutBack() {
        racketHookObj.SetActive(true);
        racketSupport.gameObject.SetActive(true);
        racketSupport.Rise(() =>
            {
                putBackTrigger.SetActive(true);
                putBackText3d.gameObject.SetActive(true);
            });
        
    }
    public void OnPutBack() {
        ClubUtil.TextLookAt(putBackText3d, playerCamera.gameObject);

        if (DataManager.instance.isPropPutBack[(int)requiredDevice])
        {
            if (GameManager.instance.gameMode == GameMode.HAPTIC_CENTER)
            {
                ClientSend.ReleaseDevice();
            }
            nextPropState = PropState.RETURNING;
        }
    }
    public void ExitPutBack() {
        putBackTrigger.SetActive(false);
        putBackText3d.gameObject.SetActive(false);
        racketSupport.Drop(() =>
        {
            racketSupport.gameObject.SetActive(false);
        });
    }
    private void StartPropStateMachine(PropState initState, Action InitFunction)
    {
        currentPropState = initState;
        nextPropState = initState;
        InitFunction();
        isPropStateMachineRunning = true;
        StartCoroutine(UpdatePropState());
    }

    private void StopPropStateMachine()
    {
        isPropStateMachineRunning = false;
        StopCoroutine(UpdatePropState());
    }

    /* Change State Functions */

    private void ChangeClubState()
    {
        clubStateExits[currentClubState]();
        clubStateInits[nextClubState]();
        currentClubState = nextClubState;
    }

    private void ChangePropState()
    {
        propStateExits[currentPropState]();
        propStateInits[nextPropState]();
        currentPropState = nextPropState;
    }

    /* Update State Functions */

    private IEnumerator UpdateClubState()
    {
        while (true)
        {
            if (currentClubState != nextClubState)
            {
                ChangeClubState();
            }
            switch (currentClubState)
            {
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

    private IEnumerator UpdatePropState()
    {
        while (true)
        {
            if (currentPropState != nextPropState)
            {
                ChangePropState();
            }
            switch (currentPropState)
            {
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