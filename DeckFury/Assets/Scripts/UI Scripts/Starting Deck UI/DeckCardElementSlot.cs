using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeckCardElementSlot : MonoBehaviour
{
    public event Action<DeckCardElementSlot> OnHoverEvent;

    [SerializeField] CardSO _currentCard;
    public CardSO CurrentCard { get => _currentCard; }
    [SerializeField] Image cardFrame;
    [SerializeField] Image cardImage;
    [SerializeField] TextMeshProUGUI cardCountText;

    [Header("Tween Settings")]
    [SerializeField] float hoverEndScale = 1;
    float originalScale;
    [SerializeField] float scaleForwardSpeed = 0.2f;
    [SerializeField] float scaleBackwardSpeed = 0.2f;
    [SerializeField] Ease scaleForwardEase = Ease.OutBack;
    [SerializeField] Ease scaleBackwardEase = Ease.OutBack;

    //Tweens
    Tween scaleForwardTween;
    Tween scaleBackwardTween;

    void Awake()
    {
        originalScale = transform.localScale.x;
    }


    public void AssignDeckElement(DeckElement deckElement)
    {
        _currentCard = deckElement.card;
        cardImage.sprite = _currentCard.GetCardImage();
        cardCountText.text = deckElement.cardCount.ToString();
        ChangeColorBasedOnCardTier(_currentCard.GetCardTier());
    }


    public void ChangeColorBasedOnCardTier(int cardTier)
    {
        cardFrame.color = cardTier switch
        {
            1 => Color.grey,
            2 => Color.cyan,
            3 => Color.yellow,
            _ => Color.grey,
        };
    }

    public void OnHover()
    {
        OnHoverEvent?.Invoke(this);
        ScaleForward();
    }

    public void ExitHover()
    {
        ScaleBackward();
    }

    public void OnClick()
    {
        ClickDeckElementEvent clickDeckElementEvent = new() { deckCardElementSlot = this};
        EventBus<ClickDeckElementEvent>.Raise(clickDeckElementEvent);
    }


    void ScaleForward()
    {
        if(scaleBackwardTween.IsActive())
            scaleBackwardTween.Kill();

        scaleForwardTween = transform.DOScale(hoverEndScale, scaleForwardSpeed).SetEase(scaleForwardEase);
    }

    void ScaleBackward()
    {
        if(scaleForwardTween.IsActive())
            scaleForwardTween.Kill();

        scaleBackwardTween = transform.DOScale(originalScale, scaleBackwardSpeed).SetEase(scaleBackwardEase);
    }





}
