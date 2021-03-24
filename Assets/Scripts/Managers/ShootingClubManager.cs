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

    public Text welcomeText;
    public float welcomeTextTime; //3f

    public GameObject addTargetDemoBtn;

    public GameObject gun;
    public GameObject propStand;
    public float propStandMinHeight;
    public float propStandMaxHeight;
    public float propStandAnimationTime;
    public AnimationCurve propStandAnimationCurve;
    public GameObject fetchTrigger;
    public GameObject fetchText;

    public GameObject returnText;
    public GameObject readyTrigger;

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
    public float returningTime;

    /* Timer */
    private float timer = 0f;

    public Transform propStandAnchor;
    public Transform questPropStandAnchor;
    public GameObject readyArea;

    public GameObject aimImage;

    public GameObject putBackTrigger;
    public GameObject putBackText;

    private bool isPropStateMachineRunning;

    public GameObject arenaSign;
    
    private GunScript gunScript;

    protected override void OnAwake()
    {

        Assert.IsNotNull(welcomeText);
        Assert.AreNotApproximatelyEqual(0f, welcomeTextTime);
        
        Assert.IsNotNull(addTargetDemoBtn);

        Assert.IsNotNull(propStand);
        Assert.AreNotApproximatelyEqual(0f, propStandAnimationTime);
        
        Assert.IsNotNull(propStandAnimationCurve);
        Assert.IsNotNull(fetchTrigger);
        Assert.IsNotNull(fetchText);
    
        Assert.IsNotNull(returnText);
        Assert.IsNotNull(readyTrigger);

        Assert.IsNotNull(readyText);
        Assert.IsNotNull(startText);
        Assert.AreNotApproximatelyEqual(0f, readyTextTime);
        Assert.AreNotApproximatelyEqual(0f, startTextTime);
        Assert.IsNotNull(progressBar);

        Assert.IsNotNull(healthBarImage);

        Assert.IsNotNull(finishText);
        Assert.AreNotApproximatelyEqual(0f, finishTextTime);

        Assert.IsNotNull(propStandAnchor);

        Assert.IsNotNull(putBackTrigger);
        Assert.IsNotNull(putBackText);

        Assert.IsNotNull(arenaSign);

        welcomeText.gameObject.SetActive(false);
        addTargetDemoBtn.SetActive(false);
        propStand.SetActive(false);
        fetchTrigger.SetActive(false);
        fetchText.SetActive(false);
        returnText.SetActive(false);
        readyText.gameObject.SetActive(false);
        startText.gameObject.SetActive(false);
        progressBar.gameObject.SetActive(false);
        healthBarImage.gameObject.SetActive(false);
        finishText.gameObject.SetActive(false);

        putBackTrigger.SetActive(false);
        putBackText.SetActive(false);

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

        if(GameManager.instance.gameMode == GameMode.QUEST) {
            //propStand.transform.position = new Vector3(0.15f, -1.5f, 0.15f);
            propStandAnchor = questPropStandAnchor;
            propStand.transform.parent = propStandAnchor;
            DataManager.instance.gun.transform.parent = propStand.transform;
            DataManager.instance.gun.transform.localPosition = Vector3.zero;
            Assert.AreNotApproximatelyEqual(0f, deliveringTime);
            Assert.AreNotApproximatelyEqual(0f, returningTime);
            returningTime = 2f;
        }

        gunScript = DataManager.instance.gun.GetComponent<GunScript>();

    }

    public override void Init() {
        //ReadyZone
        readyArea.SetActive(false);
        gun.gameObject.SetActive(false);
        readyTrigger.SetActive(false);
        propStand.SetActive(false);
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
        TargetMachine.instance.AddTargetDemo();
        isPropStateMachineRunning = true;
        InitDelivering();
        StartCoroutine(UpdatePropState());
    }

    public void OnWaiting() {
        if(!isPropStateMachineRunning) {
            nextClubState = ClubState.READY;
        }
    }

    public void ExitWaiting() {
        addTargetDemoBtn.SetActive(false);
        TargetMachine.instance.DestroyTargetDemos();
        StopCoroutine(UpdatePropState());
    }
    
    public void InitReady() {
        aimImage.SetActive(true);
        readyText.gameObject.SetActive(true);
        progressBar.gameObject.SetActive(true);
        //gun.SetActive(false);
        if(propStand.activeInHierarchy) {
            Sequence propStandDropingSequence = DOTween.Sequence();
            propStandDropingSequence.Append(propStand.transform.DOLocalMoveY(propStandMinHeight, propStandAnimationTime))
                    .SetEase(propStandAnimationCurve);
            propStandDropingSequence.Play();
        }

        progressBar.fillAmount = 0f;
    }

    public void OnReady() {
        if(DataManager.instance.isStartTextShowed[(int)currentClub]) {
            nextClubState = ClubState.GAME;
        }
        else {
        Ray ray = DataManager.instance.playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit)) {
            if(hit.transform.gameObject.name == "GazeTrigger") {
                progressBar.fillAmount += 1f / readyTextTime * Time.deltaTime;
            } else {
                progressBar.fillAmount = 0f;
            }
        }
        if(Mathf.Approximately(1f, progressBar.fillAmount)) {
            DataManager.instance.isReadyTextShowed[(int)currentClub] = true;
        }

        if(readyText.IsActive() && DataManager.instance.isReadyTextShowed[(int)currentClub]) {
            aimImage.SetActive(false);
            readyText.gameObject.SetActive(false);
            progressBar.gameObject.SetActive(false);
            startText.gameObject.SetActive(true);
            StartCoroutine(Timer.StartTimer(startTextTime, ()=>{
                DataManager.instance.isStartTextShowed[(int)currentClub] = true;
            }));
        }
        }
    }

    public void ExitReady() {
        startText.gameObject.SetActive(false);
    }

    public void InitGame() {
        Debug.Log("Init Game");
        DataManager.instance.player.SetActive(true);
        healthBarImage.gameObject.SetActive(true);
        StartCoroutine(TargetMachine.instance.StartShooting());
        if(GameManager.instance.gameMode == GameMode.QUEST) {
            StartCoroutine(gunScript.StartAutoShooting());
        }
    }

    public void OnGame() {
        if(TargetMachine.instance.AllTargetsDie()) {
            nextClubState = ClubState.RESULT;
        }
    }

    public void ExitGame() {
        //DataManager.instance.player.SetActive(false);
        healthBarImage.gameObject.SetActive(false);
        if(GameManager.instance.gameMode == GameMode.QUEST) {
            Debug.Log("stop auto shooting");
            StopCoroutine(gunScript.StartAutoShooting());
        }
        StopCoroutine(TargetMachine.instance.StartShooting());
    }

    public void InitResult() {
        DataManager.instance.isInReadyZone[(int)currentClub] = false;
        arenaSign.SetActive(false);
        finishText.gameObject.SetActive(true);
        StartCoroutine(Timer.StartTimer(finishTextTime, () =>
        {
            finishText.gameObject.SetActive(false);
            currentPropState = PropState.PUTBACK;
            nextPropState = PropState.PUTBACK;
            InitPutBack();
            isPropStateMachineRunning = true;
            StartCoroutine(UpdatePropState());
        }));
    }

    public void OnResult() {
        if(DataManager.instance.isInReadyZone[(int)currentClub] && !isPropStateMachineRunning) {
            arenaSign.SetActive(true);
        }
    }

    public void ExitResult() {
    }

    /* Prop States Methods */

    public void InitDelivering() {
        propStand.transform.parent = propStandAnchor;
        propStand.transform.localPosition = Vector3.zero;
        propStand.transform.localRotation = Quaternion.identity;
        propStand.SetActive(false);

        if(GameManager.instance.gameMode == GameMode.QUEST) {
            StartCoroutine(Timer.StartTimer(deliveringTime, () => {
                DataManager.instance.isDeviceReady[(int)requiredDevice] = true;
            }));
        }
    }

    public void OnDelivering() {
        if(DataManager.instance.isDeviceReady[(int)requiredDevice] && !propStand.activeInHierarchy) {
            Debug.Log("Arrived");
            
            propStand.transform.localPosition = new Vector3(0f, propStandMinHeight, 0f);
            propStand.SetActive(true);
            Sequence propStandRisingSequence = DOTween.Sequence();
            
            propStandRisingSequence.Append(propStand.transform.DOLocalMoveY(propStandMaxHeight, propStandAnimationTime))
                .SetEase(propStandAnimationCurve)
                .AppendCallback(() => {
                    nextPropState = PropState.FETCHING;
                    gun.gameObject.SetActive(true);
                    });
            propStandRisingSequence.Play();
        }
    }

    public void ExitDelivering() {
    }

    public void InitFetching() {
        // fetchArea
        fetchTrigger.SetActive(true);
        fetchText.gameObject.SetActive(true);
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
    }

    public void InitReturning() {
        DataManager.instance.isInReadyZone[(int)currentClub] = false;
        readyArea.SetActive(true);
        returnText.gameObject.SetActive(true);
        readyTrigger.SetActive(true);
        if(GameManager.instance.gameMode == GameMode.QUEST) {
            StartCoroutine(Timer.StartTimer(returningTime, ()=>{
                DataManager.instance.isInReadyZone[(int)currentClub] = true;
            }));
        }
    }

    public void OnReturning() {
        returnText.transform.LookAt(DataManager.instance.playerCamera.transform.position);
        Vector3 tmp = returnText.transform.eulerAngles;
        if (GameManager.instance.gameMode == GameMode.QUEST)
        {
            tmp.x = -tmp.x;
            tmp.y += 180f;
            returnText.transform.eulerAngles = tmp;
        }
        if(DataManager.instance.isInReadyZone[(int)currentClub]) {
            ExitReturning();
        }
    }

    public void ExitReturning() {
        readyArea.SetActive(false);
        returnText.gameObject.SetActive(false);
        readyTrigger.SetActive(false);
        isPropStateMachineRunning = false;
    }

    public void InitPutBack() {
        propStand.transform.localPosition = new Vector3(0f, propStandMinHeight, 0f);
        propStand.SetActive(true);
        Sequence propStandRisingSequence = DOTween.Sequence();

        propStandRisingSequence.Append(propStand.transform.DOLocalMoveY(propStandMaxHeight, propStandAnimationTime))
            .SetEase(propStandAnimationCurve)
            .AppendCallback(() =>
            {
                putBackTrigger.SetActive(true);
                putBackText.gameObject.SetActive(true);
                
            });
        propStandRisingSequence.Play();
    }

    public void OnPutBack() {
        if(DataManager.instance.isPropPutBack[(int)requiredDevice]) {
            Debug.Log("On put back exit");
            ClientSend.ReleaseDevice();
            nextPropState = PropState.RETURNING;
        }
    }

    public void ExitPutBack() {
        Debug.Log("Exit put back");
        putBackTrigger.SetActive(false);
        putBackText.gameObject.SetActive(false);
        Sequence propStandDropingSequence = DOTween.Sequence();
        propStandDropingSequence.Append(propStand.transform.DOLocalMoveY(propStandMinHeight, propStandAnimationTime))
                .SetEase(propStandAnimationCurve)
                .AppendCallback(()=>{ 
                    propStand.SetActive(false);
                });
        propStandDropingSequence.Play();
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

    void Update()
    {
        //UpdateClubState();
    }
}
