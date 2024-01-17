using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public abstract class ShopPurchasable : MonoBehaviour
{
    public ShopManager shopManager;
    public int _price;



    [SerializeField] protected GameObject spriteObject;
    [SerializeField] protected GameObject shadowSprite;
    [SerializeField] protected SpriteRenderer shopObjectSpriteRenderer;


[Header("Object Animation Properties")]
    [SerializeField, Min(0.1f)] protected float floatSpeed = 0.5f;
    [SerializeField] protected float floatHeight;
    [SerializeField] protected float shadowScaleTweenStrength = 1.02f;
    [SerializeField] Ease ease;

    [SerializeField] protected float selectedExpandSpeed = 0.5f;
    [SerializeField] protected float selectedScaleMultiplier = 1f;
    Vector3 spriteObjectOriginalScale;

    void Start()
    {
        spriteObjectOriginalScale = spriteObject.transform.localScale; 

        StartCoroutine(FloatObject());
    }

    public virtual void Purchase(){}


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
        }

    } 

    protected virtual void OnTriggerExit2D(Collider2D collider)
    {
        if(collider.CompareTag(TagNames.Player.ToString()))
        {
            spriteObject.transform.DOScale(spriteObjectOriginalScale, selectedExpandSpeed);
        }
    } 

}
