using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using DG.Tweening;

public class PropSupport : MonoBehaviour {
    
    public float minHeight;
    public float maxHeight;

    public float animationTime;
    
    public AnimationCurve curve;
    public void Rise(TweenCallback CallBack) {
        Sequence seq = DOTween.Sequence();
        seq.Append(gameObject.transform.DOLocalMoveY(maxHeight, animationTime))
            .SetEase(curve)
            .AppendCallback(CallBack);
        seq.Play();
    }

    public void Drop(TweenCallback CallBack) {
        Sequence seq = DOTween.Sequence();
        seq.Append(gameObject.transform.DOLocalMoveY(minHeight, animationTime))
            .SetEase(curve)
            .AppendCallback(CallBack);
        seq.Play();
    }
}