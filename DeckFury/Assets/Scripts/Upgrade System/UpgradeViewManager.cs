using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Manages the upgrade view in the upgrade screen
/// </summary>
public class UpgradeViewManager : MonoBehaviour
{
    [SerializeField] GameObject cardUpgradePanelPrefab;
    [SerializeField] GameObject cardUpgradePanelsParent;

    public CardDescriptionPanel selectedCardDescriptionPanel;

    [SerializeField] List<CardDescriptionPanel> cardUpgradePanels = new List<CardDescriptionPanel>();

    public CardSO SelectedUpgradeCard{get; private set;}

    void Start()
    {
        selectedCardDescriptionPanel.OnCardUpdated += DisplayUpgrades;
    }


    public void DisplayUpgradesForCard(CardSO cardSO)
    {
        foreach (Transform child in cardUpgradePanelsParent.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (var upgrade in cardSO.Upgrades)
        {
            BuildNewCardUpgradePanel(upgrade);
        }
    }

    public void DisplayUpgrades()
    {
        if(selectedCardDescriptionPanel.CurrentlyViewedCardSO == null)
        {
            return;
        }
        DisplayUpgradesForCard(selectedCardDescriptionPanel.CurrentlyViewedCardSO);
    }

    public void ClearUpgrades()
    {
        foreach (Transform child in cardUpgradePanelsParent.transform)
        {
            Destroy(child.gameObject);
        }
        SelectedUpgradeCard = null;
    }


    void BuildNewCardUpgradePanel(CardUpgradeData upgrade)
    {
        GameObject newCardUpgradePanel = Instantiate(cardUpgradePanelPrefab, cardUpgradePanelsParent.transform);
        CardDescriptionPanel upgradePanel = newCardUpgradePanel.GetComponent<CardDescriptionPanel>();
        Canvas upgradePanelCanvas = newCardUpgradePanel.GetComponent<Canvas>();
        upgradePanelCanvas.overrideSorting = false;
        upgradePanel.disableFade = true;
        upgradePanel.UpdateDescription(upgrade.UpgradedCard);
        upgradePanel.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);

        //Dynamically add event triggers to the upgrade panel
        EventTrigger upgradePanelEventTrigger = upgradePanel.GetComponent<EventTrigger>();

        EventTrigger.Entry pointerEnterEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
        pointerEnterEntry.callback.AddListener((data) => { PointerEnterUpgradePanel(upgradePanel); });
        upgradePanelEventTrigger.triggers.Add(pointerEnterEntry);

        EventTrigger.Entry pointerExitEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
        pointerExitEntry.callback.AddListener((data) => { PonterExitUpgradePanel(upgradePanel); });
        upgradePanelEventTrigger.triggers.Add(pointerExitEntry);

        EventTrigger.Entry pointerClickEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerClick };
        pointerClickEntry.callback.AddListener((data) => { SelectUpgradePanel(upgradePanel); });
        upgradePanelEventTrigger.triggers.Add(pointerClickEntry);   

        ScaleBackAndForth panelScaler = upgradePanel.gameObject.AddComponent<ScaleBackAndForth>();
        panelScaler.autoScale = false;
        panelScaler.StopScaling();
        panelScaler.scaleAmount = 0.1f;
        panelScaler.scaleDuration = 0.1f;
        panelScaler._easeType = DG.Tweening.Ease.InOutSine;

        cardUpgradePanels.Add(upgradePanel);
        
    }

    public void PointerEnterUpgradePanel(CardDescriptionPanel upgradePanel)
    {
        ScaleBackAndForth panelScaler = upgradePanel.gameObject.GetComponent<ScaleBackAndForth>();
        panelScaler.ScaleForth();
    }

    public void PonterExitUpgradePanel(CardDescriptionPanel upgradePanel)
    {
        ScaleBackAndForth panelScaler = upgradePanel.gameObject.GetComponent<ScaleBackAndForth>();
        panelScaler.ScaleBack();
    }


    public void SelectUpgradePanel(CardDescriptionPanel upgradePanel)
    {
        SelectedUpgradeCard = upgradePanel.CurrentlyViewedCardSO;
    }

}
