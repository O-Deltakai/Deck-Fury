using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

//Class that will manage all the tilemaps, systems related to tilemaps and methods to access/validate the tilemaps in various ways
//Singleton object
[RequireComponent(typeof(MapLoader))]
public class StageManager : MonoBehaviour
{
    //Singleton
    private static StageManager _instance;
    public static StageManager Instance{get {return _instance;} }

    public delegate void FinishedInitalizingTilesEvent();
    public event FinishedInitalizingTilesEvent OnFinishInitializingTiles;


    [SerializeField] MapLayoutController mapLayout;

    //groundTileMap is the Tilemap that most entities will be able to move around on.
    [field:SerializeField] public Tilemap GroundTileMap{get;private set;}

    //wallTilemap is the Tilemap that is meant to be impassable to most entities and acts as the general boundary of
    //where entities can move.
    [SerializeField] Tilemap wallTileMap;

    //VFXTilemap is used to for managing tile-based VFX, e.g. danger indicators
    [field:SerializeField] public Tilemap VFXTilemap{get; private set;}
    [field:SerializeField] public TileBase DangerVFXTile{get; private set;}


    public Dictionary<Vector3, GroundTileData> groundTileDictionary {get; private set;}
    //Seperate list of ground tiles (uses the ground tiles defined within groundTileDictionary) for when an indexer is required
    //in certain operations, like selecting a random tile on the board.
    public List<GroundTileData> groundTileList {get; private set;}

    MapLoader mapLoader;

    private void Awake()
    {
        _instance = this;
        mapLoader = GetComponent<MapLoader>();

        if(mapLayout && !mapLoader.UseMapLayoutPrefab)
        {
            SetTilemapsToMapLayout(mapLayout);
        }else
        {
            if(mapLoader.CurrentMap)
            {
                mapLayout = mapLoader.CurrentMap;
            }else
            {
                mapLayout = GetComponentInChildren<MapLayoutController>();
            }


            SetTilemapsToMapLayout(mapLayout);
        }
        InitGroundTileData();

        
    }


    void SetTilemapsToMapLayout(MapLayoutController map)
    {
        GroundTileMap = map.GetGroundTilemap();
        wallTileMap = map.GetWallTilemap();
    }

    public void SetStageToMapLayout(MapLayoutController map)
    {
        SetTilemapsToMapLayout(map);
        InitGroundTileData();         
    }

    public MapLayoutController GetMapLayout()
    {
        return mapLayout;
    }

    void SetPlayerPosition()
    {
        if(!mapLayout.SetSpawnPosition) { return; }

        print("Attempting to set play position to: " + mapLayout.PlayerSpawnPosition);
        //GameManager.Instance.player.worldTransform.position = mapLayout.PlayerSpawnPosition;
        GameManager.Instance.player.TeleportToLocation(mapLayout.PlayerSpawnPosition);

        
    }

    private void OnDestroy() 
    {
        _instance = null;    
    }


    void Start()
    {
        SetPlayerPosition();
    }


    void Update()
    {
        
    }

    private void InitializeSingleton()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.transform.parent.gameObject);
            Destroy(this.gameObject);
        }else
        {
            _instance = this;
        }
    }

    //Initializes GroundTileData by iterating through all non-empty tiles of the GroundTileMap.
    private void InitGroundTileData()
    {
        //Initializing dictionary and list of groundTiles
        groundTileDictionary = new Dictionary<Vector3, GroundTileData>();
        groundTileList = new List<GroundTileData>();


        //Going through all the tile positions within the groundTileMap and initializing a GroundTileData object at each tile position
        foreach(Vector3Int tilePosition in GroundTileMap.cellBounds.allPositionsWithin)
        {
            Vector3Int localCoords = new Vector3Int(tilePosition.x, tilePosition.y, 0);

            //If the tile position for some reason does not have a tile, continue to the next position
            if(!GroundTileMap.HasTile(localCoords)) {continue;}

            //Initialize a new GroundTileData object with variables set according to the tilePosition
            GroundTileData groundTile = new GroundTileData
            {
                localCoordinates = localCoords,
                worldPosition = GroundTileMap.CellToWorld(localCoords),
                entity = null
            };

            //Adds the new groundTile to the dictionary, allowing for quick access to the data of a tile just by giving a Vector3 as a key.
            groundTileDictionary.Add(groundTile.worldPosition, groundTile);

            //Add the same ground tile to the groundTileList, which allows for O(1) operations when using an indexer,
            //useful for random selection operations
            groundTileList.Add(groundTile);


        }

        OnFinishInitializingTiles?.Invoke();

    }

    //Returns the GroundTileData of the tile at the given tilePosition
    public GroundTileData GetGroundTileData(Vector3Int tilePosition)
    {
        if(!groundTileDictionary.ContainsKey(tilePosition))
        {
            //Debug.LogError("Could not find tile position: " + tilePosition + " in groundTileDictionary");
            return null;
        }
        //groundTileDictionary[GroundTileMap.CellToWorld(tilePosition)]
        return groundTileDictionary[GroundTileMap.CellToWorld(tilePosition)];
    }



    //Checks if the given coordinates are valid based on a number conditions. Returns true if conditions are passed,
    //false otherwise.
    public bool CheckValidTile(Vector3Int tileToCheck)
    {
        GroundTileData groundTile = GetGroundTileData(tileToCheck);
        //Check conditions of the groundTile on the tileToCheck
        if(groundTile == null)
        {
            //print("groundtiledata at: " + tileToCheck + " is null");
            return false;
        }

        //Is the tileToCheck the coordinates of a wall tile? 
        if(wallTileMap.HasTile(tileToCheck))
        {return false;}

        //Is the tile to check empty air?
        if(!GroundTileMap.HasTile(tileToCheck))
        {return false;}

        //Is the groundTile at the tileToCheck empty of other entities?
        if(groundTile.entity != null)
        {return false;}
        return true;
    }
    public bool CheckValidTile(Vector2Int tileToCheck)
    {
        Vector3Int castedTileToCheck = new Vector3Int(tileToCheck.x, tileToCheck.y, 0);
        GroundTileData groundTile = GetGroundTileData(castedTileToCheck);
        //Check conditions of the groundTile on the tileToCheck
        if(groundTile == null)
        {
            print("groundtiledata at: " + tileToCheck + " is null");
            return false;
        }

        //Is the tileToCheck the coordinates of a wall tile? 
        if(wallTileMap.HasTile(castedTileToCheck))
        {return false;}

        //Is the tile to check empty air?
        if(!GroundTileMap.HasTile(castedTileToCheck))
        {return false;}

        //Is the groundTile at the tileToCheck empty of other entities?
        if(groundTile.entity != null)
        {return false;}
        return true;
    }


    

    //Sets the entity for the ground tile at the given tilePosition
    public void SetTileEntity(StageEntity entity, Vector3Int tilePosition)
    {
        GetGroundTileData(tilePosition).entity = entity;

    }

    public void SetVFXTile(TileBase tile, List<Vector2Int> positions, float duration)
    {
        StartCoroutine(SetVFXTileCoroutine(tile, positions, duration));
    }
    public void SetVFXTile(TileBase tile, List<Vector3Int> positions, float duration)
    {
        StartCoroutine(SetVFXTileCoroutine(tile, positions, duration));
    }

    public void SetWarningTiles(List<Vector3Int> positions, float duration)
    {
        StartCoroutine(SetVFXTileCoroutine(DangerVFXTile, positions, duration));
    }
    public void SetWarningTiles(List<Vector2Int> positions, float duration)
    {
        StartCoroutine(SetVFXTileCoroutine(DangerVFXTile, positions, duration));
    }

    //Sets a tile position on the VFXTilemap to the given tile for a duration before removing the tile after said duration
    public IEnumerator SetVFXTileCoroutine(TileBase tile, Vector3Int position, float duration)
    {
        VFXTilemap.SetTile(position, tile);
        yield return new WaitForSeconds(duration);
        VFXTilemap.SetTile(position, null);
    }

    public IEnumerator SetVFXTileCoroutine(TileBase tile, List<Vector3Int> positions, float duration)
    {
        for (int i = 0; i < positions.Count; i++)
        {
            Vector3Int pos = positions[i];
            VFXTilemap.SetTile(pos, tile);
        }

        yield return new WaitForSeconds(duration);

        for (int i = 0; i < positions.Count; i++)
        {
            Vector3Int pos = positions[i];
            VFXTilemap.SetTile(pos, null);
        }
    }

    public IEnumerator SetVFXTileCoroutine(TileBase tile, List<Vector2Int> positions, float duration)
    {
        for (int i = 0; i < positions.Count; i++)
        {
            Vector2Int pos = positions[i];
            VFXTilemap.SetTile(new Vector3Int(pos.x, pos.y, 0), tile);
        }

        yield return new WaitForSeconds(duration);

        for (int i = 0; i < positions.Count; i++)
        {
            Vector2Int pos = positions[i];
            VFXTilemap.SetTile(new Vector3Int(pos.x, pos.y, 0), null);
        }
    }


}
