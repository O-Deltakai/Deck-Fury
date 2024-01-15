using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using FMODUnity;



public class EnergyController : MonoBehaviour
{

    private static EnergyController _instance;
    public static EnergyController Instance => _instance; 

    //set variable
    //max value of enery bar
    [SerializeField] float max;
    //duration for 1 energy charge
    [SerializeField] float chargeTime = 2.0f;
    //default energy
    [SerializeField] int energy = 5;
    //cool down
    [SerializeField] float coolDown = 3.0f;
    //charge time for this turn
    [SerializeField] float currentEnergyValue=0;
    //full charge, true when energy bar full
    [SerializeField] bool fullCharge = false;
    //charge for each frame
    [SerializeField] float chargeRate = 2.0f;
    [Min(0.01f)] public float chargeRateModifier = 1f;

    [SerializeField] float yAnchorPosition;

    [SerializeField] CardSelectionMenu cardSelectionMenu;

    [SerializeField] bool AlwaysFullCharge;
    [SerializeField] bool disableEnergyBar;
    StageManager stageManager;

    public Slider barSlider;
    [SerializeField] Image fillImage;
    Color fillOriginalColor;

    [SerializeField] Image highlighterBox;
    
    [SerializeField] RectTransform anchorPoint;

    [SerializeField] TextMeshProUGUI energyNotFullText;
    [SerializeField] GameObject pressTabText;

    bool fullChargeEventTrigger = true;

    [SerializeField] Image arrowImage;

[Header("SFX")]
    [SerializeField] EventReference energyFullSFX;

    bool canCharge = true;

    Coroutine CR_EnergyNotFullTextFade = null;



    private void Awake() 
    {
        if(pressTabText){ pressTabText.SetActive(false); }

        _instance = this;

    }

    void OnDestroy()
    {
        _instance = null;
    }

    private void InitializeStartVariables(){

        cardSelectionMenu.OnMenuDisabled += MoveIntoView;
        cardSelectionMenu.OnMenuActivated += MoveOutOfView;

        //rectTransform = GetComponent<RectTransform>();
        stageManager = GameErrorHandler.NullCheck(StageManager.Instance, "Stage Manager");


        yAnchorPosition = transform.parent.localPosition.y;
        max = energy * chargeTime + coolDown;

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
        if(AlwaysFullCharge)
        {
            barSlider.value = 1;
            fullCharge = true;
            return;
        }
        
        //Flash
        if(fullCharge && fullChargeEventTrigger == true)
        {
            StartCoroutine(FlashBarColorWhileFull());
            highlighterBox.DOFade(1f, 0.1f);
            fullChargeEventTrigger = false;
        }

        if(canCharge)
        {
            ChargeEnergyBar();
        }
    }


    IEnumerator FlashBarColorWhileFull()
    {
        if(!fullChargeEventTrigger){yield break;}

        while(fullCharge)
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
            if(fullCharge){yield break;}
            float flashSpeed = 1.0f/chargeRate;
            arrowImage.DOFade(1, flashSpeed).SetUpdate(false);
            yield return new WaitForSecondsRealtime(flashSpeed);
            arrowImage.DOFade(0, flashSpeed).SetUpdate(false);
            yield return new WaitForSecondsRealtime(flashSpeed);
        }
    }

    public void MoveIntoView()
    {
        if(disableEnergyBar){return;}

        anchorPoint.DOLocalMoveY(yAnchorPosition, 0.45f).SetUpdate(true);
        fullChargeEventTrigger = true;
        canCharge = true;
        CalculateEnergy();
        
        StartCoroutine(FlashBarArrowWhileCharging());
    }

    public void MoveOutOfView()
    {
        anchorPoint.DOLocalMoveY(600, 0.15f).SetUpdate(true);
        fillImage.color = fillOriginalColor;
        highlighterBox.DOFade(0f, 0.1f);

        canCharge = false;
        fullCharge = false;

        if(pressTabText){ pressTabText.SetActive(false); }

        
    }

    private void CalculateEnergy()
    {
        //currentTurnDuration = max - 4 * chargeTime - coolDown;
        //would replaced when cost system implemented in card selection menu, need public function to get consumed cost
        currentEnergyValue = 0;
        fillImage.color = fillOriginalColor;
        fullCharge = false;

        if(pressTabText){ pressTabText.SetActive(false); }

    }

    //Main method for moving the slider bar in order to fill energy
    private void ChargeEnergyBar()
    {
        if(fullCharge){return;}
        if(currentEnergyValue < max + 0.5f)
        {
            currentEnergyValue += Time.deltaTime * chargeRate * chargeRateModifier;

            if(currentEnergyValue > max)
            {
                currentEnergyValue = max;
                barSlider.value = 1;
                fullCharge = true;

                if(pressTabText){ pressTabText.SetActive(true); }
                RuntimeManager.PlayOneShot(energyFullSFX);
            }
        }
        barSlider.value = currentEnergyValue/max;
    }

    public void GrantExtraEnergy(float energy)
    {
        currentEnergyValue += energy;
    }


    //return true if energy full
    public bool EnergyIsFull()
    {
        if(!fullCharge)
        {
            
            energyNotFullText.DOFade(1, 0.1f).SetUpdate(false);
            fadeoutTween.Kill();
            if(CR_EnergyNotFullTextFade != null)
            {
                StopCoroutine(CR_EnergyNotFullTextFade);
            }
            CR_EnergyNotFullTextFade = StartCoroutine(FadeInOutEnergyNotFullText());
        }
        return fullCharge;
    }

    Tween fadeoutTween;
    IEnumerator FadeInOutEnergyNotFullText()
    {
        
        yield return new WaitForSeconds(0.5f);
        fadeoutTween = energyNotFullText.DOFade(0, 0.2f).SetUpdate(false);
        


    }

}
