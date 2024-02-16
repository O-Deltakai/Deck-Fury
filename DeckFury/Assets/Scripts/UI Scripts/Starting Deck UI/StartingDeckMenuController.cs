using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

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


    [Header("Debug Settings")]
    [SerializeField] bool _unlockAllDecks = false;

    void Start()
    {
        if (_unlockAllDecks)
        {
            UnlockAllDecks();
        }

        InititializeMenu();
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
        dimmingPanel.SetActive(false);
    }

    public void SelectDeck(DeckSO deck)
    {
        Debug.Log("Deck Selected: " + deck.name);

        if (persistentLevelController)
        {
            persistentLevelController.PlayerData.AssignDeck(deck);
        }

        HideMenu();
    }


}
