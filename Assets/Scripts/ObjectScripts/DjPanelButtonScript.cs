using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DjPanelButtonScript : MonoBehaviour {
    public Half half;
    public DjPanelGame dpGame;
    //public Half activatedHalf;
    public AudioClip missSound;
    public AudioClip goodSound;
    public AudioClip perfectSound;
    public Animation missAnimation;
    public Animation goodAnimation;
    public Animation perfectAnimation;
    public AudioSource audioSource;
    public GameObject missText;
    public GameObject goodText;
    public GameObject perfectText;

    public float textRiseHeight;
    public float textRiseTime;
    public float textFadeTime;

    private Dictionary<HitType, AudioClip> hitSoundDict;
    private Dictionary<HitType, Animation> hitAnimationDict;
    private Dictionary<HitType, GameObject> hitTextDict;
    public void Init(DjPanelGame _dpGame, DjPanelScript _djPanel, Transform _parent, Half _half) {
        dpGame = _dpGame;

        half = _half;
        missSound = dpGame.missSound;
        goodSound = dpGame.goodSound;
        perfectSound = dpGame.perfectSound;
        missAnimation = dpGame.missAnimation;
        goodAnimation = dpGame.goodAnimation;
        perfectAnimation = dpGame.perfectAnimation;
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.dopplerLevel = 0f;
        missText = ClubUtil.InstantiateOn(dpGame.missText, dpGame.buttonAnchorDict[_half].transform);
        goodText = ClubUtil.InstantiateOn(dpGame.goodText, dpGame.buttonAnchorDict[_half].transform);
        perfectText = ClubUtil.InstantiateOn(dpGame.perfectText, dpGame.buttonAnchorDict[_half].transform);

        missText.SetActive(false);
        goodText.SetActive(false);
        perfectText.SetActive(false);

        textRiseHeight = dpGame.textRiseHeight;
        textRiseTime = dpGame.textRiseTime;
        textFadeTime = dpGame.textFadeTime;
        
        hitSoundDict = new Dictionary<HitType, AudioClip>() {
            {HitType.MISS, missSound},
            {HitType.GOOD, goodSound},
            {HitType.PERFECT, perfectSound}
        };

        hitAnimationDict = new Dictionary<HitType, Animation>() {
            {HitType.MISS, missAnimation},
            {HitType.GOOD, goodAnimation},
            {HitType.PERFECT, perfectAnimation}
        };

        hitTextDict = new Dictionary<HitType, GameObject>() {
            {HitType.MISS, missText},
            {HitType.GOOD, goodText},
            {HitType.PERFECT, perfectText}
        };

        int idx = _half == Half.LEFT ? 0 : 1;
        _djPanel.OnButtonPressDown[idx] = OnPressDown;
    }
    void PlayHitTextAnimation(GameObject text) {
        Debug.Log("Text Start");
        text.SetActive(true);
        text.transform.parent = dpGame.djPanelButtonHintDict[half].transform.parent;
        text.transform.localPosition = Vector3.zero;
        text.transform.localRotation = Quaternion.identity;
        text.GetComponent<Renderer>().material.DOFade(1f, 0f);
        Sequence hitSeq = DOTween.Sequence();
        hitSeq.Append(text.transform.DOLocalMoveY(textRiseHeight, textRiseTime))
              .Append(text.GetComponent<Renderer>().material.DOFade(0f, textFadeTime))
              .AppendCallback(()=>{ text.SetActive(false); });
        hitSeq.Play();
        Debug.Log("Text End");
    }
    
    private void OnPressDown() {
        Debug.Log("Press");
        HitType hitResult = dpGame.JudgeHit(half);
        audioSource.clip = hitSoundDict[hitResult];
        audioSource.PlayOneShot(hitSoundDict[hitResult]);
        //hitAnimationDict[hitResult].Play(); // May not work
        PlayHitTextAnimation(hitTextDict[hitResult]);
    }
}