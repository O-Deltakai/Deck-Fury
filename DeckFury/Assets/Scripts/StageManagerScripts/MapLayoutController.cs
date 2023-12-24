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

     
    [SerializeField] Transform _playerSpawnPosition;
    public Vector3 PlayerSpawnPosition => _playerSpawnPosition.position;

    private void Awake() 
    {
        if (!_playerSpawnPosition)
        {
            Debug.LogWarning("Player Spawn Position was not set for this map layout, attempting to find it in scene.");
            _playerSpawnPosition = GameObject.FindGameObjectWithTag(TagNames.PlayerSpawnPosition.ToString()).transform;
        }

        if(_playerSpawnPosition)
        {
            SpriteRenderer spawnIndicator = _playerSpawnPosition.GetComponent<SpriteRenderer>();
            spawnIndicator.enabled = false;
        }else
        {
            Debug.LogError("Player Spawn Position could not be found within this map layout, player position may not be set correctly");
        }
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
