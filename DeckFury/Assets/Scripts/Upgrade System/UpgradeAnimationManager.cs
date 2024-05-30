using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UpgradeAnimationManager : MonoBehaviour
{
    [SerializeField] UpgradeViewManager upgradeViewManager;
    [SerializeField] GameObject centerUIElements;
    CardDescriptionPanel panelToAnimate;

    [Header("Move To Center Settings")]
    [SerializeField] Ease moveToCenterEase;
    [SerializeField] float moveToCenterDuration;
    [SerializeField] List<Vector3> moveCenterTweenWaypoints;
    Tween moveToCenterTween;

    [Header("Panel Expansion Settings")]
    [SerializeField] float panelExpandedSize;
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

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void InitiateUpgradeAnimation()
    {

    }

    void MoveToCenter()
    {

    }

    void ExpandPanel()
    {

    }

    void ShakePanel()
    {

    }

    void SlamDown()
    {

    }

    void ShrinkPanel()
    {

    }

}
