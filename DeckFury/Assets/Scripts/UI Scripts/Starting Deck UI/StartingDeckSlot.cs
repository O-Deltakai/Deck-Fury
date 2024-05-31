using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class StartingDeckSlot : MonoBehaviour
{
    public event Action<DeckSO> OnDeckSelected;

    [SerializeField] bool _unlocked = true;
    public bool Unlocked { get => _unlocked;
        set 
        { 
            _unlocked = value; 
            UnlockSlot(value);
        }
    }

    [SerializeField] DeckSO deckSO;
    [SerializeField] TextMeshProUGUI deckNameText;
    [SerializeField] GameObject deckElementSlotPrefab;
    [SerializeField] Transform deckElementSlotParent;

    [Header("Unlock Condition")]
    [SerializeField] GameObject unlockConditionPanel;
    [SerializeField] TextMeshProUGUI unlockConditionText;
    IReadOnlyList<StatUnlockCondition> unlockConditions;


[Header("Tween Settings")]
    [SerializeField] float hoverEndScale = 1;
    float originalScale;
    [SerializeField] float scaleForwardSpeed = 0.2f;
    [SerializeField] float scaleBackwardSpeed = 0.2f;
    [SerializeField] Ease scaleForwardEase = Ease.OutBack;
    [SerializeField] Ease scaleBackwardEase = Ease.OutBack;

    //Tweens
    Tween scaleForwardTween;
    Tween scaleBackwardTween;


    void Awake()
    {
        originalScale = transform.localScale.x;
    }

    void Start()
    {
        if (deckSO != null)
        {
            InitializeStartingDeckSlot(deckSO);
 
        }
    }

    void OnValidate()
    {
        if(unlockConditionPanel)
        {
            UnlockSlot(_unlocked);
        }
    }

    void EvaluateUnlockConditions()
    {
        unlockConditionText.text = "";

        if (unlockConditions == null || unlockConditions.Count == 0)
        {
            Unlocked = true;
            return;
        }

        for (int i = 0; i < unlockConditions.Count; i++)
        {
            var condition = unlockConditions[i];

            unlockConditionText.text += condition.ConditionName;
            if (i < unlockConditions.Count - 1)
            {
                unlockConditionText.text += ",\n\n";
            }

            if (!condition.Evaluate())
            {
                Unlocked = false;
            }
        }

    }

    public void InitializeStartingDeckSlot(DeckSO deckSO)
    {
        //If there are any deck elements in the slot, destroy them
        foreach (Transform child in deckElementSlotParent)
        {
            Destroy(child.gameObject);
        }

        this.deckSO = deckSO;
        deckNameText.text = deckSO.DeckName;

        foreach (var deckElement in deckSO.CardListReadOnly)
        {
            DeckCardElementSlot deckCardElementSlot = Instantiate(deckElementSlotPrefab, deckElementSlotParent)
                .GetComponent<DeckCardElementSlot>();

            deckCardElementSlot.AssignDeckElement(deckElement);
        }

        unlockConditions = deckSO.UnlockConditions;
        EvaluateUnlockConditions();
    }

    void UnlockSlot(bool condition)
    {
        unlockConditionPanel.SetActive(!condition);
    }


    void ScaleForward()
    {
        if(scaleBackwardTween.IsActive())
            scaleBackwardTween.Kill();

        scaleForwardTween = transform.DOScale(hoverEndScale, scaleForwardSpeed).SetEase(scaleForwardEase);
    }

    void ScaleBackward()
    {
        if(scaleForwardTween.IsActive())
            scaleForwardTween.Kill();

        scaleBackwardTween = transform.DOScale(originalScale, scaleBackwardSpeed).SetEase(scaleBackwardEase);
    }

    public void OnHover()
    {
        if(!_unlocked){return;}
        ScaleForward();
    }

    public void ExitHover()
    {
        if(!_unlocked){return;}
        ScaleBackward();
    }

    public void ClickSelectButton()
    {
        if(!_unlocked){return;}
        OnDeckSelected?.Invoke(deckSO);

    }


}
