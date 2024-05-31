using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System;

public class UpgradeAnimationManager : MonoBehaviour
{
    public event Action OnUpgradeAnimationComplete;

    [SerializeField] UpgradeViewManager upgradeViewManager;
    [SerializeField] GameObject centerUIElements;
    [SerializeField] Image brighteningPanel;
    public CardDescriptionPanel PanelToAnimate{get; private set;}

    [Header("Move To Center Settings")]
    [SerializeField] Ease moveToCenterEase;
    [SerializeField] float moveToCenterDuration;
    [SerializeField] List<Vector3> moveCenterTweenWaypoints;
    Tween moveToCenterTween;

    [Header("Panel Expansion Settings")]
    [SerializeField] Vector3 panelExpandedSize;
    [SerializeField] Ease panelExpansionEase;
    [SerializeField] float panelExpansionDuration;
    Tween panelExpansionTween;


    [Header("Panel Shake Settings")]
    public float shakeDuration = 0.12f;
    public float baseShakeStrength = 0.2f;
    public int shakeVibrato = 1000;
    public float shakeRandomness = 90;
    Tween panelShakeTween;

    [Header("Slam Down Settings")]
    [SerializeField] Vector3 slamDownLocation;
    [SerializeField] Ease slamDownEase;
    [SerializeField] float slamDownDuration;
    Tween slamDownTween;

    [Header("Shrink Settings")]
    [SerializeField] Ease panelShrinkEase;
    [SerializeField] float panelShrinkDuration;
    Tween shrinkTween;

    [Header("Brightening Panel Settings")]
    [SerializeField] float brighteningPanelDuration;
    [SerializeField] float brightAlpha;
    [SerializeField] float fadeoutPanelDuration;
    [SerializeField] Ease brighteningPanelEase;
    [SerializeField] Ease fadeoutPanelEase;

    public bool IsAnimating {get; private set;}

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void InitiateUpgradeAnimation()
    {
        if(upgradeViewManager.SelectedUpgradePanel == null){return;}

        upgradeViewManager.ResetSelectorIndicator();

        PanelToAnimate = upgradeViewManager.SelectedUpgradePanel;
        PanelToAnimate.transform.SetParent(centerUIElements.transform);
        PanelToAnimate.transform.SetAsLastSibling();
        PanelToAnimate.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.5f);
        PanelToAnimate.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f);

        IsAnimating = true;

        StartCoroutine(UpgradeAnimationCoroutine());
    }

    IEnumerator UpgradeAnimationCoroutine()
    {
        MoveToCenter();
        ExpandPanel();
        yield return new WaitForSecondsRealtime(panelExpansionDuration * 0.4f);
        BrightenPanel();
        yield return new WaitForSecondsRealtime(panelExpansionDuration * 0.4f);

        ShakePanel().OnComplete(() => 
        {
            SlamDown();
            DimPanel();
            ShrinkPanel().OnComplete(() => 
            {
                IsAnimating = false;
                OnUpgradeAnimationComplete?.Invoke();
            });
            
        });        
    }

    Tween MoveToCenter()
    {
        moveToCenterTween = PanelToAnimate.transform.DOLocalMove(moveCenterTweenWaypoints[0], moveToCenterDuration)
        .SetEase(moveToCenterEase).SetUpdate(true);

        return moveToCenterTween;
    }

    Tween ExpandPanel()
    {
        panelExpansionTween = PanelToAnimate.transform.DOScale(panelExpandedSize, panelExpansionDuration)
        .SetEase(panelExpansionEase).SetUpdate(true);

        return panelExpansionTween;
    }

    Tween ShakePanel()
    {
        panelShakeTween = PanelToAnimate.transform.DOShakeRotation(shakeDuration, baseShakeStrength, shakeVibrato, shakeRandomness)
        .SetUpdate(true);

        return panelShakeTween;
    }

    Tween SlamDown()
    {
        slamDownTween = PanelToAnimate.transform.DOLocalMove(slamDownLocation, slamDownDuration)
        .SetEase(slamDownEase).SetUpdate(true);

        return slamDownTween;
    }

    Tween ShrinkPanel()
    {
        shrinkTween = PanelToAnimate.transform.DOScale(Vector3.one, panelShrinkDuration)
        .SetEase(panelShrinkEase).SetUpdate(true);

        return shrinkTween;
    }

    Tween BrightenPanel()
    {
        brighteningPanel.gameObject.SetActive(true);
        Tween brightenTween = brighteningPanel.DOFade(brightAlpha, brighteningPanelDuration).SetEase(brighteningPanelEase).SetUpdate(true);
        return brightenTween;
    }

    Tween DimPanel()
    {
        brighteningPanel.gameObject.SetActive(true);
        Tween dimTween = brighteningPanel.DOFade(0f, fadeoutPanelDuration).SetEase(fadeoutPanelEase).SetUpdate(true);
        return dimTween;
    }

}
