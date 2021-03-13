

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

    public GameObject propStand;
    public float propStandMinHeight;
    public float propStandMaxHeight;
    public float propStandAnimationTime;
    public AnimationCurve propStandAnimationCurve;
    
    public GameObject fetchTrigger;
    public Text fetchText;

    public Text returnText;
    public GameObject readyPositionIndicator;

    /* Variables for Quest mode */
    public float deliveringTime;
    


    public Transform root;
    public GameObject gun;
    public Transform muzzle;
    public Vector3 cameraCenter;
    public float gunColdDownTime = 0.25f;
    public float shootingRange = 100f;
    private LineRenderer trajectory;
    private WaitForSeconds shotDuration = new WaitForSeconds(.07f);

    private bool gameStart = false;

    /* UI */
    public GameObject canvas;
    public Image healthBarImage;
    public Image aim;
    public Text welcomeText;
    public Text readyText;
    public Text startText;
    public Text finishText;
    private float readyTextTime = 1f;
    private float startTextTime = 1.75f;
    private float welcomeTextTime = 3f;
    private float minToArenaTime = 1f;
    public Button toGameBtn;
    public Button toArenaBtn;
    public GameObject addTargetDemoBtn;
    public GameObject propStand;

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

        Assert.IsNotNull(propStand);
        Assert.IsNotNull(propStandMinHeight);
        Assert.IsNotNull(propStandMaxHeight);
        Assert.IsNotNull(propStandAnimationTime);
        Assert.IsNotNull(propStandAnimationCurve);
        Assert.IsNotNull(fetchTrigger);
        Assert.IsNotNull(fetchText);
    
        Assert.IsNotNull(returnText);
        Assert.IsNotNull(readyPositionIndicator);

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

    void Start()
    {
        trajectory = GetComponent<LineRenderer>();
    }


    /* Club States */

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
        //canvas.GetComponent<GraphicRaycaster>().enabled = true;
        
        //TargetManager.instance.UpdateHandbook();
    }

    public void OnWaiting() {
            // TODO:
            // SubclubState
            UpdatePropState();
            /*
            if(InputManager.instance.isHit) {
                GameObject hit = InputManager.instance.hitObject;
                if(hit == toGameBtn.gameObject) {
                    ExitWaiting();
                    InitGame();
                    clubState = ClubState.GAME;
                } else if(hit == addTargetDemoBtn.gameObject) {
                    TargetManager.instance.AddTargetDemo();
                    TargetManager.instance.UpdateHandbook();
                }
            }
            */
            
            // Change to GAME clubState when player pinches on the Gun indicator. 
            // InitGame(): Activate TargetMachine
    }

    public void ExitWaiting() {
        addTargetDemoBtn.SetActive(false);
        TargetManager.instance.DestroyTargetDemos();
    }
    
    public void InitReady() {
        // Activate ready text
        // Activate aim at head center
        // Activate 
    }

    public void OnReady() {

    }

    public void ExitReady() {

    }

    public void InitGame() {
        timer = 0f;
        readyText.gameObject.SetActive(true);
    }

    public void OnGame() {
        if(gameStart) {          
            if(TargetManager.instance.AllTargetsDie()) {
                ExitGame();
                InitResult();
                clubState = ClubState.RESULT;
            }
            if(TargetManager.instance.AllRisingComplete()) {
                TargetManager.instance.GetReady();
            }
        } else {
            if (timer >= readyTextTime && timer < startTextTime)
            {
                readyText.gameObject.SetActive(false);
                startText.gameObject.SetActive(true);
                
            }
            else if (timer >= startTextTime)
            {
                startText.gameObject.SetActive(false);
                healthBarImage.gameObject.SetActive(true);
                //aim.gameObject.SetActive(true);
                GameManager.instance.OnTriggered += TriggerGun;
                gameStart = true;
            }
        }
        timer += Time.deltaTime;
    }

    public void ExitGame() {
        healthBarImage.gameObject.SetActive(false);
        aim.gameObject.SetActive(false);
        GameManager.instance.OnTriggered -= TriggerGun;
    }

    public void InitResult() {
        timer = 0f;
    }

    public void OnResult() {
        if(timer >= minToArenaTime) {
            toArenaBtn.gameObject.SetActive(true);
            canvas.GetComponent<GraphicRaycaster>().enabled = true;
        }
        if(InputManager.instance.isHit) {
            GameObject hit = InputManager.instance.hitObject;
            if(hit == toArenaBtn.gameObject) {
                GameManager.instance.ChangeSceneTo(SceneState.ARENA);
            }
        }
        timer += Time.deltaTime;
    }

    public void ExitResult() {

    }

    /* Prop States */

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
        // Setup trigger to detect whether the prop is fetched.
        // Show hint "Come and fetch me."
        fetchTrigger.SetActive(true);
        fetchText.SetActive(true);
    }

    public void OnFetching() {
        // TODO:
        // Whether the prop is fetched
        if(DataManager.instance.isDeviceFetched[(int)requiredDevice]) {
            nextPropState = PropState.RETURNING;
        }
        fetchText.transform.LookAt(DataManager.instance.playerCamera);
    }

    public void ExitFetching() {
        // TODO:
        // Deactivate trigger
        // Decativate hint
        fetchTrigger.SetActive(false);
        fetchText.SetActive(false);
        DOTween propStandDropingSequence = new DOTween.Sequence();
        propStandDropingSequence.Append(propStand.transform.DOMoveY(propStandMinHeight, propStandAnimationTime))
                .SetEase(propStandAnimationCurve));
        propStandDropingSequence.Play();
    }

    public void InitReturning() {
        // TODO:
        // Activate hint "Please go back to the ready position"
        // Activate hint on ready position
        // Setup trigger on ready position 
        returnText.SetActive(true);
        readyPositionIndicator.SetActive(true);
    }

    public void OnReturning() {
        if(DataManager.instance.isPlayerReady[(int)currentClub]) {
            ExitReturning();
            nextClubState = ClubState.GAME;
        }
    }

    public void ExitReturning() {
        returnText.SetActive(false);
        readyPositionIndicator.SetActive(false);
        // Deactivate hint text
        // Deactivate hint on floor
        // Deactivate trigger

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
            case PropState.READY:
                OnReady();
                break;
            default:
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
 
    }

    public void TriggerGun()
    {
        // TODO: PC -> Quest
        //StartCoroutine(ShotEffect());
        RaycastHit hit;
        if (Physics.Raycast(cameraCenter, Camera.main.transform.forward, out hit, shootingRange))
        {
            //trajectory.SetPosition(0, muzzle.position);
            //trajectory.SetPosition(1, hit.point);
            if (hit.collider.gameObject.tag == "Target")
            {
                if (hit.collider.gameObject.activeInHierarchy)
                {
                    TargetManager.instance.KillTarget(hit.collider.gameObject);
                    //Destroy(hit.collider.gameObject);
                }
            }
        }
    }
    private IEnumerator ShotEffect()
    {

        // Turn on our line renderer
        trajectory.enabled = true;

        //Wait for .07 seconds
        yield return shotDuration;

        // Deactivate our line renderer after waiting
        trajectory.enabled = false;
    }
}
