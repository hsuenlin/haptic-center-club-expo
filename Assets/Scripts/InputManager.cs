using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InputManager : MonoBehaviour
{
    public InputManager instance;

    // Start is called before the first frame update
    public Transform clubEnumEntry;
    public Transform shootingClubEntry;
    public Transform tennisClubEntry;
    public Transform musicGameClubEntry;
    

    public Scene shootingClubScene;
    public Scene tennisClubScene;
    public Scene musicGameClubScene;

    public enum ClubEnum {
        ClubExpo,
        ShootingClub,
        TennisClub,
        MusicGameClub
    }
    
    public ClubEnum faceTo = ClubEnum.ShootingClub;
    public ClubEnum currentScene = ClubEnum.ClubExpo;

    public Transform[] enumToEntry;

    public GameObject hitObject;

    public void isHit;

    void Awake() {
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
        enumToEntry = new Transform[] {
            clubEnumEntry,
            shootingClubEntry,
            tennisClubEntry,
            musicGameClubEntry
        };
        //DontDestroyOnLoad(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        // TODO: 
        // Pinch raycast
        // Update isHit and Hit
    }
}
