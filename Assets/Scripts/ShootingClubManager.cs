// TODO: 
// 1. Refactor
// 2. PropState
// ...

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShootingClubManager : StateSingleton
{
    // Start is called before the first frame update
    public static ShootingClubManager instance;

    public Device requiredDevice;
    
    public ClubState clubState = ClubState.ENTRY;
    public PropState propState = PropState.DELIVERING;

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
        Assert.IsNotNull(requiredDevice);
    }

    void Start()
    {
        trajectory = GetComponent<LineRenderer>();
    }

    public void InitIdle() {
        timer = 0f;
        welcomeText.gameObject.SetActive(true);
    }

    public void OnIdle() {
        if(timer > welcomeTextTime) {
            ExitIdle();
            InitWaiting();
            clubState = ClubState.WAITING;
        }
        timer += Time.deltaTime;
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

    public void InitDelivering() {
        
    }

    public void OnDelivering() {
        
    }

    public void ExitDelivering() {

    }

    public void InitFetching() {
        
    }

    public void OnFetching() {

    }

    public void ExitFetching() {

    }

    public void InitReturning() {

    }

    public void OnReturning() {

    }

    public void ExitReturning() {

    }

    public void InitReady() {
        
    }

    public void OnReady() {

    }

    public void ExitReady() {

    }

    public void OnWaiting() {
            // TODO:
            // SubclubState
            if(timer > 5f) {
                propStand.SetActive(true);
            }
            timer += Time.deltaTime;
            switch(propState) {
                case PropState.DELIVERING:
                    if(DataManager.instance.isDeviceReady[(int)sceneState]) {
                        propState = PropState.FETCHING;
                    } 
                    else { 
                        OnDelivering();
                    }
                    break;
                case PropState.FETCHING:
                    if(DataManager.instance.isPropFetched) {
                        
                    }
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
        //toGameBtn.gameObject.SetActive(false);
        TargetManager.instance.DestroyTargetDemos();
        //canvas.GetComponent<GraphicRaycaster>().enabled = false;
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

    public void ChangeClubState(SceneState nextState) {
        
    }

    public void ChangePropState(SceneState nextState) {

    }

    // Update is called once per frame
    void Update()
    {
        switch(clubState) {
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
