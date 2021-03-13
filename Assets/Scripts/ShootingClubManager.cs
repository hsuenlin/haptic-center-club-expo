using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShootingClubManager : StateSingleton
{
    
    public SceneState currentClub;
    public Device requiredDevice;
    
    public ClubState currentClubState;
    public PropState currentPropState;

    public ClubState nextClubState;
    public PropState nextPropState;

    private Dictionary<ClubState, Action> clubStateInits;
    private Dictionary<ClubState, Action> clubStateExits;

    private Dictionary<ClubState, Action> propStateInits;
    private Dictionary<ClubState, Action> propStateExits;

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
    public Text finishTextTime; // 2f

    /* Variables for Quest mode */
    public float deliveringTime;

    /* Timer */
    private float timer = 0f;

    void OnAwake()
    {
        Assert.IsNotNull(currentClub);
        Assert.IsNotNull(requiredDevice);

        Assert.IsNotNull(currentClubState);
        Assert.IsNotNull(currentPropState);
        
        Assert.IsNotNull(nextClubState);
        Assert.IsNotNull(nextPropState);

        Assert.IsNotNull(welcomeText);
        Assert.IsNotNull(welcomeTextTime);

        Assert.IsNotNull(propStand);
        Assert.IsNotNull(propStandMinHeight);
        Assert.IsNotNull(propStandMaxHeight);
        Assert.IsNotNull(propStandAnimationTime);
        Assert.IsNotNull(propStandAnimationCurve);
        Assert.IsNotNull(fetchTrigger);
        Assert.IsNotNull(fetchText);
    
        Assert.IsNotNull(returnText);
        Assert.IsNotNull(readyPositionIndicator);

        Assert.IsNotNull(readyText);
        Assert.IsNotNull(startText);
        Assert.IsNotNull(readyTextTime);
        Assert.IsNotNull(startTextTime);
        Assert.IsNotNull(progressBar);

        Assert.IsNotNull(healthBarImage);

        Assert.IsNotNull(finishText);
        Assert.IsNotNull(finishTextTime);

        if(GameManager.instance.gameMode == GameMode.QUEST) {
            Assert.IsNotNull(deliveringTime);
        }
        
        clubStateInits = new Dictionary<ClubState, Action>() {
            {ClubState.IDLE, ()=>{ InitIdle(); }},
            {ClubState.WAITING, ()=>{ InitWaiting(); }},
            {ClubState.GAME, ()=>{ InitGame(); }},
            {ClubState.RESULT, ()=>{ InitResult(); }}
        };
        clubStateExits = new Dictionary<ClubState, Action>() {
            {ClubState.IDLE, ()=>{ ExitIdle(); }},
            {ClubState.WAITING, ()=>{ ExitWaiting(); }},
            {ClubState.GAME, ()=>{ ExitGame(); }},
            {ClubState.RESULT, ()=>{ ExitResult(); }}
        };
        propStateInits = new Dictionary<PropState, Action>() {
            {PropState.DELIVERING, ()=>{ InitDelivering(); }},
            {PropState.FETCHING, ()=>{ InitFetching(); }},
            {PropState.RETURNING, ()=>{ InitReturning(); }},
            {PropState.READY, ()=>{ InitReady(); }}
        };
        propStateExits = new Dictionary<PropState, Action>() {
            {PropState.DELIVERING, ()=>{ ExitDelivering(); }},
            {PropState.FETCHING, ()=>{ ExitFetching(); }},
            {PropState.RETURNING, ()=>{ ExitReturning(); }},
            {PropState.READY, ()=>{ ExitReady(); }}
        };
    }

    /* Club States Methods */

    public void InitIdle() {
        timer = 0f;
        welcomeText.gameObject.SetActive(true);
        StartCoroutine(StartTimer(welcomTextTime, ()=>{ nextClubState = ClubState.WAITING; }));
    }

    public void OnIdle() {
    }

    public void ExitIdle() {
        welcomeText.gameObject.SetActive(false);
    }

    public void InitWaiting() {
        timer = 0f;
        addTargetDemoBtn.SetActive(true);
        TargetManager.instance.AddTargetDemo();
    }

    public void OnWaiting() {
        UpdatePropState();
    }

    public void ExitWaiting() {
        addTargetDemoBtn.SetActive(false);
        TargetManager.instance.DestroyTargetDemos();
    }
    
    public void InitReady() {
        readyText.SetActive();
        progressBar.fillAmount = 0f;
    }

    public void OnReady() {
        Ray ray = DataManager.instance.playerCamera.ViewportPortToRay(new Vector3(0.5f, 0.5f, 0f));
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
            readyText.SetActive(false);
            startText.SetActive(true);
        }
        if(startText.IsActive() && DataManager.instance.isStartTextShowed[(int)currentClub]) {
            nextClubState = ClubState.GAME;
        }
    }

    public void ExitReady() {
        startText.SetActive(false);
    }

    public void InitGame() {
        healthBarImage.gameObject.SetActive(true);
    }

    public void OnGame() {
        // TODO:
        // 1. Shoot with pinch
        // 2. Shoot with gun
        if(TargetManager.instance.AllTargetsDie()) {
            nextClubState = ClubState.RESULT;
        }
        if(TargetManager.instance.AllRisingComplete()) {
            TargetManager.instance.GetReady();
        }
    }

    public void ExitGame() {
        healthBarImage.gameObject.SetActive(false);
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
            StartCoroutine(StartTimer(deliveringTime, () => {
                DataManager.instance.isDeviceReady[(int)requiredDevice] = true;
            }));
        }
    }

    public void OnDelivering() {
        if(DataManager.instance.isDeviceReady[(int)requiredDevice] && !propStand.activeInHierarchy) {
            propStand.SetActive(true);
            DOTween propStandRisingSequence = new DOTween.Sequence();
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
        fetchText.SetActive(true);
    }

    public void OnFetching() {
        if(DataManager.instance.isDeviceFetched[(int)requiredDevice]) {
            nextPropState = PropState.RETURNING;
        }
        fetchText.transform.LookAt(DataManager.instance.playerCamera);
    }

    public void ExitFetching() {
        fetchTrigger.SetActive(false);
        fetchText.SetActive(false);
        DOTween propStandDropingSequence = new DOTween.Sequence();
        propStandDropingSequence.Append(propStand.transform.DOMoveY(propStandMinHeight, propStandAnimationTime))
                .SetEase(propStandAnimationCurve));
        propStandDropingSequence.Play();
    }

    public void InitReturning() {
        returnText.SetActive(true);
        readyPositionIndicator.SetActive(true);
    }

    public void OnReturning() {
        if(DataManager.instance.isPlayerReady[(int)currentClub]) {
            ExitReturning();
            nextClubState = ClubState.READY;
        }
    }

    public void ExitReturning() {
        returnText.SetActive(false);
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
        if(nextClubState == null) {
            clubStateExits[currentClubState]();
        }
        else if(currentClubState != nextClubState) {
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
