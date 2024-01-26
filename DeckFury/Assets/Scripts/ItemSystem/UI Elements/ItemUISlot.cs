using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ItemUISlot : MonoBehaviour
{
    public event Action<ItemSO> OnChangeItemSO;
    public event Action<ItemBase> OnChangeItemObjectRef;

    [SerializeField] Image blinkerImage;
    Vector3 originalBlinkerImageScale;
    Color originalBlinkerImageColor;


    [SerializeField] ItemSO _itemSO;
    [SerializeField] ItemBase _itemObjectRef;
    public ItemBase ItemObjectRef {get { return _itemObjectRef; }
        set
        {
            if(_itemObjectRef == value) { return; }
            _itemObjectRef = value;
            OnChangeItemObjectRef?.Invoke(value);
        } 

    }

    [Header("UI Elements")]
    [SerializeField] GameObject itemDescriptionPanel;
    [SerializeField] TextMeshProUGUI itemDescriptionText;
    [SerializeField] TextMeshProUGUI itemNameText;
    [SerializeField] GameObject indicator;


    [Header("Blink Animation Properties")]
    [SerializeField] Ease blinkEase;
    [SerializeField] float blinkSpeed = 0.3f;
    [SerializeField] Vector3 blinkerImageMaxScale = new Vector3(1.5f, 1.5f);

    Tween blinkImageScaleTween = null;
    Tween blinkFadeTween = null;

    Image itemImage;

    [SerializeField] bool TEST_AutoBlink = false;

    void Awake()
    {
        itemImage = GetComponent<Image>();
        originalBlinkerImageScale = blinkerImage.transform.localScale;
        originalBlinkerImageColor = blinkerImage.color;
    }

    void Start()
    {
        if(_itemObjectRef)
        {
            SetItem(_itemObjectRef);
        }

        itemDescriptionPanel.SetActive(false);
        indicator.SetActive(false);
    }

    void Update()
    {
        if(!TEST_AutoBlink) { return; }

        if(!blinkImageScaleTween.IsActive())
        {
            BlinkImageOnce();
        }
    }

    public void SetItem(ItemBase item)
    {
        _itemObjectRef = item;
        _itemSO = item.itemSO;

        _itemObjectRef.OnProc += BlinkImageOnce;
        itemImage.sprite = _itemSO.ItemImage;
        blinkerImage.sprite = _itemSO.ItemImage;

        itemDescriptionText.text = _itemSO.GetFormattedDescription();
        itemNameText.text = _itemSO.ItemName;
    }

    public void OnPointerEnter()
    {
        itemDescriptionPanel.SetActive(true);
        indicator.SetActive(true);
    }

    public void OnPointerExit()
    {
        itemDescriptionPanel.SetActive(false);
        indicator.SetActive(false);

    }

    public void BlinkImageOnce()
    {
        if(blinkImageScaleTween.IsActive())
        {
            blinkImageScaleTween.Kill();
            blinkFadeTween.Kill();
        }
        blinkerImage.gameObject.SetActive(true);
        blinkerImage.transform.localScale = originalBlinkerImageScale;
        blinkerImage.color = originalBlinkerImageColor;

        blinkImageScaleTween = blinkerImage.transform.DOScale(blinkerImageMaxScale, blinkSpeed).SetUpdate(true);
        blinkFadeTween = blinkerImage.DOFade(0f, blinkSpeed).SetUpdate(true).SetEase(Ease.InBack);

    }



}
