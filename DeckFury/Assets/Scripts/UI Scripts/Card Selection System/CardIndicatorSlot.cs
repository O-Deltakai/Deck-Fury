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
    public IReadOnlyList<CardObjectReference> cardMagazine;

    [SerializeField] int _index = 0;


    [SerializeField] Image _alphaCardImage;
    public Image AlphaCardImage => _alphaCardImage;
    RectTransform _alphaCardRect;

    [SerializeField] Image _betaCardImage;
    public Image BetaCardImage => _betaCardImage;
    RectTransform _betaCardRect;

    Image _currentVisibleImage;

    [Header("Tween Settings")]
    [SerializeField] float _tweenDuration = 0.25f;
    [SerializeField] Ease _tweenEase;
//Tweens
    Tween currentCardTween;
    Tween nextCardTween;

    private RectTransform rectTransform;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        if(_alphaCardImage) _alphaCardRect = _alphaCardImage.GetComponent<RectTransform>();
        if(_betaCardImage) _betaCardRect = _betaCardImage.GetComponent<RectTransform>();
    }

    void Start()
    {
        Clear();
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

        _alphaCardImage.enabled = true;
        _betaCardImage.enabled = true;

        _alphaCardRect.anchoredPosition = new Vector2(0, 0);
        _betaCardRect.anchoredPosition = new Vector2(0, 160);

    }

    public void AssignImages(Sprite currentCard)
    {
        _betaCardImage.sprite = null;
        _alphaCardImage.sprite = currentCard;
        _currentVisibleImage = _alphaCardImage;

        _alphaCardImage.enabled = true;

        _alphaCardRect.anchoredPosition = new Vector2(0, 0);
        _betaCardRect.anchoredPosition = new Vector2(0, 160);
    }

    public void Clear()
    {
        _alphaCardImage.sprite = null;
        _alphaCardImage.enabled = false;
        _betaCardImage.sprite = null;
        _betaCardImage.enabled = false;

        _alphaCardRect.anchoredPosition = new Vector2(0, 0);
        _betaCardRect.anchoredPosition = new Vector2(0, 160);        
    }

    public void CycleImage()
    {
        if(_index > cardMagazine.Count)
        {
            return;
        }

        if(currentCardTween.IsActive()){ currentCardTween.Kill(); }
        if(nextCardTween.IsActive()){ nextCardTween.Kill(); }

        //Move the current visible image down and out of view
        RectTransform currentImageRect = _currentVisibleImage.GetComponent<RectTransform>();
        currentCardTween = currentImageRect.DOAnchorPosY(-160, _tweenDuration).SetUpdate(true).SetEase(_tweenEase);        

        //Check the next index and if it is within the bounds of the cardMagazine, move the next image into view
        if(_index + 1 > cardMagazine.Count)
        {
            return;
        }

        Image nextVisibleImage;
        RectTransform nextImageRect;

        if(_currentVisibleImage == _alphaCardImage)
        {
            nextVisibleImage = _betaCardImage;
            nextImageRect = _betaCardRect;
        }
        else
        {
            nextVisibleImage = _alphaCardImage;
            nextImageRect = _alphaCardRect;
        }

        nextVisibleImage.sprite = cardMagazine[_index].cardSO.GetCardImage();
        nextCardTween = nextImageRect.DOAnchorPosY(0, _tweenDuration).SetUpdate(true).SetEase(_tweenEase).OnComplete(
            () => currentImageRect.anchoredPosition = new Vector2(0, 160));

        

        _currentVisibleImage = nextVisibleImage;


    }


}
