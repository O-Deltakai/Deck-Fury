using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeckCardElementSlot : MonoBehaviour
{
    public event Action<DeckCardElementSlot> OnHoverEvent;

    [SerializeField] CardSO cardSO;
    [SerializeField] Image cardFrame;
    [SerializeField] Image cardImage;
    [SerializeField] TextMeshProUGUI cardCountText;


    public void AssignDeckElement(DeckElement deckElement)
    {
        cardSO = deckElement.card;
        cardImage.sprite = cardSO.GetCardImage();
        cardCountText.text = deckElement.cardCount.ToString();
        ChangeColorBasedOnCardTier(cardSO.GetCardTier());
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
    }





}
