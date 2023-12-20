using System.Collections;
using System.Collections.Generic;
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
            if(value.GetComponent<MapLayoutController>())
            {
                _mapLayoutPrefab = value;
            }else
            {
                Debug.LogWarning("Cannot set map layout prefab on stage blueprint because the prefab does not contain a MapLayoutController on its root object.");
            }
        }
    }

    public StageType stageType = StageType.Combat;

    public SceneNames sceneToLoad;

    public SpawnTableSO spawnTable;


    public void GenerateStage(System.Random random)
    {
        
    }


}
