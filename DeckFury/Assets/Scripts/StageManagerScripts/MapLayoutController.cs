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

[Tooltip("If set to true, the stage manager will try to set the player at the given Player Spawn Position at the start of the stage.")]
    [SerializeField] bool _setSpawnPosition = false;
    public bool SetSpawnPosition => _setSpawnPosition;

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
