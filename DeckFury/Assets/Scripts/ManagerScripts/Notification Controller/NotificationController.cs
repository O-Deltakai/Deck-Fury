using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using FMODUnity;
using UnityEngine;

public class NotificationController : MonoBehaviour
{

    static NotificationController _instance;
    public static NotificationController Instance { get => _instance; }

    [SerializeField] NotificationTypeIconBindingSO notificationTypeIconBinding;
    [SerializeField] Transform popupAnchor;
    [SerializeField] GameObject notificationPopupPrefab;
    [SerializeField] float popupYEndPosition = -150;



    [Header("Tween Settings")]
    [SerializeField] float popupDuration = 0.5f; // How long the popup stays on screen
    [SerializeField] float popupSpeed = 0.5f; // Speed at which the popup moves to its end position
    [SerializeField] Ease popupMoveInEase = Ease.OutBack;
    [SerializeField] Ease popupMoveOutEase = Ease.InBack;


    [Header("SFX")]
    [SerializeField] EventReference notificationSFX;

    //Tweens
    Tween popupMoveInTween;
    Tween popupMoveOutTween;

    Coroutine CR_PopupDurationTimer;

    Stack<NotificationPopup> notificationStack = new Stack<NotificationPopup>();

    EventBinding<NotificationEvent> notificationEventBinding;


    private void Awake()
    {
        _instance = this;

        notificationEventBinding = new EventBinding<NotificationEvent>(RecieveNotification);
        EventBus<NotificationEvent>.Register(notificationEventBinding);
    }

    void OnDestroy()
    {
        EventBus<NotificationEvent>.Deregister(notificationEventBinding);
        _instance = null;
    }

    void Update()
    {
        if(notificationStack.Count > 0 && CR_PopupDurationTimer == null)
        {
            DisplayNotification();
        }
    }



    void RecieveNotification(NotificationEvent notificationEvent)
    {
        NotificationData notificationData = notificationEvent.notification;

        NotificationPopup notificationPopup = Instantiate(notificationPopupPrefab, popupAnchor).GetComponent<NotificationPopup>();
        Sprite icon = notificationTypeIconBinding.GetIcon(notificationData.notificationType);
        notificationPopup.Initialize(notificationData, icon);


        notificationStack.Push(notificationPopup);

    }

    void DisplayNotification()
    {
        // If there is already a popup on screen, return
        if(CR_PopupDurationTimer != null) { return; }

        RuntimeManager.PlayOneShot(notificationSFX);

        CR_PopupDurationTimer = StartCoroutine(PopupDurationTimer());
    }

    void HideNotification(GameObject notificationPopup)
    {
        notificationPopup.GetComponent<RectTransform>().DOAnchorPosY(-popupYEndPosition, popupSpeed).SetUpdate(true).SetEase(popupMoveOutEase).OnComplete(() => 
        {
            Destroy(notificationPopup);
        });
    }

    IEnumerator PopupDurationTimer()
    {
        GameObject notificationPopup = notificationStack.Pop().gameObject;
        notificationPopup.GetComponent<RectTransform>().DOAnchorPosY(popupYEndPosition, popupSpeed).SetEase(popupMoveInEase).SetUpdate(true);


        // Wait for the popup duration as well as the speed of the popup moving in/out for smooth transition
        yield return new WaitForSecondsRealtime(popupDuration + popupSpeed);
        HideNotification(notificationPopup);
        yield return new WaitForSecondsRealtime(popupSpeed);


        CR_PopupDurationTimer = null; 
    }



}
