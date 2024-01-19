using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

//UI element which show the player what cards are currently in their magazine and which card they can use next.
public class CardMagazineIndicator : MonoBehaviour
{
    [SerializeField] RectTransform anchorPoint;

    [SerializeField] PlayerCardManager playerCardManager;
    [SerializeField] CardSelectionMenu cardSelectionMenu;
    [SerializeField] List<Image> cardImages = new List<Image>();

    [SerializeField] TextMeshProUGUI firstCardText;

    Tween movementTween;

    private void Start()
    {
        MoveOutOfView();
        playerCardManager = cardSelectionMenu.PlayerCardManager;
        
        //Subscribe the RemoveCardEvent/LoadMagazineEvent from PlayerCardManager to the UpdateImages method
        playerCardManager.OnRemoveCard += UpdateImages;
        playerCardManager.OnLoadMagazine += UpdateImages;

        //When the card select menu is active, move out of view. Otherwise, move into view.
        cardSelectionMenu.OnMenuDisabled += MoveIntoView;
        cardSelectionMenu.OnMenuActivated += MoveOutOfView;
    }


    public void MoveIntoView()
    {   
        if(movementTween.IsActive())
        {
            movementTween.Kill();
        }
        movementTween = anchorPoint.DOLocalMoveY(0, 0.45f).SetUpdate(true);
    }

    public void MoveOutOfView()
    {
        if(movementTween.IsActive())
        {
            movementTween.Kill();
        }

        movementTween = anchorPoint.DOLocalMoveY(-1000, 0.15f).SetUpdate(true);
    }


    //Clears the images from the cardImages and iterates through the cardMagazine from playerCardManager, setting the image
    //of the corresponding cardImage to that of the card image from the magazine element.
    public void UpdateImages()
    {
        foreach(Image image in cardImages)
        {
            image.sprite = null;
        }

        for(int i = 0; i < playerCardManager.CardMagazine.Count ; i++) 
        {
            
            cardImages[i].sprite = playerCardManager.CardMagazine[i].cardSO.GetCardImage();
        }
        UpdateFirstCardText();
    }

    void UpdateFirstCardText()
    {
        if(firstCardText == null){return;}
        if(playerCardManager.MagazineIsEmpty())
        {
            firstCardText.text = "";
            return;
        }
        
        firstCardText.text = playerCardManager.CardMagazine[0].cardSO.CardName;
    }

}
