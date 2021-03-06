using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingClubManager : MonoBehaviour
{
    // Start is called before the first frame update
    public static ShootingClubManager instance;
    public enum ClubState
    {
        ENTRY = -1,
        IDLE = 0,
        WAITING = 1,
        GAME = 2,
        RESULT = 3
    };

    public ClubState clubState = ClubState.ENTRY;

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
    public Image healthBarImage;
    public Image aim;
    public Text welcomeText;
    public Text readyText;
    public Text startText;
    public Text finishText;
    private float readyTextTime = 1f;
    private float startTextTime = 1.75f;
    private float welcomTextTime = 3f;
    private float minToArenaTime = 1f;
    public GameObject toGameBtn;
    public GameObject toArenaBtn;
    public GameObject addTargetDemoBtn;

    /* Timer */
    private float timer = 0f;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    void Start()
    {
        trajectory = GetComponent<LineRenderer>();
    }

    private void InitIdle() {
        timer = 0f;
        welcomeText.gameObject.SetActive(true);
    }

    private void ExitIdle() {
        welcomeText.gameObject.SetActive(false);
    }

    private void InitWaiting() {
        GameManager.instance.OnDeviceReady += () => { toGameBtn.SetActive(true); };
    }

    private void ExitWaiting() {
        isGunIndicatorHit = false;
    }

    private void InitGame() {
        timer = 0f;
        readyText.gameObject.SetActive(true);
    }

    private void ExitGame() {
        healthBarImage.gameObject.SetActive(false);
        aim.gameObject.SetActive(false);
        GameManager.instance.OnTriggered -= TriggerGun;
    }

    private void InitResult() {
        timer = 0f;
        isGunIndicatorHit = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(state == ClubState.ENTRY) {
            InitIdle();
            state = ClubState.IDLE;
        }
        if(state == ClubState.IDLE) {
            // TODO: Add welcome text to scene
            if(timer > welcomeTextTime) {
                ExitIdle();
                InitWaiting();
                state = ClubState.WAITING;
            }
            timer += Time.deltaTime;
        } else if(state == ClubState.WAITING) {
            if(InputManager.instance.isHit) {
                GameObject hit = InputManager.instance.hitObject;
                if(hit == toGameBtn) {
                    ExitWaiting();
                    InitGame();
                    state = ClubState.GAME;
                } else if(hit == addTargetDemoBtn) {
                    TargetManager.instance.AddTargetDemo();
                    TargetManager.instance.UpdateHandbook();
                }
            }
            
            // Change to GAME state when player pinches on the Gun indicator. 
            // InitGame(): Activate TargetMachine
        } else if(state == ClubState.GAME) {
            if(gameStart) {          
                if(TargetManager.instance.AllTargetsDie()) {
                    ExitGame();
                    InitResult();
                    state = ClubState.RESULT;
                }
                if(TargerManager.instance.AllRisingComplete()) {
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
        } else if(state == ClubState.RESULT) {
            // TODO:
            // Show the shooting result.
            if(timer >= minToArenaTime) {
                toArenaBtn.SetActive(true);
            }
            if(InputManager.instance.isHit) {
                GameObject hit = InputManager.instance.hitObject;
                if(hit == toArenaBtn) {
                    GameManager.ChangeSceneTo(SceneState.ARENA);
                }
            }
            timer += Time.deltaTime;
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
