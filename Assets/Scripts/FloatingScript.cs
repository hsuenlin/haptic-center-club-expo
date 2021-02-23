using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FloatingScript : MonoBehaviour
{

    public float rotationFrequency = 1f;
    public float bouncingFrequency = 1f;
    public float bouncingRange = 0.15f;

    // Start is called before the first frame update
    void Start()
    {
        DOTween.SetTweensCapacity(2000, 100);
        Sequence upAndDown = DOTween.Sequence();
        upAndDown.Append(transform.DOLocalMoveY(transform.position.y + bouncingRange, 1.25f).SetEase(Ease.InOutSine))
            .Append(transform.DOLocalMoveY(transform.position.y, 1.25f).SetEase(Ease.InOutSine))
            .SetLoops(-1);
        upAndDown.Play();
        Sequence infRotation = DOTween.Sequence();
        infRotation.Append(transform.DORotate(new Vector3(0, 360, 0), 2.5f, RotateMode.WorldAxisAdd).SetEase(Ease.Linear))
            .SetLoops(-1);
        infRotation.Play();
    }

    // Update is called once per frame

    void GoUp() {
        transform.DOLocalMoveY(1, 1).OnComplete(() => {GoDown();});
    }
    void GoDown() {
        transform.DOLocalMoveY(0, 1).OnComplete(() => { GoUp(); });
    }
    void Update()
    {
        
    }
}
