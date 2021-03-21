using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ClubUtil : MonoBehaviour {    
    public static GameObject InstantiateOn(GameObject _obj, Transform _transform) {
        GameObject obj = Instantiate(_obj);
        obj.transform.parent = _transform;
        obj.transform.position = Vector3.zero;
        obj.transform.rotation = Quaternion.identity;
        return obj;
    }
}