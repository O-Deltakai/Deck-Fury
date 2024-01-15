using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A singleton object that is used by stages to set stage variables/states like the Spawn Table to use, the player's stats and
/// the map layout. This object should only exist once the player has started
/// a run and begins in the StageSelectionScene.
/// </summary>
public class PersistentLevelController : MonoBehaviour
{

    private static PersistentLevelController _instance;
    public static PersistentLevelController Instance{get{return _instance;}}
    SceneLoader sceneLoader;
    GameManager gameManager;
    StageSelectionManager stageSelectionManager;

    //The starting deck is the actual scriptable object deck that the game uses as the base-line deck for the player's GameDeck.
    //The starting deck shouldn't actually be modified.
    [SerializeField] DeckSO startingDeck;

    [field:SerializeField] public PlayerDataContainer PlayerData{get ; private set;}
    [field:SerializeField] public SpawnTableSO StageSpawnTable{get; private set;}

    public bool playerIsAtFinalStage { get; private set; } = false ;

    [SerializeField] GameObject _currentMapPrefab;
    public GameObject CurrentMapPrefab => _currentMapPrefab;


    //References to the hp/shields numbers on the stage selection screen
    [SerializeField] TextMeshProUGUI hpText;
    [SerializeField] TextMeshProUGUI shieldsText;


    private void InitializePersistentSingleton()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAwakeStates();
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }        
    }

    private void OnDestroy() 
    {
        _instance = null;
        PlayerData.OnPlayerDataModified -= SetHpShieldText;
    }

    private void InitializeAwakeStates()
    {
        //Intialize player's game deck using the startindDeck
        foreach(DeckElement deckElement in startingDeck.CardList)
        {
            PlayerData.CurrentDeck.CardList.Add(deckElement);
        }

        gameManager = GameErrorHandler.NullCheck(GameManager.Instance, "Game Manager");
        sceneLoader = GameErrorHandler.NullCheck(GameManager.Instance.GetComponent<SceneLoader>(), "Scene Loader");

    }

    private void Awake() 
    {
        InitializePersistentSingleton();


    }

    private void Start() 
    {
        stageSelectionManager = FindObjectOfType<StageSelectionManager>();
        PlayerData.OnPlayerDataModified += SetHpShieldText;
        SetHpShieldText();
    }

    void SetHpShieldText()
    {
        hpText.text = ":" + PlayerData.CurrentHP.ToString();
        shieldsText.text = ":" + PlayerData.BaseShieldHP.ToString();
    }


    public void LoadMapStage(MapStage stage, bool setPlayerStage = true)
    {
        string sceneName = stage.sceneToLoadName.ToString();
        StageSpawnTable = stage.spawnTable;
        StageType stageType = stage.TypeOfStage;
        playerIsAtFinalStage = stage.IsFinalStage;

        _currentMapPrefab = stage.mapLayoutPrefab;


        stageSelectionManager.SetPlayerStage(stage);


        GameManager.Instance.NextSceneIsNotInBattle = false;
        sceneLoader.LoadSceneAdditive(sceneName);
    }




}
