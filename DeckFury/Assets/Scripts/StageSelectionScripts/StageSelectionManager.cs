using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class StageSelectionManager : MonoBehaviour
{
    private static StageSelectionManager _instance;
    public static StageSelectionManager Instance{get {return _instance;} }

    GameManager gameManager;
    PersistentLevelController levelController;

    [SerializeField] TextMeshProUGUI totalScoreText;
    [SerializeField] GameObject travelLineIndicator;
    [SerializeReference] GameObject stageMap;
    [SerializeReference] GameObject mapBackground;

    [SerializeField] MapPoolSO mapPool;


    MapLevel[] mapLevels;

    public MapStage currentPlayerLocation;

    private void Awake() 
    {
        _instance = this;    
    }

    private void OnDestroy() 
    {
        //Set static instance to null so that it can be garbage collected
        _instance = null;    
    }

    void Start()
    {
        travelLineIndicator.SetActive(false);
        mapLevels = stageMap.GetComponentsInChildren<MapLevel>();
        InitializeMapLevels();
        
        gameManager = GameErrorHandler.NullCheck(GameManager.Instance, "GameManager");
        levelController = GameErrorHandler.NullCheck( PersistentLevelController.Instance, "PersistentLevelController");
        levelController.PlayerData.OnPlayerDataModified += SetTotalScoreText;

    }

    // Update is called once per frame
    void Update()
    {
        SetTotalScoreText();   
    }

    void InitializeMapLevels()
    {
        for(int i = 0; i < mapLevels.Length ; i++) 
        {
            //Disable map stages beyond the first level
            mapLevels[i].levelIndex = i;
            if(i > 0)
            {
                foreach(MapStage mapStages in mapLevels[i].GetStages())
                {
                    mapStages.StageButton.interactable = false;
                }
            }    
        }
    }

    void SetTotalScoreText()
    {
        totalScoreText.text = levelController.PlayerData.CurrentScore.ToString();
    }

    public void SetPlayerStage(MapStage stage)
    {
        MapLevel mapLevel = stage.mapLevel;

        //Update interactable stage buttons on the stage map
        if(mapLevel.levelIndex < mapLevels.Length - 1)
        {
            foreach(MapStage mapStages in mapLevels[mapLevel.levelIndex + 1].GetStages())
            {
                mapStages.StageButton.interactable = true;
            }
        }

        DuplicateTravelIndicator();
        foreach(MapStage mapStages in mapLevel.GetStages())
        {
            mapStages.StageButton.interactable = false;
        }
        currentPlayerLocation.playerIsHere = false;
        currentPlayerLocation = stage;
        stage.playerIsHere = true;
        stage.StageButton.interactable = false;
    }

    //Using a manually set line ui image, automatically set the length and angle of this ui image to be a line between two map stages.
    void DrawLineBetweenStages(MapStage startLocation, MapStage destination)
    {
        if(destination.StageButton.interactable == false)
        {
            return;
        }
        RectTransform lineTransform = travelLineIndicator.GetComponent<RectTransform>();

        Vector3 startPosition = startLocation.RightPoint.position;
        Vector3 endPosition = destination.LeftPoint.position;

        Vector3 lineDirection = endPosition - startPosition;
        float distance = lineDirection.magnitude;

        lineTransform.position = startPosition + lineDirection * 0.5f; //Midpoint
        lineTransform.sizeDelta = new Vector2(distance, lineTransform.sizeDelta.y);
        lineTransform.localEulerAngles = new Vector3(0, 0, GetAngleFromVector(lineDirection));

        travelLineIndicator.SetActive(true);
    }

    public void DrawLineBetweenPlayerAndStage(MapStage destination)
    {
        DrawLineBetweenStages(currentPlayerLocation, destination);
    }

    float GetAngleFromVector(Vector3 direction)
    {
        direction = direction.normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        if (angle < 0)
        {
            angle += 360;
        } 
        return angle;
    }

    public void DisableTravelIndicator()
    {
        travelLineIndicator.SetActive(false);
    }

    void DuplicateTravelIndicator()
    {
        Instantiate(travelLineIndicator, mapBackground.transform);
    }


    public void ReturnToMainMenu()
    {
        GameManager.Instance.MainMenuButton();
    }

}
