using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Used to set information required for a map stage on the stage selection scene to function.
/// </summary>
[Serializable]
public class StageBlueprint
{
    [SerializeReference] public LevelBlueprint parentLevel;


    [SerializeField] GameObject _mapLayoutPrefab;
    public GameObject MapLayoutPrefab { get { return _mapLayoutPrefab; }
        set
        {
            if (value.GetComponent<MapLayoutController>())
            {
                _mapLayoutPrefab = value;
            } else
            {
                Debug.LogError("Cannot set map layout prefab on stage blueprint because the prefab does not contain a MapLayoutController on its root object.");
            }
        }
    }

    public StageType stageType = StageType.Combat;

    public SceneNames sceneToLoad;

    public SpawnTableSO spawnTable;



    public void GenerateStage(System.Random random, ZoneBlueprint zone)
    {

        List<StageType> availableStageTypes = zone.CurrentStageTypes;
        StageType chosenStage;

        //Grab a stage from the available stage types generated by the zone
        //Go from the bottom of the list so removal operations are O(1)
        if(availableStageTypes.Count - 1 >= 0)
        {
            chosenStage = availableStageTypes[availableStageTypes.Count - 1];
            availableStageTypes.RemoveAt(availableStageTypes.Count - 1); 
        }else
        {
            Debug.LogWarning("No more stage types found within the CurrentStageTypes of the current zone. Defaulted stage type to Combat." +
            " This normally shouldn't happen - something may have gone wrong with the procedural generation algorithm.");
            chosenStage = StageType.Combat;
            
        }

        stageType = chosenStage;

        
        if(chosenStage == StageType.RestPoint)
        {
            sceneToLoad = SceneNames.RestStage;
        }else
        if(chosenStage == StageType.Shop)
        {
            sceneToLoad = SceneNames.ShopStage;
        }

        if(chosenStage == StageType.Combat || chosenStage == StageType.EliteCombat)
        {
            sceneToLoad = SceneNames.GenericCombatStage;
        }


    }




}
