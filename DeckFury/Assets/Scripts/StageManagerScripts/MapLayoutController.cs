using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapLayoutController : MonoBehaviour
{
    [SerializeField] Tilemap GroundTilemap;
    [SerializeField] Tilemap WallTilemap;

    [field:SerializeField] public Tilemap OuterSpawnZoneMap{get; private set;}
    [field:SerializeField] public Tilemap InnerSpawnZoneMap{get; private set;}


    private void Awake() 
    {
            
    }


    public Tilemap GetGroundTilemap()
    {
        return GroundTilemap;
    }

    public Tilemap GetWallTilemap()
    {
        return WallTilemap;
    }



}
