using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class MapStage : MonoBehaviour
{
    [field:SerializeField] public RectTransform RightPoint {get; private set;}
    [field:SerializeField] public RectTransform LeftPoint {get; private set;}
    [field:SerializeField] public Button StageButton {get; private set;}

    [SerializeField] GameObject mapPreviewPopup;
    [SerializeField] Image mapPreviewImage;
    public GameObject mapLayoutPrefab;


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


    [field:SerializeField] public bool IsFinalStage { get; private set; } = false;
    [SerializeField] bool IsMysteryStage;

    void Awake() 
    {
        mapStageIcon = GetComponent<Image>();
        StageButton = GetComponent<Button>();    
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

        if(toggle)
        {
            CreateMapPreview();
            mapPreviewPopup.SetActive(true);
        }else
        {
            mapPreviewPopup.SetActive(false);
        }
    }

    //Dispatcher method that sets the stage and initializes required variables for that stage type
    public void SetStageType(StageType stageType)
    {

        switch (stageType) 
        {
            case StageType.StartingPoint:
            mapStageIcon.sprite = startingPointIcon;
            SetStartingPoint();
            break;

            case StageType.Combat:
            mapStageIcon.sprite = combatIcon;
            SetCombatStage();
            break;

            case StageType.EliteCombat:
            mapStageIcon.sprite = eliteCombatIcon;
            SetEliteCombatStage();
            break;

            case StageType.Shop:
            mapStageIcon.sprite = shopIcon;
            SetShopStage();
            break;

            case StageType.Mystery:
            mapStageIcon.sprite = mysteryIcon;
            SetMysteryStage();
            break;
            
            case StageType.Boss:
            mapStageIcon.sprite = bossIcon;
            SetBossStage();
            break;

            case StageType.RestPoint:
            mapStageIcon.sprite = restIcon;
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
            StageType.EliteCombat,
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

    public void LoadStageButton()
    {
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
