using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class RewardMenuController : MonoBehaviour
{
    public delegate void FinishSelectRewardEvent();
    public event FinishSelectRewardEvent OnSelectReward;


    [SerializeField] Vector3 OutOfViewAnchor;
    [SerializeField] Vector3 InViewAnchor;

    CardSelectionMenu cardSelectionMenu; 

    RectTransform rectTransform;

    [SerializeField] List<CardDescriptionPanel> rewardSlots;

    private void Awake() 
    {
        rectTransform = GetComponent<RectTransform>();    
        cardSelectionMenu = FindObjectOfType<CardSelectionMenu>();
    }

    void Start()
    {
        GenerateRewards();
    }

    void GenerateRewards()
    {
        int numberOfCardRewards = rewardSlots.Count;
        CardSO[] cardPool = GetCardsFromResources("Cards");
        for(int i = 0; i < numberOfCardRewards; i++) 
        {
            int randomInt = Random.Range(0, cardPool.Length);
            rewardSlots[i].UpdateDescription(cardPool[randomInt]); 

        }
    }

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
    }

    public void MoveOutOfView()
    {
        rectTransform.DOLocalMove(OutOfViewAnchor, 0.25f).SetUpdate(true);
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

        MoveOutOfView();
        OnSelectReward?.Invoke();

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

}
