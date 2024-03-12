using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class controls all of the dynamic systems and elements of a stage like the map prefab, the player's stats etc.
/// and also takes care of completing the stage and setting necessary persistent values for the PLC like player stats.
/// </summary>
public class StageStateController : MonoBehaviour
{
    private static StageStateController _instance;
    public static StageStateController Instance {get{return _instance;}}

    CardSelectionMenu cardSelectionMenu; 
    RewardMenuController rewardMenuController;
    ScoreManager scoreManager;
    PersistentLevelController levelController;
    GameManager gameManager;
    PlayerController player;



    public bool SceneIsAdditive = false;
    public StageType _stageType;
    [field:SerializeField] public SpawnTableSO SpawnTable {get; private set;}
    [field:SerializeField] public PlayerDataContainer PlayerData {get; private set;}
    GameObject _stageMapPrefab;
    public GameObject StageMapPrefab => _stageMapPrefab;

    [SerializeField] PortalToStageSelect exitPortal;

    public bool isFinalStage = false;


    /// <summary>
    /// Shorthand for "Using Persistent Level Controller (PLC)". Allows other stage critical objects to check if the stage is using the PLC
    /// in which case it will try to set values to be according values within the StageStateController (which takes its values from the PLC if it exists).
    /// </summary>
    public bool UsingPLC {get; private set;} = false;

    private void Awake() 
    {
        _instance = this;


        if(PersistentLevelController.Instance != null || FindObjectOfType<PersistentLevelController>() != null)
        {
            levelController = GameErrorHandler.NullCheck(PersistentLevelController.Instance, "Persistent Level Controller");
            UsingPLC = true;
            SceneIsAdditive = true;
            SpawnTable = levelController.StageSpawnTable;

            _stageMapPrefab = PersistentLevelController.Instance.CurrentMapPrefab;
            _stageType = PersistentLevelController.Instance.StageType;

            isFinalStage = PersistentLevelController.Instance.playerIsAtFinalStage;
            //Remember that PlayerData is a class, not a struct, so any modifications made to the PlayerData here will be reflected
            //in the levelController PlayerData.
            PlayerData = levelController.PlayerData;

        }

        gameManager = GameErrorHandler.NullCheck(GameManager.Instance, "GameManager");
        if(!rewardMenuController)
        {
            rewardMenuController = FindObjectOfType<RewardMenuController>();
        }
        if(!scoreManager)
        {
            scoreManager = FindObjectOfType<ScoreManager>();
        }
        cardSelectionMenu = FindObjectOfType<CardSelectionMenu>();


    }

    private void OnDestroy() 
    {
        _instance = null;    
    }

    private void Start() 
    {
        player = gameManager.player;
        SetPlayerStats(PlayerData, player);        
    }

    void SetPlayerStats(PlayerDataContainer playerData, PlayerController player)
    {
        player.CurrentHP = playerData.CurrentHP;
        player.ShieldHP = playerData.BaseShieldHP;
        player.Armor = playerData.BaseArmor;
        player.Defense = playerData.BaseDefense;



    }

    public void CompleteStage()
    {
        GameManager.Instance.player.invincible = true;
        cardSelectionMenu.CanBeOpened = false;
        StartCoroutine(CompleteStageTimer(1));
        rewardMenuController.OnFinishSelectingReward += OpenPortal;
    }

    IEnumerator CompleteStageTimer(float duration)
    {
        yield return new WaitForSecondsRealtime(duration);
        rewardMenuController.MoveIntoView();


    }

    public void OpenPortal()
    {
        //Create a list of positions that are around the player that are between 2 - 4 tiles away from the player,
        //then place the portal location in the first valid tile. This doesn't take into account ALL situations (e.g. the player is completely trapped
        //in a 4 tile radius), but such a situation would be exceedingly rare and would be indicative of other problems that require solving first.
        foreach(Vector3Int vector3Int in VectorDirections.Vector3IntAll)
        {
            Vector3Int position2TilesAway = player.currentTilePosition + vector3Int * 2;
            Vector3Int position3TilesAway = player.currentTilePosition + vector3Int * 3;
            Vector3Int position4TilesAway = player.currentTilePosition + vector3Int * 4;

            if(StageManager.Instance.CheckValidTile(position2TilesAway))
            {
                exitPortal.transform.position = position2TilesAway;
                break;
            }
            if(StageManager.Instance.CheckValidTile(position3TilesAway))
            {
                exitPortal.transform.position = position3TilesAway;
                break;
            }
            if(StageManager.Instance.CheckValidTile(position4TilesAway))
            {
                exitPortal.transform.position = position4TilesAway;
                break;
            }                
        }

        exitPortal.gameObject.SetActive(true);        

    }


    public void EnterExitPortal()
    {
        StartCoroutine(TriggerScoringPanel(0.5f));
    }

    IEnumerator TriggerScoringPanel(float delay = 0)
    {
        yield return new WaitForSeconds(delay);
        ScoreMenuController scoreMenu = scoreManager.GetComponent<ScoreMenuController>();
        scoreMenu.MoveIntoView();
        scoreMenu.StartScorePresentation();
    }


    public void ReturnToStageSelect()
    {
        if(!SceneIsAdditive)
        {
            print("Current active scene was not loaded additively, cannot return to stage select.");
            return;
        }
        
        WrapUpStage();

        SceneLoader sceneLoader = gameManager.GetComponent<SceneLoader>();

        sceneLoader.UnloadActiveSceneThenReturn(SceneNames.StageSelectionScene.ToString());

    }

    public void GoToEndingStage()
    {
        if(!SceneIsAdditive)
        {
            print("Current active scene was not loaded additively, cannot return to stage select.");
            return;
        }
        
        WrapUpStage();

        SceneLoader sceneLoader = gameManager.GetComponent<SceneLoader>();

        sceneLoader.ChangeSceneAdditive(SceneNames.EndingStage.ToString());        
    }


    //Deals with setting player data to values from the stage and any other operations that need to be completed before returning to
    //the stage select.
    void WrapUpStage()
    {
        PlayerData.CurrentHP = player.CurrentHP;
        //PlayerData.CurrentScore += scoreManager.StageScoreTotal;

    }



}
