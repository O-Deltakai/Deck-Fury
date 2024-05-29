using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeManager : MonoBehaviour
{
    [SerializeField] UpgradeViewManager upgradeViewManager;
    [SerializeField] DeckViewManager deckViewManager;
    [SerializeField] CardDescriptionPanel selectedCardDescriptionPanel;

    [field:SerializeField] public Button UpgradeCardButton {get; private set;}
    [SerializeField] Button cancelButton;

    [Header("UI Animation Settings")]
    [SerializeField] UIWaypointTraverser deckViewWaypointTraverser;
    [SerializeField] UIWaypointTraverser upgradeViewWaypointTraverser;
    [SerializeField] UIWaypointTraverser centerUIWaypointTraverser;
    [SerializeField] Image dimmingPanel;
    [SerializeField] Canvas upgradeCanvas;
    [SerializeField] GraphicRaycaster upgradeCanvasGraphicRaycaster;

    void Awake()
    {
        upgradeViewManager.selectedCardDescriptionPanel = selectedCardDescriptionPanel;
        deckViewManager.selectedCardDescriptionPanel = selectedCardDescriptionPanel;

        UpgradeCardButton.interactable = false;

        UpgradeCardButton.onClick.AddListener(OnClickUpgradeButton);
        cancelButton.onClick.AddListener(OnClickCancelButton);
    }

    void Start()
    {
        upgradeCanvas.gameObject.SetActive(false);
        MoveUIOutOfView();

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

    public void OnClickCancelButton()
    {
        MoveUIOutOfView();   
    }

    public void MoveUiIntoView()
    {
        upgradeCanvas.gameObject.SetActive(true);

        deckViewWaypointTraverser.TraverseToWaypoint(0);
        upgradeViewWaypointTraverser.TraverseToWaypoint(0);
        centerUIWaypointTraverser.TraverseToWaypoint(0);

        dimmingPanel.gameObject.SetActive(true);
        dimmingPanel.DOFade(0.4f, 0.25f).SetUpdate(true);
    }

    public void MoveUIOutOfView()
    {
        deckViewWaypointTraverser.TraverseToWaypoint(1);
        upgradeViewWaypointTraverser.TraverseToWaypoint(1);
        centerUIWaypointTraverser.TraverseToWaypoint(1);
        
        dimmingPanel.DOFade(0, 0.35f).OnComplete(() => upgradeCanvas.gameObject.SetActive(false)).SetUpdate(true);
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
