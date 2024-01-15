using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Tilemaps;
using System;
using Random = UnityEngine.Random;
using TMPro;

public class SpawnManager : MonoBehaviour
{
    public delegate void SpawnedNewWaveEventHandler(List<StageEntity> entities);
    public event SpawnedNewWaveEventHandler OnSpawnNewWave;    

    public delegate void StageClearedEventHandler();
    public event StageClearedEventHandler OnAllWavesCleared;

    public delegate void WaveStartEventHandler(int waveNumber);
    public event WaveStartEventHandler OnWaveBegin;

    public delegate void WaveEndEventHandler(int waveNumber);
    public event WaveEndEventHandler OnWaveEnd;


    //Inputs the current round
    public delegate void RoundCounterChangeEvent(int round);
    public event RoundCounterChangeEvent OnRoundChange;

    private static SpawnManager _instance;
    public static SpawnManager Instance{get{return _instance;}}

    PlayerController player;

    [SerializeReference] Tilemap OuterSpawnZoneTilemap;
    [SerializeReference] Tilemap InnerSpawnZoneTilemap;
    List<Vector3Int> outerSpawnPositions = new List<Vector3Int>();
    List<Vector3Int> innerSpawnPositions = new List<Vector3Int>();

    [SerializeField] StageManager stageManager;
    [SerializeField] EnergyController energyController;
    [SerializeField] List<GameObject> NPCSpawnTable;
    [SerializeField] SpawnTableSO StageSpawnTable;
    [SerializeField] TextMeshProUGUI waveCounterText;
    public int currentWaveCounter = 0;

    [SerializeField] GameObject SpawnObjectParent;
    static List<StageEntity> _currentActiveNPCS = new List<StageEntity>();
    public static IReadOnlyList<StageEntity> CurrentActiveNPCs => _currentActiveNPCS;
    public static int ActiveNPCCount{get {return _currentActiveNPCS.Count;}}
    [SerializeReference] List<StageEntity> ActiveStageEntities = new List<StageEntity>();


    [SerializeField] bool DisableAllSpawning = false;
    [SerializeField] bool UseSpawnTable = true;


    [SerializeField] int roundNumber = 1;
    [SerializeField] int maxRoundNumber = 10;
    [SerializeField] bool StopRandomSpawning = false;



    private void Awake() 
    {
        _instance = this;
        
        energyController = FindObjectOfType<EnergyController>();

    }

    public static List<StageEntity> GetActiveStageEntities()
    {
        return _currentActiveNPCS;   
    }

    void Start()
    {
        if(StageStateController.Instance != null)
        {
            if(StageStateController.Instance.SpawnTable != null)
            {
                StageSpawnTable = StageStateController.Instance.SpawnTable;
                OnAllWavesCleared += StageStateController.Instance.CompleteStage;
                UseSpawnTable = true;
                StopRandomSpawning = true;
            }
        }
        


        stageManager = GameErrorHandler.NullCheck(StageManager.Instance, "Stage Manager");


        OuterSpawnZoneTilemap = stageManager.GetMapLayout().OuterSpawnZoneMap;
        InnerSpawnZoneTilemap = stageManager.GetMapLayout().InnerSpawnZoneMap;

        if(OuterSpawnZoneTilemap != null && InnerSpawnZoneTilemap != null)
        {
            InitializeSpawnZones();
        }else
        {
            Debug.LogWarning("Outer spawn zone or Inner spawn zone have not been correctly set within the StageManager, spawning with OuterMap or InnerMap spawn type will not work.");
        }

        player = GameErrorHandler.NullCheck(GameManager.Instance.player, "PlayerController");

        if(DisableAllSpawning){ return; }

        if(UseSpawnTable)
        {
            OnWaveEnd += SendNextWave;
            BeginWave();
        }


    }

    //Adds in all the tile positions that have been painted in the OuterSpawnZoneTilemap and InnerSpawnZoneTilemap to lists of
    //Vector3Ints for use when spawning enemies
    void InitializeSpawnZones()
    {

        outerSpawnPositions = GetTilePositions(OuterSpawnZoneTilemap);
        innerSpawnPositions = GetTilePositions(InnerSpawnZoneTilemap);

    }

    //Returns a list of Vector3Ints that define all the positions on the given tilemap that actually have a tile.
    List<Vector3Int> GetTilePositions(Tilemap tilemap)
    {
        List<Vector3Int> positions = new List<Vector3Int>();
        BoundsInt bounds = tilemap.cellBounds;

        foreach (Vector3Int pos in bounds.allPositionsWithin)
        {
            if (tilemap.HasTile(pos))
            {
                positions.Add(pos);
            }
        }

        return positions;
    }


    private void OnDestroy() 
    {
        _instance = null;
        _currentActiveNPCS.Clear();    
        ActiveStageEntities.Clear();
    }


    void Update()
    {

        if(!StopRandomSpawning && _currentActiveNPCS.Count == 0)
        {
            if(roundNumber >= maxRoundNumber)
            {
                InitializeRound(maxRoundNumber);
            }else
            {
                InitializeRound(roundNumber++);
            }
        }

    }

    void UpdateWaveCounter()
    {
        if(waveCounterText)
        {
            waveCounterText.text = (currentWaveCounter + 1).ToString() + "/" + StageSpawnTable.WaveList.Count;
        }
    }

    public void SetSpawnTable(SpawnTableSO spawnTable)
    {
        StageSpawnTable = spawnTable;
    }

    void BeginWave()
    {
        print("Attempting to spawn new wave: " + currentWaveCounter);
        if(UseSpawnTable)
        {
            SendNextWave(currentWaveCounter);
        }
    }

    void BeginSpawning()
    {
        if(StopRandomSpawning){return;}
        InitializeRound(1);
    }

    void SendNextWave(int waveIndex)
    {
        if(waveIndex >= StageSpawnTable.WaveList.Count)
        {
            OnAllWavesCleared?.Invoke();
            print("Stage has been cleared");
            return;
        }
        InitializeWave(StageSpawnTable, waveIndex);
        UpdateWaveCounter();

    }

    void InitializeWave(SpawnTableSO spawnTable, int waveIndex)
    {
        int actualWaveIndex = waveIndex;

        if(waveIndex < 0)
        {
            actualWaveIndex = 0;
            Debug.LogWarning("Wave index was below 0, set to wave 0 instead");
        }else
        if(waveIndex >= spawnTable.WaveList.Count)
        {
            actualWaveIndex = spawnTable.WaveList.Count - 1;
            Debug.LogWarning("Wave index was greater than wave list count, set to the highest wave instead");
        }
    

        WaveTable wave = spawnTable.WaveList[actualWaveIndex];
        SpawnWave(wave);
    }
    void SpawnWave(WaveTable waveTable)
    {
        foreach(NPCSpawnData npcSpawn in waveTable.NPCSpawns)
        {
            switch (npcSpawn.SpawnZone) 
            {
                case SpawnType.OuterMap:
                SpawnNPCWithSpawnZone(npcSpawn, SpawnType.OuterMap);
                break;

                case SpawnType.InnerMap:
                SpawnNPCWithSpawnZone(npcSpawn, SpawnType.InnerMap);
                break;

                case SpawnType.NearPlayer:
                SpawnNPCNearOrAwayFromPlayer(npcSpawn, SpawnType.NearPlayer);
                break;

                case SpawnType.AwayFromPlayer:
                SpawnNPCNearOrAwayFromPlayer(npcSpawn, SpawnType.AwayFromPlayer);
                break;

                case SpawnType.Random:
                SpawnNPCOnRandomPosition(npcSpawn);
                break;

                default:
                break;
            }
        }
        OnSpawnNewWave?.Invoke(_currentActiveNPCS);
    }

    Vector3Int? FindValidPosition(List<Vector3Int> positions)
    {

        foreach(Vector3Int position in positions)
        {
            if(stageManager.CheckValidTile(position))
            {
                return position;
            }
        }

        return null;

    }

    //Safety net method: If spawn count is less than 0, set it to 1. 
    //Hard cap spawn count at 50 - spawn counts of more than 20 should already be very rare, any more than 50 is probably a mistake.
    int ValidateSpawnCount(NPCSpawnData npcSpawnData)
    {
        int actualSpawnCount;
        if(npcSpawnData.SpawnCount <= 0)
        {
            Debug.LogWarning("NPC Spawn Data for: " + npcSpawnData.NPCPrefab.name + " had a spawn count of less than or equal to 0, set actual spawn count to 1." 
            + "Make sure the spawn count for this npc is greater than 0.");
            actualSpawnCount = 1;
        }else
        if(npcSpawnData.SpawnCount > 50)
        {
            Debug.LogWarning("NPC Spawn Data for: " + npcSpawnData.NPCPrefab.name + " had a spawn count of more than 50, which is probably a mistake. Set actual spawn count to 50." 
            + "Double check the spawn count and make sure it is not set obscenely high.");
            actualSpawnCount = 50;
        }else
        {
            return npcSpawnData.SpawnCount;
        }

        return actualSpawnCount;
    }

    void SpawnNPCNearOrAwayFromPlayer(NPCSpawnData npcSpawnData, SpawnType spawnType)
    {
        if(spawnType != SpawnType.NearPlayer && spawnType != SpawnType.AwayFromPlayer)
        {
            Debug.LogError("Incorrect spawn type used for this method, only use NearPlayer or AwayFromPlayer SpawnType for this method.");
            return;
        }

        print("Attempting to spawn npcs: " +npcSpawnData.NPCPrefab.name + " using spawn type: " + spawnType.ToString());

        List<Vector3Int> actualSpawnZone = new List<Vector3Int>();
        int actualSpawnCount = ValidateSpawnCount(npcSpawnData);

        //Find valid positions around the player and add them to the actualSpawnZone
        if(spawnType == SpawnType.NearPlayer)
        {
            //Create a list of positions that are around the player that are between 2 and 3 tiles away from the player.
            foreach(Vector3Int vector3Int in VectorDirections.Vector3IntAll)
            {
                Vector3Int position2TilesAway = player.currentTilePosition + vector3Int * 2;
                Vector3Int position3TilesAway = player.currentTilePosition + vector3Int * 3;
                if(stageManager.CheckValidTile(position2TilesAway))
                {
                    actualSpawnZone.Add(position2TilesAway);
                }
                if(stageManager.CheckValidTile(position3TilesAway))
                {
                    actualSpawnZone.Add(position3TilesAway);
                }
            }
        }else
        {
            //Create a list of positions that are around the player that are between 4 and 5 tiles away from the player.
            foreach(Vector3Int vector3Int in VectorDirections.Vector3IntAll)
            {
                Vector3Int position4TilesAway = player.currentTilePosition + vector3Int * 4;
                Vector3Int position5TilesAway = player.currentTilePosition + vector3Int * 5;
                if(stageManager.CheckValidTile(position4TilesAway))
                {
                    actualSpawnZone.Add(position4TilesAway);
                }
                if(stageManager.CheckValidTile(position5TilesAway))
                {
                    actualSpawnZone.Add(position5TilesAway);
                }
            }
        }        

        if (actualSpawnZone.Count == 0)
        {
            Debug.LogWarning("No valid spawn positions were found around the player. Aborted spawning " + npcSpawnData.NPCPrefab.name);
            return;
        }

        //Begin spawning in npcs in the actualSpawnZone
        for(int i = 0; i < actualSpawnCount; i++) 
        {
            int index = Random.Range(0, actualSpawnZone.Count);
            int retryCount = 0;

            Vector3Int spawnPosition = actualSpawnZone[index];

            while(!stageManager.CheckValidTile(spawnPosition) && retryCount < 20)
            {
                index = Random.Range(0, actualSpawnZone.Count);
                spawnPosition = actualSpawnZone[index];
                retryCount++;
            }

            //Fail-safe for if random tile finder could not find a valid tile, it will instead iterate through the entire spawn zone to try find a valid tile.
            //This prevents the extremely rare occurence where the random number generator somehow generates the same number 20 times in a row, or if the spawn
            //zone is close to full and the randomizer just didn't manage to land on a free tile.
            if(!stageManager.CheckValidTile(spawnPosition))
            {
                for(int j = 0; j < actualSpawnZone.Count; j++) 
                {
                    spawnPosition = actualSpawnZone[j];
                    if(stageManager.CheckValidTile(spawnPosition))
                    {
                        break;
                    }    
                }

            }

            //If even after iterating through the entire spawn zone it could not find a valid tile, log a warning and return.
            if(!stageManager.CheckValidTile(spawnPosition))
            {
                Debug.LogWarning("Could not find a valid spawn point in " + spawnType.ToString() + "for npc: " + npcSpawnData.NPCPrefab.name + 
                 " it appears the spawn zone is full or completely invalid - skipped spawning this npc");
                return;
            }

            SpawnNPCPrefab(npcSpawnData.NPCPrefab, spawnPosition);
                                      
        }
    }

    void SpawnNPCWithSpawnZone(NPCSpawnData npcSpawnData, SpawnType spawnZone)
    {
        if(OuterSpawnZoneTilemap == null || InnerSpawnZoneTilemap == null){return;}
        if(spawnZone != SpawnType.OuterMap && spawnZone != SpawnType.InnerMap)
        {
            Debug.LogError("Incorrect spawn type used for this method, only use OuterMap or InnerMap SpawnType for this method.");
            return;
        }

        print("Attempting to spawn npcs: " +npcSpawnData.NPCPrefab.name + " using spawn type: " + spawnZone.ToString());


        List<Vector3Int> actualSpawnZone;
        int actualSpawnCount = ValidateSpawnCount(npcSpawnData);//Make sure the spawn count is greater than 0 but less than 51.

        if(spawnZone == SpawnType.OuterMap)
        {
            actualSpawnZone = outerSpawnPositions;
        }else
        {
            actualSpawnZone = innerSpawnPositions;
        }

        for(int i = 0; i < actualSpawnCount; i++) 
        {

            int index = Random.Range(0, actualSpawnZone.Count);
            int retryCount = 0;

            Vector3Int spawnPosition = actualSpawnZone[index];
            GroundTileData randomTile;

            if(stageManager.groundTileDictionary.ContainsKey(spawnPosition))
            {
                randomTile = stageManager.groundTileDictionary[spawnPosition];
            }else
            {
                randomTile = null;
                Debug.LogWarning("One of the spawn tiles for the" + spawnZone.ToString() + " at position: " + spawnPosition +
                " appears to not be on the GroundTileMap, double check that all painted spawn zones are within the GroundTilemap. Skipped spawning this npc.");
                continue;
            }


            while(!stageManager.CheckValidTile(spawnPosition) && retryCount < 20)
            {
                index = Random.Range(0, actualSpawnZone.Count);
                spawnPosition = actualSpawnZone[index];
                retryCount++;
            }

            //Fail-safe for if random tile finder could not find a valid tile, it will instead iterate through the entire spawn zone to try find a valid tile.
            //This prevents the extremely rare occurence where the random number generator somehow generates the same number 20 times in a row, or if the spawn
            //zone is close to full and the randomizer just didn't manage to land on a free tile.
            if(!stageManager.CheckValidTile(spawnPosition))
            {
                for(int j = 0; j < actualSpawnZone.Count; j++) 
                {
                    spawnPosition = actualSpawnZone[index];
                    if(stageManager.CheckValidTile(spawnPosition))
                    {
                        break;
                    }    
                }

            }

            //If even after iterating through the entire spawn zone it could not find a valid tile, log a warning and return.
            if(!stageManager.CheckValidTile(spawnPosition))
            {
                Debug.LogWarning("Could not find a valid spawn point in " + spawnZone.ToString() + "for npc: " + npcSpawnData.NPCPrefab.name + 
                 " it appears the spawn zone is full or close to full - skipped spawning this npc");
                return;
            }
            
            randomTile = stageManager.groundTileDictionary[outerSpawnPositions[index]];

            SpawnNPCPrefab(npcSpawnData.NPCPrefab, randomTile.localCoordinates);


        }
        

    }

    // Gives the spawn position. 
    public Vector3Int PredictNPCSpawnPosition(Vector3Int position, bool safetyNet = true)
    {
        Vector3Int actualSpawnPosition = position;

        if (!stageManager.CheckValidTile(position) && safetyNet == false)
        {
            Debug.LogWarning("Cannot predict npc spawn position at: " + position + " because it is invalid.");
            return Vector3Int.zero; // will return null
        }
        else if (!stageManager.CheckValidTile(position))
        {
            foreach (Vector3Int pos in VectorDirections.Vector3IntAll)
            {
                if (stageManager.CheckValidTile(actualSpawnPosition + pos))
                {
                    actualSpawnPosition += pos;
                    break;
                }
                if (stageManager.CheckValidTile(actualSpawnPosition + pos * 2))
                {
                    actualSpawnPosition += pos * 2;
                    break;
                }
            }
        }

        if (!stageManager.CheckValidTile(actualSpawnPosition))
        {
            Debug.LogWarning("Could not predict npc spawn position at or near: " + position + ".");
            return Vector3Int.zero; // return null
        }

        return actualSpawnPosition;
    }



    /// <summary>
    /// Spawns a given NPC prefab - this prefab must have an EntityWrapper component on the root object or it will abort spawning.
    /// If safetyNet is set to true (which it is by default), then method will try to find a valid position to spawn the npc in a 2 tile radius before aborting.
    /// </summary>
    /// <param name="npcPrefab"></param>
    /// <param name="position"></param>
    /// <param name="safetyNet"></param>
    /// <returns></returns>
   public StageEntity SpawnNPCPrefab(GameObject npcPrefab, Vector3Int position, bool safetyNet = true)
        {
            Vector3Int actualSpawnPosition = position;
            if(npcPrefab.GetComponent<EntityWrapper>() == null)
            {
                Debug.LogWarning("The given npc prefab:" + npcPrefab.name + "does not have an EntityWrapper component on its root object. Aborted spawning.");
                return null;
            }

            if(!stageManager.CheckValidTile(position) && safetyNet == false)
            {
                Debug.LogWarning("Cannot spawn npc at position: " + position + " because it is invalid. Aborted spawning.");
                return null;
            }else
            //Safety net check where if the given position isn't valid, try and find a valid position in a radius of 2 tiles and spawn the entity there instead.
            if(!stageManager.CheckValidTile(position))
            {
                foreach(Vector3Int pos in VectorDirections.Vector3IntAll)
                {
                    if(stageManager.CheckValidTile(actualSpawnPosition + pos))
                    {
                        actualSpawnPosition += pos;
                        break;
                    }
                    if(stageManager.CheckValidTile(actualSpawnPosition + pos * 2))
                    {
                        actualSpawnPosition += pos * 2;
                        break;
                    }
                }
            }

            if(!stageManager.CheckValidTile(actualSpawnPosition))
            {
                Debug.LogWarning("Could not spawn npc at or near position: " + position + ". Aborted spawning.");
                return null;
            }


            GameObject npcObject = Instantiate(npcPrefab, actualSpawnPosition, Quaternion.identity, SpawnObjectParent.transform);
            StageEntity npcEntity = npcObject.GetComponent<EntityWrapper>().stageEntity;
            _currentActiveNPCS.Add(npcEntity);
            npcEntity.OnDestructionEvent += RemoveNPCFromActiveList;
            ActiveStageEntities = _currentActiveNPCS;
            return npcEntity;

        }




    void SpawnNPCOnRandomPosition(NPCSpawnData npcSpawnData)
    {
        int actualSpawnCount = ValidateSpawnCount(npcSpawnData);//Make sure the spawn count is greater than 0 but less than 51.

        print("Attempting to spawn npcs: " +npcSpawnData.NPCPrefab.name + " use spawn type: " + SpawnType.Random);


        for(int i = 0; i < npcSpawnData.SpawnCount; i++) 
        {
            int index = Random.Range(0, stageManager.groundTileList.Count);
            int retryCount = 0;
            Vector3Int spawnPosition;

            //Find a random position within the ground tile map
            while(!stageManager.CheckValidTile(stageManager.groundTileList[index].localCoordinates) && retryCount < 20)
            {
                index = Random.Range(0, stageManager.groundTileList.Count);
                retryCount++;
            }
            spawnPosition = stageManager.groundTileList[index].localCoordinates;

            //Fail-safe for if random tile finder could not find a valid tile, it will instead iterate through the entire spawn zone to try find a valid tile.
            //This prevents the extremely rare occurence where the random number generator somehow generates the same number 20 times in a row, or if the spawn
            //zone is close to full and the randomizer just didn't manage to land on a free tile.
            if(!stageManager.CheckValidTile(spawnPosition))
            {
                for(int j = 0; j < stageManager.groundTileList.Count; j++) 
                {
                    spawnPosition = stageManager.groundTileList[index].localCoordinates;
                    if(stageManager.CheckValidTile(spawnPosition))
                    {
                        break;
                    }    
                }
            }    

            SpawnNPCPrefab(npcSpawnData.NPCPrefab, spawnPosition);
        }
    }

    //Initializes a round by getting a number of random tiles on the board and instantiating NPCs from the NPCSpawnTable on those random tiles.
    //Also subscribes the RemoveNPCFromActiveList method to the death event of each active npcs so that they are removed from the list
    //whenever that NPC triggers its death event.
    public void InitializeRound(int round)
    {

        for(int i = 0; i < roundNumber; i++) 
        {
            //Get a random index to use to get a random tile from the groundTileList defined in the StageManager class.
            int index = Random.Range(0, stageManager.groundTileList.Count);
            GroundTileData randomTile = stageManager.groundTileList[index];

            //If the chosen random tile already has an entity on it, decrement the iterator and find another random tile.
            if(randomTile.entity != null)
            {
                i--;
                continue;
            }

            
            int randomIndex = Random.Range(0, NPCSpawnTable.Count);
            GameObject npcObject = Instantiate(NPCSpawnTable[randomIndex], randomTile.localCoordinates, Quaternion.identity, SpawnObjectParent.transform);

            _currentActiveNPCS.Add(npcObject.GetComponent<EntityWrapper>().stageEntity);
            ActiveStageEntities = _currentActiveNPCS;
        }

        //Subscribing the DeathEvent on all active NPCs to the RemoveNPCFromActiveList method.
        foreach(StageEntity npc in _currentActiveNPCS)
        {
            npc.OnDestructionEvent += RemoveNPCFromActiveList;
        }

        OnRoundChange?.Invoke(round);
    }

    void InitalizeRoundWaveTable()
    {
        
    }


    public void RemoveNPCFromActiveList(StageEntity entity, Vector3Int deathPosition)
    {
        bool removeSuccess;
        removeSuccess = _currentActiveNPCS.Remove(entity);
        if(!removeSuccess)
        {
            Debug.LogWarning("Could not remove entity: " + entity.name + " from active npc list, entity does not exist in the list.", this);
        }

        ActiveStageEntities = _currentActiveNPCS;
        if(_currentActiveNPCS.Count == 0)
        {
            currentWaveCounter++;
            OnWaveEnd?.Invoke(currentWaveCounter);
        }
    }




}
