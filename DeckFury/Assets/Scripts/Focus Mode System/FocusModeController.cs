using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FocusModeController : MonoBehaviour
{
    static FocusModeController _instance;
    public static FocusModeController Instance => _instance;

    public static bool IsFocusModeActive { get; private set; }


    [SerializeField, Min(0)] int _numberOfActions = 5;
    public int NumberOfActions => _numberOfActions;

    [SerializeField, Min(0)] float _focusModeDuration = 10;
    public float FocusModeDuration => _focusModeDuration;

    [SerializeField] int _currentActionsRemaining = 5;
    public int CurrentActionsRemaining => _currentActionsRemaining;


    Coroutine CR_FocusModeTimer = null;





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
        
    }


    public void ActivateFocusMode()
    {

    }

    public void DeactivateFocusMode()
    {

    }

    IEnumerator FocusModeTimer(float duration)
    {
        yield return new WaitForSecondsRealtime(duration);

        DeactivateFocusMode();

    }



}
