using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class ProceduralGenerator : MonoBehaviour
{
    public event Action OnCompleteGeneration;

    [SerializeField] MapPoolSO _mapLayoutPool;
    public MapPoolSO MapLayoutPool => _mapLayoutPool;

    [SerializeField] SpawnTablePoolSO spawnTablePool;
    public SpawnTablePoolSO SpawnTablePool => spawnTablePool;

    [SerializeField] CardPoolSO _runCardPool;
    public CardPoolSO RunCardPool => _runCardPool;

    [SerializeField] ItemPoolSO _runItemPoolSO;
    public ItemPoolSO RunItemPoolSO => _runItemPoolSO;

    [SerializeField] GameObject stageMap;

    [SerializeField] string stringSeed;
    [SerializeField] TextMeshProUGUI mapSeedText;


    /// <summary>
    /// Used as the seed for randomization purposes. Will be derived from the stringSeed.
    /// </summary>
    [SerializeField] int savedSeed;
    System.Random random;

[Header("Prefabs")]
    [SerializeField] GameObject mapStagePrefab;
    [SerializeField] GameObject mapLevelPrefab;


    [SerializeField] bool testGeneration = false;
    [SerializeField] bool generateUsingGivenStringSeed = false;

    [SerializeReference] ZoneBlueprint zoneBlueprint;



    void Awake()
    {
        if(testGeneration)
        {
            InitializeZoneBlueprint();
            GenerateStageMap(zoneBlueprint);
            
        }
    }

    void Start()
    {
        if(PersistentLevelController.Instance)
        {
            PersistentLevelController.Instance.runRandomGenerator = random;
        }
        OnCompleteGeneration?.Invoke();
    }


    void GenerateSeed()
    {
        if(!string.IsNullOrEmpty(GameManager.Instance.CustomMapSeed))
        {
            stringSeed = GameManager.Instance.CustomMapSeed;
        }else
        {
            stringSeed = GenerateRandomString(10);
        }

        savedSeed = DJB2Hash(stringSeed);
        mapSeedText.text = stringSeed;
    }

    string GenerateRandomString(int length)
    {
        System.Random stringRandomizer = new System.Random();

        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, length)
        .Select(s => s[stringRandomizer.Next(s.Length)]).ToArray());
    }

    void InitializeZoneBlueprint()
    {
        if(!generateUsingGivenStringSeed)
        {
            GenerateSeed();
        }else
        {
            if(string.IsNullOrWhiteSpace(stringSeed))
            {
                stringSeed = GenerateRandomString(10);
            }

            savedSeed = DJB2Hash(stringSeed);
            mapSeedText.text = stringSeed;
        }
        random = new System.Random(savedSeed);

        zoneBlueprint = new ZoneBlueprint
        {
            mapLayoutPool = MapLayoutPool,
            spawnTablePool = spawnTablePool
            
        };
        zoneBlueprint.GenerateZone(random);

    }

    void GenerateStageMap(ZoneBlueprint blueprint)
    {
        foreach(var levelBlueprint in blueprint.LevelBlueprints)
        {
            MapLevel mapLevel = Instantiate(mapLevelPrefab, stageMap.transform).GetComponent<MapLevel>();
            
            foreach(var stageBlueprint in levelBlueprint.StageBlueprints)
            {
                MapStage mapStage = Instantiate(mapStagePrefab, mapLevel.transform).GetComponent<MapStage>();

                mapStage.mapLayoutPrefab = stageBlueprint.MapLayoutPrefab;
                mapStage.TypeOfStage = stageBlueprint.stageType;
                mapStage.sceneToLoadName = stageBlueprint.sceneToLoad;

                mapStage.spawnTable = stageBlueprint.spawnTable;
                mapStage.IsFinalStage = stageBlueprint.IsFinalStage;
            }

        }


    }

/// <summary>
/// Returns a deterministically generated integer with a given string using the DJB2 hash function
/// </summary>
/// <param name="str"></param>
/// <returns></returns>
    public static int DJB2Hash(string str)
    {
        int hash = 5381;
        foreach (char c in str)
        {
            hash = (hash << 5) + hash + c; /* hash * 33 + c */
        }
        return hash;        
    }


}
