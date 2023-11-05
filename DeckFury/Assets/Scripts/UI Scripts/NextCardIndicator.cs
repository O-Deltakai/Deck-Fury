using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class NextCardIndicator : MonoBehaviour
{
    [SerializeField] PlayerController player;//Must be set in inspector
    PlayerCardManager playerCardManager;
    SpriteRenderer cardSprite;

    void Awake()
    {
        playerCardManager = player.GetComponent<PlayerCardManager>();
        cardSprite = GetComponent<SpriteRenderer>();
        playerCardManager.OnLoadMagazine += UpdateCardImage;
        playerCardManager.OnRemoveCard += UpdateCardImage;

    }
    

    void Start()
    {
        cardSprite.enabled = false;
    }

    void UpdateCardImage()
    {
        if(!playerCardManager.MagazineIsEmpty())
        {
            cardSprite.sprite = playerCardManager.CardMagazine[0].cardSO.GetCardImage();
            cardSprite.enabled = true;
        }else
        {
            cardSprite.enabled = false;
        }


    }

}
