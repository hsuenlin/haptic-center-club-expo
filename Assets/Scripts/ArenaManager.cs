using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaManager : MonoBehaviour
{

    public Transform shootingClubEntry;
    public Transform tennisClubEntry;
    public Transform musicGameClubEntry;
    public SceneState faceTo = SceneState.SHOOTING_CLUB;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Camera.main.transform.LookAt(shootingClubEntry);
            Debug.Log("Face to shooting club.");
            faceTo = SceneState.SHOOTING_CLUB;
        }
        else if (Input.GetKeyDown(KeyCode.G))
        {
            Camera.main.transform.LookAt(tennisClubEntry);
            Debug.Log("Face to tennis club.");
            faceTo = SceneState.TENNIS_CLUB;
        }
        else if (Input.GetKeyDown(KeyCode.B))
        {
            Camera.main.transform.LookAt(musicGameClubEntry);
            Debug.Log("Face to music game club.");
            faceTo = SceneState.MUSICGAME_CLUB;
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            GameManager.instance.ChangeSceneTo(faceTo);
            Debug.Log("Go to " + faceTo.ToString());
        }
    }
}
