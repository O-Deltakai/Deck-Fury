using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using FMODUnity;
using UnityEngine;

public class ComboKillAnnouncer : MonoBehaviour
{
    [SerializeField] ScoreManager scoreManager;

    [SerializeField] GameObject comboKillPopupPrefab;
    [SerializeField] Transform popupParent;
    [SerializeField] Transform initialPoint;
    [SerializeField] Transform stayPoint;
    [SerializeField] Transform endPoint;

    [Header("Popup Tween Settings")]
    [SerializeField] float stayDuration = 1f;
    [SerializeField] float entrySpeed = 0.15f;
    [SerializeField] float exitSpeed = 0.15f;
    [SerializeField] Ease entryEase = Ease.OutCubic;
    [SerializeField] Ease exitEase = Ease.OutCubic;

    [Header("Popup Shake Settings")]
    [SerializeField] float doubleKillShakeStrength = 3f;
    [SerializeField] float tripleKillShakeStrength = 6f;
    [SerializeField] float quadraKillShakeStrength = 8f;
    [SerializeField] float monsterKillShakeStrength = 9f;

    [Header("Popup Number Color Settings")]
    [SerializeField] Color doubleKillColor = Color.white;
    [SerializeField] Color tripleKillColor = Color.white;
    [SerializeField] Color quadraKillColor = Color.white;
    [SerializeField] Color monsterKillColor = Color.white;

    [Header("Popup SFX")]
    [SerializeField] EventReference comboKillSFX;


    [Header("Debug Options")]
    [SerializeField] bool testPopup = false;
    [SerializeField] int testComboCount = 2;

    Queue<GameObject> popupQueue = new();
    EventBinding<ComboKillEvent> comboKillEventBinding;

    private void Awake()
    {
        comboKillEventBinding = new EventBinding<ComboKillEvent>(HandleComboKillEvent);
    }

    void OnEnable()
    {
        EventBus<ComboKillEvent>.Register(comboKillEventBinding);
    }

    void OnDisable()
    {
        EventBus<ComboKillEvent>.Deregister(comboKillEventBinding);
    }

    private void HandleComboKillEvent(ComboKillEvent comboEvent)
    {
        StartCoroutine(TriggerPopupCoroutine(comboEvent.comboCount, GetComboName(comboEvent.comboCount)));
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(testPopup)
        {
            testPopup = false;
            StartCoroutine(TriggerPopupCoroutine(testComboCount, GetComboName(testComboCount)));
        }
    }

    string GetComboName(int comboCount)
    {
        if (comboCount == 2)
        {
            return "Double Kill";
        }
        else if (comboCount == 3)
        {
            return "Triple Kill";
        }
        else if (comboCount == 4)
        {
            return "Quadra Kill";
        }
        else if (comboCount >= 5)
        {
            return "Monster Kill!";
        }
        else
        {
            return "Combo Kill";
        }
    }

    Color GetComboNumberColor(int comboCount)
    {
        if (comboCount == 2)
        {
            return doubleKillColor;
        }
        else if (comboCount == 3)
        {
            return tripleKillColor;
        }
        else if (comboCount == 4)
        {
            return quadraKillColor;
        }
        else if (comboCount >= 5)
        {
            return monsterKillColor;
        }
        else
        {
            return Color.white;
        }
    }

    IEnumerator TriggerPopupCoroutine(int comboCount, string comboName)
    {
        GameObject popup = Instantiate(comboKillPopupPrefab, popupParent);
        popup.transform.localPosition = initialPoint.localPosition;

        //Set popup values
        ComboKillPopup popupScript = popup.GetComponent<ComboKillPopup>();
        popupScript.SetNumber(comboCount);
        popupScript.SetComboName(comboName);
        popupScript.SetNumberColor(GetComboNumberColor(comboCount));

        popupScript.NumberTextShaker.baseShakeStrength = GetShakeStrength(comboCount);
        popupScript.NumberTextShaker.StartShaking();

        //Movement
        Vector3 stayPosition = stayPoint.localPosition;

        //If there are more than 1 popup in the queue, adjust the stay position
        if(popupQueue.Count > 0)
        {
            stayPosition = new Vector3(stayPoint.localPosition.x, stayPoint.localPosition.y - popupScript.rectTransform.rect.height * popupQueue.Count - 1,
            stayPoint.localPosition.z);
        }

        popupQueue.Enqueue(popup);
        popup.transform.DOLocalMove(stayPosition, entrySpeed).SetEase(entryEase).SetUpdate(true);

        yield return new WaitForSeconds(stayDuration + entrySpeed);

        popupQueue.Dequeue();
        UpdatePopupPositions();

        popup.transform.DOLocalMove(endPoint.localPosition, exitSpeed).SetEase(exitEase).OnComplete(() => 
        {
            Destroy(popup);
        }).SetUpdate(true);

    }

/// <summary>
/// Updates the positions of the popups in the queue as the popups come and go.
/// </summary>
    void UpdatePopupPositions()
    {
        if(popupQueue.Count == 0)
        {
            return;
        }

        int index = 0;
        foreach (var popup in popupQueue)
        {
            float popupHeight = popup.GetComponent<ComboKillPopup>().rectTransform.rect.height;
            Vector3 stayPosition = new Vector3(stayPoint.localPosition.x, stayPoint.localPosition.y - popupHeight * index,
            stayPoint.localPosition.z);

            popup.transform.DOLocalMove(stayPosition, entrySpeed).SetEase(entryEase).SetUpdate(true);
            index++;
        }
    }

    float GetShakeStrength(int comboCount)
    {
        if (comboCount == 2)
        {
            return doubleKillShakeStrength;
        }
        else if (comboCount == 3)
        {
            return tripleKillShakeStrength;
        }
        else if (comboCount == 4)
        {
            return quadraKillShakeStrength;
        }
        else if (comboCount >= 5)
        {
            return monsterKillShakeStrength;
        }
        else
        {
            return 0f;
        }
    }
}
