using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using FMODUnity;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public abstract class ShopPurchasable : MonoBehaviour
{
    public event Action<int> OnSetPrice;

    public ShopManager shopManager;
    [SerializeField] int _price;
    public int Price {get { return _price; }
        set
        {
            _price = value;
            OnSetPrice?.Invoke(value);
        }
    
    }



    [SerializeField] protected GameObject spriteObject;
    [SerializeField] protected GameObject shadowSprite;
    [SerializeField] protected SpriteRenderer shopObjectSpriteRenderer;
    [SerializeField] protected Image shopObjectImage;


    [SerializeField] protected TextMeshProUGUI _priceTagText;
    public TextMeshProUGUI PriceTagText => _priceTagText;
    [SerializeField] protected TextMeshPro _worldPriceTag;
    public TextMeshPro WorldPriceTag => _worldPriceTag;


    [SerializeField] protected GameObject contextPopup;

    [SerializeField] protected GameObject descriptionPanelCanvas;

    [SerializeField] bool _playerInRange = false;
    public bool PlayerInRange => _playerInRange;

    [SerializeField] protected bool _selected = false;
    public bool Selected => _selected;

    public bool onSale;
    [SerializeField] Color onSaleTextColor;


[Header("Object Animation Properties")]
    [SerializeField, Min(0.1f)] protected float floatSpeed = 0.5f;
    [SerializeField] protected float floatHeight;
    [SerializeField] protected float shadowScaleTweenStrength = 1.02f;
    [SerializeField] Ease ease;

    [SerializeField] protected float selectedExpandSpeed = 0.5f;
    [SerializeField] protected float selectedScaleMultiplier = 1f;
    Vector3 spriteObjectOriginalScale;

    [Header("SFX")]
    [SerializeField] protected EventReference selectSFX;
    [SerializeField] protected EventReference deselectSFX;



    protected virtual void Awake()
    {
        OnSetPrice += SetPriceTag;
    }

    protected virtual void Start()
    {
        spriteObjectOriginalScale = spriteObject.transform.localScale; 
        contextPopup.SetActive(false);

        

        StartCoroutine(FloatObject());
    }

    void Update()
    {
        if(_playerInRange)
        {
            if(Keyboard.current.eKey.wasPressedThisFrame)
            {
                if(_selected)
                {
                    Deselect();
                    _selected = false;
                }else
                {
                    Select();
                    _selected = true;
                }
                
            }

            if(Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                if(_selected)
                {
                    Purchase();
                }
            }
        }
    }


    public virtual void Select(){}
    public virtual void Deselect(){}
    public virtual void Purchase(){}

    public void SetOnSale(bool condtion)
    {
        if(condtion)
        {
            onSale = true;
            PriceTagText.color = onSaleTextColor;
            PriceTagText.text += "!";
        }
    }

    IEnumerator FloatObject()
    {

        while(true)
        {
            spriteObject.transform.DOLocalMoveY(spriteObject.transform.localPosition.y + floatHeight, floatSpeed).SetEase(ease);
            shadowSprite.transform.DOScale(shadowSprite.transform.localScale * shadowScaleTweenStrength, floatSpeed).SetEase(ease);

            yield return new WaitForSeconds(floatSpeed);
            spriteObject.transform.DOLocalMoveY(spriteObject.transform.localPosition.y - floatHeight, floatSpeed).SetEase(ease);
            shadowSprite.transform.DOScale(shadowSprite.transform.localScale / shadowScaleTweenStrength, floatSpeed).SetEase(ease);

            yield return new WaitForSeconds(floatSpeed);


        }

    }

    protected virtual void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.CompareTag(TagNames.Player.ToString()))
        {
            spriteObject.transform.DOScale(spriteObjectOriginalScale * selectedScaleMultiplier, selectedExpandSpeed);
            _playerInRange = true;
        }

    } 

    protected virtual void OnTriggerExit2D(Collider2D collider)
    {
        if(collider.CompareTag(TagNames.Player.ToString()))
        {
            spriteObject.transform.DOScale(spriteObjectOriginalScale, selectedExpandSpeed);
            _playerInRange = false;
            contextPopup.SetActive(false);
            Deselect();
            _selected = false;
        }
    } 

    protected virtual void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.CompareTag(TagNames.Player.ToString()))
        {
            contextPopup.SetActive(true);
        }
    }

    void SetPriceTag(int value)
    {
        if(onSale)
        {
            PriceTagText.color = onSaleTextColor;
            PriceTagText.text = "$:" + value + "!";
            WorldPriceTag.color = onSaleTextColor;
            WorldPriceTag.text = "$" + value + "!";        
        }else
        {
            PriceTagText.text = "$:" + value;
            WorldPriceTag.text = "$" + value;
        }

    }


}
