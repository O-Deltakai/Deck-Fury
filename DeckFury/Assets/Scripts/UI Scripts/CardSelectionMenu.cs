using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.InputSystem;
using FMODUnity;

//Script that handles functionality of the card selection menu
public class CardSelectionMenu : MonoBehaviour
{
    static CardSelectionMenu _instance;
    public static CardSelectionMenu Instance {get { return _instance; }}
    

    public delegate void SelectSpecificCardEvent(CardSO card);
    public event SelectSpecificCardEvent OnSelectSpecificCard;


    public delegate void PreviewingStageEvent();
    public event PreviewingStageEvent OnPreviewStage;

    public delegate void UnpreviewStageEvent();
    public event UnpreviewStageEvent OnUnpreviewStage;

    //Event for when the menu is activated
    public delegate void ActivateMenuEvent();
    public event ActivateMenuEvent OnMenuActivated;

    //Event for when the menu is disabled
    public delegate void DisableMenuEvent();
    public event DisableMenuEvent OnMenuDisabled;

[Tooltip("If set to true, the Card Selection Menu will open immediately on starting the game.")]
    public bool BeginActivated = true;
    public bool CanBeOpened = true;


    [field:SerializeField] public CardPoolManager CardPoolManager{get; private set;} //Should be set in inspector to improve performance
    [field:SerializeField] public PlayerCardManager PlayerCardManager {get;private set;} //Should be set in inspector to improve performance

    [SerializeField] GameObject cardSelectPanel; //Must be set in inspector
    [SerializeField] GameObject cardLoadPanel; //Must be set in inspector

    //List of references to all the card slots within the cardSelectPanel and cardLoad panel
    [SerializeField] List<CardSlot> selectableCardSlots;
    [SerializeField] List<CardSlot> cardLoadSlots;

    //List of run-time references to all the cardObjectReferences within the card slots within the cardSelectPanel and cardLoadPanel
    [SerializeField] List<CardObjectReference> cardObjectReferencesInSelectPanel;
    [SerializeField] List<CardObjectReference> cardObjectReferencesInLoadPanel;

    RectTransform rectTransform;

    [SerializeField] float OutOfViewXAnchor;
    [SerializeField] float InViewXAnchor;

    [SerializeField] float PreviewOutOfViewYAnchor;
    [SerializeField] float PreviewInViewYAnchor;
    [SerializeField] RectTransform PreviewArrowIcon;
    bool isPreviewing = false;

    [Header("SFX")]
    [SerializeField] EventReference activateMenuSFX;
    [SerializeField] EventReference clickOKSFX;
    [SerializeField] EventReference previewStageSFX;
    [SerializeField] EventReference unpreviewStageSFX;



    public bool isActive{get; private set;} = false;
    public bool canUsePreviewButton = true;

    private void Awake() 
    {
        _instance = this;


        rectTransform = GetComponent<RectTransform>();

        CardPoolManager = GameErrorHandler.NullCheck(CardPoolManager, "CardPoolManager");

    }

    void Start()
    {
        if(PlayerCardManager == null)
        {
            PlayerCardManager = GameManager.Instance.player.GetComponent<PlayerCardManager>();
            if(PlayerCardManager == null)
            {
                PlayerCardManager = FindObjectOfType<PlayerCardManager>();
                if(PlayerCardManager != null)
                {Debug.LogWarning("CardSelectionMenu could not find PlayerCardManager through GameManager," +
                " something may have gone wrong with the player reference in GameManager.");}
                else
                {Debug.LogWarning("CardSelectionMenu could not find Player prefab in scene, CardSelectionMenu will not work!");}
            }
        }

        
        CardPoolManager.OnCompletePooling += PopulateCardSelect;

        InitializeMenu();

        if(BeginActivated)
        {
            ActivateMenu();
        }

    }

    // Update is called once per frame
    void Update()
    {
        if(isActive && canUsePreviewButton)
        {
            if(Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                PreviewButton();
            }        
        }
    }

    void OnDestroy()
    {
        _instance = null;
    }

    public void InitializeMenu()
    {


        for(int i = 0; i < selectableCardSlots.Count ; i++)
        {
            if(!selectableCardSlots[i].IsEmpty())
            {
                cardObjectReferencesInSelectPanel.Add(selectableCardSlots[i].cardObjectReference);
            }

            selectableCardSlots[i].slotIndex = i;

        }

        for(int i = 0; i < cardLoadSlots.Count ; i++) 
        {
            cardLoadSlots[i].slotIndex = i;
        }

    }

    //Method for activating the menu and moving it into view. Will repopulate the card select with fresh cards on activation.
    public void ActivateMenu()
    {
        if(!CanBeOpened){return;}
        if(isActive){return;}

        PopulateCardSelect();
        for(int i = 0; i < selectableCardSlots.Count ; i++)
        {
            if(!selectableCardSlots[i].IsEmpty())
            {
                cardObjectReferencesInSelectPanel.Add(selectableCardSlots[i].cardObjectReference);
            }

        }
        rectTransform.DOLocalMoveX(InViewXAnchor, 0.25f).SetUpdate(true);

        isActive = true;
        OnMenuActivated?.Invoke();

        GameManager.PauseGame();
    }

    //Moves the menu out of view so it cannot be interacted with
    public void DisableMenu()
    {
        rectTransform.DOLocalMoveX(OutOfViewXAnchor, 0.25f).SetUpdate(true);   

        isActive = false;
        OnMenuDisabled?.Invoke();

        GameManager.UnpauseGame();
    }

    public void PreviewButton()
    {
        if(!isPreviewing)
        {
            PreviewStage();
            isPreviewing = true;
        }else
        {
            UnPreviewStage();
            isPreviewing = false;
        }
    }


    //Move menu upwards out of view when previewing stage
    void PreviewStage()
    {
        rectTransform.DOLocalMoveY(PreviewOutOfViewYAnchor, 0.25f).SetUpdate(true);
        PreviewArrowIcon.eulerAngles = new Vector3(0,0,-90);   
        OnPreviewStage?.Invoke();
    }

    //Move menu back down minto view to see card select
    void UnPreviewStage()
    {
        rectTransform.DOLocalMoveY(PreviewInViewYAnchor, 0.25f).SetUpdate(true);
        PreviewArrowIcon.eulerAngles = new Vector3(0,0, 90);   
        OnUnpreviewStage?.Invoke();

    }


    //Populates the Card Selection Panel with CardObjectReferences from the CardPoolManager. Uses a random index to grab a random card
    //from the CardObjectReferences and updates each of the card slots with said CardObjectReference
    private void PopulateCardSelect()
    {
        List<CardObjectReference> currentDeck = CardPoolManager.CardObjectReferences;

        //Wipe the panels clean of any leftover card objects and clear all card slots
        cardObjectReferencesInSelectPanel.Clear();
        cardObjectReferencesInLoadPanel.Clear();
        foreach(CardSlot cardSlot in selectableCardSlots)
        {cardSlot.ClearCardSlot();}
        foreach(CardSlot cardSlot in cardLoadSlots)
        {cardSlot.ClearCardSlot();}


        //Randomization of card hand logic

        List<CardObjectReference> tempDeck = new List<CardObjectReference>();
        foreach(CardObjectReference card in currentDeck)
        {
            tempDeck.Add(card);
        }

        foreach(CardSlot cardSlot in selectableCardSlots)
        {
            if(tempDeck.Count <= 0)
            {
                break;
            }
            int randomIndex = Random.Range(0, tempDeck.Count);
            cardSlot.ChangeCard(tempDeck[randomIndex]);
            tempDeck.RemoveAt(randomIndex);
        }


    }


    //Logic for what happens when clicking on a card slot in the cardSelectPanel
    //Clicking on a slot in the load panel should return the card within the card slot to the previous select slot that it was originally taken from.
    //The card load panel should then update by moving all cards forward in the list to fill the gap left by the clicked slot.
    public void OnClickCardSlotLoadPanel(GameObject cardSlot)
    {
        CardSlot loadSlot = cardSlot.GetComponent<CardSlot>();

        if(loadSlot.IsEmpty())
        {
            Debug.LogWarning("loadSlot is empty but still interactable, which shouldn't be happening - something may have gone wrong.", loadSlot);
            return;
        }

        cardObjectReferencesInLoadPanel.Remove(loadSlot.cardObjectReference);

        loadSlot.TransferCardToSlot(loadSlot.previousTransfererSlot);
        cardObjectReferencesInSelectPanel.Add(loadSlot.previousTransfererSlot.cardObjectReference);

        //Update other loadSlots by moving forward all cards to fill in gaps
        for(int i = loadSlot.slotIndex + 1; i < cardLoadSlots.Count ; i++) 
        {
            if(cardLoadSlots[i].IsEmpty())
            {break;}

            cardLoadSlots[i].ReplaceCardSlot(cardLoadSlots[i - 1]);            

        }

    }

    //Logic for what happens when clicking on a card slot in the cardLoadPanel
    //Clicking on a slot in the select panel should transfer the card object reference stored within to the bottom of card load panel list
    public void OnClickCardSlotSelectPanel(GameObject cardSlot)
    {
        CardSlot selectSlot = cardSlot.GetComponent<CardSlot>();

        if(selectSlot.IsEmpty())
        {
            Debug.LogWarning("loadSlot is empty but still interactable, which shouldn't be happening - something may have gone wrong.", selectSlot);
            return;
        }

        //print("Clicked card: " + selectSlot.cardObjectReference.cardSO.CardName);



        if(cardObjectReferencesInLoadPanel.Count < cardLoadSlots.Count)//Prevent user from selecting more cards than there is capacity in the load panel
        {
            CardSlot loadSlot = cardLoadSlots[cardObjectReferencesInLoadPanel.Count];

            cardObjectReferencesInSelectPanel.Remove(selectSlot.cardObjectReference);
            selectSlot.TransferCardToSlot(loadSlot);

            cardObjectReferencesInLoadPanel.Add(loadSlot.cardObjectReference);

            OnSelectSpecificCard?.Invoke(loadSlot.cardObjectReference.cardSO);
            
           
        }

    }

    //Logic for what happens when clicking the OK button
    public void OnClickOKButton()
    {
        PlayerCardManager.LoadCardMagazine(cardObjectReferencesInLoadPanel);
        RuntimeManager.PlayOneShot(clickOKSFX);
        DisableMenu();
    }


}
