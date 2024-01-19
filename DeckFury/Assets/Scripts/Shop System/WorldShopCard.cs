using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class WorldShopCard : ShopPurchasable
{
    event Action<CardSO> OnChangeCard;

    [SerializeField] CardSO _card;
    public CardSO Card {get{ return _card; } 
        set
        {
            _card = value;
            OnChangeCard?.Invoke(value);
        }
    
    }


    [SerializeField] CardDescriptionPanel cardDescriptionPanel;




    [Header("Animate Opening Description Panel Settings")]
    [SerializeField] float expandSpeed = 0.5f;
    [SerializeField] float shrinkSpeed = 0.5f;

    Vector3 originalDescriptionPanelScale;

    Coroutine CR_ToggleDescriptionPanel = null;


    protected override void Awake()
    {
        base.Awake();
        OnChangeCard += SetCard;
    }

    protected override void Start()
    {
        base.Start();

        originalDescriptionPanelScale = cardDescriptionPanel.gameObject.transform.localScale;
        cardDescriptionPanel.transform.localScale = Vector3.zero;
        
        if(Card)
        {
            shopObjectSpriteRenderer.sprite = Card.GetCardImage();
        }

    }


    public override void Select()
    {
        if(CR_ToggleDescriptionPanel != null) { return; }
        if(Card) cardDescriptionPanel.UpdateDescription(Card);

        CR_ToggleDescriptionPanel = StartCoroutine(ToggleDescriptionPanel(true));
        _selected = true;
    }

    public override void Deselect()
    {
        if(CR_ToggleDescriptionPanel != null) { return; }

        CR_ToggleDescriptionPanel = StartCoroutine(ToggleDescriptionPanel(false));
        _selected = false;
        
    }

    public override void Purchase()
    {
        shopManager.PurchaseCard(this);
    }

    void SetCard(CardSO otherCard)
    {
        _card = otherCard;
        shopObjectSpriteRenderer.sprite = _card.GetCardImage();
    }


    IEnumerator ToggleDescriptionPanel(bool toggle)
    {
        if(toggle)
        {
            descriptionPanelCanvas.SetActive(true);
            cardDescriptionPanel.transform.DOScale(originalDescriptionPanelScale, expandSpeed);
        }else
        {
            cardDescriptionPanel.transform.DOScale(Vector3.zero, shrinkSpeed);
            yield return new WaitForSeconds(shrinkSpeed);
            descriptionPanelCanvas.SetActive(false);
        }

        CR_ToggleDescriptionPanel = null;

    }


}
