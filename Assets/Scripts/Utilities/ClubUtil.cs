using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ClubUtil : MonoBehaviour {    
    public static GameObject InstantiateOn(GameObject _obj, Transform _transform) {
        GameObject obj = Instantiate(_obj);
        obj.transform.parent = _transform;
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;
        return obj;
    }

    public static void Attach(Transform src, Transform dest)
    {
        src.parent = dest;
        src.localPosition = Vector3.zero;
        src.localRotation = Quaternion.identity;
    }

    public static void Attach(GameObject src, GameObject dest)
    {
        src.transform.parent = dest.transform;
        src.transform.localPosition = Vector3.zero;
        src.transform.localRotation = Quaternion.identity;
    }

    public static void TextLookAt(GameObject obj, GameObject target) {
        obj.transform.LookAt(target.transform.position, Vector3.up);
        
        Vector3 tmp = obj.transform.eulerAngles;
        tmp.x = -tmp.x;
        tmp.y += 180f;
        obj.transform.eulerAngles = tmp;
    }

    public static void SetLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;

        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, layer);
        }
    }
}