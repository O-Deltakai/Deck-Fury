using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using FMODUnity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeManager : MonoBehaviour
{
    UpgradeAnimationManager upgradeAnimationManager;

    [SerializeField] DeckViewManager deckViewManager;
    [SerializeField] CanvasGroup deckViewCanvasGroup;

    [SerializeField] UpgradeViewManager upgradeViewManager;
    [SerializeField] CanvasGroup upgradeViewCanvasGroup;


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

    [Header("Upgrade Mechanics")]
    [SerializeField] TextMeshProUGUI instructionText;
    [Min(0)] public int numberOfUpgrades = 1;
    int currentUpgradeCount = 0;

    [Header("SFX")]
    [SerializeField] EventReference onUpgradeSFX;

    [Header("Debug Options")]
    [SerializeField] bool debugMode = false;

    void Awake()
    {
        upgradeAnimationManager = GetComponent<UpgradeAnimationManager>();

        upgradeViewManager.selectedCardDescriptionPanel = selectedCardDescriptionPanel;
        deckViewManager.selectedCardDescriptionPanel = selectedCardDescriptionPanel;

        UpgradeCardButton.interactable = false;

        UpgradeCardButton.onClick.AddListener(OnClickUpgradeButton);
        cancelButton.onClick.AddListener(OnClickCancelButton);

        PLC = PersistentLevelController.Instance;

        upgradeViewManager.OnUpgradeCardSelected += CheckUpgradeButtonInteractable;
        deckViewManager.OnCardSlotSelected += CheckUpgradeButtonInteractable;

        //When the upgrade animation is complete, take the CardSO from the animated panel and swap it with the selected card description panel
        //then disable the animated panel
        upgradeAnimationManager.OnUpgradeAnimationComplete += UpgradeAnimationCompletePostEvents;
    }

    void Start()
    {
        if(numberOfUpgrades < 0)
        {
            Debug.LogWarning("Number of upgrades is less than 0, setting to 0");
            numberOfUpgrades = 0;
        }

        UpdateInstructionText();
        upgradeCanvas.gameObject.SetActive(false);
        MoveUIOutOfView();

    }

    void UpgradeAnimationCompletePostEvents()
    {
        SwapCardDescriptionPanelCardSO(upgradeAnimationManager.PanelToAnimate);
        deckViewCanvasGroup.interactable = true;
        upgradeViewCanvasGroup.interactable = true;
        deckViewCanvasGroup.blocksRaycasts = true;
        upgradeViewCanvasGroup.blocksRaycasts = true;
    }

    void SwapCardDescriptionPanelCardSO(CardDescriptionPanel otherPanel)
    {
        selectedCardDescriptionPanel.UpdateDescription(otherPanel.CurrentlyViewedCardSO);
        otherPanel.gameObject.SetActive(false);

    }

    void UpdateInstructionText()
    {
        if(numberOfUpgrades - currentUpgradeCount > 0)
        {
            instructionText.text = "Number of upgrades available: " +"<color=green><u><size=150%>"+ (numberOfUpgrades - currentUpgradeCount) + "</color></size></u>";
        }else
        {
            instructionText.text = "Number of upgrades available: " +"<color=red><u><size=150%>"+ (numberOfUpgrades - currentUpgradeCount) + "</color></size></u>";
        }
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
        GameManager.currentGameState = GameManager.GameState.InMenu;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        upgradeCanvas.gameObject.SetActive(true);

        deckViewWaypointTraverser.TraverseToWaypoint(0);
        upgradeViewWaypointTraverser.TraverseToWaypoint(0);
        centerUIWaypointTraverser.TraverseToWaypoint(0);

        dimmingPanel.gameObject.SetActive(true);
        dimmingPanel.DOFade(0.4f, 0.25f).SetUpdate(true);
    }

    public void MoveUIOutOfView()
    {
        GameManager.currentGameState = GameManager.GameState.Realtime;
        
        deckViewWaypointTraverser.TraverseToWaypoint(1);
        upgradeViewWaypointTraverser.TraverseToWaypoint(1);
        centerUIWaypointTraverser.TraverseToWaypoint(1);

        dimmingPanel.DOFade(0, 0.35f).OnComplete(() => upgradeCanvas.gameObject.SetActive(false)).SetUpdate(true);
        UpgradeCardButton.interactable = false;

        selectedCardDescriptionPanel.gameObject.SetActive(false);

        upgradeViewManager.ClearUpgrades();
        deckViewManager.ResetSelectorIndicator();

    }

    void UpgradeCard()
    {
        if(!CheckValidUpgrade())
        {
            Debug.LogError("Invalid upgrade");
            return;
        }

        if(currentUpgradeCount >= numberOfUpgrades && !debugMode)
        {
            print("Upgrade count exceeded");
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

        //Prevent the player from interacting with the UI while the upgrade animation is playing
        deckViewCanvasGroup.interactable = false;
        deckViewCanvasGroup.blocksRaycasts = false;
        upgradeViewCanvasGroup.interactable = false;
        upgradeViewCanvasGroup.blocksRaycasts = false;
        upgradeAnimationManager.InitiateUpgradeAnimation();

        upgradeViewManager.ClearUpgrades();
        UpgradeCardButton.interactable = false;

        currentUpgradeCount++;

        UpdateInstructionText();
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
        if(currentUpgradeCount >= numberOfUpgrades && !debugMode)
        {
            return false;
        }

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
