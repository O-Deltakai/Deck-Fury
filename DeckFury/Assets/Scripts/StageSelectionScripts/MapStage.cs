using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;
using Random = UnityEngine.Random;
using DG.Tweening;
using FMODUnity;
using FMOD.Studio;

public class MapStage : MonoBehaviour
{
    event Action<bool> OnButtonSetInteractable;


    [field:SerializeField] public RectTransform RightPoint {get; private set;}
    [field:SerializeField] public RectTransform LeftPoint {get; private set;}
    [HideInInspector] public Button StageButton {get; private set;}
    public bool buttonIsInteractable { get { return StageButton.interactable; } 
        set 
        {
            StageButton.interactable = value; 
            OnButtonSetInteractable?.Invoke(value);
        }
    }

    [Header("Stage Preview Popup Settings")]
    [SerializeField] GameObject StagePreviewPopup;
    [SerializeField] Image mapPreviewImage;
    [SerializeField] EnemyCountPreviewController enemyCountPreviewController;

    Tween previewSlideIntoViewTween;
    Tween previewSlideOutOfViewTween;
    [SerializeField] Ease slideIntoViewEase;
    [SerializeField] Ease slideOutOfViewEase;
    [SerializeField] float previewSlideDuration = 0.15f;


    [HideInInspector] public GameObject mapLayoutPrefab;
    public bool playerIsHere = false;

    [field:SerializeField] public StageType TypeOfStage {get; set;}

    public MapLevel mapLevel;

    public SpawnTableSO spawnTable;
    public SceneNames sceneToLoadName;
    SceneLoader sceneLoader;
    PersistentLevelController levelController;
    StageSelectionManager stageSelectionManager;
    Image mapStageIcon;

[Header("Stage Type Icons")]
    [SerializeReference] Sprite startingPointIcon;
    [SerializeReference] Sprite combatIcon;
    [SerializeReference] Sprite eliteCombatIcon;
    [SerializeReference] Sprite shopIcon;
    [SerializeReference] Sprite mysteryIcon;
    [SerializeReference] Sprite bossIcon;
    [SerializeReference] Sprite restIcon;

    [SerializeField] Image buttonIcon;
    [SerializeField] Image whiteDropShadow;

    [field:SerializeField] public bool IsFinalStage { get; set; } = false;
    [SerializeField] bool IsMysteryStage;

 [Header("SFX")]
    [SerializeField] EventReference hoverSFX;
    EventInstance hoverSFXInstance;

//Tweens
    Tween scaleBackAndForthTween;

    void Awake() 
    {
        mapStageIcon = GetComponent<Image>();
        StageButton = GetComponent<Button>();    

        ScaleBackAndForth();

        OnButtonSetInteractable += (bool isInteractable) => 
        {
            if(isInteractable)
            {
                ScaleBackAndForth();   
            }else
            {
                scaleBackAndForthTween.Kill();
            }
        };

    }

    // Start is called before the first frame update
    void Start()
    {
        sceneLoader = GameManager.Instance.GetComponent<SceneLoader>();
        if(sceneLoader == null)
        {
            sceneLoader = FindObjectOfType<SceneLoader>();
            Debug.LogWarning("SceneLoader could not be found from GameManager instance, something may have gone wrong with GameManager initialization."
            + "Attempting to use FindObjectOfType to find SceneLoader");
            if(sceneLoader == null)
            {
                Debug.LogError("SceneLoader could not be found within the scene at all, button will not function.");
            }
        }
        levelController = GameErrorHandler.NullCheck(PersistentLevelController.Instance, "PersistentLevelController");
        stageSelectionManager = GameErrorHandler.NullCheck(StageSelectionManager.Instance, "StageSelectionManager");

        SetStageType(TypeOfStage);

        StagePreviewPopup.transform.localScale = new Vector3(0, 0.9f, 0.9f);

        hoverSFXInstance = RuntimeManager.CreateInstance(hoverSFX);
    }

    public void GenerateStageValues()
    {
        int difficultyTier = mapLevel.LevelTier;
    }


    void CreateMapPreview()
    {
        if(mapPreviewImage.sprite != null){ return; }

        GameObject mapInstance = Instantiate(mapLayoutPrefab, new Vector3(-50, 0, 0), Quaternion.identity);
        MapPreviewGenerator previewGenerator = mapInstance.GetComponent<MapPreviewGenerator>();
        mapPreviewImage.sprite = previewGenerator.GeneratePreviewSprite();
        mapPreviewImage.preserveAspect = true;
        Destroy(mapInstance);
    }

    public void ToggleMapPreview(bool toggle)
    {
        if(!StageButton.interactable) { return; }
        if(!mapLayoutPrefab) { return; }
        if(IsMysteryStage){ return; }
        if(TypeOfStage == StageType.RestPoint) { return; }
        if(TypeOfStage == StageType.Shop) { return; }

        if(toggle)
        {
            CreateMapPreview();
            AssignPreviewElements();
            SlidePreviewIntoView();
        }else
        {
            SlidePreviewOutOfView();
        }
    }

    void AssignPreviewElements()
    { 
        if(enemyCountPreviewController)
        {
            enemyCountPreviewController.SetPreviewBySpawnTable(spawnTable);
        }
    }

    //Dispatcher method that sets the stage and initializes required variables for that stage type
    public void SetStageType(StageType stageType)
    {

        switch (stageType) 
        {
            case StageType.StartingPoint:
            buttonIcon.sprite = startingPointIcon;
            whiteDropShadow.sprite = startingPointIcon;
            SetStartingPoint();
            break;

            case StageType.Combat:
            buttonIcon.sprite = combatIcon;
            whiteDropShadow.sprite = combatIcon;
            SetCombatStage();
            break;

            case StageType.EliteCombat:
            buttonIcon.sprite = eliteCombatIcon;
            whiteDropShadow.sprite = eliteCombatIcon;
            SetEliteCombatStage();
            break;

            case StageType.Shop:
            buttonIcon.sprite = shopIcon;
            whiteDropShadow.sprite = shopIcon;
            SetShopStage();
            break;

            case StageType.Mystery:
            buttonIcon.sprite = mysteryIcon;
            whiteDropShadow.sprite = mysteryIcon;
            SetMysteryStage();
            break;
            
            case StageType.Boss:
            buttonIcon.sprite = bossIcon;
            whiteDropShadow.sprite = bossIcon;
            SetBossStage();
            break;

            case StageType.RestPoint:
            buttonIcon.sprite = restIcon;
            whiteDropShadow.sprite = restIcon;
            SetRestStage();
            break;

            default :
                break;
        }
    }
    void SetStartingPoint()
    {
        TypeOfStage = StageType.StartingPoint;
    }
    void SetCombatStage()
    {
        TypeOfStage = StageType.Combat;
    }
    void SetEliteCombatStage()
    {
        TypeOfStage = StageType.EliteCombat;
    }
    void SetShopStage()
    {
        TypeOfStage = StageType.Shop;
    }
    void SetMysteryStage()
    {
        StageType[] validStages = new StageType[]
        {
            StageType.Combat,
            //StageType.EliteCombat,
            StageType.Shop,
            StageType.RestPoint
        };
        int randomStageIndex = Random.Range(0, validStages.Length);

        TypeOfStage = validStages[randomStageIndex];
        IsMysteryStage = true;
    }
    void SetBossStage()
    {
        TypeOfStage = StageType.Boss;
    }
    void SetRestStage()
    {
        TypeOfStage = StageType.RestPoint;
    }

    void SlidePreviewIntoView()
    {
        if(previewSlideOutOfViewTween.IsActive()){ previewSlideOutOfViewTween.Kill(); }

        StagePreviewPopup.SetActive(true);

        previewSlideIntoViewTween = StagePreviewPopup.transform.DOScaleX(0.9f, previewSlideDuration).SetEase(slideIntoViewEase);
    }

    void SlidePreviewOutOfView()
    {
        if(previewSlideIntoViewTween.IsActive()){ previewSlideIntoViewTween.Kill(); }

        previewSlideOutOfViewTween = StagePreviewPopup.transform.DOScaleX(0, previewSlideDuration).SetEase(slideOutOfViewEase).OnComplete
        (() => StagePreviewPopup.SetActive(false));
    }

    public void OnHover()
    {
        if(!StageButton.interactable) { return; }

        if(scaleBackAndForthTween.IsActive()){ scaleBackAndForthTween.Kill(); }

        whiteDropShadow.gameObject.SetActive(true);
        buttonIcon.transform.DOScale(1.2f, 0.1f);
        whiteDropShadow.transform.DOScale(1.2f, 0.1f);
        hoverSFXInstance.start();
    }

    public void ExitHover()
    {
        if(!StageButton.interactable) { return; }

        ScaleBackAndForth();
        whiteDropShadow.gameObject.SetActive(false);
        buttonIcon.transform.DOScale(1f, 0.1f);
        whiteDropShadow.transform.DOScale(1f, 0.1f);

    }

    void ScaleBackAndForth()
    {
        if(scaleBackAndForthTween.IsActive()){ scaleBackAndForthTween.Kill(); }
        transform.localScale = new Vector3(1, 1, 1);
        scaleBackAndForthTween = transform.DOScale(1.2f, 0.5f).SetLoops(-1, LoopType.Yoyo);
    }


    public void LoadStageButton()
    {
        scaleBackAndForthTween.Kill();
        transform.localScale = new Vector3(1, 1, 1);
        if(sceneLoader == null)
        {
            Debug.LogError("SceneLoader is null! something's gone wrong, MapStage button will not function.");
            return;
        }
        GameManager.Instance.NextSceneIsNotInBattle = false;
        sceneLoader.LoadScene(sceneToLoadName.ToString());

    }

    public void SelectStageButton()
    {
        ToggleMapPreview(false);
        levelController.LoadMapStage(this);
    }

    public void DrawLineToThisStage()
    {
        if(stageSelectionManager)
        {
            stageSelectionManager.DrawLineBetweenPlayerAndStage(this);
        }
    }

    public void DisableTravelIndicator()
    {
        if(stageSelectionManager)
        {
            stageSelectionManager.DisableTravelIndicator();
        }
    }

}
