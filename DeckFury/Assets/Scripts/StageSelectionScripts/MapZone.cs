using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapZone : MonoBehaviour
{
    [SerializeField] List<MapLevel> _mapLevels;
    public List<MapLevel> MapLevels{get{ return _mapLevels; }}

    public List<MapLevel> GenerateMapLevels()
    {
        List<MapLevel> generatedLevels = new List<MapLevel>();



        foreach(var level in generatedLevels)
        {
            level.mapZone = this;
        }

        return generatedLevels;
    }



}
