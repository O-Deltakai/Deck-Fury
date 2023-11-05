using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapLevel : MonoBehaviour
{
    [SerializeField] List<MapStage> stages;
    public int levelIndex;


    private void Awake() 
    {
        GetStagesFromChildren();


    }

    void GetStagesFromChildren()
    {
        foreach(MapStage stage in GetComponentsInChildren<MapStage>()) 
        {
            stage.mapLevel = this;
            stages.Add(stage);    
        }

    }


    public List<MapStage> GetStages()
    {
        return stages;
    }


}
