using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using OculusSampleFramework;
using UnityEngine.Assertions;

public class MusicGameClubManager : SceneManager<MusicGameClubManager> {
    public SceneState currentClub;
    public Device requiredDevice;
    private DjPanelScript djPanel;
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

    private PropSupport panelSupport;

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

    private bool isStartTextShowed;
    private bool isReadyTextShowed;
    
    private bool isPropStateMachineRunning;

    public Image aimImage;

    public GameObject arenaSign;
    public Text finishText2d;
    public float finishTextTime;

    public GameObject putBackTrigger;
    public GameObject putBackText3d;
    private GameObject panelRoot;
    public GameObject questDjPanelSupportAnchor;
    public PunchBeatGame pbGame;
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
        Debug.Log("Music Game Club Init");

        DataManager.instance.requestDevice = Device.PANEL;

        
        playerCamera = DataManager.instance.playerCamera;
        djPanel = DataManager.instance.djPanelObj.GetComponent<DjPanelScript>();
        djPanel.gameObject.SetActive(false);
        panelSupport = DataManager.instance.djPanelSupportObj.GetComponent<PropSupport>();
        panelSupport.gameObject.SetActive(false);
        panelSupport.minHeight = propSupportMinHeight;
        panelSupport.maxHeight = propSupportMaxHeight;
        panelSupport.animationTime = propSupportAnimationTime;
        panelSupport.curve = propSupportAnimationCurve;
        fetchTrigger.SetActive(false);
        fetchText3d.SetActive(false);
        readyTrigger.SetActive(false);
        readyAnchor.SetActive(false);
        readyText2d.gameObject.SetActive(false);
        progressBarImage.gameObject.SetActive(false);
        startText2d.gameObject.SetActive(false);
        
        isReadyTextShowed = false;
        isStartTextShowed = false;
        isPropStateMachineRunning = false;
        aimImage.gameObject.SetActive(false);
        arenaSign.SetActive(false);
        finishText2d.gameObject.SetActive(false);
        panelRoot = DataManager.instance.panelRoot;
        if (GameManager.instance.gameMode == GameMode.QUEST)
        {
            ClubUtil.Attach(djPanel.gameObject, panelSupport.gameObject);
            djPanel.gameObject.transform.localEulerAngles = new Vector3(0f, 90f, 0f);
            ClubUtil.Attach(panelSupport.gameObject, questDjPanelSupportAnchor);
            Assert.AreNotApproximatelyEqual(0f, deliveringTime);
            Assert.AreNotApproximatelyEqual(0f, returningTime);
            returningTime = 2f;
        }
        else
        {
            ClubUtil.Attach(djPanel.gameObject, panelRoot);
            ClubUtil.Attach(panelSupport.gameObject, panelRoot);
        }
        
        InitIdle();
        StartCoroutine(UpdateClubState());
    }
    public override void Exit() {
        Debug.Log("Music Game Club Exit");
        StopCoroutine(UpdateClubState());
    }

    public void InitIdle() {
        welcomeText.gameObject.SetActive(true);
        StartCoroutine(Timer.StartTimer(welcomeTextTime, ()=>{ nextClubState = ClubState.WAITING; }));
    }

    public void OnIdle() {
    }
    
    public void ExitIdle() {
        welcomeText.gameObject.SetActive(false);
    }

    public void InitWaiting() {
        DataManager.instance.isInReadyZone = false;
        StartPropStateMachine(PropState.DELIVERING, InitDelivering);
        pbGame.gameObject.SetActive(true);
        pbGame.Init();
        pbGame.Run();
    }

    public void OnWaiting() {
        if (DataManager.instance.isInReadyZone && !isPropStateMachineRunning)
        {
            nextClubState = ClubState.READY;
            //ExitWaiting();
        }
    }

    public void ExitWaiting() {
        StopPropStateMachine();
        pbGame.End();
        pbGame.gameObject.SetActive(false);
    }

    public void InitDelivering() {
        if(GameManager.instance.gameMode == GameMode.QUEST) {
            StartCoroutine(Timer.StartTimer(deliveringTime, () => {
                DataManager.instance.isDeviceReady[(int)requiredDevice] = true;
            }));
        }
    }

    public void OnDelivering() {
        if(DataManager.instance.isDeviceReady[(int)requiredDevice]) {
            nextPropState = PropState.FETCHING;
            //djPanel.gameObject.transform.parent = panelSupport.gameObject.transform;
            djPanel.gameObject.SetActive(false);
            panelSupport.gameObject.SetActive(true);
            panelSupport.Rise(()=>{});
        }
    }
    
    public void ExitDelivering() {}

    public void InitFetching() {
        StartCoroutine(Timer.StartTimer(1.5f, ()=>{
            djPanel.gameObject.SetActive(true);
            fetchTrigger.SetActive(true);
            fetchText3d.gameObject.SetActive(true);
        }));
    }

    public void OnFetching() {
        if(DataManager.instance.isDeviceFetched[(int)requiredDevice]) {
            DataManager.instance.isInReadyZone = true;
            ExitFetching();
            isPropStateMachineRunning = false;
            
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
        aimImage.gameObject.transform.DOLocalMoveZ(-100f, 0f);
        readyText2d.gameObject.SetActive(true);
        progressBarImage.gameObject.SetActive(true);
        progressBarImage.fillAmount = 0f;

        isReadyTextShowed = false;
        isStartTextShowed = false;

        //panelSupport.Drop(() => { panelSupport.gameObject.SetActive(false); });
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
        DjPanelGame.gameObject.SetActive(true);
        DjPanelGame.Init();
        DjPanelGame.Run();
        DjPanelGame.OnGameOver += () => { nextClubState = ClubState.RESULT; };
    }

    public void OnGame() {
    }

    public void ExitGame() {
        DjPanelGame.End();
        DjPanelGame.gameObject.SetActive(false);
    }

    public void InitResult() {
        DataManager.instance.isInReadyZone = false;
        isPropStateMachineRunning = true;
        arenaSign.SetActive(false);
        finishText2d.gameObject.SetActive(true);
        StartCoroutine(Timer.StartTimer(finishTextTime, () =>
        {
            finishText2d.gameObject.SetActive(false);
            StartPropStateMachine(PropState.RETURNING, InitReturning);
        }));
    }
    public void OnResult() {
        if (DataManager.instance.isInReadyZone && !isPropStateMachineRunning)
        {
            StopPropStateMachine();
            djPanel.transform.parent = panelSupport.transform;
            panelSupport.Drop(() =>
                {
                    panelSupport.gameObject.SetActive(false);
                    if (GameManager.instance.gameMode == GameMode.HAPTIC_CENTER)
                    {
                        ClientSend.ReleaseDevice();
                    }
                });
            arenaSign.SetActive(true);
            foreach (GameObject rayToolObj in DataManager.instance.rayTools)
            {
                rayToolObj.SetActive(true);
            }
        }
    }
    public void ExitResult() {
    }
    public void InitPutBack() {
        panelSupport.gameObject.SetActive(true);
        panelSupport.Rise(() =>
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
        panelSupport.Drop(() =>
        {
            panelSupport.gameObject.SetActive(false);
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
            } else {
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
            } else {
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
            }
            yield return null;
        }
    }
}