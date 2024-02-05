using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;
using Unity.Mathematics;
using System;
using UnityEngine.UI;

public class FocusModeController : MonoBehaviour
{
    public event Action OnDecrementActions;
    public event Action OnActivateFocusMode;
    public event Action OnDeactivateFocusMode;

    static FocusModeController _instance;
    public static FocusModeController Instance => _instance;

    public static bool IsFocusModeActive { get; private set; } = false;

[Header("Focus Mode Properties")]
    [Tooltip("Number of actions the player can perform while in focus mode")]
    [SerializeField, Min(1)] int _maxNumberOfActions = 5;
    public int MaxNumberOfActions {get => _maxNumberOfActions; 
    set
    {
        if(value < 1){value = 1;}
        _maxNumberOfActions = value;}
    }
    

    [Tooltip("Duration of focus mode before it expires")]
    [SerializeField, Min(0)] float _focusModeDuration = 10;
    public float FocusModeDuration => _focusModeDuration;

    [Tooltip("Number of actions remaining in focus mode")]
    [SerializeField] int _currentActionsRemaining = 5;
    public int CurrentActionsRemaining => _currentActionsRemaining;

    [Tooltip("Current time remaining in focus mode")]
    [SerializeField] float _timeRemaining = 10;
    public float TimeRemaining => _timeRemaining;

    [SerializeField] bool _canActivateFocusMode = true;
    public bool CanActivateFocusMode => _canActivateFocusMode;

    Coroutine CR_FocusModeTimer = null;

    [Header("UI Elements")]

    [SerializeField] GameObject _focusModeUI;

    [Header("Actions Left UI")]
    [SerializeField] GameObject _actionsLeftElement;
    [SerializeField] TextMeshProUGUI _actionsCounterText;
    [SerializeField] TextMeshProUGUI _blinkerText;
    [SerializeField] TextMeshProUGUI _centerScreenBlinkerText;

    [Header("Focus Mode Duration UI")]
    [SerializeField] GameObject _focusMeterElement;
    [SerializeField] ResourceMeter _focusModeDurationMeter;
    [SerializeField] TextMeshProUGUI _durationTimerText;

    [Header("Focus Mode Bar Visuals")]
    [SerializeField] Image _focusModeDurationBarFill;
    [SerializeField] Color _focusModeNotFullBarColor;
    [SerializeField] Color _focusModeFullBarColor;


    [SerializeField] GameObject _focusModeOnTextObject;
    Vector3 _focusModeOnTextStartPos;

    [Header("Focus Mode Mechanics Settings")]
    [SerializeField] float _restoreFocusAmountOnOpenCardSelect = 0.25f;
    [SerializeField] float _restoreFocusAmountOnKillEnemy = 0.05f;

    [Header("Testing Properties")]
    [SerializeField] bool testButtonQ = false;
    [SerializeField] float speedUpTimeDuration = 1f;
    [SerializeField] float speedUpTimeGrowthRate = 1f;

#region Tweens
    Tween TW_actionsLeftSlideTween;
    Vector3 actionsLeftElementStartPos;

    Tween TW_focusModeOnTextTween;

    Tween TW_blinkerTextScaleTween;
    Tween TW_blinkerTextFadeTween;
    Tween TW_centerScreenBlinkerTextScaleTween;
    Tween TW_centerScreenBlinkerTextFadeTween;


#endregion
    void OnDestroy()
    {
        _instance = null;
    }

    void Awake()
    {
        _instance = this;
        actionsLeftElementStartPos = _actionsLeftElement.transform.localPosition;
        _focusModeOnTextStartPos = _focusModeOnTextObject.transform.localPosition;

    }
    
    void Start()
    {
        _focusModeDurationMeter.SetMaxFloatValue(_focusModeDuration);
        _focusModeDurationMeter.CurrentFloatValue = _focusModeDuration;
        _focusModeDurationBarFill.color = _focusModeFullBarColor;


        _durationTimerText.gameObject.SetActive(false);
        _actionsLeftElement.SetActive(false);
        _focusModeOnTextObject.SetActive(false);
        _centerScreenBlinkerText.gameObject.SetActive(false);

        CardSelectionMenu.Instance.OnMenuActivated += () => RestoreFocus(_restoreFocusAmountOnOpenCardSelect);
        GameManager.Instance.player.OnKillSpecificEnemy += (enemy) =>
        {
            if(enemy.EnemyData.EnemyTier == 0)
            {
                RestoreFocus(_restoreFocusAmountOnKillEnemy);
            }else
            {
                RestoreFocus(_restoreFocusAmountOnKillEnemy * enemy.EnemyData.EnemyTier);
            }
        };
            
    }

    void Update()
    {
        if(testButtonQ)
        {
            if(Input.GetKeyDown(KeyCode.Q))
            {
                if(!IsFocusModeActive)
                {
                    ActivateFocusMode();
                }else
                {
                    DeactivateFocusMode();
                }
            }
        }
    }

/// <summary>
/// Restore a percentage of the focus meter with a value between 0 and 1
/// </summary>
/// <param name="amount"></param>
    public void RestoreFocus(float amount)
    {
        if(IsFocusModeActive){ return; }
        if(_canActivateFocusMode){ return; }
        if(amount < 0) { return; }
        if(amount > 1) { amount = 1; }

        print("Restoring focus: " + amount);

        float restoredValue = _focusModeDuration * amount;
        _focusModeDurationMeter.CurrentFloatValue += restoredValue;

        if(_focusModeDurationMeter.CurrentFloatValue >= _focusModeDuration)
        {
            _canActivateFocusMode = true;
            _focusModeDurationBarFill.color = _focusModeFullBarColor;
        }
    }


    void BlinkActionsLeftText()
    {
        float blinkDuration = 0.3f;

    //Blinker text
        if(TW_blinkerTextScaleTween.IsActive()){TW_blinkerTextScaleTween.Kill();}
        if(TW_blinkerTextFadeTween.IsActive()){TW_blinkerTextFadeTween.Kill();}

        _blinkerText.text = _currentActionsRemaining.ToString();
        _blinkerText.alpha = 1;
        _blinkerText.transform.localScale = Vector3.one;
        _blinkerText.gameObject.SetActive(true);

        TW_blinkerTextScaleTween = _blinkerText.transform.DOScale(3f, blinkDuration).SetUpdate(true).SetEase(Ease.OutQuint);
        TW_blinkerTextFadeTween = _blinkerText.DOFade(0, blinkDuration).SetUpdate(true).SetEase(Ease.Linear);

    //Center screen blinker text
        if(TW_centerScreenBlinkerTextScaleTween.IsActive()){TW_centerScreenBlinkerTextScaleTween.Kill();}
        if(TW_centerScreenBlinkerTextFadeTween.IsActive()){TW_centerScreenBlinkerTextFadeTween.Kill();}

        _centerScreenBlinkerText.text = _currentActionsRemaining.ToString();
        _centerScreenBlinkerText.alpha = 0.25f;
        _centerScreenBlinkerText.transform.localScale = Vector3.one;
        _centerScreenBlinkerText.gameObject.SetActive(true);

        TW_centerScreenBlinkerTextScaleTween = _centerScreenBlinkerText.transform.DOScale(2f, blinkDuration).SetUpdate(true).SetEase(Ease.OutQuint);
        TW_centerScreenBlinkerTextFadeTween = _centerScreenBlinkerText.DOFade(0, blinkDuration).SetUpdate(true).SetEase(Ease.Linear);
    }

    public void TriggerFocusMode()
    {
        if(_canActivateFocusMode)
        {
            ActivateFocusMode();
            _canActivateFocusMode = false;
        }
    }

    public void ActivateFocusMode()
    {
        //Assign states
        _canActivateFocusMode = false;
        IsFocusModeActive = true;
        _currentActionsRemaining = _maxNumberOfActions;
        _timeRemaining = _focusModeDuration;
        _focusModeDurationMeter.CurrentFloatValue = _timeRemaining;

        _actionsCounterText.text = _currentActionsRemaining.ToString();
        _durationTimerText.gameObject.SetActive(true);
        _actionsLeftElement.SetActive(true);
        _focusModeDurationBarFill.color = _focusModeFullBarColor;

        StagePostProcessingController.Instance.TriggerFocusModePostProcessing();

        PlayerController player = GameManager.Instance.player;
        player.OnPerformAction += DecrementActions;
        player.OnPerformAction += BlinkActionsLeftText;
        player.GetComponent<AfterImageGenerator>().Play();
        EnergyController.Instance.HideEnergyBar();
        CardSelectionMenu.Instance.CanBeOpened = false;


        StartFocusModeTimer();

        TimeManager.Instance.LerpTimeToZero(1f, 8f);
        
        OnActivateFocusMode?.Invoke();

        if(TW_actionsLeftSlideTween.IsActive())
        {
            TW_actionsLeftSlideTween.Kill();
        }

        _actionsLeftElement.transform.localPosition = new Vector3(-1100, actionsLeftElementStartPos.y, actionsLeftElementStartPos.z);
        TW_actionsLeftSlideTween = _actionsLeftElement.transform.DOLocalMove(actionsLeftElementStartPos, 0.2f).SetUpdate(true).SetEase(Ease.OutCirc);

        //Animate focus mode on text
        if(TW_focusModeOnTextTween.IsActive()){TW_focusModeOnTextTween.Kill();}
        _focusModeOnTextObject.SetActive(true);
        _focusModeOnTextObject.transform.localPosition = new Vector3(_focusModeOnTextStartPos.x, 1000, _focusModeOnTextStartPos.z);
        TW_focusModeOnTextTween = _focusModeOnTextObject.transform.DOLocalMoveY(_focusModeOnTextStartPos.y, 0.25f).SetUpdate(true).SetEase(Ease.InOutSine);

    }

    public void DeactivateFocusMode()
    {
    
        IsFocusModeActive = false;
        _focusModeDurationMeter.CurrentFloatValue = 0;
        _durationTimerText.gameObject.SetActive(false);
        _focusModeDurationBarFill.color = _focusModeNotFullBarColor;



        PlayerController player = GameManager.Instance.player;
        player.OnPerformAction -= DecrementActions;
        player.OnPerformAction -= BlinkActionsLeftText;
        player.GetComponent<AfterImageGenerator>().Stop();

        TimeManager.Instance.LerpTimeToOne(speedUpTimeDuration, speedUpTimeGrowthRate);
        StagePostProcessingController.Instance.ResetFocusModePostProcessing();

        StartCoroutine(WaitUntilTimeScaleIsOne());

        static IEnumerator WaitUntilTimeScaleIsOne()
        {
            yield return new WaitUntil(() => Time.timeScale == 1);

            EnergyController.Instance.UnhideEnergyBar();
            CardSelectionMenu.Instance.CanBeOpened = true;
        }



        StopFocusModeTimer();

        OnDeactivateFocusMode?.Invoke();

        if(TW_actionsLeftSlideTween.IsActive()){TW_actionsLeftSlideTween.Kill();}
        TW_actionsLeftSlideTween = _actionsLeftElement.transform.DOLocalMoveX(-1100, 0.2f).SetUpdate(true).SetEase(Ease.InBack).
        OnComplete(() => _actionsLeftElement.SetActive(false));



        //Animate focus mode on text
        if(TW_focusModeOnTextTween.IsActive()){TW_focusModeOnTextTween.Kill();}
        TW_focusModeOnTextTween = _focusModeOnTextObject.transform.DOLocalMoveY(700, 0.25f).SetUpdate(true).SetEase(Ease.InOutSine).
        OnComplete(() => _focusModeOnTextObject.SetActive(false));




    }

    void DecrementActions()
    {
        _currentActionsRemaining--;
        OnDecrementActions?.Invoke();
        _actionsCounterText.text = _currentActionsRemaining.ToString();

        if(_currentActionsRemaining <= 0)
        {
            DeactivateFocusMode();
        }
    }

    public void StartFocusModeTimer()
    {
        CR_FocusModeTimer = StartCoroutine(FocusModeTimer(_focusModeDuration));
    }

    public void StopFocusModeTimer()
    {
        if(CR_FocusModeTimer != null)
        {
            StopCoroutine(CR_FocusModeTimer);
        }
    }

    IEnumerator FocusModeTimer(float duration)
    {
        while(_timeRemaining > 0)
        {
            _timeRemaining -= Time.unscaledDeltaTime;
            _focusModeDurationMeter.CurrentFloatValue = _timeRemaining;
            _durationTimerText.text = _timeRemaining.ToString("F2") + "s";
            yield return null;
        }
        DeactivateFocusMode();

        CR_FocusModeTimer = null;

    }



}
