using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapLayoutController : MonoBehaviour
{
    [SerializeField] GameObject _mapPrefab;
    public GameObject MapPrefab {get { return _mapPrefab; }}

    [SerializeField] Tilemap GroundTilemap;
    [SerializeField] Tilemap WallTilemap;

    [field:SerializeField] public Tilemap OuterSpawnZoneMap{get; private set;}
    [field:SerializeField] public Tilemap InnerSpawnZoneMap{get; private set;}

    [SerializeField] Vector3 _playerSpawnPosition;
    public Vector3 PlayerSpawnPosition => _playerSpawnPosition;

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
