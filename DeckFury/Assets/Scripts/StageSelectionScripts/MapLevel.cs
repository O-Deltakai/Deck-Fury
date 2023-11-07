using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapLevel : MonoBehaviour
{
    [SerializeField] List<MapStage> stages;
    [SerializeField] int _levelTier;
    public int LevelTier{get { return _levelTier; }}

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
