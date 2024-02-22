using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System.Linq;
using FMODUnity;

public class RewardMenuController : MonoBehaviour
{
    static RewardMenuController _instance;
    public RewardMenuController Instance => _instance;

    public delegate void FinishSelectRewardEvent();
    public event FinishSelectRewardEvent OnFinishSelectingReward;


    [SerializeField] Vector3 OutOfViewAnchor;
    [SerializeField] Vector3 InViewAnchor;
    [SerializeField] Vector3 AdditionalRewardsInViewAnchor;
    [SerializeField] Vector3 AdditionalRewardsOutOfViewAnchor;


    CardSelectionMenu cardSelectionMenu; 

    RectTransform rectTransform;
    [SerializeField] List<CardDescriptionPanel> rewardSlots;

    [Header("Additional Rewards Panel")]
    [SerializeField] GameObject additionalRewardsListParent;
    [SerializeField] GameObject genericRewardSlotPrefab;
    [SerializeField] Sprite moneyRewardSprite;

    [SerializeField] List<RewardableElement> additionalRewards = new List<RewardableElement>();

    [Header("SFX")]
    [SerializeField] EventReference rewardMenuOpenSFX;


    System.Random random;

    private void Awake() 
    {
        if(_instance != null)
        {
            Debug.LogError("There is more than one RewardMenuController within the scene which should not be happening.");
        }

        _instance = this;
        rectTransform = GetComponent<RectTransform>();    
        cardSelectionMenu = FindObjectOfType<CardSelectionMenu>();
    }

    void Start()
    {
        if(PersistentLevelController.Instance)
        {
            random = PersistentLevelController.Instance.runRandomGenerator;
        }else
        {
            random = new System.Random();
        }
        GenerateCardRewards();

        if(StageStateController.Instance)
        {
            if(StageStateController.Instance._stageType == StageType.EliteCombat)
            {
                CreateItemReward(GetRandomItemFromResources(random));
            }
        }

    }

    void OnDestroy()
    {
        _instance = null;
    }


    void GenerateCardRewards()
    {
        int numberOfCardRewards = rewardSlots.Count;

        List<CardSO> selectedCards = new();


        CardSO[] cardPool;
        if(PersistentLevelController.Instance)
        {
            cardPool = PersistentLevelController.Instance.RunCardPool.CardPool.ToArray();
        }else
        {
            cardPool = GetCardsFromResources("Cards");
        }

        for(int i = 0; i < numberOfCardRewards; i++) 
        {
            CardSO pickedCard = cardPool[random.Next(0, cardPool.Length)];
            //Make sure the card is not already in the list
            while(selectedCards.Contains(pickedCard))
            {
                pickedCard = cardPool[random.Next(0, cardPool.Length)];
            }

            selectedCards.Add(pickedCard);
            rewardSlots[i].UpdateDescription(selectedCards[i]); 

        }
    }

    ItemSO GetRandomItemFromResources(System.Random random)
    {
        ItemSO[] itemPool = Resources.LoadAll<ItemSO>("Items");
        ItemSO randomItem = itemPool[random.Next(0, itemPool.Count())];

        while(randomItem.ItemPrefab == null)
        {
            randomItem = itemPool[random.Next(0, itemPool.Count())];
        }

        return randomItem;
        

    }

    void GenerateAdditionalRewards(List<RewardableElement> rewardList)
    {



    }


    //Placeholder for now - just grabs all the cards from the resources folder
    CardSO[] GetCardsFromResources(string path)
    {
        CardSO[] cardResources = Resources.LoadAll<CardSO>(path);
        return cardResources;
    }

    public void MoveIntoView()
    {
        rectTransform.DOLocalMove(InViewAnchor, 0.25f).SetUpdate(true);
        if(cardSelectionMenu)
        {
            cardSelectionMenu.CanBeOpened = false;
        }

        GameManager.currentGameState = GameManager.GameState.InMenu;
        Cursor.visible = true;
    }

    public void MoveOutOfView()
    {
        rectTransform.DOLocalMove(OutOfViewAnchor, 0.25f).SetUpdate(true);

        GameManager.currentGameState = GameManager.GameState.Realtime;


        cardSelectionMenu.CanBeOpened = true;

    }

    void MoveAdditionalRewardsIntoView()
    {
        rectTransform.DOLocalMove(AdditionalRewardsInViewAnchor, 0.25f).SetUpdate(true);
    }

    void MoveAdditionalRewardsOutOfView()
    {
        rectTransform.DOLocalMove(AdditionalRewardsOutOfViewAnchor, 0.25f).SetUpdate(true);
        GameManager.currentGameState = GameManager.GameState.Realtime;
        cardSelectionMenu.CanBeOpened = true;        
    }

    public void ClickRewardButton(CardDescriptionPanel rewardSlot)
    {
        SendRewardToDeck(rewardSlot.CurrentlyViewedCardSO);
        rewardSlot.gameObject.SetActive(false);
        foreach(var slot in rewardSlots)
        {
            slot.GetComponent<Button>().interactable = false;
        }

        if(additionalRewards.Count == 0)
        {
            MoveOutOfView();
            OnFinishSelectingReward?.Invoke();
        }else
        {
            MoveAdditionalRewardsIntoView();
        }


    }

    public void SkipCardReward()
    {
        if(additionalRewards.Count == 0)
        {
            MoveOutOfView();
            OnFinishSelectingReward?.Invoke();
        }else
        {
            MoveAdditionalRewardsIntoView();
        }
    }

    public void ContinueButton()
    {
        MoveAdditionalRewardsOutOfView();
        OnFinishSelectingReward?.Invoke();
    }


    void SendRewardToDeck(CardSO card)
    {
        if(StageStateController.Instance && CardPoolManager.Instance)
        {
            DeckElement deckElement = StageStateController.Instance.PlayerData.AddCardToDeck(card, 1);
            CardPoolManager.Instance.AddDeckElementToPool(deckElement);
        }else
        if(PersistentLevelController.Instance)
        {
            PersistentLevelController.Instance.PlayerData.AddCardToDeck(card, 1);
        }else
        {
            Debug.LogError("There is nowhere to send the reward to - the RewardMenuCanvas prefab may be in the wrong place.");
        }

    }

    void CreateAdditionalReward(RewardableElement rewardableElement)
    {

    }

    void CreateCardReward(CardSO cardSO)
    {
        
    }

    void CreateItemReward(ItemSO itemSO)
    {
        ItemReward itemReward = new ItemReward
        {
            rewardItemSO = itemSO, 
            rewardName = itemSO.ItemName,
            rewardSprite = itemSO.ItemImage,
            rewardType = RewardType.Item
        };

        additionalRewards.Add(itemReward);

        GenericRewardSlot genericRewardSlot = Instantiate(genericRewardSlotPrefab, additionalRewardsListParent.transform).GetComponent<GenericRewardSlot>();
        genericRewardSlot.RewardElement = itemReward;
        genericRewardSlot.Initialize();
    }

    void CreateMoneyReward(int amount)
    {
        MoneyReward moneyReward = new MoneyReward
        {
            rewardName = "+$" + amount,
            rewardSprite = moneyRewardSprite,
            rewardType = RewardType.Money
        };

        additionalRewards.Add(moneyReward);

        GenericRewardSlot genericRewardSlot = Instantiate(genericRewardSlotPrefab, additionalRewardsListParent.transform).GetComponent<GenericRewardSlot>();
        genericRewardSlot.RewardElement = moneyReward;
        genericRewardSlot.Initialize();
    }

}
