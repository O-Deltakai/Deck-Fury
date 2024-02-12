using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls the visual indicator slot for a currently loaded card
/// </summary>
public class CardIndicatorSlot : MonoBehaviour
{
    [SerializeField] Image _alphaCardImage;
    public Image AlphaCardImage => _alphaCardImage;
    RectTransform _alphaCardRect;

    [SerializeField] Image _betaCardImage;
    public Image BetaCardImage => _betaCardImage;
    RectTransform _betaCardRect;

    Image _currentVisibleImage;

    [Header("Tween Settings")]
    [SerializeField] float _tweenDuration = 0.25f;
    [SerializeField] Ease _tweenEase = Ease.OutSine;
//Tweens
    Tween currentCardTween;
    Tween nextCardTween;

    private RectTransform rectTransform;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        _alphaCardRect = _alphaCardImage.GetComponent<RectTransform>();
        _betaCardRect = _betaCardImage.GetComponent<RectTransform>();
    }

    void Start()
    {
        _currentVisibleImage = _alphaCardImage;   
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AssignImages(Sprite currentCard, Sprite nextCard)
    {
        _alphaCardImage.sprite = currentCard;
        _currentVisibleImage = _alphaCardImage;
        _betaCardImage.sprite = nextCard;

        _alphaCardRect.anchoredPosition = new Vector2(0, 0);
        _betaCardRect.anchoredPosition = new Vector2(0, 160);

    }

    public void AssignImages(Sprite currentCard)
    {
        _betaCardImage.sprite = null;
        _alphaCardImage.sprite = currentCard;
        _currentVisibleImage = _alphaCardImage;

        _alphaCardRect.anchoredPosition = new Vector2(0, 0);
        _betaCardRect.anchoredPosition = new Vector2(0, 160);
    }

    public void CycleImage()
    {
        if(currentCardTween.IsActive()){ currentCardTween.Kill(); }
        if(nextCardTween.IsActive()){ nextCardTween.Kill(); }

        if(_betaCardImage != null)
        {
            Image nextVisibleImage;

            if(_currentVisibleImage == _alphaCardImage)
            {
                nextVisibleImage = _betaCardImage;
            }
            else
            {
                nextVisibleImage = _alphaCardImage;
            }

            RectTransform nextImageRect = nextVisibleImage.GetComponent<RectTransform>();
            nextCardTween = nextImageRect.DOAnchorPosY(0, _tweenDuration).SetUpdate(true).SetEase(_tweenEase);
        }


        RectTransform currentImageRect = _currentVisibleImage.GetComponent<RectTransform>();
        currentCardTween = currentImageRect.DOAnchorPosY(-160, _tweenDuration).SetUpdate(true);



    }


}
