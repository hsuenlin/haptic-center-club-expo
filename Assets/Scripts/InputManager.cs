using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InputManager : MonoBehaviour
{
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
        if(Input.GetKeyDown(KeyCode.R)) {
            Camera.main.transform.LookAt(shootingClubEntry);
            Debug.Log("Face to shooting club.");
            faceTo = ClubEnum.ShootingClub;
        }
        else if(Input.GetKeyDown(KeyCode.G)) {
            Camera.main.transform.LookAt(tennisClubEntry);
            Debug.Log("Face to tennis club.");
            faceTo = ClubEnum.TennisClub;
        }
        else if(Input.GetKeyDown(KeyCode.B)) {
            Camera.main.transform.LookAt(musicGameClubEntry);
            Debug.Log("Face to music game club.");
            faceTo = ClubEnum.MusicGameClub;
        }
        else if(Input.GetKeyDown(KeyCode.Space)) {
            currentScene = faceTo;
            Debug.Log("Go to " + faceTo.ToString());
            foreach(GameObject g in SceneManager.GetActiveScene().GetRootGameObjects()){
                if(g.name != "InputManager") {
                    g.SetActive (false);
                }
            }
            SceneManager.LoadScene((int)faceTo, LoadSceneMode.Additive);
        }
        else if(Input.GetKeyDown(KeyCode.Backspace)) {
            if(currentScene != ClubEnum.ClubExpo) {
                SceneManager.UnloadSceneAsync((int)currentScene);
                foreach(GameObject g in SceneManager.GetActiveScene().GetRootGameObjects()){
                    g.SetActive (true);
                }
                faceTo = currentScene;
                currentScene = ClubEnum.ClubExpo;
                Camera.main.transform.LookAt(enumToEntry[(int)faceTo]);
            }
        }
    }
}
