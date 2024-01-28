using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using FMODUnity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WorldShopItem : ShopPurchasable
{
    event Action<ItemSO> OnChangeItem;

    [SerializeField] ItemSO _itemSO;
    public ItemSO ItemSO {get{ return _itemSO; } 
        set
        {
            _itemSO = value;
            OnChangeItem?.Invoke(value);
        }
    
    }


    [Header("Item Description Panel Elements")]
    [SerializeField] GameObject itemDescriptionPanel;
    [SerializeField] TextMeshProUGUI itemName;
    [SerializeField] TextMeshProUGUI itemDescription;


    [Header("Animate Opening Description Panel Settings")]
    [SerializeField] float expandSpeed = 0.5f;
    [SerializeField] float shrinkSpeed = 0.5f;

    Vector3 originalDescriptionPanelScale;





    Coroutine CR_ToggleDescriptionPanel = null;


    protected override void Awake()
    {
        base.Awake();
        OnChangeItem += SetItem;
    }

    protected override void Start()
    {
        base.Start();
        originalDescriptionPanelScale = itemDescriptionPanel.transform.localScale;
        
        if(ItemSO)
        {
            shopObjectImage.sprite = ItemSO.ItemImage;
        }

    }


    public override void Select()
    {
        if(CR_ToggleDescriptionPanel != null) { return; }
        //if(Item) cardDescriptionPanel.UpdateDescription(Item);

        CR_ToggleDescriptionPanel = StartCoroutine(ToggleDescriptionPanel(true));
        _selected = true;

        RuntimeManager.PlayOneShot(selectSFX);
    }

    public override void Deselect()
    {
        if(CR_ToggleDescriptionPanel != null) { return; }

        CR_ToggleDescriptionPanel = StartCoroutine(ToggleDescriptionPanel(false));
        _selected = false;
        
    }

    public override void Purchase()
    {
        shopManager.PurchaseItem(this);
    }

    IEnumerator ToggleDescriptionPanel(bool toggle)
    {
        if(toggle)
        {
            descriptionPanelCanvas.transform.localScale = Vector3.zero;
            descriptionPanelCanvas.SetActive(true);
            descriptionPanelCanvas.transform.DOScale(1, expandSpeed);
        }else
        {
            descriptionPanelCanvas.transform.DOScale(Vector3.zero, shrinkSpeed);
            yield return new WaitForSeconds(shrinkSpeed);
            descriptionPanelCanvas.SetActive(false);
        }

        CR_ToggleDescriptionPanel = null;

    }

    public void SetItem(ItemSO item)
    {  
        _itemSO = item;

        shopObjectImage.sprite = _itemSO.ItemImage;

        itemDescription.text = _itemSO.GetFormattedDescription();
        itemName.text = _itemSO.ItemName;

        SetPanelColorBasedOnItem(_itemSO);
    }

    void SetPanelColorBasedOnItem(ItemSO itemSO)
    {
        Image panelImage = itemDescriptionPanel.GetComponent<Image>();
        switch (itemSO.Rarity)
        {
            case 1:
                panelImage.color = itemSO.ItemColorPalette.CommonColor;
                break;

            case 2:
                panelImage.color = itemSO.ItemColorPalette.UncommonColor;
                break;

            case 3:
                panelImage.color = itemSO.ItemColorPalette.RareColor;
                break;

            case 4:
                panelImage.color = itemSO.ItemColorPalette.VeryRareColor;
                break;

            default:
                break;


        }
    }


}
