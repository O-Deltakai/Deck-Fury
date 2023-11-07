using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapLevel : MonoBehaviour
{
    public MapZone mapZone;
    [SerializeField] List<MapStage> stages;
    [SerializeField] int _levelTier;
    public int LevelTier{get { return _levelTier; }}

    public int levelIndex;


    private void Awake() 
    {
        GetStagesFromChildren();


    }

    public List<MapStage> GenerateMapStages()
    {
        List<MapStage> generatedStages = new List<MapStage>();


        return generatedStages;
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
