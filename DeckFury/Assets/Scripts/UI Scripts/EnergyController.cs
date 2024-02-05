using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using FMODUnity;
using System;



public class EnergyController : MonoBehaviour
{
    public event Action OnFullCharge;


    private static EnergyController _instance;
    public static EnergyController Instance => _instance; 


    [SerializeField] bool _fullCharge = false;

    [Min(0.01f)] public float chargeRateModifier = 1f;

    [SerializeField] float yAnchorPosition;

    [SerializeField] CardSelectionMenu cardSelectionMenu;

    [SerializeField] bool AlwaysFullCharge;
    [SerializeField] bool disableEnergyBar;



    [SerializeField] Image fillImage;
    Color fillOriginalColor;

    [SerializeField] Image highlighterBox;
    
    [SerializeField] RectTransform anchorPoint;

    [SerializeField] TextMeshProUGUI energyNotFullText;
    [SerializeField] GameObject pressTabText;

    bool fullChargeEventTrigger = true;

    [SerializeField] Image arrowImage;

[Header("Energy Bar Settings")]
    [Tooltip("The time it takes to charge the energy bar to full")]
    [SerializeField] float energyChargeTime = 8f;
    [SerializeField] ResourceMeter energyMeter;

    public float CurrentEnergyValue
    {
        get => energyMeter.CurrentFloatValue;
        set => energyMeter.CurrentFloatValue = value;
    }

    public float MaxEnergy => energyMeter.MaxFloatValue;

[Header("SFX")]
    [SerializeField] EventReference energyFullSFX;

    bool canCharge = true;


    Coroutine CR_ChargeEnergyCoroutine = null;

#region Tweens

    Tween energyBarMovementTween;

#endregion


    private void Awake() 
    {
        _instance = this;
        if(pressTabText){ pressTabText.SetActive(false); }


        energyMeter.SetMaxFloatValue(energyChargeTime);

    }

    void OnDestroy()
    {
        _instance = null;
    }

    private void InitializeStartVariables()
    {

        cardSelectionMenu.OnMenuDisabled += MoveIntoView;
        cardSelectionMenu.OnMenuActivated += MoveOutOfView;


        yAnchorPosition = transform.parent.localPosition.y;


        fillOriginalColor = fillImage.color;
 
        arrowImage.color = fillOriginalColor;

    }

    // Start is called before the first frame update
    void Start()
    {
        InitializeStartVariables();
        MoveOutOfView();

    }



    public void DisableEnergyBar()
    {
        AlwaysFullCharge = true;
        disableEnergyBar = true;
    }

    public void EnableEnergyBar()
    {
        AlwaysFullCharge = false;
        disableEnergyBar = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void HideEnergyBar()
    {
        if(energyBarMovementTween.IsActive()){energyBarMovementTween.Kill();}
        energyBarMovementTween = anchorPoint.DOLocalMoveY(600, 0.15f).SetUpdate(true);
    }

    public void UnhideEnergyBar()
    {
        if(energyBarMovementTween.IsActive()){energyBarMovementTween.Kill();}
        energyBarMovementTween = anchorPoint.DOLocalMoveY(yAnchorPosition, 0.45f).SetUpdate(true);
    }

    public void MoveIntoView()
    {
        if(disableEnergyBar){return;}

        if(energyBarMovementTween.IsActive()){energyBarMovementTween.Kill();}
        energyBarMovementTween = anchorPoint.DOLocalMoveY(yAnchorPosition, 0.45f).SetUpdate(true);
        
        ResetEnergyMeter();
        StartCoroutine(FlashBarArrowWhileCharging());
    }

    public void MoveOutOfView()
    {
        if(energyBarMovementTween.IsActive()){energyBarMovementTween.Kill();}
        energyBarMovementTween = anchorPoint.DOLocalMoveY(600, 0.15f).SetUpdate(true);
        fillImage.color = fillOriginalColor;
        highlighterBox.DOFade(0f, 0.1f);

        _fullCharge = false;

        if(pressTabText){ pressTabText.SetActive(false); }
        StopCharging();
    }

    public void StopCharging()
    {
        if(CR_ChargeEnergyCoroutine != null)
        {
            StopCoroutine(CR_ChargeEnergyCoroutine);
        }
    }

    public void ResumeCharging()
    {
        if(CR_ChargeEnergyCoroutine != null)
        {
            StopCoroutine(CR_ChargeEnergyCoroutine);
        }
        CR_ChargeEnergyCoroutine = StartCoroutine(ChargeEnergyCoroutine());
    }


    public void ResetEnergyMeter()
    {
        if(CR_ChargeEnergyCoroutine != null)
        {
            StopCoroutine(CR_ChargeEnergyCoroutine);
        }

        energyMeter.CurrentFloatValue = 0;
        if(pressTabText){ pressTabText.SetActive(false); }

        _fullCharge = false;

        CR_ChargeEnergyCoroutine = StartCoroutine(ChargeEnergyCoroutine());
    }



    Tween energyNotFullTextFadeTween;
    //return true if energy full
    public bool EnergyIsFull()
    {
        if(AlwaysFullCharge){return true;} 

        if(!_fullCharge)
        {
            if(energyNotFullTextFadeTween.IsActive())
            {
                energyNotFullTextFadeTween.Kill();
            }
            energyNotFullTextFadeTween = energyNotFullText.DOFade(1, 0.1f).SetUpdate(true).
            OnComplete(() => energyNotFullTextFadeTween = energyNotFullText.DOFade(0, 0.2f).SetUpdate(true).SetDelay(0.5f));
        }
        return _fullCharge;
    }


#region Coroutines
    IEnumerator ChargeEnergyCoroutine()
    {
        if(_fullCharge){yield break;}

        while(energyMeter.CurrentFloatValue < energyMeter.MaxFloatValue)
        {
            energyMeter.CurrentFloatValue += Time.deltaTime * chargeRateModifier;
            yield return null;
        }
        
        _fullCharge = true;

        energyMeter.CurrentFloatValue = energyMeter.MaxFloatValue;
        if(pressTabText){ pressTabText.SetActive(true); }
        RuntimeManager.PlayOneShot(energyFullSFX);
        StartCoroutine(FlashBarColorWhileFull());
        highlighterBox.DOFade(1f, 0.1f);

        CR_ChargeEnergyCoroutine = null;
        OnFullCharge?.Invoke();
        
    }

    IEnumerator FlashBarColorWhileFull()
    {
        if(!fullChargeEventTrigger){yield break;}

        while(_fullCharge)
        {
            fillImage.DOColor(Color.green, 0.5f).SetUpdate(true);
            yield return new WaitForSecondsRealtime(0.5f);
            fillImage.DOColor(fillOriginalColor, 0.5f).SetUpdate(true);
            yield return new WaitForSecondsRealtime(0.5f);

        }
    }

    IEnumerator FlashBarArrowWhileCharging()
    {
        while(true)
        {
            if(_fullCharge){yield break;}
            float flashSpeed = 1.0f/chargeRateModifier;
            arrowImage.DOFade(1, flashSpeed).SetUpdate(false);
            yield return new WaitForSecondsRealtime(flashSpeed);
            arrowImage.DOFade(0, flashSpeed).SetUpdate(false);
            yield return new WaitForSecondsRealtime(flashSpeed);
        }
    }


#endregion

}
