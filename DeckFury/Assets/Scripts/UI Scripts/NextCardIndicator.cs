using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class NextCardIndicator : MonoBehaviour
{
    [SerializeField] PlayerController player;//Must be set in inspector
    PlayerCardManager playerCardManager;

    [SerializeField] SpriteRenderer _firstCardSprite;
    [SerializeField] SpriteRenderer _secondCardSprite;

    SpriteRenderer currentVisibleCard;

    [Header("Tween Properties")]
    [SerializeField] float _tweenDuration = 0.15f;
    [SerializeField] Ease _tweenEase;

    Tween currentCardTween;
    Tween nextCardTween;

    Tween currentCardScaleTween;
    Tween nextCardScaleTween;

    void Awake()
    {
        playerCardManager = player.GetComponent<PlayerCardManager>();
        playerCardManager.OnLoadMagazine += UpdateCardImage;
        playerCardManager.OnRemoveCard += CycleImage;

    }
    

    void Start()
    {
        _firstCardSprite.enabled = false;
    }

    void CycleImage()
    {
        if(currentCardTween.IsActive()){ currentCardTween.Complete(); }
        if(nextCardTween.IsActive()){ nextCardTween.Complete(); }
        if(currentCardScaleTween.IsActive()){ currentCardScaleTween.Complete(); }
        if(nextCardScaleTween.IsActive()){ nextCardScaleTween.Complete(); }

        currentCardTween = currentVisibleCard.gameObject.transform.DOLocalMoveY(-5, _tweenDuration).SetUpdate(true).SetEase(_tweenEase);
        currentCardScaleTween = currentVisibleCard.gameObject.transform.DOScale(0.9f, 0.1f).SetUpdate(true).SetEase(Ease.InOutSine);


        if(playerCardManager.MagazineIsEmpty()){ return; }

        SpriteRenderer nextVisibleCard = currentVisibleCard == _firstCardSprite ? _secondCardSprite : _firstCardSprite;
        nextVisibleCard.sprite = playerCardManager.CardMagazine[0].cardSO.GetCardImage();


        nextCardTween = nextVisibleCard.gameObject.transform.DOLocalMoveY(0, _tweenDuration).SetEase(_tweenEase).SetUpdate(true).OnComplete(
            () =>
                {
                    nextCardScaleTween = nextVisibleCard.gameObject.transform.DOScale(1.1f, 0.1f).SetEase(Ease.InOutSine).SetUpdate(true);
                    currentVisibleCard.transform.localPosition = new Vector3(0, 5, 0);
                    currentVisibleCard = nextVisibleCard;
                });



    }


    void UpdateCardImage()
    {
        if(playerCardManager.MagazineIsEmpty()){ return; }

        _firstCardSprite.sprite = playerCardManager.CardMagazine[0].cardSO.GetCardImage();

        if (playerCardManager.CardMagazine.Count > 1)
        {
            _secondCardSprite.sprite = playerCardManager.CardMagazine[1].cardSO.GetCardImage();
        }

        currentVisibleCard = _firstCardSprite;
        currentVisibleCard.transform.localPosition = new Vector3(0, 0, 0);
        _secondCardSprite.transform.localPosition = new Vector3(0, 5, 0);

        _firstCardSprite.enabled = true;
        _secondCardSprite.enabled = true;
    }

}
