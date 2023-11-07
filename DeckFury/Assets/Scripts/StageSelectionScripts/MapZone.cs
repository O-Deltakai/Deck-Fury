using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapZone : MonoBehaviour
{
    [SerializeField] List<MapLevel> _mapLevels;
    public List<MapLevel> MapLevels{get{ return _mapLevels; }}

    public void GenerateMapLevels()
    {
        
    }



}
