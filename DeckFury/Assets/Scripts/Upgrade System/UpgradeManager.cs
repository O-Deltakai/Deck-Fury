using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeManager : MonoBehaviour
{
    [SerializeField] UpgradeViewManager upgradeViewManager;
    [SerializeField] DeckViewManager deckViewManager;
    [SerializeField] CardDescriptionPanel selectedCardDescriptionPanel;

    [field:SerializeField] public Button UpgradeCardButton {get; private set;}
    [SerializeField] Button cancelButton;

    void Awake()
    {
        upgradeViewManager.selectedCardDescriptionPanel = selectedCardDescriptionPanel;
        deckViewManager.selectedCardDescriptionPanel = selectedCardDescriptionPanel;
    }

    public void OnClickUpgradeButton()
    {
        if(upgradeViewManager.SelectedUpgradeCard == null || selectedCardDescriptionPanel.CurrentlyViewedCardSO == null)
        {
            Debug.LogError("Selected upgrade card or selected card is null");
            return;
        }
        UpgradeCard(selectedCardDescriptionPanel.CurrentlyViewedCardSO, upgradeViewManager.SelectedUpgradeCard);
    }

    void UpgradeCard(CardSO card, CardSO upgradedCard)
    {
        if(card == null || upgradedCard == null)
        {
            Debug.LogError("Card or upgraded card is null");
            return;
        }
        if(card == upgradedCard)
        {
            Debug.LogError("Card and upgraded card are the same");
            return;
        }
        if(!card.HasUpgrades)
        {
            Debug.LogWarning("Card does not have any upgrades");
            return;
        }
        // Check if the given upgraded card is a valid upgrade for the given card
        if(card.Upgrades.Find(x => x.UpgradedCard == upgradedCard) == null)
        {
            Debug.LogError("The given upgraded card: " + upgradedCard.name + " is not a valid upgrade for the given card: " + card.name);
            return;
        }

    }


}
