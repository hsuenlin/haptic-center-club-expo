using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingClubManager : MonoBehaviour
{
    // Start is called before the first frame update
    public static ShootingClubManager instance;
    public GameObject gun;
    public Transform muzzle;
    public Vector3 cameraCenter;
    public float gunColdDownTime = 0.25f;
    public float shootingRange = 100f;
    private float timer = 0f;
    private LineRenderer trajectory;
    
    private WaitForSeconds shotDuration = new WaitForSeconds(.07f);

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
    
    public void TriggerGun() {
        if(timer >= gunColdDownTime) {
            StartCoroutine(ShotEffect());
            RaycastHit hit;
            if(Physics.Raycast(cameraCenter, Camera.main.transform.forward, out hit, shootingRange)) {
                Debug.Log(hit.collider.gameObject);
                //trajectory.SetPosition(0, muzzle.position);
                //trajectory.SetPosition(1, hit.point);
                if(hit.collider.gameObject.tag == "Target") {
                    Destroy(hit.collider.gameObject);
                }
            }
        }
    }
    
    void Start()
    {
        cameraCenter = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0f));
        trajectory = GetComponent<LineRenderer>();
        timer = gunColdDownTime + 1;
    }

    // Update is called once per frame
    void Update()
    {
        if(timer < gunColdDownTime) {
            timer += Time.deltaTime;
        }

        if(Input.GetKeyDown(KeyCode.X)) {
            TriggerGun(); // 0.2105 0.717 0.5
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
