using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TennisSenpai : MonoBehaviour
{
    // Start is called before the first frame update
    public Vector3 startingPosition;
    public Quaternion startingRotation;

    void Start() {
        startingPosition = transform.position;
        startingRotation = transform.rotation;
    }
    void ResetTennisSenpai() {
        transform.position = startingPosition;
        transform.rotation = startingRotation;
    }
}
