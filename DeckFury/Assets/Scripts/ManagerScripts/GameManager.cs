using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    private static GameManager _instance;
    public static GameManager Instance{get {return _instance;} }


    public delegate void SetCriticalReferencesEventHandler();
    public event SetCriticalReferencesEventHandler OnSetCriticalReferences;

    [SerializeField] GameObject menuScreenCanvas;

    [SerializeField] PlayerController playerSerializedReference;
    [SerializeField] Camera mainCameraReference;
    public static Camera mainCamera{get; private set;}
    public PlayerController player{get; private set;}


    [SerializeField] bool NotInBattle = false;
    public bool NextSceneIsNotInBattle{get{return NotInBattle;} set{NotInBattle = value;}}
    

    public static bool GameIsPaused{get; private set;}
    public static bool DeveloperNoteHasBeenShown = false;

    [SerializeField] GameObject defeatScreen;
    [SerializeField] float defeatScreenYAnchor;

    [SerializeField] GameObject defeatedByPanel;
    [SerializeField] TextMeshProUGUI causeOfDeathNote;
    [SerializeField] Image causeOfDeathAttackerImage;

    SceneLoader sceneLoader;
    public GlobalResourceManager ResourceManager{ get; private set; }

    [SerializeField] PauseMenuController _pauseMenu;
    public PauseMenuController PauseMenu => _pauseMenu;

    [SerializeField] string _customMapSeed;
    public string CustomMapSeed => _customMapSeed;


    public enum GameState
    {
        Realtime,
        Slowmotion,
        InMenu
    }

/// <summary>
/// Used by the pause menu to decide whether or not to unpause the game (setting time-scale to 1). If the game state is currently in menu (in card select) then
/// the pause menu won't unpause the game.
/// </summary>
    public static GameState currentGameState = GameState.Realtime;




    void Awake() 
    {

        InitializePersistentSingleton();

    }

    private void InitializePersistentSingleton()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            IntializeAwakeStates();
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }        
    }

    void IntializeAwakeStates()
    {
        sceneLoader = GetComponent<SceneLoader>();
        ResourceManager = GetComponent<GlobalResourceManager>();
        SceneManager.sceneLoaded += ChangedActiveScene;
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = -1;

        if(SceneManager.GetActiveScene().buildIndex == 0)
        {return;}

        // if(!NotInBattle)
        // {
        //     InitializeBattleCriticalVariables();
        // }


    }

    void InitializeBattleCriticalVariables()
    {
        CleanStaticReferences();
        
        print("Initializing battle critical variables");
        player = null;

        playerSerializedReference = FindObjectOfType<PlayerController>();
        if(playerSerializedReference == null)
        {
            Debug.LogWarning("Player object could not be find in the current scene, Player prefab should be placed in scene.");
        }

        mainCameraReference = FindObjectOfType<Camera>();

        player = playerSerializedReference;
        mainCamera = mainCameraReference;

        
        StartCoroutine(WaitToSetPlayerDefeatEvent());
        StartCoroutine(InvokeCriticalReferencesEvent());

    }

    IEnumerator InvokeCriticalReferencesEvent()
    {
        yield return new WaitForEndOfFrame();
        OnSetCriticalReferences?.Invoke();

    }

    IEnumerator WaitToSetPlayerDefeatEvent()
    {
        yield return new WaitForSecondsRealtime(0.1f);
        playerSerializedReference.OnPlayerDefeat += ShowDefeatScreen;
        playerSerializedReference.OnCauseOfDeath += SetCauseOfDefeatNote;

    }




    void SetCauseOfDefeatNote(string deathNote, AttackPayload payload, StageEntity entity)
    {
        if(!string.IsNullOrWhiteSpace(deathNote))
        {
            causeOfDeathNote.text = deathNote;
            defeatedByPanel.SetActive(true);
        }else
        {
            defeatedByPanel.SetActive(false);
            return;
        }
        if(payload.attackerSprite != null)
        {
            Sprite attackerSprite = payload.attackerSprite;

            causeOfDeathAttackerImage.sprite = attackerSprite;
            causeOfDeathAttackerImage.enabled = true;
        

        }else
        {
            causeOfDeathAttackerImage.enabled = false;

        }

    }

    void CleanStaticReferences()
    {
        if(playerSerializedReference != null || player != null)
        {
            player = null;
            playerSerializedReference = null;
        }


    }

    //Method that is called when a new scene is loaded
    void ChangedActiveScene(Scene nextScene, LoadSceneMode loadSceneMode)
    {
    
        if(SceneManager.GetActiveScene().buildIndex == 0)
        {return;}

        menuScreenCanvas.GetComponent<RectTransform>().ForceUpdateRectTransforms();

        CleanStaticReferences();

        if(playerSerializedReference != null)
        {
            playerSerializedReference.OnPlayerDefeat -= ShowDefeatScreen;
            playerSerializedReference.OnCauseOfDeath -= SetCauseOfDefeatNote;

            player.OnPlayerDefeat -= ShowDefeatScreen;
            player = null;
            playerSerializedReference = null;
        }

        if(!NotInBattle)
        {
            InitializeBattleCriticalVariables();
        }

        menuScreenCanvas.GetComponent<GraphicRaycaster>().enabled = false;
        currentGameState = GameState.Realtime;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

    }


    public static void PauseGame()
    {
        Time.timeScale = 0;
        GameIsPaused = true;
    }

    public static void UnpauseGame()
    {
        Time.timeScale = 1;
        GameIsPaused = false;
    }

    void ShowDefeatScreen()
    {
        menuScreenCanvas.GetComponent<GraphicRaycaster>().enabled = true;
        
        //Need to disable and then re-enable it in order to refresh the buttons so they get highlighted the next time you hover over them.
        //Not sure why it doesn't refresh on its own, but this will fix the issue for now.
        menuScreenCanvas.SetActive(false);
        menuScreenCanvas.SetActive(true);


        defeatScreen.transform.DOLocalMoveY(0, 1f).SetUpdate(true).SetEase(Ease.OutBounce);
        int currentDeaths = GlobalPlayerStatsManager.GetPlayerPrefStat(GlobalPlayerStatsManager.StatKey.NumberOfDeaths, out bool exists);
        GlobalPlayerStatsManager.SetPlayerPrefStat(GlobalPlayerStatsManager.StatKey.NumberOfDeaths, currentDeaths + 1);
        PlayerPrefsManager.SavePlayerPrefs();        

        ScoreManager.Instance.SavePlayerStatPrefs();
        AchievementManager.CheckAchievementsAsync();
    }

    public void ReplayStageButton(Button button)
    {
        defeatScreen.transform.DOLocalMoveY(defeatScreenYAnchor, 0.5f).SetUpdate(true).SetEase(Ease.OutBounce);
        if(playerSerializedReference != null)
        {
            playerSerializedReference.OnPlayerDefeat -= ShowDefeatScreen;
            playerSerializedReference.OnCauseOfDeath -= SetCauseOfDefeatNote;

            player.OnPlayerDefeat -= ShowDefeatScreen;
            player = null;
            playerSerializedReference = null;
        }
        sceneLoader.ReloadCurrentScene();

        menuScreenCanvas.GetComponent<GraphicRaycaster>().enabled = false;
        button.GetComponent<RectTransform>().ForceUpdateRectTransforms();

    }

    public void MainMenuButton(Button button = null)
    {
        defeatScreen.transform.DOLocalMoveY(defeatScreenYAnchor, 0.5f).SetUpdate(true).SetEase(Ease.OutBounce);
        if(playerSerializedReference != null)
        {
            playerSerializedReference.OnPlayerDefeat -= ShowDefeatScreen;
            playerSerializedReference.OnCauseOfDeath -= SetCauseOfDefeatNote;
            player.OnPlayerDefeat -= ShowDefeatScreen;
            player = null;
            playerSerializedReference = null;
        }

        
        sceneLoader.LoadScene(SceneNames.MainMenu.ToString());
        if(PersistentLevelController.Instance)
        {
            Destroy(FindAnyObjectByType<PersistentLevelController>().gameObject);
        }

        menuScreenCanvas.GetComponent<GraphicRaycaster>().enabled = false;
        if(button != null)
        {
            button.GetComponent<RectTransform>().ForceUpdateRectTransforms();
        }

        UnpauseGame();
        currentGameState = GameState.Realtime;
        _customMapSeed = "";
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        PlayerPrefsManager.SavePlayerPrefs();
        AchievementManager.CheckAchievementsAsync();
    }



    public void DisableMenuCanvasGraphicRaycaster()
    {
        menuScreenCanvas.GetComponent<GraphicRaycaster>().enabled = false;
    }
    
    public void SetCustomMapSeed(string seed)
    {
        _customMapSeed = seed;
    }


}
