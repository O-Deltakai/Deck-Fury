using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public abstract class ShopPurchasable : MonoBehaviour
{
    public ShopManager shopManager;
    public int _price;

    [SerializeField] protected GameObject spriteObject;

    [SerializeField, Min(0.1f)] protected float floatSpeed = 0.5f;
    [SerializeField] protected float floatHeight;

    [SerializeField] Ease ease;

    void Start()
    {
        StartCoroutine(FloatObject());
    }

    public void Purchase()
    {

    }


    IEnumerator FloatObject()
    {

        while(true)
        {
            spriteObject.transform.DOLocalMoveY(spriteObject.transform.localPosition.y + floatHeight, floatSpeed).SetEase(ease);
            yield return new WaitForSeconds(floatSpeed);
            spriteObject.transform.DOLocalMoveY(spriteObject.transform.localPosition.y - floatHeight, floatSpeed).SetEase(ease);
            yield return new WaitForSeconds(floatSpeed);


        }


    }

}
