using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class StartingDeckMenuController : MonoBehaviour
{
    [SerializeField] PersistentLevelController persistentLevelController;

    [SerializeField] GameObject dimmingPanel;

    [SerializeField] List<DeckSO> availableStartingDecks;
    [SerializeField] GameObject startingDeckSlotPrefab;
    [SerializeField] Transform startingDeckSlotParent;

    [Header("Tween Settings")]
    [SerializeField] float slideOutOfViewDuration = 0.25f;
    [SerializeField] Ease slideOutOfViewEase = Ease.OutSine;

    //Tweens
    Tween slideOutOfViewTween;
    Tween dimmingPanelFadeTween;

    [Header("Debug Settings")]
    [SerializeField] bool _unlockAllDecks = false;

    void Start()
    {

        InititializeMenu();
        if (_unlockAllDecks)
        {
            UnlockAllDecks();
        }
    }

    void OnValidate()
    {
        if (_unlockAllDecks)
        {
            UnlockAllDecks();
        }
    }

    void InititializeMenu()
    {
        foreach (var deck in availableStartingDecks)
        {
            GameObject newSlot = Instantiate(startingDeckSlotPrefab, startingDeckSlotParent);
            StartingDeckSlot slot = newSlot.GetComponent<StartingDeckSlot>();
            slot.InitializeStartingDeckSlot(deck);
            slot.OnDeckSelected += SelectDeck;
        }
    }

    void UnlockAllDecks()
    {
        foreach (Transform slot in startingDeckSlotParent)
        {
            StartingDeckSlot startingDeckSlot = slot.GetComponent<StartingDeckSlot>();
            startingDeckSlot.Unlocked = true;
        }
    }

    public void ShowMenu()
    {
        dimmingPanel.SetActive(true);
    }

    public void HideMenu()
    {
        if (slideOutOfViewTween.IsActive())
        {
            slideOutOfViewTween.Kill();
        }

        if(dimmingPanelFadeTween.IsActive())
        {
            dimmingPanelFadeTween.Kill();
        }

        dimmingPanelFadeTween = dimmingPanel.GetComponent<Image>().DOFade(0, slideOutOfViewDuration).SetUpdate(true).SetEase(Ease.OutSine)
        .OnComplete(() => dimmingPanel.SetActive(false));
        dimmingPanel.GetComponent<Image>().raycastTarget = false;

        slideOutOfViewTween = transform.DOLocalMoveY(-1000, slideOutOfViewDuration).SetEase(slideOutOfViewEase).SetUpdate(true);

    }

    public void SelectDeck(DeckSO deck)
    {
        //Debug.Log("Deck Selected: " + deck.name);

        if (persistentLevelController)
        {
            persistentLevelController.PlayerData.AssignDeck(deck);
        }

        RaiseDeckSelectedEvent(deck);

        HideMenu();
    }

    void RaiseDeckSelectedEvent(DeckSO deck)
    {
        EventBus<SelectStartingDeckEvent>.Raise(new SelectStartingDeckEvent(deck));
    }


}
