using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;
using Unity.Mathematics;
using System;

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
    [SerializeField, Min(0)] int _numberOfActions = 5;
    public int NumberOfActions => _numberOfActions;

    [Tooltip("Duration of focus mode before it expires")]
    [SerializeField, Min(0)] float _focusModeDuration = 10;
    public float FocusModeDuration => _focusModeDuration;

    [Tooltip("Number of actions remaining in focus mode")]
    [SerializeField] int _currentActionsRemaining = 5;
    public int CurrentActionsRemaining => _currentActionsRemaining;

    [Tooltip("Time remaining in focus mode")]
    [SerializeField] float _timeRemaining = 10;
    public float TimeRemaining => _timeRemaining;

    Coroutine CR_FocusModeTimer = null;

    [Header("UI Elements")]

    [SerializeField] GameObject _focusModeUI;
    [SerializeField] GameObject _actionsLeftElement;
    [SerializeField] TextMeshProUGUI _actionsCounterText;

    [SerializeField] GameObject _focusMeterElement;
    [SerializeField] ResourceMeter _focusModeDurationMeter;
    [SerializeField] TextMeshProUGUI _durationTimerText;



    [Header("Testing Properties")]
    [SerializeField] bool testButtonQ = false;
    [SerializeField] float speedUpTimeDuration = 1f;
    [SerializeField] float speedUpTimeGrowthRate = 1f;

#region Tweens
    Tween TW_actionsLeftSlideTween;
    Vector3 actionsLeftElementStartPos;

#endregion
    void OnDestroy()
    {
        _instance = null;
    }

    void Awake()
    {
        _instance = this;
        actionsLeftElementStartPos = _actionsLeftElement.transform.localPosition;

    }
    
    void Start()
    {
        _focusModeDurationMeter.SetMaxFloatValue(_focusModeDuration);
        _focusModeDurationMeter.CurrentFloatValue = _focusModeDuration;

        _durationTimerText.gameObject.SetActive(false);
        _actionsLeftElement.SetActive(false);

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


    public void ActivateFocusMode()
    {   
        //Assign states
        IsFocusModeActive = true;
        _currentActionsRemaining = _numberOfActions;
        _timeRemaining = _focusModeDuration;
        _focusModeDurationMeter.CurrentFloatValue = _timeRemaining;

        _actionsCounterText.text = _currentActionsRemaining.ToString();
        _durationTimerText.gameObject.SetActive(true);
        _actionsLeftElement.SetActive(true);



        PlayerController player = GameManager.Instance.player;
        player.OnPerformAction += DecrementActions;
        player.GetComponent<AfterImageGenerator>().Play();


        StartFocusModeTimer();

        TimeManager.Instance.LerpTimeToZero(1f, 8f);
        
        OnActivateFocusMode?.Invoke();

        if(TW_actionsLeftSlideTween.IsActive())
        {
            TW_actionsLeftSlideTween.Kill();
        }

        _actionsLeftElement.transform.localPosition = new Vector3(-1100, actionsLeftElementStartPos.y, actionsLeftElementStartPos.z);
        TW_actionsLeftSlideTween = _actionsLeftElement.transform.DOLocalMove(actionsLeftElementStartPos, 0.2f).SetUpdate(true).SetEase(Ease.OutCirc);


    }

    public void DeactivateFocusMode()
    {
    
        IsFocusModeActive = false;
        _focusModeDurationMeter.CurrentFloatValue = 0;
        _durationTimerText.gameObject.SetActive(false);

        PlayerController player = GameManager.Instance.player;
        player.OnPerformAction -= DecrementActions;
        player.GetComponent<AfterImageGenerator>().Stop();

        TimeManager.Instance.LerpTimeToOne(speedUpTimeDuration, speedUpTimeGrowthRate);


        StopFocusModeTimer();

        OnDeactivateFocusMode?.Invoke();

        if(TW_actionsLeftSlideTween.IsActive())
        {
            TW_actionsLeftSlideTween.Kill();
        }

        IEnumerator TimerBeforeDisable()
        {
            yield return new WaitForSecondsRealtime(0.5f);
            _actionsLeftElement.SetActive(false);
        }
        TW_actionsLeftSlideTween = _actionsLeftElement.transform.DOLocalMoveX(-1100, 0.2f).SetUpdate(true).SetEase(Ease.InBack);
        StartCoroutine(TimerBeforeDisable());

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

        //yield return new WaitForSecondsRealtime(duration);

        DeactivateFocusMode();

        CR_FocusModeTimer = null;

    }



}
