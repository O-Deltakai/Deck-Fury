using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Used to set information required for a map stage on the stage selection scene to function.
/// </summary>
public class StageBlueprint
{
    GameObject _mapLayoutPrefab;
    public GameObject MapLayoutPrefab { get { return _mapLayoutPrefab; }
        set
        {
            if (value.GetComponent<MapLayoutController>())
            {
                _mapLayoutPrefab = value;
            } else
            {
                Debug.LogWarning("Cannot set map layout prefab on stage blueprint because the prefab does not contain a MapLayoutController on its root object.");
            }
        }
    }

    public StageType stageType = StageType.Combat;

    public SceneNames sceneToLoad;

    public SpawnTableSO spawnTable;

    StageType[] validStages = new StageType[]
    {
        StageType.Combat,
        StageType.EliteCombat,
        StageType.Shop,
        StageType.RestPoint,
        StageType.Mystery
    };


    public void GenerateStage(System.Random random, ZoneBlueprint zone)
    {
        StageType chosenStage = validStages[random.Next(0, validStages.Count())];


    }

    bool ValidateStageType(StageType stageType, ZoneBlueprint zone)
    {
        switch (stageType)
        {
            case StageType.EliteCombat:
                if(zone.TotalNumberOfEliteStages < GeneratorValues.MAX_ELITES_PER_ZONE)
                {
                    return true;
                }

                break;

            case StageType.Shop:
                if(zone.TotalNumberOfShops < GeneratorValues.MAX_SHOPS_PER_ZONE)
                {
                    return true;
                }

                break;

            case StageType.RestPoint:
                if(zone.TotalNumberOfRests < GeneratorValues.MAX_RESTS_PER_ZONE)
                {
                    return true;
                }

                break;
            
            default:
                return false;
                
        }

        return false;


    }



}
