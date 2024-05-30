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

    PersistentLevelController PLC;

    void Awake()
    {
        upgradeViewManager.selectedCardDescriptionPanel = selectedCardDescriptionPanel;
        deckViewManager.selectedCardDescriptionPanel = selectedCardDescriptionPanel;

        UpgradeCardButton.interactable = false;

        UpgradeCardButton.onClick.AddListener(OnClickUpgradeButton);
        cancelButton.onClick.AddListener(OnClickCancelButton);

        PLC = PersistentLevelController.Instance;

        upgradeViewManager.OnUpgradeCardSelected += CheckUpgradeButtonInteractable;
        deckViewManager.OnCardSlotSelected += CheckUpgradeButtonInteractable;
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
        UpgradeCard();
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
        UpgradeCardButton.interactable = false;

    }

    void UpgradeCard()
    {
        if(!CheckValidUpgrade())
        {
            Debug.LogError("Invalid upgrade");
            return;
        }

        if(PLC)
        {
            PLC.PlayerData.AddCardToDeck(upgradeViewManager.SelectedUpgradeCard, 1);
            PLC.PlayerData.RemoveCardFromDeck(selectedCardDescriptionPanel.CurrentlyViewedCardSO, 1);
        }

        if(CardPoolManager.Instance)
        {
            DeckElement upgradeDeckElement = new DeckElement()
            {
                card = upgradeViewManager.SelectedUpgradeCard,
                cardCount = 1
            };
            CardPoolManager.Instance.AddDeckElementToPool(upgradeDeckElement);
            CardPoolManager.Instance.SetCardReferenceInvisible(selectedCardDescriptionPanel.CurrentlyViewedCardSO);
        }
        
        deckViewManager.RemoveCurrentCardSlot();

        UpgradeCardButton.interactable = false;

    }

    void CheckUpgradeButtonInteractable()
    {
        if(CheckValidUpgrade())
        {
            UpgradeCardButton.interactable = true;
        }else
        {
            UpgradeCardButton.interactable = false;
        }
    }

    bool CheckValidUpgrade()
    {
        CardSO targetCard = selectedCardDescriptionPanel.CurrentlyViewedCardSO;
        CardSO upgradeCard = upgradeViewManager.SelectedUpgradeCard;

        if(!targetCard) {return false;}
        if(!upgradeCard) {return false;}  

        if(targetCard.Upgrades.Find(x => x.UpgradedCard == upgradeCard) == null)
        {
            Debug.LogError("The given upgraded card: " + upgradeCard.name + " is not a valid upgrade for the given card: " + targetCard.name);
            return false;
        }

        return true;
    }


}
