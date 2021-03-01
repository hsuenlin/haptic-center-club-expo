using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackerManager : MonoBehaviour
{
    public Dictionary<int, Transform> Trackers;
    public Transform Tracker1Transform;
    public Transform Tracker2Transform;

    public static TrackerManager instance;
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(instance);
        }
        instance = this;
        Trackers = new Dictionary<int, Transform>();
        Trackers.Add(1, Tracker1Transform);
        Trackers.Add(2, Tracker2Transform);

    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
