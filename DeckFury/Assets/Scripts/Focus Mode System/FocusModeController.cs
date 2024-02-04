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


    [SerializeField, Min(0)] int _numberOfActions = 5;
    public int NumberOfActions => _numberOfActions;

    [SerializeField, Min(0)] float _focusModeDuration = 10;
    public float FocusModeDuration => _focusModeDuration;

    [SerializeField] int _currentActionsRemaining = 5;
    public int CurrentActionsRemaining => _currentActionsRemaining;
    [SerializeField] float _timeRemaining = 10;
    public float TimeRemaining => _timeRemaining;

    Coroutine CR_FocusModeTimer = null;

    [Header("UI Elements")]

    [SerializeField] GameObject _focusModeUI;
    [SerializeField] TextMeshProUGUI _actionsCounterText;
    [SerializeField] TextMeshProUGUI _durationTimerText;
    [SerializeField] ResourceMeter _focusModeDurationMeter;

    [SerializeField] ResourceMeter _focusMeter;

    [SerializeField] bool testButtonQ = false;

    [Header("Testing Properties")]
    [SerializeField] float speedUpTimeDuration = 1f;
    [SerializeField] float speedUpTimeGrowthRate = 1f;

    void OnDestroy()
    {
        _instance = null;
    }

    void Awake()
    {
        _instance = this;
    
    }
    
    void Start()
    {
        _focusModeDurationMeter.SetMaxFloatValue(_focusModeDuration);
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


        IsFocusModeActive = true;
        _currentActionsRemaining = _numberOfActions;
        _timeRemaining = _focusModeDuration;

        _focusModeUI.SetActive(true);
        _actionsCounterText.text = _currentActionsRemaining.ToString();


        PlayerController player = GameManager.Instance.player;
        player.OnPerformAction += DecrementActions;
        player.GetComponent<AfterImageGenerator>().Play();


        StartFocusModeTimer();

        TimeManager.Instance.LerpTimeToZero(1f, 8f);
        

        OnActivateFocusMode?.Invoke();

    }

    public void DeactivateFocusMode()
    {
        

        IsFocusModeActive = false;
        _focusModeUI.SetActive(false);


        PlayerController player = GameManager.Instance.player;
        player.OnPerformAction -= DecrementActions;
        player.GetComponent<AfterImageGenerator>().Stop();

        TimeManager.Instance.LerpTimeToOne(speedUpTimeDuration, speedUpTimeGrowthRate);


        StopFocusModeTimer();

        OnDeactivateFocusMode?.Invoke();
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
