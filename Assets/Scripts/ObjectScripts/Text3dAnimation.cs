using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Text3dAnimation : MonoBehaviour {
    public float riseY;
    public float riseTime;
    public AnimationCurve curve;
    
    public void RiseFadeInOut(TweenCallback CallBack) {
        gameObject.transform.DOLocalMoveY(gameObject.transform.localPosition.y - riseY, 0f);
        Sequence seq = DOTween.Sequence();
        gameObject.transform.GetComponent<Renderer>().material.DOFade(0f, 0f);
        gameObject.transform.GetComponent<Renderer>().material.DOFade(1f, riseTime);
        seq.Append(gameObject.transform.DOLocalMoveY(riseY, riseTime))
           .Append(gameObject.transform.GetComponent<Renderer>().material.DOFade(0f, 2*riseTime))
           .SetEase(curve)
           .AppendCallback(CallBack);
        seq.Play();
    }
}