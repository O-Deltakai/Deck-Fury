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


    public bool playerIsHere = false;

    [field:SerializeField] public StageType TypeOfStage {get; set;}

    public MapLevel mapLevel;

    public GameObject mapLayoutPrefab; 
    public SpawnTableSO spawnTable;
    public SceneNames sceneToLoadName;
    SceneLoader sceneLoader;
    PersistentLevelController levelController;
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


        SetStageType(TypeOfStage);


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
        levelController.LoadMapStage(this);
    }

}
