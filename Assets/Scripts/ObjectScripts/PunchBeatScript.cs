using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PunchBeatScript : MonoBehaviour {
    public PunchBeatGame pbGame;
    public Half activatedHalf;
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
    public void Init(PunchBeatGame _pbGame, Transform _parent) {
        pbGame = _pbGame;

        activatedHalf = pbGame.GetActivatedHalf();
        missSound = pbGame.missSound;
        goodSound = pbGame.goodSound;
        perfectSound = pbGame.perfectSound;
        missAnimation = pbGame.missAnimation;
        goodAnimation = pbGame.goodAnimation;
        perfectAnimation = pbGame.perfectAnimation;
        audioSource = new AudioSource();
        missText = ClubUtil.InstantiateOn(pbGame.missText, gameObject.transform);
        goodText = ClubUtil.InstantiateOn(pbGame.goodText, gameObject.transform);
        perfectText = Instantiate(pbGame.perfectText, gameObject.transform);

        textRiseHeight = pbGame.textRiseHeight;
        textRiseTime = pbGame.textRiseTime;
        textFadeTime = pbGame.textFadeTime;
        
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
    }
    void PlayHitTextAnimation(GameObject text) {
        text.transform.parent = pbGame.punchBeatDict[activatedHalf].transform;
        text.transform.localPosition = Vector3.zero;
        text.transform.localRotation = Quaternion.identity;
        text.GetComponent<Renderer>().material.DOFade(1f, 0f);
        text.SetActive(true);
        Sequence hitSeq = DOTween.Sequence();
        hitSeq.Append(text.transform.DOLocalMoveY(textRiseHeight, textRiseTime))
              .Append(text.GetComponent<Renderer>().material.DOFade(0f, textFadeTime));
        hitSeq.Play();
    }
    
    void OnTriggerEnter(Collider other) {
        if(other.tag == "Hand") {
            HitType hitResult = pbGame.JudgeHit();
            audioSource.PlayOneShot(hitSoundDict[hitResult]);
            //hitAnimationDict[hitResult].Play(); // May not work
            PlayHitTextAnimation(hitTextDict[hitResult]);
        }
    }
    
}