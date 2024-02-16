using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class CardInspectionPanel : MonoBehaviour
{
    [SerializeField] Image dimmingPanel;
    [SerializeField] CardDescriptionPanel cardDescriptionPanel;
    EventBinding<ClickDeckElementEvent> clickDeckElementEvent;

    [Header("Tween Settings")]
    [SerializeField] float dimmingPanelFadeDuration = 0.25f;
    [SerializeField] Ease dimmingPanelFadeEase = Ease.OutSine;
    Tween dimmingPanelTween;

    void OnEnable()
    {
        clickDeckElementEvent = new EventBinding<ClickDeckElementEvent>(HandleCardInspectionEvent);
        EventBus<ClickDeckElementEvent>.Register(clickDeckElementEvent);
    }

    void OnDisable()
    {
        EventBus<ClickDeckElementEvent>.Deregister(clickDeckElementEvent);
    }


    void HandleCardInspectionEvent(ClickDeckElementEvent clickDeckElementEvent)
    {
        if(dimmingPanelTween.IsActive())
        {
            dimmingPanelTween.Kill();
        }

        dimmingPanel.gameObject.SetActive(true);
        cardDescriptionPanel.UpdateDescription(clickDeckElementEvent.deckCardElementSlot.CurrentCard);
        cardDescriptionPanel.gameObject.SetActive(true);
        dimmingPanelTween = dimmingPanel.DOFade(0.6f, dimmingPanelFadeDuration).SetEase(dimmingPanelFadeEase);
    }

    public void ExitInspection()
    {
        if(dimmingPanelTween.IsActive())
        {
            dimmingPanelTween.Kill();
        }
        dimmingPanelTween = dimmingPanel.DOFade(0, 0.1f).SetEase(Ease.OutCubic).OnComplete(() => dimmingPanel.gameObject.SetActive(false));
    }

}
